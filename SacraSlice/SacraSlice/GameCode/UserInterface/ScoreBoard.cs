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
        SpriteFont font;
        Game game;
        Wrapper<int> score;
        public ScoreBoard(Game game, Wrapper<int> score)
        {
            this.game = game;
            font = game.Content.Load<SpriteFont>("Fonts/Font");
            this.score = score;
        }

        public void Draw(SpriteBatch sb)
        {
            var b = game.GraphicsDevice.Viewport.Bounds;

            var s = "Score: " + score;

            var v = font.MeasureString(s);

            float centerX = b.Center.X - v.X / 2;
            float y = 20;

            sb.DrawString(font, s, new Vector2(centerX, y + 2), Color.Black, 0, new Vector2(), 1f, SpriteEffects.None, 0);
            sb.DrawString(font, s, new Vector2(centerX, y), Color.GhostWhite, 0, new Vector2(), 1f, SpriteEffects.None, 0);
        }

    }
}
