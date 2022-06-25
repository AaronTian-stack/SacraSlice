using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using MonoGame.Extended.Screens;
using System.Diagnostics;

namespace SacraSlice.GameCode.Screens
{
    public class BlankScreen :  GameScreen
    {
        GameContainer game;
        public BlankScreen(GameContainer game) : base(game)
        {
           this.game = game;
        }

        bool trans;
        float timer;
        public override void Draw(GameTime gameTime)
        {
            timer += gameTime.GetElapsedSeconds();
            GraphicsDevice.Clear(Color.Black);
            if (!trans && timer > 1f)
            {
                game.LoadScreen(game.play, 0.5f);
                trans = true;
            }

        }

        public override void Update(GameTime gameTime)
        {
            
        }
        
        public override void LoadContent()
        {
            timer = 0;
            MediaPlayer.Stop();
            trans = false;

        }

    }
}
