using System.Linq;
using GameFeel.Component;
using GameFeel.UI;

namespace GameFeel.GameObject.Environment
{
    public class SignArrow : EnvironmentObject
    {
        private ZoneTransitionArea closestTransitionArea;

        public override void _Ready()
        {
            base._Ready();
            GetNode<SelectableComponent>("SelectableComponent").Connect(nameof(SelectableComponent.SelectEnter), this, nameof(OnSelectEnter));
            GetNode<SelectableComponent>("SelectableComponent").Connect(nameof(SelectableComponent.SelectLeave), this, nameof(OnSelectLeave));

            CallDeferred(nameof(SetupClosestArea));
        }

        private void SetupClosestArea()
        {
            closestTransitionArea = GameZone.Instance.ZoneTransitionAreas.OrderBy(x => x.GlobalPosition.DistanceSquaredTo(GlobalPosition)).FirstOrDefault();
        }

        private void OnSelectEnter()
        {
            if (closestTransitionArea != null)
            {
                TooltipUI.ShowItemTooltip(closestTransitionArea.ToZoneId);
            }
        }

        private void OnSelectLeave()
        {
            TooltipUI.HideTooltip();
        }
    }
}