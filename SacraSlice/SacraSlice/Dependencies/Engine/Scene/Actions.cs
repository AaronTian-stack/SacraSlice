using MonoGame.Extended.Collections;
using SacraSlice.Dependencies.Engine.Scene.ActionClasses;
using System.Collections.Generic;

namespace SacraSlice.Dependencies.Engine.Scene
{
    public class Actions // TODO: pool all actions
    {
        static Pool<MoveToAction> moveToPool = new Pool<MoveToAction>(() => new MoveToAction(null, 0, 0, 0, Interpolation.linear));
        static Pool<MoveByAction> moveByPool = new Pool<MoveByAction>(() => new MoveByAction(null, 0, 0, 0, Interpolation.linear));
        static Pool<FadeAction> fadeActionPool = new Pool<FadeAction>(() => new FadeAction(null, 0, 0, Interpolation.linear));

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



    }



}
