using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using SacraSlice.Dependencies.Engine;
using SacraSlice.Dependencies.Engine.InterfaceLayout;
using SacraSlice.Dependencies.Engine.Scene;
using SacraSlice.Dependencies.Engine.Scene.ActionClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SacraSlice.GameCode.Managers
{
    public class Narrator
    {
        public Actor actor = new Actor();
        Actor HSLA = new Actor();
        GraphicsDevice gd;
        EventHandler eh, sound;
        EventAction ev, soundAction;
        public bool rainbow;
        /// <summary>
        /// Send text here to be displayed at bottom of screen, with transitions up and down from offscreen
        /// </summary>
        public Narrator(Game game)
        {
            actor.x = -1; actor.y = -1;
            this.gd = game.GraphicsDevice;
            eh += Dequeue;
            ev = new EventAction(actor, eh);
            float v = 359f;
            HSLA.AddAction(new RepeatAction(HSLA,
                Actions.MoveFrom(HSLA, 0, 0, v, 0, 1.5f, Interpolation.linear)
                , Actions.MoveFrom(HSLA, v, 0, 0, 0, 1.5f, Interpolation.linear)
                ));
            d = new Random();
            sound += PlaySound;
            soundAction = new EventAction(actor, sound);
        }
        HSLColor hsl = new HSLColor(0, 1, 0.5f);
        int letterCounter, max;
        float counter;
        Random d;
        public void Draw(SpriteBatch sb, float dt)
        {

            actor.Act(dt);
            HSLA.Act(dt);
            string s;
            string f;
            float fl;

            Color c = Color.GhostWhite;
            hsl.H = Math.Clamp(HSLA.x, 0, 360);
            if (messages.TryPeek(out s) && fonts.TryPeek(out f) && scales.TryPeek(out fl))
            {
                if (s.Equals("Perfect!"))
                    c = hsl.ToRgbColor();
                TextDrawer.DrawTextStatic(f, s, new Vector2(actor.x, actor.y), fl, c, Color.Black, 1, 1, 1, 5);

                /*if (newString)
                {
                    //max = s.Length;
                    newString = false;
                    //letterCounter = 0;
                    
                }

                if (counter > 0.1f)
                {
                    GameContainer.sounds["Talk"].Play();
                    letterCounter += 2;
                    counter = 0;
                }
                    
                if (letterCounter < max)
                    counter += dt;*/
            }
            
            


        }
        public void PlaySound(object sender, EventArgs e)
        {
            var sp = GameContainer.sounds["Whistle"].CreateInstance();
            //sp.Pitch = d.NextSingle(0, 1f);
            sp.Volume = 0.5f;
            sp.Play();
        }

        bool newString = true;
        private void Dequeue(object sender, EventArgs e)
        {
            string s;
            messages.TryDequeue(out s);
            fonts.TryDequeue(out s);
            scales.TryDequeue(out float f);
            rainbow = false;
            newString = true;
        }

        Queue<string> messages = new Queue<string>();
        Queue<string> fonts = new Queue<string>();
        Queue<float> scales = new Queue<float>();
        /// <summary>
        /// Offsets are in percentages of the screen, with 0 being left / top and 1 being right / bottom
        /// </summary>
        public void AddMessage(string font, string message, float duration, float readDelay, float endDelay, float size, Interpolation interpolationIn, Interpolation interpolationOut,
            float startX, float startY, float endX, float endY, bool sound = true)
        {

            messages.Enqueue(message);
            fonts.Enqueue(font);
            scales.Enqueue(size);

            if(sound)
                actor.AddAction(soundAction);

            actor.AddAction(Actions.MoveFrom(actor, startX, startY, endX, endY, duration, interpolationIn));

            

            actor.AddAction(Actions.Delay(actor, readDelay));

            actor.AddAction(Actions.MoveFrom(actor, endX, endY, startX, startY, duration, interpolationOut));

            actor.AddAction(Actions.Delay(actor, endDelay));

            actor.AddAction(ev);

        }

        public void AddAction(Dependencies.Engine.Scene.Action a)
        {
            this.actor.AddAction(a);
        }
    }
}
