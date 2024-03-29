﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.ViewportAdapters;
using SacraSlice.Dependencies.Engine;
using SacraSlice.Dependencies.Engine.InterfaceLayout;
using SacraSlice.Dependencies.Engine.Scene;
using SacraSlice.Dependencies.Engine.Scene.ActionClasses;
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
           sound += PlaySound;
            var viewportAdapter = new BoxingViewportAdapter(game.Window,
                 GraphicsDevice, 1, 1);
            camera = new OrthographicCamera(viewportAdapter);
        }

        Actor a = new Actor();
        Actor a1 = new Actor();
        OrthographicCamera camera;
        public override void Draw(GameTime gameTime)
        {
            a.Act(gameTime.GetElapsedSeconds());
            a1.Act(gameTime.GetElapsedSeconds());
            GraphicsDevice.Clear(Color.Black);

            GameContainer._spriteBatch.Begin(transformMatrix: camera.GetViewMatrix());

            GameContainer._spriteBatch.FillRectangle(camera.BoundingRectangle, Color.White);

            GameContainer._spriteBatch.End();
            Color c = Color.Black;
            c.A = a.color.A;

            Color c1 = Color.Black;
            c1.A = a1.color.A;

            GameContainer._spriteBatch.Begin(samplerState: SamplerState.LinearWrap, 
                blendState: BlendState.NonPremultiplied);

            TextDrawer.DrawTextStatic("Main Font", "a game by", new Vector2(a1.x, a1.y), .1f, Color.White, c1,
                2, 2, 2, 3);

            TextDrawer.DrawTextStatic("Main Font", "qhenki", new Vector2(a.x, a.y), .2f, Color.White, c,
                2, 2, 2, 5);

            GameContainer._spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            if (a.actions.Count == 0 && a1.actions.Count == 0)
                game.LoadScreen(game.title, 3f);

            var ks = KeyboardExtended.GetState();
            if (ks.WasKeyJustDown(Keys.F))
            {
                game.FullScreen();
            }
        }
        EventHandler sound;
        public override void LoadContent()
        {
            a.x = -1; a.y = -1;
            a1.x = -1; a1.y = -1;
            a.ClearActions();
            Color start = Color.White;
            start.A = 0;
            a.AddAction(Actions.Delay(a, 1f));
            a.AddAction(new EventAction(a, sound));
            a.AddAction(new ParallelAction(a,
                Actions.MoveFrom(a, 0.5f, 0.55f, 0.5f, 0.48f, 2, Interpolation.swingOut)
                ,Actions.ColorAction(a, start, Color.White, 2, Interpolation.smooth)));
            a.AddAction(Actions.Delay(a, 3f));


            a1.AddAction(Actions.Delay(a1, 0.5f)); // a game by
            a1.AddAction(new EventAction(a1, sound));
            a1.AddAction(new ParallelAction(a1,
                Actions.MoveFrom(a1, 0.5f, 0.5f, 0.5f, 0.40f, 2, Interpolation.swingOut)
                , Actions.ColorAction(a1, start, Color.White, 2, Interpolation.smooth)));


        }

        public void PlaySound(object sender, EventArgs e)
        {
            GameContainer.sounds["Coin"].Play();
        }

    }
}
