using GameFeel.Singleton;

namespace GameFeel.DesignTool
{
    public class QuestEventEntityEngaged : QuestEventNode
    {
        private QuestItemSelector _itemSelector;

        public override void _Ready()
        {
            base._Ready();
            Model.EventId = GameEventDispatcher.ENTITY_ENGAGED;
            _itemSelector = GetNode<QuestItemSelector>("VBoxContainer/HBoxContainer/QuestItemSelector");
            _itemSelector.Connect(nameof(QuestItemSelector.ItemSelected), this, nameof(OnItemSelected));
        }

        protected override void UpdateControls()
        {
            base.UpdateControls();
            _itemSelector.SetItemId(Model.ItemId);
        }

        private void OnItemSelected(string id)
        {
            Model.ItemId = id;
        }
    }
}