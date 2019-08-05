using Godot;

namespace GameFeel.Component
{
    public class HitEffectComponent : Node
    {
        [Export]
        private ShaderMaterial _shaderMaterial;
        [Export]
        private NodePath _shadedNodePath;
        [Export]
        private NodePath _damageReceiverComponentPath;

        private Node2D _shadedNode;
        private Tween _shaderTween;

        public override void _Ready()
        {
            _shadedNode = GetNode<Node2D>(_shadedNodePath);
            _shaderTween = GetNode<Tween>("ShaderTween");
            _shadedNode.Material = _shaderMaterial;
            if (_damageReceiverComponentPath != null)
            {
                GetNodeOrNull<DamageReceiverComponent>(_damageReceiverComponentPath)?.Connect(nameof(DamageReceiverComponent.DamageReceived), this, nameof(OnDamageReceived));
            }
        }

        private void PlayHitShadeTween()
        {
            _shaderTween.ResetAll();
            _shaderTween.InterpolateProperty(
                _shaderMaterial,
                "shader_param/_hitShadePercent",
                1.0f,
                0f,
                .35f,
                Tween.TransitionType.Quad,
                Tween.EaseType.In
            );
            _shaderTween.Start();
        }

        private void OnDamageReceived(float damage)
        {
            PlayHitShadeTween();
        }
    }
}