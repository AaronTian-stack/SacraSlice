using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace SacraSlice.Dependencies.Engine.InterfaceLayout
{
    public class ShapeAligner
    {
        public static GraphicsDevice gd;
        public static RectangleF GenerateRect(float x, float y, Vector2 scale)
        {
            var view = gd.Viewport;

            x *= view.Width;
            y *= view.Height;

            float targetX = scale.X * view.Width;
            float targetY = scale.Y * view.Height;

            return new RectangleF(x - targetX * 0.5f, y - targetY * 0.5f, targetX, targetY);
        }
    }
}
