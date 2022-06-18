using MonoGame.Extended.Collections;

namespace SacraSlice.Dependencies.Engine.Scene.ActionClasses
{
    /// <summary>
    /// Waits specified amount of time
    /// </summary>
    public class DelayAction : Action
    {
        public float duration;

        public Pool<DelayAction> pool;
        public DelayAction(Actor a, float duration) : base(a)
        {
            this.duration = duration;
        }

        public override bool Act(float elapsedTime)
        {
           
            timer += elapsedTime;
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
