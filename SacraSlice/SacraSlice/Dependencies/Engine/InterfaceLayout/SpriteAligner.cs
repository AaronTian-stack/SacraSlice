using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace SacraSlice.Dependencies.Engine.InterfaceLayout
{
    public static class SpriteAligner
    {

        public static GraphicsDevice gd;
        public static SpriteBatch sb;
        public static void DrawSprite(Sprite s, float x, float y, Vector2 scale, float depth = 0)
        {
            var view = gd.Viewport;

            x *= view.Width;
            y *= view.Height;

            float targetX = scale.X * view.Width;
            float targetY = scale.Y * view.Height; 

            scale.X = targetX / s.Textureregion.width;
            scale.Y = targetY / s.Textureregion.height; //scale.X = targetY / s.Textureregion.height;

            s.Scale = scale;

            s.Position = new Vector2(x, y);
            s.Draw(sb, depth);
        }

        public static void DrawSprite(Sprite s, float x, float y, float scaleHeight, float depth = 0)
        {
            var view = gd.Viewport;

            x *= view.Width;
            y *= view.Height;

            float targetY = scaleHeight * view.Height;

            scaleHeight = targetY / s.Textureregion.height;

            s.Scale = new Vector2(scaleHeight);

            s.Position = new Vector2(x, y);
            s.Draw(sb, depth);
        }

        static Bag<(Sprite s, Vector2 pos, Vector2 scale, float depth)> batch
            = new Bag<(Sprite s, Vector2 pos, Vector2 scale, float depth)>();
        public static void BatchQueue(Sprite s, Vector2 pos, Vector2 scale, float depth = 0)
        {
            batch.Add((s, pos, scale, depth));
        }

        public static void BatchDraw(OrthographicCamera camera)
        {

            foreach (var batch in batch)
            {

                var pos = camera.WorldToScreen(batch.pos);
                var view = gd.Viewport;
                batch.s.Scale = batch.scale;
                batch.s.Position = pos;
                batch.s.Draw(sb);

            }
            batch.Clear();
        }

    }
}
