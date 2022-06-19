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
using SacraSlice.Dependencies.Engine.Animation;
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


            entityFactory.CreateCamera(camera, viewportAdapter);
            

            float am = 0.02f;
            a.AddAction(new RepeatAction(a, 
                Actions.MoveFrom(a, 0, 0, 0, am, 2, Interpolation.smooth)
                , Actions.MoveFrom(a, 0, am, 0, 0, 2, Interpolation.smooth)));
        }

        Actor a = new Actor();
        bool b;
        float timer, AnimationTimer;
        Sprite s = new Sprite();
        public override void Draw(GameTime gameTime)
        {
            var sb = GameContainer._spriteBatch;

            a.Act(gameTime.GetElapsedSeconds());
            GraphicsDevice.Clear(Color.Black);

            sb.Begin(SpriteSortMode.BackToFront, samplerState: SamplerState.PointClamp
                , transformMatrix: camera.ViewMatrix
                , blendState: BlendState.NonPremultiplied
                );
            world.Draw(gameTime);

            AnimationTimer += gameTime.GetElapsedSeconds();
            float y = 35;
            s.Scale = new Vector2(ppm);
            s.Textureregion = ball.GetKeyFrame(AnimationTimer);
            s.Position = new Vector2(0, y);
            s.Draw(sb);

            s.Textureregion = box.GetKeyFrame(AnimationTimer);
            s.Position = new Vector2(25, y);
            s.Draw(sb);


            sb.End();


            sb.Begin(SpriteSortMode.BackToFront, 
                samplerState: SamplerState.PointWrap
                , blendState: BlendState.NonPremultiplied
                );
            TextDrawer.DrawTextStatic("Title Font", "SacraSlice", new Vector2(.5f, .4f + a.y), .3f, Color.White, Color.Black,
              2, 2, 2, 20);

            timer += gameTime.GetElapsedSeconds();
            if(timer > 1f)
            {
                b = !b;
                timer = 0;
            }

            if(b)
                TextDrawer.DrawTextStatic("Main Font", "Press Enter to Start", new Vector2(.5f, .6f), .1f, Color.White, Color.Black,
                  2, 2, 2, 10);

            // render all enemy type heads below

           

            GameContainer._spriteBatch.End();
           

        }
        Animation<TextureRegion> ball =
            new Animation<TextureRegion>("ball", 0.5f,
                GameContainer.atlas.FindRegions("sphere"), PlayMode.LOOP);
        Animation<TextureRegion> box =
           new Animation<TextureRegion>("cube", 0.5f,
               GameContainer.atlas.FindRegions("cube"), PlayMode.LOOP);

        bool load;

        public override void Update(GameTime gameTime)
        {
            world.Update(gameTime);
            var k = KeyboardExtended.GetState();
            if (k.WasKeyJustDown(Keys.Enter))
            {
                load = true;
                game.LoadScreen(game.play, 2f);
                MediaPlayer.Stop();
                // play blip sound
                GameContainer.sounds["Select"].Play();

            }

            if (!load && k.WasKeyJustDown(Keys.F))
            {
                game.FullScreen();
            }
        }

        public override void LoadContent()
        {
            load = false;
            MediaPlayer.Play(GameContainer.songs["JustFine"]);
            MediaPlayer.Volume = 1;
            MediaPlayer.IsRepeating = true;
        }

    }
}
