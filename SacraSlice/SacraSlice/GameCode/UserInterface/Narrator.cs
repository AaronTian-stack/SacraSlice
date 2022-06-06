using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        Actor a = new Actor();
        GraphicsDevice gd;
        EventHandler eh;
        EventAction ev;
        /// <summary>
        /// Send text here to be displayed at bottom of screen, with transitions up and down from offscreen
        /// </summary>
        public Narrator(Game game)
        {
            this.gd = game.GraphicsDevice;
            eh += Dequeue;
            ev = new EventAction(a, eh);
        }

        public void Draw(SpriteBatch sb, float dt)
        {
            a.Act(dt);
            string s;
            string f;
            float fl;
            if (messages.TryPeek(out s) && fonts.TryPeek(out f) && scales.TryPeek(out fl))
                TextDrawer.DrawTextStatic(f, s, new Vector2(a.x, a.y), fl, Color.White, Color.Black, 1, 1, 1, 5);
        }
        
        private void Dequeue(object sender, EventArgs e)
        {
            string s;
            messages.TryDequeue(out s);
            fonts.TryDequeue(out s);
            scales.TryDequeue(out float f);
        }

        Queue<string> messages = new Queue<string>();
        Queue<string> fonts = new Queue<string>();
        Queue<float> scales = new Queue<float>();
        /// <summary>
        /// Offsets are in percentages of the screen, with 0 being left / top and 1 being right / bottom
        /// </summary>
        public void AddMessage(string font, string message, float duration, float delay, float size, Interpolation interpolationIn, Interpolation interpolationOut,
            float startX, float startY, float endX, float endY)
        {

            messages.Enqueue(message);
            fonts.Enqueue(font);
            scales.Enqueue(size);

            a.AddAction(Actions.MoveFrom(a, startX, startY, endX, endY, duration, interpolationIn));

            a.AddAction(Actions.Delay(a, delay));

            a.AddAction(Actions.MoveFrom(a, endX, endY, startX, startY, duration, interpolationOut));

            a.AddAction(ev);

        }
    }
}
