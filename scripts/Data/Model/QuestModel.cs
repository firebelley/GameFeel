using System;
using Godot;

namespace GameFeel.Data.Model
{
    public class QuestModel
    {
        public string Id;
        public string DisplayName;
        public Vector2 NodePosition;

        public QuestModel()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}