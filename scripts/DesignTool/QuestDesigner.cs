using System.Collections.Generic;
using GameFeel.Singleton;
using Godot;
using GodotTools.Extension;
using Newtonsoft.Json;

namespace GameFeel.DesignTool
{
    public class QuestDesigner : Control
    {
        private GraphEdit _graphEdit;
        private WindowDialog _questEventDialog;
        private FileDialog _openFileDialog;
        private ResourcePreloader _resourcePreloader;

        public override void _Ready()
        {
            GetTree().SetScreenStretch(SceneTree.StretchMode.Disabled, SceneTree.StretchAspect.Ignore, new Vector2(1920, 1080));
            _graphEdit = GetNode<GraphEdit>("VBoxContainer/GraphEdit");
            _resourcePreloader = GetNode<ResourcePreloader>("ResourcePreloader");
            _questEventDialog = GetNode<WindowDialog>("WindowDialog");
            _openFileDialog = GetNode<FileDialog>("OpenFileDialog");

            var itemList = _questEventDialog.GetNode<ItemList>("VBoxContainer/ItemList");
            foreach (var key in GameEventDispatcher.GameEventMapping.Keys)
            {
                var evt = GameEventDispatcher.GameEventMapping[key];
                itemList.AddItem(evt.DisplayName);
            }
            itemList.Connect("item_activated", this, nameof(OnQuestEventItemActivated));

            GetNode("VBoxContainer/HBoxContainer/AddStageNode").Connect("pressed", this, nameof(OnAddStageNodePressed));
            GetNode("VBoxContainer/HBoxContainer/AddStartNode").Connect("pressed", this, nameof(OnAddStartNodePressed));
            GetNode("VBoxContainer/HBoxContainer/AddEventNode").Connect("pressed", this, nameof(OnAddEventNodePressed));
            GetNode("VBoxContainer/HBoxContainer/SaveButton").Connect("pressed", this, nameof(OnSaveButtonPressed));
            GetNode("VBoxContainer/HBoxContainer/OpenButton").Connect("pressed", this, nameof(OnOpenButtonPressed));

            _graphEdit.Connect("connection_request", this, nameof(OnConnectionRequest));
            _graphEdit.Connect("disconnection_request", this, nameof(OnDisconnectRequest));

            _openFileDialog.Connect("file_selected", this, nameof(OnFileSelected));
        }

        private QuestEventNode GetQuestEventNodeFromGuid(string guid)
        {
            switch (guid.ToString())
            {
                case GameEventDispatcher.PLAYER_INVENTORY_ITEM_ADDED:
                    return _resourcePreloader.InstanceScene<QuestEventPlayerInventoryItemAdded>();
                case GameEventDispatcher.ENTITY_KILLED:
                    return _resourcePreloader.InstanceScene<QuestEventEntityKilled>();
            }
            return null;
        }

        private void OnAddStageNodePressed()
        {
            var node = _resourcePreloader.InstanceScene<QuestStageNode>();
            _graphEdit.AddChild(node);
            node.Connect(nameof(QuestNode.CloseRequest), this, nameof(OnCloseRequest));
        }

        private void OnAddStartNodePressed()
        {
            var node = _resourcePreloader.InstanceScene<QuestStartNode>();
            _graphEdit.AddChild(node);
            node.Connect(nameof(QuestNode.CloseRequest), this, nameof(OnCloseRequest));
        }

        private void OnAddEventNodePressed()
        {
            _questEventDialog.PopupCentered();
        }

        private void OnConnectionRequest(string from, int fromPort, string to, int toPort)
        {
            _graphEdit.ConnectNode(from, fromPort, to, toPort);
        }

        private void OnDisconnectRequest(string from, int fromPort, string to, int toPort)
        {
            _graphEdit.DisconnectNode(from, fromPort, to, toPort);
        }

        private void OnCloseRequest(QuestNode questNode)
        {
            foreach (Godot.Collections.Dictionary connection in _graphEdit.GetConnectionList())
            {
                var from = (string) connection["from"];
                var to = (string) connection["to"];
                var fromPort = (int) connection["from_port"];
                var toPort = (int) connection["to_port"];
                if (from == questNode.GetName() || to == questNode.GetName())
                {
                    if (_graphEdit.IsNodeConnected(from, fromPort, to, toPort))
                    {
                        _graphEdit.DisconnectNode(from, fromPort, to, toPort);
                    }
                }
            }
            questNode.QueueFree();
        }

        private void OnQuestEventItemActivated(int idx)
        {
            var keylist = new List<string>(GameEventDispatcher.GameEventMapping.Keys);
            var guid = keylist[idx];
            var node = GetQuestEventNodeFromGuid(guid);
            if (node != null)
            {
                _graphEdit.AddChild(node);
                node.SetTitle(GameEventDispatcher.GameEventMapping[guid].DisplayName);
                node.Connect(nameof(QuestNode.CloseRequest), this, nameof(OnCloseRequest));
            }
            _questEventDialog.Hide();
        }

        private void OnSaveButtonPressed()
        {
            var saveModel = new QuestSaveModel();
            foreach (Godot.Collections.Dictionary connection in _graphEdit.GetConnectionList())
            {
                var from = (string) connection["from"];
                var to = (string) connection["to"];
                var fromPort = (int) connection["from_port"];
                var toPort = (int) connection["to_port"];

                var fromQuestNode = _graphEdit.GetNode(from) as QuestNode;
                var fromModel = fromQuestNode.Model;
                var toQuestNode = _graphEdit.GetNode(to) as QuestNode;
                var toModel = toQuestNode.Model;

                StoreData(saveModel, fromQuestNode);
                StoreData(saveModel, toQuestNode);

                saveModel.AddRightConnection(fromModel.Id, toModel.Id);
            }
            var json = JsonConvert.SerializeObject(saveModel);
            var file = new File();
            file.Open("res://test.quest", (int) File.ModeFlags.Write);
            file.StoreLine(json);
            file.Close();
        }

        private void StoreData(QuestSaveModel saveModel, QuestNode node)
        {
            if (node is QuestStartNode qsn)
            {
                var fromModel = qsn.Model;
                saveModel.Start.Id = fromModel.Id;
                saveModel.Start.DisplayName = fromModel.DisplayName;
            }
            else if (node is QuestEventNode qen)
            {
                var fromModel = qen.Model;
                saveModel.AddEvent((QuestEventNode.QuestEventModel) fromModel);
            }
            else if (node is QuestStageNode qstn)
            {
                var fromModel = qstn.Model;
                saveModel.AddStage((QuestStageNode.QuestStageModel) fromModel);
            }
        }

        private void Load(string path)
        {
            foreach (var node in _graphEdit.GetChildren())
            {
                if (node is QuestNode qn)
                {
                    qn.QueueFree();
                }
            }
            var file = new File();
            file.Open(path, (int) File.ModeFlags.Read);
            var json = file.GetAsText();
            file.Close();
            var saveModel = JsonConvert.DeserializeObject<QuestSaveModel>(json);

            var idToNodeMappings = new Dictionary<string, QuestNode>();
            idToNodeMappings.Clear();

            var qsn = _resourcePreloader.InstanceScene<QuestStartNode>();
            _graphEdit.AddChild(qsn);
            qsn.LoadModel(saveModel.Start);
            idToNodeMappings.Add(qsn.Model.Id, qsn);

            foreach (var model in saveModel.Events)
            {
                var evt = _resourcePreloader.InstanceScene<QuestEventNode>();
                _graphEdit.AddChild(evt);
                evt.LoadModel(model);
                idToNodeMappings.Add(evt.Model.Id, evt);
            }

            foreach (var model in saveModel.Stages)
            {
                var stage = _resourcePreloader.InstanceScene<QuestStageNode>();
                _graphEdit.AddChild(stage);
                stage.LoadModel(model);
                idToNodeMappings.Add(stage.Model.Id, stage);
            }

            foreach (var sourceId in saveModel.RightConnections.Keys)
            {
                foreach (var toId in saveModel.RightConnections[sourceId])
                {
                    var fromNode = idToNodeMappings[sourceId];
                    var toNode = idToNodeMappings[toId];

                    _graphEdit.ConnectNode(fromNode.GetName(), 0, toNode.GetName(), 0);
                }
            }
        }

        private void OnOpenButtonPressed()
        {
            _openFileDialog.PopupCenteredRatio();
        }

        private void OnFileSelected(string path)
        {
            Load(path);
        }
    }
}