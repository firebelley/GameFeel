using System.Collections.Generic;
using Godot;

namespace GameFeel.GameObject
{
    [Tool]
    public class SpawnZone : Node2D
    {
        [Export]
        private float _radius
        {
            get
            {
                return _radiusProperty;
            }
            set
            {
                _radiusProperty = value;
                Update();
            }
        }
        private float _radiusProperty;

        [Export]
        private int _maxSpawned = 5;
        [Export]
        private PackedScene _spawnedScene;

        private Timer _timer;
        private Timer _spawnTimer;
        private List<Node2D> _spawnedNodes = new List<Node2D>();

        public override void _Ready()
        {
            _timer = GetNode<Timer>("Timer");
            _spawnTimer = GetNode<Timer>("SpawnTimer");
            _timer.Connect("timeout", this, nameof(OnTimerTimeout));
            _spawnTimer.Connect("timeout", this, nameof(OnSpawnTimerTimeout));

            if (!Engine.IsEditorHint())
            {
                CallDeferred(nameof(SpawnAll));
            }
        }

        public override void _Draw()
        {
            if (Engine.IsEditorHint())
            {
                DrawCircle(Vector2.Zero, _radius, new Color(1f, 1f, 1f, .25f));
            }
        }

        private void SpawnAll()
        {
            while (_spawnedNodes.Count < _maxSpawned)
            {
                Spawn();
            }
        }

        private void Spawn()
        {
            var node = _spawnedScene.Instance() as Node2D;
            GameWorld.EntitiesLayer.AddChild(node);
            node.GlobalPosition = GetPointInSpawnArea();
            _spawnedNodes.Add(node);
        }

        private Vector2 GetPointInSpawnArea()
        {
            var dist = Main.RNG.RandfRange(0f, _radius);
            var angle = Main.RNG.RandfRange(0f, 2f * Mathf.Pi);
            return GlobalPosition + Vector2.Right.Rotated(angle) * dist;
        }

        private void ClearAndStartSpawnTimer()
        {
            for (int i = 0; i < _spawnedNodes.Count; i++)
            {
                if (!IsInstanceValid(_spawnedNodes[i]))
                {
                    _spawnedNodes.RemoveAt(i);
                    i--;
                }
            }

            StartSpawnTimer();
        }

        private void StartSpawnTimer()
        {
            if (_spawnedNodes.Count < _maxSpawned && _spawnTimer.IsStopped())
            {
                _spawnTimer.Start();
            }
        }

        private void OnTimerTimeout()
        {
            ClearAndStartSpawnTimer();
        }

        private void OnSpawnTimerTimeout()
        {
            Spawn();
            StartSpawnTimer();
        }
    }
}