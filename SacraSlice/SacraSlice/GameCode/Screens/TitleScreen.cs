using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.ViewportAdapters;
using MonoGameSaveManager;
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
        float timer, AnimationTimer, ampTimer;
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

            // render all enemy type heads below
            AnimationTimer += gameTime.GetElapsedSeconds();

            var speed = 1.3f;
            ampTimer += gameTime.GetElapsedSeconds() * speed;

            float y = 37;

            var off = 5f;

            var amp = -3;

            var offY = amp * MathF.Sin(ampTimer);

            s.Textureregion = banana.GetKeyFrame(AnimationTimer);
            s.Position = new Vector2(-50, y + offY);
            s.Draw(sb);

            offY = amp * MathF.Sin(ampTimer - off);

            s.Textureregion = card.GetKeyFrame(AnimationTimer);
            s.Position = new Vector2(-25, y + offY);
            s.Draw(sb);

            offY = amp * MathF.Sin(ampTimer - off * 2);

            s.Scale = new Vector2(ppm);
            s.Textureregion = ball.GetKeyFrame(AnimationTimer);
            s.Position = new Vector2(0, y + offY);
            s.Draw(sb);

            offY = amp * MathF.Sin(ampTimer - off * 3);

            s.Textureregion = box.GetKeyFrame(AnimationTimer);
            s.Position = new Vector2(25, y + offY);
            s.Draw(sb);

            offY = amp * MathF.Sin(ampTimer - off * 4);

            s.Textureregion = pien.GetKeyFrame(AnimationTimer);
            s.Position = new Vector2(50, y + offY);
            s.Draw(sb);

            sb.End();


            sb.Begin(SpriteSortMode.BackToFront, 
                samplerState: SamplerState.PointWrap
                , blendState: BlendState.NonPremultiplied
                );
            TextDrawer.DrawTextStatic("Title Font", "SacraSlice", new Vector2(.5f, .35f + a.y), .3f, Color.White, Color.Black,
              2, 2, 2, 20);

            timer += gameTime.GetElapsedSeconds();
            if(timer > 1f)
            {
                b = !b;
                timer = 0;
            }

            if(b)
                TextDrawer.DrawTextStatic("Main Font", "Press Enter to Start", new Vector2(.5f, .535f), .1f, Color.White, Color.Black,
                  2, 2, 2, 10);

            TextDrawer.DrawTextStatic("Main Font", "Highscore: "+score, new Vector2(.5f, .605f), .06f, Color.White, Color.Black,
                  2, 2, 2, 8);


            float yt = 0.7f;
            if(deleteTimer > 0)
            {
                TextDrawer.DrawTextStatic("Main Font", 
                    "DELETING DATA "+(delete - deleteTimer).ToString("0.0"), 
                    new Vector2(.5f, yt), .06f, Color.White, Color.Black,
                  2, 2, 2, 8);
            }
            var k = KeyboardExtended.GetState();
            if (k.IsKeyDown(Keys.Delete) && deleted)
            {
                TextDrawer.DrawTextStatic("Main Font", 
                    "DATA DELETED", new Vector2(.5f, yt), .06f, Color.White, Color.Black,
                 2, 2, 2, 8);
            }
           

            GameContainer._spriteBatch.End();
           

        }
        Animation<TextureRegion> ball =
            new Animation<TextureRegion>("ball", 0.5f,
                GameContainer.atlas.FindRegions("sphere"), PlayMode.LOOP);
        Animation<TextureRegion> box =
           new Animation<TextureRegion>("cube", 0.5f,
               GameContainer.atlas.FindRegions("cube"), PlayMode.LOOP);
        Animation<TextureRegion> card =
           new Animation<TextureRegion>("box", 0.5f,
               GameContainer.atlas.FindRegions("cardboard"), PlayMode.LOOP);
        Animation<TextureRegion> banana =
           new Animation<TextureRegion>("banana", 0.5f,
               GameContainer.atlas.FindRegions("banana"), PlayMode.LOOP);
        Animation<TextureRegion> pien =
           new Animation<TextureRegion>("pien", 0.5f,
               GameContainer.atlas.FindRegions("pien"), PlayMode.LOOP);

        bool load, deleted;
        float deleteTimer;
        float delete = 3f;
        public override void Update(GameTime gameTime)
        {
            world.Update(gameTime);
            var k = KeyboardExtended.GetState();
            if (!load && AnimationTimer > 2 && k.WasKeyJustDown(Keys.Enter))
            {
                load = true;
                game.LoadScreen(game.play, 2f);
                MediaPlayer.Stop();
                // play blip sound
                GameContainer.sounds["Select"].Play();
            }
            if (!deleted && k.IsKeyDown(Keys.Delete))
            {
                deleteTimer += gameTime.GetElapsedSeconds();
                if(deleteTimer > delete)
                {
                    deleted = true;
                    // delete high score
                    SaveManager mySave = new IsolatedStorageSaveManager("SacraSlice", "mysave.dat");
                    mySave.Data.highScore = 0;
                    mySave.Save();
                    score = 0;
                }
            }
            else deleteTimer = 0;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || k.IsKeyDown(Keys.Escape))
                game.Exit();

            if (!load && k.WasKeyJustDown(Keys.F))
            {
                game.FullScreen();
            }
        }
        int score;
        public override void LoadContent()
        {
            load = false;
            MediaPlayer.Play(GameContainer.songs["JustFine"]);
            MediaPlayer.Volume = 1;
            MediaPlayer.IsRepeating = true;
            SaveManager mySave = new IsolatedStorageSaveManager("SacraSlice", "mysave.dat");
            mySave.Load();
            score = mySave.Data.highScore;
            AnimationTimer = 0;
        }

    }
}
