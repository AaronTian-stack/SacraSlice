using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SacraSlice.Dependencies.Engine;
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
            //DebugLog.Print("actor", a.x + " " + a.y);
            string s;
            string f;
            if (messages.TryPeek(out s) && fonts.TryPeek(out f))
                TextDrawer.DrawText(sb, f, s, new Vector2(a.x, a.y), 1, Color.White, Color.Black, 2);
            

        }
        
        private void Dequeue(object sender, EventArgs e)
        {
            string s;
            messages.TryDequeue(out s);
            fonts.TryDequeue(out s);
        }

        Queue<string> messages = new Queue<string>();
        Queue<string> fonts = new Queue<string>();
        /// <summary>
        /// Offsets are in percentages of the screen, with 0 being left / top and 1 being right / bottom
        /// </summary>
        public void AddMessage(string font, string message, float duration, Interpolation interpolationIn, Interpolation interpolationOut,
            float startOffsetX, float startOffsetY, float endOffsetX, float endOffsetY)
        {
            // starting offset

            a.x = gd.Viewport.Width * startOffsetX;
            DebugLog.Print("X", a.x.ToString());


 
            a.y = gd.Viewport.Height * startOffsetY;
            DebugLog.Print("Y", a.y.ToString());

            // ending offset

            float endX = gd.Viewport.Width * endOffsetX;

            float endY = gd.Viewport.Height * endOffsetY;
            DebugLog.Print("endY", endY.ToString());

            var sf = TextDrawer.GetFont(font);
            
            var di = sf.MeasureString(message);


            messages.Enqueue(message);
            fonts.Enqueue(font);

            a.x -= di.X / 2;
            endX -= di.X / 2;

            a.AddAction(Actions.MoveTo(a, endX, endY, duration, interpolationIn));

            //a.x = endX;
            //a.y = endY;

            a.AddAction(Actions.MoveTo(a, a.x, a.y, duration, interpolationOut));

            a.AddAction(ev);

        }
    }
}
