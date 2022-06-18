using MonoGame.Extended.Collections;
using System;

namespace SacraSlice.Dependencies.Engine.Scene.ActionClasses
{
    /// <summary>
    /// Moves actor by certain vector
    /// </summary>
    public class MoveByAction : Action
    {
        public float x, y, duration;
        public Interpolation interpolation;

        public Pool<MoveByAction> pool;
        public MoveByAction(Actor a, float x, float y, float duration, Interpolation interpolation) : base(a)
        {
            this.x = x; this.y = y; this.duration = duration; this.interpolation = interpolation;
        }

        public override bool Act(float elapsedTime)
        {
            
            timer += elapsedTime;

            float percent = interpolation.Apply(0, 1, MathF.Min(1, timer / elapsedTime));

            a.x += x * percent;
            a.y += y * percent;
            if (timer >= duration)
            {
                if (pool != null && !poolOverride)
                    pool.Free(this);
                return true;
            }
            return false;

        }

    }
}
