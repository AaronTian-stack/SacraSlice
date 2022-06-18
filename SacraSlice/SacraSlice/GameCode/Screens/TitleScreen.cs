using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.ViewportAdapters;
using SacraSlice.Dependencies.Engine;
using SacraSlice.Dependencies.Engine.ECS.Component;
using SacraSlice.Dependencies.Engine.ECS.Systems;
using SacraSlice.Dependencies.Engine.InterfaceLayout;
using SacraSlice.Dependencies.Engine.Scene;
using SacraSlice.GameCode.GameECS;
using SacraSlice.GameCode.GameECS.GameSystems;
using System;
using System.Collections.Generic;
using System.Text;

namespace SacraSlice.GameCode.Screens
{
    public class TitleScreen : GameScreen
    {
        GameContainer game;
        World world;
        GameCamera camera;
        float ppm = 0.25f;
        float dt = 1 / 30f;
        public TitleScreen(GameContainer game) : base(game)
        {
            this.game = game;

            var slope = new Timer();
            slope.GetTimer("slope").Value = -0.7f;
            var entityFactory = new EntityFactory();

            world = new WorldBuilder()

                .AddSystem(new TimerUpdater(new Wrapper<float>(dt), dt))
                .AddSystem(new BackGroundSystem(GameContainer._spriteBatch, slope, ppm, false))
                .AddSystem(new CameraTracker())

                .Build();

            entityFactory.Initialize(world, GraphicsDevice, dt);

            var viewportAdapter = new BoxingViewportAdapter(game.Window, 
                GraphicsDevice, (int)(512 * ppm), (int)(288 * ppm));

            camera = new GameCamera(viewportAdapter)
            {
                Zoom = 1
            };

            Position p = new Position();
            p.SetAllPosition(new Vector2(0, -30));

            entityFactory.CreateCamera(camera, viewportAdapter, p);

            float am = 0.02f;
            a.AddAction(new RepeatAction(a, 
                Actions.MoveFrom(a, 0, 0, 0, am, 2, Interpolation.smooth)
                , Actions.MoveFrom(a, 0, am, 0, 0, 2, Interpolation.smooth)));
        }

        Actor a = new Actor();
        bool b;
        float timer;
        /// add buttons
        public override void Draw(GameTime gameTime)
        {
            a.Act(gameTime.GetElapsedSeconds());
            GraphicsDevice.Clear(Color.Black);

            GameContainer._spriteBatch.Begin(SpriteSortMode.BackToFront, samplerState: SamplerState.PointClamp
                , transformMatrix: camera.ViewMatrix
                , blendState: BlendState.NonPremultiplied
                );
            world.Draw(gameTime);
            GameContainer._spriteBatch.End();


            GameContainer._spriteBatch.Begin(SpriteSortMode.BackToFront, 
                samplerState: SamplerState.LinearWrap
                , blendState: BlendState.NonPremultiplied
                );
            TextDrawer.DrawTextStatic("Main Font", "SacraSlice", new Vector2(.5f, .4f + a.y), .3f, Color.White, Color.Black,
              2, 2, 2, 5);

            timer += gameTime.GetElapsedSeconds();
            if(timer > 1f)
            {
                b = !b;
                timer = 0;
            }

            if(b)
            TextDrawer.DrawTextStatic("Main Font", "Press Enter to Start", new Vector2(.5f, .6f), .1f, Color.White, Color.Black,
              2, 2, 2, 5);

            GameContainer._spriteBatch.End();
           

        }

        public override void Update(GameTime gameTime)
        {
            world.Update(gameTime);
            if (KeyboardExtended.GetState().WasKeyJustDown(Keys.Enter))
            {
                game.LoadScreen(game.play, 2f);
                MediaPlayer.Stop();
                // play blip sound
            }
                
        }

        public override void LoadContent()
        {
            game.play = new PlayScreen(game);
            MediaPlayer.Play(GameContainer.songs["JustFine"]);
            MediaPlayer.IsRepeating = true;
        }

    }
}
