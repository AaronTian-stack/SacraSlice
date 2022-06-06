using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;
using System;

namespace SacraSlice.Dependencies.Engine.Scene.ActionClasses
{
    /// <summary>
    /// Interpolates to a color
    /// </summary>
    public class ColorAction : Action
    {
        public float duration;
        public Color startColor, c;
        public Interpolation interpolation;

        public Pool<ColorAction> pool;
        public ColorAction(Actor a, Color startColor, Color c, float duration, Interpolation interpolation) : base(a)
        {
            this.duration = duration; this.interpolation = interpolation;
            this.c = c;
            this.startColor = startColor;
        }

        public override bool Act(float elapsedTime)
        {

            timer += elapsedTime;

            a.color.R = (byte)interpolation.apply(startColor.R, c.R, MathF.Min(1, timer / duration));
            a.color.G = (byte)interpolation.apply(startColor.G, c.G, MathF.Min(1, timer / duration));
            a.color.B = (byte)interpolation.apply(startColor.B, c.B, MathF.Min(1, timer / duration));
            a.color.A = (byte)interpolation.apply(startColor.A, c.A, MathF.Min(1, timer / duration));

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
