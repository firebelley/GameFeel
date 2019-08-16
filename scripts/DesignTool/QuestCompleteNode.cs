using System;
using GameFeel.Data.Model;
using Godot;

namespace GameFeel.DesignTool
{
    public class QuestCompleteNode : QuestNode
    {
        private LineEdit _rewardLineEdit;
        private SpinBox _rewardAmountSpinBox;

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
            _rewardLineEdit = GetNode<LineEdit>("VBoxContainer/HBoxContainer/LineEdit");
            _rewardAmountSpinBox = GetNode<SpinBox>("VBoxContainer/HBoxContainer2/SpinBox");

            _rewardLineEdit.Connect("text_changed", this, nameof(OnTextChanged));
            _rewardAmountSpinBox.Connect("value_changed", this, nameof(OnValueChanged));
        }

        public override void LoadModel(QuestModel questModel)
        {
            base.LoadModel(questModel);
            Model = (QuestCompleteModel) questModel;
        }

        protected override void UpdateControls()
        {
            _rewardLineEdit.Text = Model.RewardItemId;
            _rewardAmountSpinBox.Value = Model.RewardItemAmount;
        }

        private void OnTextChanged(string text)
        {
            Model.RewardItemId = _rewardLineEdit.Text;
        }

        private void OnValueChanged(float value)
        {
            Model.RewardItemAmount = (int) _rewardAmountSpinBox.Value;
        }
    }
}