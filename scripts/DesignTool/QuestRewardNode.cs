using GameFeel.Data.Model;
using Godot;

namespace GameFeel.DesignTool
{
    public class QuestRewardNode : QuestNode
    {
        public new QuestRewardModel Model
        {
            get
            {
                return (QuestRewardModel) base.Model;
            }
            set
            {
                base.Model = value;
            }
        }

        private QuestItemSelector _itemSelector;
        private SpinBox _amountSpinBox;
        private LineEdit _idLineEdit;

        public override void _Ready()
        {
            base._Ready();
            Model = new QuestRewardModel();
            _itemSelector = GetNode<QuestItemSelector>("VBoxContainer/QuestItemSelector");
            _amountSpinBox = GetNode<SpinBox>("VBoxContainer/HBoxContainer/SpinBox");
            _idLineEdit = GetNode<LineEdit>("VBoxContainer/LineEdit");

            _itemSelector.Connect(nameof(QuestItemSelector.ItemSelected), this, nameof(OnItemSelected));
            _amountSpinBox.Connect("value_changed", this, nameof(OnValueChanged));
        }

        public override void LoadModel(QuestModel questModel)
        {
            base.LoadModel(questModel);
            Model = (QuestRewardModel) questModel;
        }

        protected override void UpdateControls()
        {
            _itemSelector.SetItemId(Model.ItemId);
            _amountSpinBox.Value = Model.Amount;
            _idLineEdit.Text = Model.Id;
        }

        private void OnItemSelected(string id)
        {
            Model.ItemId = id;
        }

        private void OnValueChanged(float value)
        {
            Model.Amount = (int) _amountSpinBox.Value;
        }
    }
}