using GameFeel.Resource;
using GameFeel.Singleton;
using Godot;
using GodotApiTools.Extension;

namespace GameFeel.UI
{
    public class TooltipUI : Control
    {
        private const string ANIM_BOUNCE_IN = "ControlBounceIn";
        private const int TOP_OFFSET = 16;

        [Export]
        private NodePath _panelContainerPath;
        [Export]
        private NodePath _animationPlayerPath;
        [Export]
        private NodePath _nameLabelPath;
        [Export]
        private NodePath _weaponLabelPath;
        [Export]
        private NodePath _pickupLabelPath;
        [Export]
        private NodePath _talkLabelPath;

        private static TooltipUI _instance;
        private static PanelContainer _panelContainer;
        private static Label _nameLabel;
        private static AnimationPlayer _animationPlayer;
        private static Label _weaponLabel;
        private static Label _pickupLabel;
        private static Label _talkLabel;

        public override void _Ready()
        {
            _instance = this;
            this.SetNodesByDeclaredNodePaths();
            HideTooltip();
            _panelContainer.Connect("resized", this, nameof(OnPanelResized));
            GameEventDispatcher.Instance.Connect(nameof(GameEventDispatcher.EventZoneChanged), this, nameof(OnZoneChanged));
        }

        public override void _Process(float delta)
        {
            _panelContainer.RectSize = Vector2.Zero;
            var position = Cursor.GetUIPosition();
            position.x -= _panelContainer.RectSize.x / 2f;
            position.y += TOP_OFFSET;
            var bottomRight = position + _panelContainer.RectSize;

            if (position.x < 0)
            {
                position.x = 0;
            }
            if (bottomRight.x > RectSize.x)
            {
                position.x -= (bottomRight.x - RectSize.x);
            }

            if (position.y < 0)
            {
                position.y = 0;
            }
            if (bottomRight.y > RectSize.y)
            {
                position.y -= (bottomRight.y - RectSize.y);
            }

            _panelContainer.RectPosition = position;
        }

        public static void ShowItemTooltip(string itemId)
        {
            if (MetadataLoader.LootItemIdToEquipmentMetadata.ContainsKey(itemId))
            {
                ShowEquipment(MetadataLoader.LootItemIdToEquipmentMetadata[itemId]);
            }
            else if (MetadataLoader.LootItemIdToMetadata.ContainsKey(itemId))
            {
                ShowItem(MetadataLoader.LootItemIdToMetadata[itemId]);
            }
            else if (MetadataLoader.EntityIdToMetadata.ContainsKey(itemId))
            {
                ShowEntity(MetadataLoader.EntityIdToMetadata[itemId]);
            }
            else if (MetadataLoader.ZoneIdToMetadata.ContainsKey(itemId))
            {
                ShowZone(MetadataLoader.ZoneIdToMetadata[itemId]);
            }
        }

        public static void ShowInventoryItemTooltip(string itemId)
        {
            if (MetadataLoader.LootItemIdToEquipmentMetadata.ContainsKey(itemId))
            {
                var metadata = MetadataLoader.LootItemIdToEquipmentMetadata[itemId];
                _weaponLabel.Show();
                _nameLabel.Text = metadata.DisplayName;
                ShowTooltip();
            }
            else if (MetadataLoader.LootItemIdToMetadata.ContainsKey(itemId))
            {
                _nameLabel.Text = MetadataLoader.LootItemIdToMetadata[itemId].DisplayName;
                ShowTooltip();
            }
        }

        private static void ShowEquipment(MetadataLoader.EquipmentMetadata metadata)
        {
            _weaponLabel.Show();
            _pickupLabel.Show();
            _nameLabel.Text = metadata.DisplayName;
            ShowTooltip();
        }

        private static void ShowItem(MetadataLoader.Metadata metadata)
        {
            _nameLabel.Text = metadata.DisplayName;
            _pickupLabel.Show();
            ShowTooltip();
        }

        private static void ShowEntity(MetadataLoader.Metadata metadata)
        {
            _nameLabel.Text = metadata.DisplayName;
            _talkLabel.Show();
            ShowTooltip();
        }

        private static void ShowZone(MetadataLoader.Metadata metadata)
        {
            _nameLabel.Text = "To " + metadata.DisplayName;
            ShowTooltip();
        }

        public static void HideTooltip()
        {
            _weaponLabel.Hide();
            _talkLabel.Hide();
            _pickupLabel.Hide();
            _panelContainer.Hide();
            _instance.SetProcess(false);
        }

        private static void ShowTooltip()
        {
            _panelContainer.Show();
            _instance.SetProcess(true);
            if (_animationPlayer.IsPlaying())
            {
                _animationPlayer.Seek(0f, true);
            }
            _animationPlayer.PlaybackSpeed = 1f;
            _animationPlayer.Play(ANIM_BOUNCE_IN);
        }

        private void OnPanelResized()
        {
            _panelContainer.RectPivotOffset = _panelContainer.RectSize / 2f;
        }

        private void OnZoneChanged(string eventGuid, string zoneId)
        {
            HideTooltip();
        }
    }
}