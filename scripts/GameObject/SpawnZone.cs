using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotTools.Extension;

namespace GameFeel.GameObject
{
    [Tool]
    public class SpawnZone : Node2D
    {
        private const int MAX_SPAWN_TRIES = 3;

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
        private int _totalToSpawn;
        [Export]
        private PackedScene _spawnedScene;

        private Timer _cullTimer;
        private Timer _spawnTimer;
        private Node2D _worldDetectionArea;
        private List<Node2D> _spawnedNodes = new List<Node2D>();
        private List<RayCast2D> _spawnRaycasts = new List<RayCast2D>();

        private int _currentSpawned;

        public override void _Ready()
        {
            _cullTimer = GetNode<Timer>("Timer");
            _spawnTimer = GetNode<Timer>("SpawnTimer");
            _worldDetectionArea = GetNode<Node2D>("WorldDetectionArea");
            _spawnRaycasts = _worldDetectionArea.GetChildren<RayCast2D>();

            _cullTimer.Connect("timeout", this, nameof(OnTimerTimeout));
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
            // only try to spawn x amount of times
            // can hang if using while loop due to physics collision checks
            for (int i = 0; i < _maxSpawned; i++)
            {
                Spawn();
            }
        }

        private void Spawn()
        {
            Vector2 position = Vector2.Zero;
            for (int i = 0; i < MAX_SPAWN_TRIES; i++)
            {
                position = GetPointInSpawnArea();
                _worldDetectionArea.GlobalPosition = position;

                _spawnRaycasts.ForEach(x =>
                {
                    x.ForceRaycastUpdate();
                    x.Enabled = true;
                });

                if (_spawnRaycasts.Any(x => x.IsColliding()))
                {
                    continue;
                }

                var node = _spawnedScene.Instance() as Node2D;
                GameZone.EntitiesLayer.AddChild(node);
                node.GlobalPosition = position;
                _spawnedNodes.Add(node);
                _currentSpawned++;

                break;
            }

            _spawnRaycasts.ForEach(x => x.Enabled = false);
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

        private void CheckDone()
        {
            if (_currentSpawned >= _totalToSpawn && _totalToSpawn > 0)
            {
                _spawnTimer.Stop();
                _cullTimer.Stop();
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