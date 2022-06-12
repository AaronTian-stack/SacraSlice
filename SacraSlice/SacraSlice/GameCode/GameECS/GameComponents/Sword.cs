using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using SacraSlice.Dependencies.Engine;
using SacraSlice.Dependencies.Engine.Scene;
using SacraSlice.Dependencies.Engine.Scene.ActionClasses;
using SacraSlice.GameCode.Screens;
using System;
using System.Collections.Generic;
using System.Text;

namespace SacraSlice.GameCode.GameECS.GameComponents
{
    public class Sword
    {
        // has a sword sprite and some methods for picking random floating position offset and rotation angle

        public Sprite s, hand;
        public Actor a = new Actor();
        public Sword(int seed)
        {
            s = new Sprite(GameContainer.atlas.FindRegion("sword"));
            hand = new Sprite(GameContainer.atlas.FindRegion("hand"));
            random = new FastRandom(seed);
            waitingTime = random.NextSingle(0, duration);


            float duration2 = 0.5f;
            float n = 1f;
            a.AddAction(new RepeatAction(a
                , new MoveFromAction(a, 0, n, 0, -n, duration2, Interpolation.smooth)
                , new MoveFromAction(a, 0, -n, 0, n, duration2, Interpolation.smooth)));

        }

        FastRandom random;
        public void Update()
        {

        }
        float waitingTime;
        float timer;
        float rotation;
        float min = 0.1f;
        float duration = 2f;
        public void ChangeAngle(float dt)
        {

            // Change rotataion after random intervals
            if (!swing)
                timer += dt;
            if (timer > waitingTime)
            {
                timer = 0;
                waitingTime = random.NextSingle(min, duration);
                rotation = random.NextSingle(-60, 90);
                //DebugLog.Print("Sword Rotate", duration);
            }
            
        }

        bool swing;
        public void Swing()
        {
            swing = true;
            float turn = -200;
            a1.AddAction(Actions.MoveFrom(a1, s.Rotation, 0, s.Rotation + turn, 0, 0.15f, Interpolation.smooth));
            a1.AddAction(Actions.MoveFrom(a1, s.Rotation + turn, 0, s.Rotation, 0, 0.3f, Interpolation.swingOut));
        }
        Actor a1 = new Actor();
        public void Draw(SpriteBatch sb, float dt, float ppm)
        {
            s.Scale *= ppm;
            hand.Scale *= ppm;

            a1.Act(dt);

            if (a1.actions.Count == 0)
                swing = false;

            if (!swing)
            {
                s.Rotation = Interpolation.linear.apply(s.Rotation, rotation, 0.11f * 120f / PlayScreen.frameRate);
                hand.Rotation = s.Rotation;
            }
            else
            {
                s.Rotation = a1.x;
                hand.Rotation = s.Rotation;
            }
           
            s.Draw(sb, 0.004f);
            hand.Draw(sb, 0.003f);
            s.Scale /= ppm;
            hand.Scale /= ppm;
        }


    }
}
