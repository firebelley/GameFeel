using GameFeel.Data.Model;
using Godot;

namespace GameFeel.DesignTool
{
    public class QuestCompleteNode : QuestNode
    {
        private QuestItemSelector _rewardItemSelector;
        private SpinBox _rewardAmountSpinBox;
        private LineEdit _idLineEdit;

        public new QuestCompleteModel Model
        {
            get
            {
                return (QuestCompleteModel) base.Model;
            }
            set
            {
                base.Model = value;
            }
        }

        public override void _Ready()
        {
            base._Ready();
            Model = new QuestCompleteModel();
            _rewardItemSelector = GetNode<QuestItemSelector>("VBoxContainer/HBoxContainer/QuestItemSelector");
            _rewardAmountSpinBox = GetNode<SpinBox>("VBoxContainer/HBoxContainer2/SpinBox");
            _idLineEdit = GetNode<LineEdit>("VBoxContainer/HBoxContainer3/LineEdit");

            _rewardItemSelector.Connect(nameof(QuestItemSelector.ItemSelected), this, nameof(OnItemSelected));
            _rewardAmountSpinBox.Connect("value_changed", this, nameof(OnValueChanged));
        }

        public override void LoadModel(QuestModel questModel)
        {
            base.LoadModel(questModel);
            Model = (QuestCompleteModel) questModel;
        }

        protected override void UpdateControls()
        {
            _rewardItemSelector.SetItemId(Model.RewardItemId);
            _rewardAmountSpinBox.Value = Model.RewardItemAmount;
            _idLineEdit.Text = Model.Id;
        }

        private void OnItemSelected(string id)
        {
            Model.RewardItemId = id;
        }

        private void OnValueChanged(float value)
        {
            Model.RewardItemAmount = (int) _rewardAmountSpinBox.Value;
        }
    }
}