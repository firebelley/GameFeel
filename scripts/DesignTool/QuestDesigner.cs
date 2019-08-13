using Godot;
using GodotTools.Extension;

namespace GameFeel.DesignTool
{
    public class QuestDesigner : Control
    {
        private GraphEdit _graphEdit;
        private ResourcePreloader _resourcePreloader;

        public override void _Ready()
        {
            GetTree().SetScreenStretch(SceneTree.StretchMode.Disabled, SceneTree.StretchAspect.Ignore, new Vector2(1920, 1080));
            _graphEdit = GetNode<GraphEdit>("GraphEdit");
            _resourcePreloader = GetNode<ResourcePreloader>("ResourcePreloader");
            GetNode("HBoxContainer/AddStageNode").Connect("pressed", this, nameof(OnAddStageNodePressed));
            GetNode("HBoxContainer/AddStartNode").Connect("pressed", this, nameof(OnAddStartNodePressed));

            _graphEdit.Connect("connection_request", this, nameof(OnConnectionRequest));
            _graphEdit.Connect("disconnection_request", this, nameof(OnDisconnectRequest));
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
    }
}