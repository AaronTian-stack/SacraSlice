using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;
using SacraSlice.Dependencies.Engine.Scene.ActionClasses;
using System.Collections.Generic;

namespace SacraSlice.Dependencies.Engine.Scene
{
    public class Actions // TODO: pool all actions
    {
        static Pool<MoveToAction> moveToPool = new Pool<MoveToAction>(() => new MoveToAction(null, 0, 0, 0, Interpolation.linear));
        static Pool<MoveFromAction> moveFromPool = new Pool<MoveFromAction>(() => new MoveFromAction(null, 0, 0, 0, 0, 0, Interpolation.linear));
        static Pool<MoveByAction> moveByPool = new Pool<MoveByAction>(() => new MoveByAction(null, 0, 0, 0, Interpolation.linear));
        static Pool<FadeAction> fadeActionPool = new Pool<FadeAction>(() => new FadeAction(null, 0, 0, Interpolation.linear));
        static Pool<DelayAction> delayPool = new Pool<DelayAction>(() => new DelayAction(null, 0));
        static Pool<ColorAction> colorPool = new Pool<ColorAction>(() => new ColorAction(null, new Color(), new Color(), 0, Interpolation.linear));
        public static MoveToAction MoveTo(Actor a, float x, float y, float duration, Interpolation interpolation)
        {
            MoveToAction moveTo = moveToPool.Obtain();
            moveTo.pool = moveToPool;

            moveTo.a = a;
            moveTo.startX = a.x;
            moveTo.startY = a.y;

            moveTo.x = x;
            moveTo.y = y;
            moveTo.duration = duration;
            moveTo.interpolation = interpolation;

            return moveTo;
        }

        public static MoveFromAction MoveFrom(Actor a, float startX, float startY, float x, float y, float duration, Interpolation interpolation)
        {
            MoveFromAction m = moveFromPool.Obtain();
            m.pool = moveFromPool;

            m.a = a;

            m.startX = startX;
            m.startY = startY;

            m.x = x;
            m.y = y;
            m.duration = duration;
            m.interpolation = interpolation;

            return m;
        }

        public static MoveByAction MoveBy(Actor a, float x, float y, float duration, Interpolation interpolation)
        {
            MoveByAction moveTo = moveByPool.Obtain();
            moveTo.pool = moveByPool;

            moveTo.a = a;
            //moveTo.startX = a.x;
            //moveTo.startY = a.y;

            moveTo.x = x;
            moveTo.y = y;
            moveTo.duration = duration;
            moveTo.interpolation = interpolation;

            return moveTo;
        }

        public static FadeAction Fade(Actor a, float fade, float duration, Interpolation interpolation)
        {
            FadeAction fader = fadeActionPool.Obtain();
            fader.pool = fadeActionPool;

            fader.a = a;
            fader.fade = fade;
            fader.startFade = a.color.A;
            fader.duration = duration;
            fader.interpolation = interpolation;

            return fader;
        }

        public static FadeAction FadeIn(Actor a, float duration, Interpolation interpolation)
        {
            return Fade(a, 1, duration, interpolation);
        }

        public static FadeAction FadeOut(Actor a, float duration, Interpolation interpolation)
        {
            return Fade(a, 0, duration, interpolation);
        }

        public static DelayAction Delay(Actor a, float duration)
        {
            var d = delayPool.Obtain();
           
            d.a = a;

            d.duration = duration;

            return d;
        }

        public static ColorAction ColorAction(Actor a, Color startColor, Color color, float duration, Interpolation i)
        {
            var d = colorPool.Obtain();
            d.a = a;
            d.startColor = startColor;
            d.c = color;
            d.duration = duration;
            d.interpolation = i;
            return d;
        }



    }



}
