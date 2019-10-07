using Godot;

namespace GameFeel.GameObject.Effect
{
    public class EntityDeath : Particles2D
    {
        public override void _Ready()
        {
            var tween = GetNode<Tween>("Tween");
            tween.InterpolateProperty(
                Material,
                "shader_param/u_percentage",
                0f,
                1f,
                .25f,
                Tween.TransitionType.Cubic,
                Tween.EaseType.In
            );
            tween.Start();
        }

        public void SetTextureFromNode(Node node)
        {
            if (node is AnimatedSprite animatedSprite)
            {
                SetTextureFromAnimatedSprite(animatedSprite);
            }
            else if (node is Sprite sprite)
            {
                SetTextureFromSprite(sprite);
            }
            else
            {
                Texture = null;
            }
        }

        private void SetTextureFromSprite(Sprite sprite)
        {
            Texture = sprite.Texture;
        }

        private void SetTextureFromAnimatedSprite(AnimatedSprite animatedSprite)
        {
            var currentAnimation = animatedSprite.Animation;
            Texture = animatedSprite.Frames.GetFrame(currentAnimation, animatedSprite.Frame);
        }
    }
}