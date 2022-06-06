using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SacraSlice.Dependencies.Engine;
using System;
using System.Collections.Generic;
using System.Text;

namespace SacraSlice.GameCode.UserInterface
{
    public class ScoreBoard
    {
        //SpriteFont font;
        Game game;
        Wrapper<int> score;
        public ScoreBoard(Game game, Wrapper<int> score)
        {
            this.game = game;

            

            this.score = score;
        }

        public void Draw(SpriteBatch sb)
        {
            var b = game.GraphicsDevice.Viewport.Bounds;

            var s = "Score: " + score;

            var f = TextDrawer.GetFont("LanaPixel72");

            var v = f.MeasureString(s);

            float centerX = b.Center.X - v.X / 2;

            TextDrawer.DrawText(sb, "LanaPixel72", s, new Vector2(centerX, 20), 1f, Color.GhostWhite, Color.Black,
                1, 1, 1, 4);
        }

    }
}
