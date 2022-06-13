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

            eh += CheckDefending;
            ev = new EventAction(a1, eh);
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

        EventHandler eh;
        EventAction ev;
        public void CheckDefending(object sender, EventArgs e)
        {
            if (!PlayScreen.energy.defending)
            {
                PlayScreen.life.Value--;
                Color w = new Color(1, 1, 1, 0.5f);
                PlayScreen.flash.priority.AddAction(Actions.ColorAction(PlayScreen.flash.priority
                    , new Color(139 / 255f, 0 / 255f, 0 / 255f, .3f), w, 0.1f, Interpolation.smooth));
                PlayScreen.flash.priority.AddAction(Actions.ColorAction(PlayScreen.flash.priority
                    , w, new Color(0, 0, 0, 0), 0.1f, Interpolation.smooth));
            }
            else
            {
                ///Color.CornflowerBlue;
                // flash the screen, blue then white very quickly
                //Color.darkblue
                bool dont = false;
                if (PlayScreen.energy.realClick < 0.1f)
                {
                    // perfect parry
                    float scale = 0.2f;
                    int add = 2;
                    PlayScreen.score.Value += add;
                    PlayScreen.justAdded = add;
                    PlayScreen.narrator.AddMessage("Main Font", "Perfect!",
                    duration: 0.5f, delay: 0.5f, size: scale, Interpolation.swingOut, Interpolation.smooth, 0.5f, 1 + scale / 2, 0.5f, 1 - scale / 2);
                    dont = true;
                }
                //DebugLog.Print("bruh", PlayScreen.energy.realClick);

                if (!dont && PlayScreen.energy.energyTimer < 0.25f)
                {
                    float scale = 0.2f;
                    PlayScreen.narrator.AddMessage("Main Font", "Close Call!",
                    duration: 0.5f, delay: 0.5f, size: scale, Interpolation.swingOut, Interpolation.smooth, 0.5f, 1 + scale / 2, 0.5f, 1 - scale / 2);
                }
                /*else if (!dont)
                {
                    float scale = 0.2f;
                    PlayScreen.narrator.AddMessage("Main Font", "Nice Parry!",
                    duration: 0.5f, delay: 0.5f, size: scale, Interpolation.swingOut, Interpolation.smooth, 0.5f, 1 + scale / 2, 0.5f, 1 - scale / 2);
                }*/

                Color w = new Color(1, 1, 1, 0.5f);
                PlayScreen.flash.priority.AddAction(Actions.ColorAction(PlayScreen.flash.priority
                    , new Color(0 / 255f, 0 / 255f, 139 / 255f, .5f), w, 0.1f, Interpolation.smooth));
                PlayScreen.flash.priority.AddAction(Actions.ColorAction(PlayScreen.flash.priority
                    , w, new Color(0,0,0,0), 0.1f, Interpolation.smooth));

            }
        }

        bool swing;
        public void Swing()
        {
            swing = true;
            float turn = -200;
            float a = 0.4f;
            a1.AddAction(Actions.MoveFrom(a1, s.Rotation, 0, s.Rotation - turn * a, 0, 0.6f, Interpolation.smooth));
            a1.AddAction(Actions.MoveFrom(a1, s.Rotation - turn * a, 0, s.Rotation + turn, 0, 0.15f, Interpolation.smooth));
            a1.AddAction(ev);
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
