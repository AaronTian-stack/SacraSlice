using MonoGame.Extended.Collections;
using System;

namespace SacraSlice.Dependencies.Engine.Scene.ActionClasses
{
    /// <summary>
    /// Moves actor to certain vector
    /// </summary>
    public class MoveFromAction : Action
    {
        public float startX, startY;
        public float x, y, duration;
        public Interpolation interpolation;

        public Pool<MoveFromAction> pool;
        public MoveFromAction(Actor a, float startX, float startY, float endX, float endY, float duration, Interpolation interpolation) : base(a)
        {
            this.startX = startX; this.startY = startY;
            this.x = endX; this.y = endY; this.duration = duration; this.interpolation = interpolation;
        }

        public override bool Act(float elapsedTime)
        {

            if (a.x == x && a.y == y)
                return true;

            timer += elapsedTime;

            a.x = interpolation.Apply(startX, x, MathF.Min(1, timer / duration));
            a.y = interpolation.Apply(startY, y, MathF.Min(1, timer / duration));

            if (timer >= duration)
            {
                if (pool != null && !poolOverride)
                    pool.Free(this);
                return true;
            }
            else return false;

        }

    }
}
