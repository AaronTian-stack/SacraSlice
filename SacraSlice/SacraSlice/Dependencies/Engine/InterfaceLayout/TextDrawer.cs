using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SacraSlice.Dependencies.Engine.InterfaceLayout
{
    public static class TextDrawer
    {
        static Dictionary<string, BitmapFont> lookup = new Dictionary<string, BitmapFont>();
        public static GraphicsDevice gd;
        public static SpriteBatch sb;
        public static void AddFont(string name, BitmapFont font)
        {
            lookup.Add(name, font);
        }

        public static BitmapFont GetFont(string s)
        {
            return lookup[s];
        }

        public static Vector2 PixelsToPercent(Vector2 pixels)
        {
            var view = gd.Viewport.Bounds;
            return new Vector2(pixels.X / view.Width, pixels.Y / view.Height);
        }

        public static Vector2 PercentToPixels(Vector2 percent)
        {
            var view = gd.Viewport.Bounds;
            return new Vector2(percent.X * view.Width, percent.Y * view.Height);
        }

        public static void DrawTextStatic(string font, string text, Vector2 pos, float scale, Color color,
            Color outlineColor, float outlineWidth = 0, float depth = 0)
        {
            DrawTextStatic(font, text, pos, scale, color, outlineColor, outlineWidth, outlineWidth, outlineWidth, outlineWidth, depth);
        }

        public static Size2 MeasureFont(string font, string text)
        {
            var sf = lookup[font];
            return sf.MeasureString(text);
        }

        public static void DrawTextStatic(string font, string text, Vector2 pos, float scale, Color color, Color outlineColor,
           float outlineLeft = 0, float outlineRight = 0, float outlineUp = 0, float outlineDown = 0, float depth = 0)
        {
            var sf = lookup[font];

            var di = sf.MeasureString(text);

            var view = gd.Viewport;


            //var targetX = scale * gd.Viewport.Width;
            var targetY = scale * gd.Viewport.Height; // number of pixels you want
            //var scaleX = targetX / di.X;
            scale = targetY / di.Height;


            pos.X = pos.X * view.Width;
            pos.Y = pos.Y * view.Height;

            pos.X -= scale * di.Width / 2;
            pos.Y -= scale * di.Height / 2;

            DrawString(font, text, pos, color, outlineColor, scale, outlineLeft, outlineRight, outlineUp, outlineDown, depth);
        }

        public static void DrawString(string font, string text, Vector2 pos, Color color, Color outlineColor, float scale,
            float outlineLeft = 0, float outlineRight = 0, float outlineUp = 0, float outlineDown = 0, float depth = 0)
        {
            var sf = lookup[font];
            float off = 0.00001f;
            if (outlineDown != 0)
                sb.DrawString(sf, text, new Vector2(pos.X, pos.Y + outlineDown), outlineColor, 0, new Vector2(), scale, SpriteEffects.None, depth + off);
            if (outlineUp != 0)
                sb.DrawString(sf, text, new Vector2(pos.X, pos.Y - outlineUp), outlineColor, 0, new Vector2(), scale, SpriteEffects.None, depth + off);
            if (outlineRight != 0)
                sb.DrawString(sf, text, new Vector2(pos.X + outlineRight, pos.Y), outlineColor, 0, new Vector2(), scale, SpriteEffects.None, depth + off);
            if (outlineLeft != 0)
                sb.DrawString(sf, text, new Vector2(pos.X - outlineLeft, pos.Y), outlineColor, 0, new Vector2(), scale, SpriteEffects.None, depth + off);

            if (outlineDown != 0 && outlineRight != 0)
                sb.DrawString(sf, text, new Vector2(pos.X + outlineRight, pos.Y + outlineDown), outlineColor, 0, new Vector2(), scale, SpriteEffects.None, depth + off);
            if (outlineUp != 0 && outlineRight != 0)
                sb.DrawString(sf, text, new Vector2(pos.X + outlineRight, pos.Y - outlineUp), outlineColor, 0, new Vector2(), scale, SpriteEffects.None, depth + off);
            if (outlineDown != 0 && outlineLeft != 0)
                sb.DrawString(sf, text, new Vector2(pos.X - outlineLeft, pos.Y + outlineDown), outlineColor, 0, new Vector2(), scale, SpriteEffects.None, depth + off);
            if (outlineUp != 0 && outlineLeft != 0)
                sb.DrawString(sf, text, new Vector2(pos.X - outlineLeft, pos.Y - outlineUp), outlineColor, 0, new Vector2(), scale, SpriteEffects.None, depth + off);

            sb.DrawString(sf, text, pos, color, 0, new Vector2(), scale, SpriteEffects.None, depth);
        }

        static List<(string, string, Vector2, Color, Color, float, float, float, float, float, float)> batch
            = new List<(string, string, Vector2, Color, Color, float, float, float, float, float, float)>();
        public static void BatchQueue(string font, string text, Vector2 pos, Color color, Color outlineColor, float scale,
            float outlineLeft = 0, float outlineRight = 0, float outlineUp = 0, float outlineDown = 0, float depth = 0)
        {
            batch.Add((font, text, pos, color, outlineColor, scale, outlineLeft, outlineRight, outlineUp, outlineDown, depth));
        }

        public static void BatchDraw(OrthographicCamera camera)
        {

            foreach (var batch in batch)
            {
                var pos = camera.WorldToScreen(batch.Item3);
                var view = gd.Viewport;

                pos.X /= view.Width;
                pos.Y /= view.Height;

                DrawTextStatic(batch.Item1, batch.Item2, pos, batch.Item6 * camera.Zoom, batch.Item4, batch.Item5, batch.Item7, batch.Item8
                    , batch.Item9, batch.Item10, batch.Item11);
            }
            batch.Clear();
        }
    }
}
