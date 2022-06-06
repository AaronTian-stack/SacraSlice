using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SacraSlice.Dependencies.Engine.InterfaceLayout
{
    public static class SpriteAligner
    {

        public static GraphicsDevice gd;
        public static SpriteBatch sb;
        public static void DrawSprite(Sprite s, float x, float y, float depth = 0)
        {
            var view = gd.Viewport;

            x *= view.Width;
            y *= view.Height;

            s.Position = new Vector2(x, y);
            s.Draw(sb, depth);
        }
    }
}
