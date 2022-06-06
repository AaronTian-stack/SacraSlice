using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;
using System.Collections.Generic;

namespace SacraSlice.Dependencies.Engine.Scene
{
    public class Actor
    {
        public float x, y;
        public float scaleX = 1, scaleY = 1;
        public float rotation;
        public Color color = Color.White;

        private List<Action> actions = new List<Action>();

        public Actor() { }
        public Actor(float x, float y)
        {
            this.x = x; this.y = y;
        }

        public void Act(float elapsedTime)
        {
            if (actions.Count == 0) return;

            if (actions[0].Act(elapsedTime))
            {
                actions[0].timer = 0;
                actions.RemoveAt(0);
            }
        }

        public void AddAction(Action a)
        {
            actions.Add(a);
        }

        public void ClearActions()
        {
            foreach(Action a in actions)
            {
                a.timer = 999;
                a.Act(0);
            }
            actions.Clear();
        }
    }
}
