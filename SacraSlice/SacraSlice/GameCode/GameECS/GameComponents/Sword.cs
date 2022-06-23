using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collections;
using SacraSlice.Dependencies.Engine;
using SacraSlice.Dependencies.Engine.ECS.Component;
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

        public Sprite s = new Sprite(GameContainer.atlas.FindRegion("sword"));
        public Sprite hand = new Sprite(GameContainer.atlas.FindRegion("hand"));
        public Actor a = new Actor();
        Timer t;
        public Sword(int seed, Timer t)
        {
            this.t = t;
            random = new FastRandom(seed);
            waitingTime = random.NextSingle(0, duration);
            t.GetSwitch("have sword").Value = true;

            float duration2 = 0.5f;
            float n = 1f;
            a.AddAction(new RepeatAction(a
                , new MoveFromAction(a, 0, n, 0, -n, duration2, Interpolation.smooth)
                , new MoveFromAction(a, 0, -n, 0, n, duration2, Interpolation.smooth)));

            eh += CheckDefending;
            ev = new EventAction(a1, eh);
            sound += PlaySound;
            soundEvent = new EventAction(a1, sound);
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
      

        EventHandler eh, sound;
        EventAction ev, soundEvent;
        public void CheckDefending(object sender, EventArgs e)
        {
            t.GetSwitch("Attacking").Value = false;
            if (t.GetSwitch("Shrink") || t.GetSwitch("RESET"))
                return;

            
            if (!PlayScreen.energy.defending)
            {
                DebugLog.Print(this, "LOST HEALTH");
                PlayScreen.life.Value--;
                Color w = new Color(1, 1, 1, 0.5f);
                PlayScreen.flash.priority.AddAction(Actions.ColorAction(PlayScreen.flash.priority
                    , new Color(139, 0, 0, .3f), w, 0.1f, Interpolation.smooth));
                PlayScreen.flash.priority.AddAction(Actions.ColorAction(PlayScreen.flash.priority
                    , w, new Color(0, 0, 0, 0), 0.1f, Interpolation.smooth));
                if (random.Next(0, 2) == 1)
                    GameContainer.sounds["HeavyImpact"].Play();
                else
                    GameContainer.sounds["PunchHard"].Play();

                GameContainer.sounds["PlayerHurt"].Play();
            }
            else
            {
                GameContainer.sounds["Shield"].Play();

                ///Color.CornflowerBlue;
                // flash the screen, blue then white very quickly
                //Color.darkblue
                bool dont = false;

                //DebugLog.Print("Energy", PlayScreen.energy.realClick);

                if (PlayScreen.energy.realClick < 0.15f)
                {
                    GameContainer.sounds["Perfect"].Play();
                    // perfect parry
                    PlayScreen.narrator.rainbow = true;
                    float scale = 0.2f;
                    int add = 2;
                    PlayScreen.score.Value += add;
                    PlayScreen.justAdded = add;
                    PlayScreen.narrator.AddMessage("Main Font", "Perfect!",
                    duration: 0.5f, readDelay: 0.5f, endDelay: 0, size: scale, 
                    Interpolation.swingOut, Interpolation.smooth, 0.5f, 1 + scale / 2, 0.5f, 1 - scale / 2
                    , sound: false);
                    dont = true;
                }
                //DebugLog.Print("bruh", PlayScreen.energy.realClick);

                if (!dont && PlayScreen.energy.energyTimer < 0.25f)
                {
                    float scale = 0.2f;
                    PlayScreen.narrator.AddMessage("Main Font", "Close Call!",
                    duration: 0.5f, readDelay: 0.5f, endDelay: 0, size: scale, 
                    Interpolation.swingOut, Interpolation.smooth, 0.5f, 1 + scale / 2, 0.5f, 1 - scale / 2
                    , sound: false);
                }
                /*else if (!dont)
                {
                    float scale = 0.2f;
                    PlayScreen.narrator.AddMessage("Main Font", "Nice Parry!",
                    duration: 0.5f, delay: 0.5f, size: scale, Interpolation.swingOut, Interpolation.smooth, 0.5f, 1 + scale / 2, 0.5f, 1 - scale / 2);
                }*/

                Color w = new Color(1, 1, 1, 0.5f);
                PlayScreen.flash.priority.AddAction(Actions.ColorAction(PlayScreen.flash.priority
                    , new Color(0, 0, 139, .5f), w, 0.1f, Interpolation.smooth));
                PlayScreen.flash.priority.AddAction(Actions.ColorAction(PlayScreen.flash.priority
                    , w, new Color(0,0,0,0), 0.1f, Interpolation.smooth));

            }
        }

        bool swing;

        public void PlaySound(object sender, EventArgs e)
        {
            GameContainer.sounds["SwordStrike"].Play();
        }

        public void Reset()
        {
            a1.actions.Clear();
        }
        public void Swing(float attackTime)
        {
            if (a1.actions.Count != 0 || swing)
                return;
            swing = true;
            float turn = -200;
            float a = 0.4f;
            //DebugLog.Print("Sword", "Swung");
            
            a1.AddAction(Actions.MoveFrom(a1, s.Rotation, 0, s.Rotation - turn * a, 0, attackTime, Interpolation.smooth));
            a1.AddAction(soundEvent);
            a1.AddAction(Actions.MoveFrom(a1, s.Rotation - turn * a, 0, s.Rotation + turn, 0, 0.2f, Interpolation.smooth));
            a1.AddAction(ev);
            a1.AddAction(Actions.MoveFrom(a1, s.Rotation + turn, 0, s.Rotation, 0, 0.3f, Interpolation.swingOut));
        }
        Actor a1 = new Actor();

        Bag<Vector2> rotations = new Bag<Vector2>();
        float rotationTimer;

        //float saveRotation;
        public void Draw(SpriteBatch sb, float dt, float ppm)
        {

            s.Scale *= ppm;
            hand.Scale *= ppm;

            a1.Act(dt);

            if (a1.actions.Count == 0)
                swing = false;

            if (!swing)
            {
                s.Rotation = Interpolation.linear.Apply(s.Rotation, rotation, 0.11f * dt * 120f);
                rotations.Clear();
            }
            else
            {
                
                rotationTimer += dt;
                if (rotationTimer > 0.05f)
                {
                    rotationTimer = 0;
                    while (rotations.Count > 1)
                        rotations.RemoveAt(0);
                    rotations.Add(new Vector2(s.Rotation, 1));
                }

                for (int i = 0; i < rotations.Count; i++)
                {
                    var r = rotations[i];
                    s.Rotation = r.X;
                    s.Color.A = (byte)((i * 1f / rotations.Count) * 255f);
                    s.Draw(sb, 0.004f + i * 0.000001f);
                    r.Y -= 255f * dt * 120f;
                }
                s.Rotation = a1.x;
            }

          
            s.Color.A = 255;
            s.Draw(sb, 0.004f);
            hand.Rotation = s.Rotation;
            if (t.GetSwitch("pien"))
                hand.Textureregion = GameContainer.atlas.FindRegion("handYellow");
            else
                hand.Textureregion = GameContainer.atlas.FindRegion("hand");
            hand.Draw(sb, 0.003f);
            s.Scale /= ppm;
            hand.Scale /= ppm;
        }


    }
}
