using System.Collections.Generic;
using GameFeel.Component.Subcomponent;
using Godot;
using GodotTools.Extension;

namespace GameFeel.UI
{
    public class DialogueOptionsContainer : VBoxContainer
    {
        [Signal]
        public delegate void DialogueOptionSelected(DialogueItem dialogueItem);

        private ResourcePreloader _resourcePreloader;
        private Label _introLabel;
        private Container _container;

        public override void _Ready()
        {
            _introLabel = GetNode<Label>("IntroductionLabel");
            _container = GetNode<VBoxContainer>("VBoxContainer");
            _resourcePreloader = GetNode<ResourcePreloader>("ResourcePreloader");
        }

        public void SetIntroductionText(string text)
        {
            _introLabel.Text = text;
        }

        public void LoadOptions(List<DialogueItem> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                var button = _resourcePreloader.InstanceScene<DialogueOptionButton>();
                button.Text = items[i].Title;
                _container.AddChild(button);
                button.Connect("pressed", this, nameof(OnButtonPressed), new Godot.Collections.Array() { items[i] });

                if (i % 2 == 0)
                {
                    button.OffsetAnimation();
                }
            }
        }

        private void OnButtonPressed(DialogueItem dialogueItem)
        {
            EmitSignal(nameof(DialogueOptionSelected), dialogueItem);
        }
    }
}