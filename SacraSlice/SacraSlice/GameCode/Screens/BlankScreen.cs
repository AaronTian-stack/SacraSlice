using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;
namespace SacraSlice.GameCode.Screens
{
    public class BlankScreen :  GameScreen
    {
        GameContainer game;
        public BlankScreen(GameContainer game) : base(game)
        {
           this.game = game;
        }

    
        public override void Draw(GameTime gameTime)
        {
           
            GraphicsDevice.Clear(Color.Black);

        }

        public override void Update(GameTime gameTime)
        {
            
        }
        
        public override void LoadContent()
        {
           

        }

    }
}
