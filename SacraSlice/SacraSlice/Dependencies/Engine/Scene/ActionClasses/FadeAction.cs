using MonoGame.Extended.Collections;
using System;

namespace SacraSlice.Dependencies.Engine.Scene.ActionClasses
{
    /// <summary>
    /// Interpolates alpha to specified value
    /// </summary>
    public class FadeAction : Action
    {
        public float duration, fade, startFade;
        public Interpolation interpolation;

        public Pool<FadeAction> pool;
        public FadeAction(Actor a, float fade, float duration, Interpolation interpolation) : base(a)
        {
            this.duration = duration; this.interpolation = interpolation;
            if (a != null)
                startFade = a.color.A;
        }

        public override bool Act(float elapsedTime)
        {
            
            timer += elapsedTime;

            a.color.A = (byte)interpolation.Apply(startFade, fade, MathF.Min(1, timer / duration));
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
