using MonoGame.Extended.Collections;
using System;

namespace SacraSlice.Dependencies.Engine.Scene.ActionClasses
{
    /// <summary>
    /// Moves actor to certain vector
    /// </summary>
    public class MoveToAction : Action
    {
        public float startX, startY;
        public float x, y, duration;
        public Interpolation interpolation;

        public Pool<MoveToAction> pool;
        public MoveToAction(Actor a, float x, float y, float duration, Interpolation interpolation) : base(a)
        {
            if (a != null)
            {
                //startX = a.x;
                //startY = a.y;
            }
            this.x = x; this.y = y; this.duration = duration; this.interpolation = interpolation;
        }
        bool start;
        public override bool Act(float elapsedTime)
        {

            if (!start)
            {
                startX = a.x; startY = a.y;
                start = true;
            }

            if (a.x == x && a.y == y)
                return true;

            timer += elapsedTime;

            a.x = interpolation.apply(startX, x, MathF.Min(1, timer / duration));
            a.y = interpolation.apply(startY, y, MathF.Min(1, timer / duration));

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
