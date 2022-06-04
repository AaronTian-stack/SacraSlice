using System;

namespace SacraSlice.Dependencies.Engine
{
    public abstract class Interpolation
    {
        abstract public float apply(float a);

        float roundingError = 0.000001f;

        public float apply(float start, float end, float a)
        {
            return start + (end - start) * apply(a);
        }
        internal class Linear : Interpolation
        {
            public override float apply(float a)
            {
                return a;
            }
        }
        static public readonly Interpolation linear = new Interpolation.Linear();
        internal class Smooth : Interpolation
        {
            public override float apply(float a)
            {
                return a * a * (3 - 2 * a);
            }
        }
        static public readonly Interpolation smooth = new Interpolation.Smooth();
        internal class Smooth2 : Interpolation
        {
            public override float apply(float a)
            {
                a = a * a * (3 - 2 * a);
                return a * a * (3 - 2 * a);
            }
        }
        static public readonly Interpolation smooth2 = new Interpolation.Smooth2();
        internal class Smoother : Interpolation
        {
            public override float apply(float a)
            {
                return a * a * a * (a * (a * 6 - 15) + 10);
            }
        }

        static public readonly Interpolation smoother = new Interpolation.Smoother();

        static public readonly Interpolation pow2 = new Pow(2);
        static public readonly PowIn pow2In = new PowIn(2);
        static public readonly PowIn slowFast = pow2In;
        static public readonly PowOut pow2Out = new PowOut(2);
        static public readonly PowOut fastSlow = pow2Out;

        internal class Pow2InInverse : Interpolation
        {
            public override float apply(float a)
            {
                if (a < roundingError) return 0;
                return MathF.Sqrt(a);
            }
        }

        static public readonly Interpolation pow2InInverse = new Interpolation.Pow2InInverse();

        internal class Pow2OutInverse : Interpolation
        {
            public override float apply(float a)
            {
                if (a < roundingError) return 0;
                if (a > 1) return 1;
                return 1 - MathF.Sqrt(-(a - 1));
            }
        }

        static public readonly Interpolation pow2OutInverse = new Interpolation.Pow2OutInverse();

        static public readonly Pow pow3 = new Pow(3);
        static public readonly PowIn pow3In = new PowIn(3);
        static public readonly PowOut pow3Out = new PowOut(3);

        internal class Pow3InInverse : Interpolation
        {
            public override float apply(float a)
            {
                return MathF.Cbrt(a);
            }
        }
        static public readonly Interpolation pow3InInverse = new Interpolation.Pow3InInverse();

        internal class Pow3OutInverse : Interpolation
        {
            public override float apply(float a)
            {
                return 1 - MathF.Cbrt(-(a - 1));
            }
        }

        static public readonly Interpolation pow3OutInverse = new Interpolation.Pow3OutInverse();

        static public readonly Pow pow4 = new Pow(4);
        static public readonly PowIn pow4In = new PowIn(4);
        static public readonly PowOut pow4Out = new PowOut(4);

        static public readonly Pow pow5 = new Pow(5);
        static public readonly PowIn pow5In = new PowIn(5);
        static public readonly PowOut pow5Out = new PowOut(5);

        internal class Sine : Interpolation
        {
            public override float apply(float a)
            {
                return (1 - MathF.Cos(a * MathF.PI)) / 2;
            }
        }

        static public readonly Interpolation sine = new Interpolation.Sine();
        internal class SineIn : Interpolation
        {
            public override float apply(float a)
            {
                return 1 - MathF.Cos(a * MathF.PI / 2);
            }
        }

        static public readonly Interpolation sineIn = new Interpolation.SineIn();
        internal class SineOut : Interpolation
        {
            public override float apply(float a)
            {
                return MathF.Sin(a * MathF.PI / 2);
            }
        }

        static public readonly Interpolation sineOut = new Interpolation.SineOut();

        static public readonly Exp exp10 = new Exp(2, 10);
        static public readonly ExpIn exp10In = new ExpIn(2, 10);
        static public readonly ExpOut exp10Out = new ExpOut(2, 10);

        static public readonly Exp exp5 = new Exp(2, 5);
        static public readonly ExpIn exp5In = new ExpIn(2, 5);
        static public readonly ExpOut exp5Out = new ExpOut(2, 5);

        internal class Circle : Interpolation
        {
            public override float apply(float a)
            {
                if (a <= 0.5f)
                {
                    a *= 2;
                    return (1 - MathF.Sqrt(1 - a * a)) / 2;
                }
                a--;
                a *= 2;
                return (MathF.Sqrt(1 - a * a) + 1) / 2;
            }
        }

        static public readonly Interpolation circle = new Interpolation.Circle();
        internal class CircleIn : Interpolation
        {
            public override float apply(float a)
            {
                return 1 - MathF.Sqrt(1 - a * a);
            }
        }

        static public readonly Interpolation circleIn = new Interpolation.CircleIn();

        internal class CircleOut : Interpolation
        {
            public override float apply(float a)
            {
                a--;
                return MathF.Sqrt(1 - a * a);
            }
        }

        static public readonly Interpolation circleOut = new Interpolation.CircleOut();

        static public readonly Elastic elastic = new Elastic(2, 10, 7, 1);
        static public readonly ElasticIn elasticIn = new ElasticIn(2, 10, 6, 1);
        static public readonly ElasticOut elasticOut = new ElasticOut(2, 10, 7, 1);

        static public readonly Swing swing = new Swing(1.5f);
        static public readonly SwingIn swingIn = new SwingIn(2f);
        static public readonly SwingOut swingOut = new SwingOut(2f);

        static public readonly Bounce bounce = new Bounce(4);
        static public readonly BounceIn bounceIn = new BounceIn(4);
        static public readonly BounceOut bounceOut = new BounceOut(4);

        public class Exp : Interpolation
        {
            public readonly float value, power, min, scale;
            public Exp(float value, float power)
            {
                this.value = value;
                this.power = power;
                min = MathF.Pow(value, -power);
                scale = 1 / (1 - min);
            }

            public override float apply(float a)
            {
                if (a <= 0.5f) return (MathF.Pow(value, power * (a * 2 - 1)) - min) * scale / 2;
                return (2 - (MathF.Pow(value, -power * (a * 2 - 1)) - min) * scale) / 2;
            }
        }

        public class ExpIn : Exp
        {
            public ExpIn(float value, float power) : base(value, power) { }
            public override float apply(float a)
            {
                return (MathF.Pow(value, power * (a - 1)) - min) * scale;
            }
        }

        public class ExpOut : Exp
        {
            public ExpOut(float value, float power) : base(value, power) { }
            public override float apply(float a)
            {
                return 1 - (MathF.Pow(value, -power * a) - min) * scale;
            }
        }

        public class Elastic : Interpolation
        {
            public readonly float value, power, scale, bounces;

            public Elastic(float value, float power, int bounces, float scale)
            {
                this.value = value;
                this.power = power;
                this.scale = scale;
                this.bounces = bounces * MathF.PI * (bounces % 2 == 0 ? 1 : -1);
            }

            public override float apply(float a)
            {
                if (a <= 0.5f)
                {
                    a *= 2;
                    return MathF.Pow(value, power * (a - 1)) * MathF.Sin(a * bounces) * scale / 2;
                }
                a = 1 - a;
                a *= 2;
                return 1 - MathF.Pow(value, power * (a - 1)) * MathF.Sin((a) * bounces) * scale / 2;
            }
        }

        public class ElasticIn : Elastic
        {
            public ElasticIn(float value, float power, int bounces, float scale) : base(value, power, bounces, scale) { }
            public override float apply(float a)
            {
                if (a >= 0.99) return 1;
                return MathF.Pow(value, power * (a - 1)) * MathF.Sin(a * bounces) * scale;
            }
        }

        public class ElasticOut : Elastic
        {
            public ElasticOut(float value, float power, int bounces, float scale) : base(value, power, bounces, scale) { }
            public override float apply(float a)
            {
                if (a == 0) return 0;
                a = 1 - a;
                return (1 - MathF.Pow(value, power * (a - 1)) * MathF.Sin(a * bounces) * scale);
            }
        }

        public class Bounce : BounceOut
        {
            public Bounce(float[] widths, float[] heights) : base(widths, heights) { }

            public Bounce(int bounces) : base(bounces) { }

            private float output(float a)
            {
                float test = a + widths[0] / 2;
                if (test < widths[0]) return test / (widths[0] / 2) - 1;
                return base.apply(a);
            }

            public override float apply(float a)
            {
                if (a <= 0.5f) return (1 - output(1 - a * 2)) / 2;
                return output(a * 2 - 1) / 2 + 0.5f;
            }
        }

        public class BounceOut : Interpolation
        {
            public readonly float[] widths, heights;

            public BounceOut(float[] widths, float[] heights)
            {
                if (widths.Length != heights.Length)
                    throw new ArgumentException("Must be the same number of widths and heights.");
                this.widths = widths;
                this.heights = heights;
            }

            public BounceOut(int bounces)
            {
                if (bounces < 2 || bounces > 5) throw new ArgumentException("bounces cannot be < 2 or > 5: " + bounces);
                widths = new float[bounces];
                heights = new float[bounces];
                heights[0] = 1;
                switch (bounces)
                {
                    case 2:
                        widths[0] = 0.6f;
                        widths[1] = 0.4f;
                        heights[1] = 0.33f;
                        break;
                    case 3:
                        widths[0] = 0.4f;
                        widths[1] = 0.4f;
                        widths[2] = 0.2f;
                        heights[1] = 0.33f;
                        heights[2] = 0.1f;
                        break;
                    case 4:
                        widths[0] = 0.34f;
                        widths[1] = 0.34f;
                        widths[2] = 0.2f;
                        widths[3] = 0.15f;
                        heights[1] = 0.26f;
                        heights[2] = 0.11f;
                        heights[3] = 0.03f;
                        break;
                    case 5:
                        widths[0] = 0.3f;
                        widths[1] = 0.3f;
                        widths[2] = 0.2f;
                        widths[3] = 0.1f;
                        widths[4] = 0.1f;
                        heights[1] = 0.45f;
                        heights[2] = 0.3f;
                        heights[3] = 0.15f;
                        heights[4] = 0.06f;
                        break;
                }
                widths[0] *= 2;
            }

            public override float apply(float a)
            {
                if (a == 1) return 1;
                a += widths[0] / 2;
                float width = 0, height = 0;
                for (int i = 0, n = widths.Length; i < n; i++)
                {
                    width = widths[i];
                    if (a <= width)
                    {
                        height = heights[i];
                        break;
                    }
                    a -= width;
                }
                a /= width;
                float z = 4 / width * height * a;
                return 1 - (z - z * a) * width;
            }
        }

        public class BounceIn : BounceOut
        {

            public BounceIn(float[] widths, float[] heights) : base(widths, heights) { }
            public BounceIn(int bounces) : base(bounces) { }
            public override float apply(float a)
            {
                return 1 - base.apply(1 - a);
            }
        }

        public class Swing : Interpolation
        {

            private readonly float scale;

            public Swing(float scale)
            {
                this.scale = scale * 2;
            }

            public override float apply(float a)
            {
                if (a <= 0.5f)
                {
                    a *= 2;
                    return a * a * ((scale + 1) * a - scale) / 2;
                }
                a--;
                a *= 2;
                return a * a * ((scale + 1) * a + scale) / 2 + 1;
            }
        }

        public class SwingOut : Interpolation
        {

            private readonly float scale;

            public SwingOut(float scale)
            {
                this.scale = scale;
            }

            public override float apply(float a)
            {
                a--;
                return a * a * ((scale + 1) * a + scale) + 1;
            }
        }

        public class SwingIn : Interpolation
        {

            private readonly float scale;

            public SwingIn(float scale)
            {
                this.scale = scale;
            }

            public override float apply(float a)
            {
                return a * a * ((scale + 1) * a - scale);
            }
        }

    } // stop writing interpolation classes
    public class Pow : Interpolation
    {
        public readonly int power;
        public Pow(int power)
        {
            this.power = power;
        }

        public override float apply(float a)
        {
            if (a <= 0.5f) return MathF.Pow(a * 2, power) / 2;
            return MathF.Pow((a - 1) * 2, power) / (power % 2 == 0 ? -2 : 2) + 1;
        }
    }

    public class PowIn : Pow
    {
        public PowIn(int power) : base(power) { }
        public override float apply(float a)
        {
            return MathF.Pow(a, power);
        }
    }

    public class PowOut : Pow
    {
        public PowOut(int power) : base(power) { }
        public override float apply(float a)
        {
            return MathF.Pow(a - 1, power) * (power % 2 == 0 ? -1 : 1) + 1;
        }
    }
}
