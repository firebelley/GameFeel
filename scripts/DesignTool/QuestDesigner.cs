using System.Collections.Generic;
using GameFeel.Data.Model;
using GameFeel.Singleton;
using Godot;
using GodotTools.Extension;
using Newtonsoft.Json;

namespace GameFeel.DesignTool
{
    public class QuestDesigner : Control
    {
        private GraphEdit _graphEdit;
        private WindowDialog _eventSelectorDialog;
        private WindowDialog _nodeSelectorDialog;
        private FileDialog _openFileDialog;
        private FileDialog _saveFileDialog;
        private ResourcePreloader _resourcePreloader;

        public override void _Ready()
        {
            GetTree().SetScreenStretch(SceneTree.StretchMode.Mode2d, SceneTree.StretchAspect.Ignore, new Vector2(1920, 1080));
            OS.SetWindowMaximized(true);

            _graphEdit = GetNode<GraphEdit>("VBoxContainer/GraphEdit");
            _resourcePreloader = GetNode<ResourcePreloader>("ResourcePreloader");
            _eventSelectorDialog = GetNode<WindowDialog>("EventSelectorDialog");
            _nodeSelectorDialog = GetNode<WindowDialog>("NodeSelectorDialog");
            _openFileDialog = GetNode<FileDialog>("OpenFileDialog");
            _saveFileDialog = GetNode<FileDialog>("SaveFileDialog");

            var eventItemList = _eventSelectorDialog.GetNode<ItemList>("VBoxContainer/ItemList");
            foreach (var key in GameEventDispatcher.GameEventMapping.Keys)
            {
                var evt = GameEventDispatcher.GameEventMapping[key];
                eventItemList.AddItem(evt.DisplayName);
            }
            eventItemList.Connect("item_activated", this, nameof(OnQuestEventItemActivated));

            var nodeItemList = _nodeSelectorDialog.GetNode<ItemList>("VBoxContainer/ItemList");
            nodeItemList.AddItem(nameof(QuestStartNode));
            nodeItemList.AddItem(nameof(QuestStageNode));
            nodeItemList.AddItem(nameof(QuestEventNode));
            nodeItemList.AddItem(nameof(QuestCompleteNode));
            nodeItemList.Connect("item_activated", this, nameof(OnNodeSelectorSelected));

            GetNode("VBoxContainer/HBoxContainer/AddNode").Connect("pressed", this, nameof(OnAddNodePressed));
            GetNode("VBoxContainer/HBoxContainer/SaveButton").Connect("pressed", this, nameof(OnSaveButtonPressed));
            GetNode("VBoxContainer/HBoxContainer/OpenButton").Connect("pressed", this, nameof(OnOpenButtonPressed));

            _graphEdit.Connect("connection_request", this, nameof(OnConnectionRequest));
            _graphEdit.Connect("disconnection_request", this, nameof(OnDisconnectRequest));

            _openFileDialog.Connect("file_selected", this, nameof(OnFileSelected));
            _saveFileDialog.Connect("file_selected", this, nameof(OnSaveFileSelected));
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

        private T AddQuestNode<T>() where T : QuestNode
        {
            var node = _resourcePreloader.InstanceScene<T>();
            AddQuestNode(node);
            return node;
        }

        private QuestNode AddQuestNode(QuestNode n)
        {
            _graphEdit.AddChild(n);
            n.Connect(nameof(QuestNode.CloseRequest), this, nameof(OnCloseRequest));
            return n;
        }

        private void StoreData(QuestSaveModel saveModel, QuestNode node)
        {
            if (node is QuestStartNode qsn)
            {
                saveModel.Start = qsn.Model;
            }
            else if (node is QuestEventNode qen)
            {
                saveModel.Events.Add(qen.Model);
            }
            else if (node is QuestStageNode qstn)
            {
                saveModel.Stages.Add(qstn.Model);
            }
            else if (node is QuestCompleteNode qcn)
            {
                saveModel.Completions.Add(qcn.Model);
            }
        }

        private void Load(string path)
        {
            foreach (var node in _graphEdit.GetChildren())
            {
                if (node is QuestNode qn)
                {
                    qn.GetParent().RemoveChild(qn);
                    qn.QueueFree();
                }
            }
            var file = new File();
            file.OpenCompressed(path, (int) File.ModeFlags.Read, (int) File.CompressionMode.Gzip);
            var json = file.GetAsText();
            file.Close();
            var saveModel = JsonConvert.DeserializeObject<QuestSaveModel>(json);

            var idToNodeMappings = new Dictionary<string, QuestNode>();
            idToNodeMappings.Clear();

            var qsn = AddQuestNode<QuestStartNode>();
            qsn.LoadModel(saveModel.Start);
            idToNodeMappings.Add(qsn.Model.Id, qsn);

            foreach (var model in saveModel.Events)
            {
                var evt = GetQuestEventNodeFromGuid(model.EventId);
                AddQuestNode(evt);
                evt.LoadModel(model);
                idToNodeMappings.Add(evt.Model.Id, evt);
            }

            foreach (var model in saveModel.Stages)
            {
                var stage = AddQuestNode<QuestStageNode>();
                stage.LoadModel(model);
                idToNodeMappings.Add(stage.Model.Id, stage);
            }

            foreach (var model in saveModel.Completions)
            {
                var completion = AddQuestNode<QuestCompleteNode>();
                completion.LoadModel(model);
                idToNodeMappings.Add(completion.Model.Id, completion);
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
            InvalidateFileDialogs();
        }

        private void Save(string path)
        {
            var saveModel = new QuestSaveModel();
            // save all node data
            foreach (var node in _graphEdit.GetChildren())
            {
                if (node is QuestNode qn)
                {
                    StoreData(saveModel, qn);
                }
            }

            // establish connections
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

                saveModel.AddRightConnection(fromModel.Id, toModel.Id);
            }
            var json = JsonConvert.SerializeObject(saveModel);
            var file = new File();
            file.OpenCompressed(path, (int) File.ModeFlags.Write, (int) File.CompressionMode.Gzip);
            file.StoreLine(json);
            file.Close();
            InvalidateFileDialogs();
        }

        private void InvalidateFileDialogs()
        {
            _saveFileDialog.Invalidate();
            _openFileDialog.Invalidate();
        }

        private void OnAddNodePressed()
        {
            _nodeSelectorDialog.PopupCenteredRatio();
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
                AddQuestNode(node);
            }
            _eventSelectorDialog.Hide();
        }

        private void OnSaveButtonPressed()
        {
            _saveFileDialog.PopupCenteredRatio();
        }

        private void OnOpenButtonPressed()
        {
            _openFileDialog.PopupCenteredRatio();
        }

        private void OnFileSelected(string path)
        {
            Load(path);
        }

        private void OnSaveFileSelected(string path)
        {
            Save(path);
        }

        private void OnNodeSelectorSelected(int idx)
        {
            _nodeSelectorDialog.Hide();
            var nodeName = _nodeSelectorDialog.GetNode<ItemList>("VBoxContainer/ItemList").GetItemText(idx);
            if (nodeName == nameof(QuestEventNode))
            {
                _eventSelectorDialog.PopupCentered();
                return;
            }
            var node = _resourcePreloader.InstanceScene<QuestNode>(nodeName);
            AddQuestNode(node);
        }
    }
}