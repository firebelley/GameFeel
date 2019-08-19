using System.Collections.Generic;
using GameFeel.Component;
using GameFeel.GameObject.Loot;
using Godot;
using GodotTools.Extension;
using GodotTools.Util;

namespace GameFeel.Singleton
{
    public class MetadataLoader : Node
    {
        public static Dictionary<string, ResourceInfo> LootItemIdToInfo = new Dictionary<string, ResourceInfo>();
        public static Dictionary<string, ResourceInfo> EntityIdToInfo = new Dictionary<string, ResourceInfo>();

        public struct ResourceInfo
        {
            public string Id { get; private set; }
            public string DisplayName { get; private set; }
            public string ResourcePath { get; private set; }

            public ResourceInfo(string id, string displayName, string resourcePath)
            {
                Id = id;
                DisplayName = displayName;
                ResourcePath = resourcePath;
            }
        }

        private delegate void FullPathLoader(string fullPath);

        public override void _Ready()
        {
            LoadMetadata();
        }

        private void LoadMetadata()
        {
            LoadScenesInDir("res://scenes/GameObject/Loot/", LoadItem);
            LoadScenesInDir("res://scenes/GameObject/", LoadEntity);
        }

        private void LoadScenesInDir(string dirPath, FullPathLoader fullPathLoader)
        {
            var dir = new Directory();
            var err = dir.Open(dirPath);
            if (err != Error.Ok)
            {
                Logger.Error("Could not load items code " + (int) err);
                return;
            }

            dir.ListDirBegin();

            while (true)
            {
                var path = dir.GetNext();
                if (string.IsNullOrEmpty(path))
                {
                    break;
                }

                if (path.EndsWith(".tscn"))
                {
                    fullPathLoader(dirPath + path);
                }
            }

            dir.ListDirEnd();
        }

        private void LoadItem(string fullPath)
        {
            var node = GD.Load<PackedScene>(fullPath).Instance();
            if (node is LootItem li && li.Id != "Null")
            {
                var info = new ResourceInfo(li.Id, li.DisplayName, fullPath);
                LootItemIdToInfo[li.Id] = info;
            }
            node.QueueFree();
        }

        private void LoadEntity(string fullPath)
        {
            // TODO: use an entity data component to store data about an entity
            var node = GD.Load<PackedScene>(fullPath).Instance();
            var entityId = node.GetFirstNodeOfType<DeathEffectComponent>()?.EntityId ?? string.Empty;
            if (!string.IsNullOrEmpty(entityId))
            {
                var info = new ResourceInfo(entityId, node.GetName(), fullPath);
                EntityIdToInfo[entityId] = info;
            }
            node.QueueFree();
        }
    }
}