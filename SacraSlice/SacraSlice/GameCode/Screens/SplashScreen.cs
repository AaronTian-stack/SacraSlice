using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Screens;
using SacraSlice.Dependencies.Engine;
using SacraSlice.Dependencies.Engine.InterfaceLayout;
using SacraSlice.Dependencies.Engine.Scene;
using System;
using System.Collections.Generic;
using System.Text;

namespace SacraSlice.GameCode.Screens
{
    public class SplashScreen :  GameScreen
    {
        GameContainer game;
        public SplashScreen(GameContainer game) : base(game)
        {
           this.game = game;
        }

        Actor a = new Actor();
        Actor a1 = new Actor();
        public override void Draw(GameTime gameTime)
        {
            a.Act(gameTime.GetElapsedSeconds());
            a1.Act(gameTime.GetElapsedSeconds());
            GraphicsDevice.Clear(Color.White);

            GameContainer._spriteBatch.Begin(samplerState: SamplerState.LinearWrap, blendState: BlendState.NonPremultiplied);

            Color c = Color.Black;
            c.A = a.color.A;

            Color c1 = Color.Black;
            c1.A = a1.color.A;

            // PlumbumPall
            // qikentie
            // qikentite
            // qikentify
            // qhenki

            TextDrawer.DrawTextStatic("Main Font", "a game by", new Vector2(a1.x, a1.y), .1f, Color.White, c1,
                2, 2, 2, 3);

            TextDrawer.DrawTextStatic("Main Font", "qhenki", new Vector2(a.x, a.y), .2f, Color.White, c,
                2, 2, 2, 5);

            GameContainer._spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            if (a.actions.Count == 0 && a1.actions.Count == 0)
                game.LoadScreen(game.play, 3f);
        }

        public override void LoadContent()
        {
            a.x = -1; a.y = -1;
            a1.x = -1; a1.y = -1;
            a.ClearActions();
            Color start = Color.White;
            start.A = 0;
            a.AddAction(Actions.Delay(a, 1f));
            a.AddAction(new ParallelAction(a,
                Actions.MoveFrom(a, 0.5f, 0.55f, 0.5f, 0.5f, 2, Interpolation.swingOut)
                ,Actions.ColorAction(a, start, Color.White, 2, Interpolation.smooth)));
            a.AddAction(Actions.Delay(a, 3f));

            a1.AddAction(Actions.Delay(a1, 0.5f));
            a1.AddAction(new ParallelAction(a1,
                Actions.MoveFrom(a1, 0.5f, 0.5f, 0.5f, 0.42f, 2, Interpolation.swingOut)
                , Actions.ColorAction(a1, start, Color.White, 2, Interpolation.smooth)));


        }

    }
}
