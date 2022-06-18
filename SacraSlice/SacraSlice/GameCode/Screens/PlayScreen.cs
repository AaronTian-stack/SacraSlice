using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.ViewportAdapters;
using SacraSlice.Dependencies.Engine;
using SacraSlice.Dependencies.Engine.ECS.Component;
using SacraSlice.Dependencies.Engine.ECS.Systems;
using SacraSlice.Dependencies.Engine.InterfaceLayout;
using SacraSlice.Dependencies.Engine.Scene;
using SacraSlice.Dependencies.Engine.Scene.ActionClasses;
using SacraSlice.GameCode.GameECS;
using SacraSlice.GameCode.GameECS.GameComponents;
using SacraSlice.GameCode.GameECS.GameSystems;
using SacraSlice.GameCode.Managers;
using SacraSlice.GameCode.UserInterface;
using System;
using System.Collections.Generic;

namespace SacraSlice.GameCode.Screens
{
    public class PlayScreen : GameScreen
    {

        World world;

        EntityFactory entityFactory;

        public static GameCamera camera;

        Stack<InputManager> inputStack = new Stack<InputManager>();

        DebugLog debug = new DebugLog();

        public static float gravity = 0.12f;

        public static float ppm = 0.25f;

        private RenderTarget2D renderTarget;

        public static Wrapper<bool> editMode = new Wrapper<bool>(false);

        public static Vector2 mouseCoordinate;

        public ScoreBoard scoreBoard;

        public static Wrapper<int> score = new Wrapper<int>(0);
        public static Wrapper<int> life = new Wrapper<int>(3);
        public static Wrapper<int> enemiesOnScreen = new Wrapper<int>(0);

        public static int ScoreToStartHardMode = 5; // 5

        public static int ScoreToSpawnDemon = 50; // 50

        public static int ScoreWhenHardModeStarted;

        public static int justAdded = new Wrapper<int>(0);


        public static int MaxEnemiesOnScreen = 3; // 3

        public static float lifeTimeBasic = 4;
        public static float lifeTimeEnemyBegin = 10;

        static float level = 20;
        public static float LifeTimeEnemyVariable
        {
            get
            {
                if(hardEnemiesSpawn)
                    return Math.Max(4, lifeTimeEnemyBegin - ((score - ScoreWhenHardModeStarted) / level));
                return lifeTimeEnemyBegin;
            }
        }

        public static float spawnGapStart = 2;
        public static float SpawnGapVariable
        {
            get
            {
                if(hardEnemiesSpawn)
                    return Math.Max(0.25f, spawnGapStart - ((score - ScoreWhenHardModeStarted) / level));
                return spawnGapStart;
            }
        }

        public static bool SpawnControl = true;

        public static bool hardEnemiesSpawn = false;

        public static ShieldEnergy energy;

        public static Narrator narrator;
        public static ScreenFlash flash;
        GameContainer game;
        public PlayScreen(GameContainer game) : base(game)
        {
            this.game = game;
            debugWindow = new DebugWindow(debug, ticks, dt);

            entityFactory = new EntityFactory();
            narrator = new Narrator(game);
            flash = new ScreenFlash(GraphicsDevice);

            var slope = new Timer();
            slope.GetTimer("slope").Value = -0.7f;
            InitalizeEvents();
            world = new WorldBuilder()

                // Update Systems

                .AddSystem(new PositionModifier(gravity)) // gravity
                .AddSystem(new HitBoxChange())
                .AddSystem(new StateUpdater(editMode))
                .AddSystem(new PositionClamp())
                .AddSystem(new RopeUpdate())

                // Animation Systems

                .AddSystem(new AnimationUpdater())
                .AddSystem(new SpriteAnimation(dt, dtStatic))
                .AddSystem(new SquashAnimation())

                // Render update

                .AddSystem(new TimerUpdater(dt, dtStatic))
                .AddSystem(new StateDrawUpdate(GameContainer._spriteBatch, dt, dtStatic))

                // Drawing Systems

                .AddSystem(new BackGroundSystem(GameContainer._spriteBatch, slope, ppm, true))

                .AddSystem(new PositionInterpolator())
                .AddSystem(new ImGuiEntityDraw())
                .AddSystem(new SpriteRenderer(GameContainer._spriteBatch, ppm))
                .AddSystem(new HitBoxRenderer(GameContainer._spriteBatch, ppm))
                .AddSystem(new ShadowSystem(GameContainer._spriteBatch, ppm))
                

                // Rope


                .AddSystem(new RopePositioner())
                .AddSystem(new RopeClamperSystem(ppm))
                .AddSystem(new RopeRenderer(GameContainer._spriteBatch, ppm))

                .AddSystem(new CameraTracker())


                // Game Systems

                .AddSystem(new SwordSquash())
                .AddSystem(new SwordDraw(GameContainer._spriteBatch, ppm, dt, dtStatic))
                .AddSystem(new SpawnerSystem(entityFactory, ppm, score, dt, dtStatic))
                .AddSystem(new DemonSystem(GameContainer._spriteBatch, ppm))

                .Build();


            

            entityFactory.Initialize(world, GraphicsDevice, dtStatic);

            int w = 640;
            int h = 360;

            var viewportAdapter = new BoxingViewportAdapter(game.Window, GraphicsDevice, (int)(512 * ppm), (int)(288 * ppm));

            camera = new GameCamera(viewportAdapter);
            camera.Zoom = 0.8f;
            renderTarget = new RenderTarget2D(GraphicsDevice, w, h);


            entityFactory.CreateDemon();


            var e = world.CreateEntity();
            Position p = new Position();
            p.SetAllPosition(new Vector2(0, -30));

            e.Attach("CAMERA TARGET");
            e.Attach(p);

            e = world.CreateEntity();
            e.Attach("BACKGROUND SLOPE");
            e.Attach(slope);

            entityFactory.CreateCamera(camera, viewportAdapter, p);

            energy = new ShieldEnergy(camera);

            scoreBoard = new ScoreBoard();

        }

        Cursor cursor = new Cursor(0.01f, 10);
        public static MouseStateExtended mouse;
        public static DebugWindow debugWindow;
        public static Rectangle ndr;
        public static bool ShowEnergy;
        public override void Draw(GameTime gameTime)
        {
            var dr = GraphicsDevice.PresentationParameters.Bounds;
            var view = GraphicsDevice.Viewport;
            view.Width = dr.Width;
            view.Height = dr.Height;
            view.X = dr.X;
            view.Y = dr.Y;
            GraphicsDevice.Viewport = view;

            GraphicsDevice.Clear(Color.Black);
            mouse = MouseExtended.GetState();
            if (inputStack.Count > 0)
                inputStack.Peek().Poll();


            // calculate how much you must scale the renderTarget vertically


            // TODO: add X clamping
            
            //Rectangle ndr;
            var v = camera._viewportAdapter;
            if ( (dr.Width * 1f / dr.Height) > (v.VirtualWidth * 1f / v.VirtualHeight) ) 
                // if you are wider than 16:9
            {
                // scale based off y
                var xPixels = dr.Height * v.VirtualWidth / v.VirtualHeight;
                ndr = new Rectangle((dr.Width - xPixels) / 2, 0, xPixels, dr.Height);
            }
            else
            {
                var yPixels = dr.Width * v.VirtualHeight / v.VirtualWidth;
                ndr = new Rectangle(0, (dr.Height - yPixels) / 2, dr.Width, yPixels);
            }


            view = GraphicsDevice.Viewport;
            view.Width = ndr.Width;
            view.Height = ndr.Height;
            view.X = ndr.X;
            view.Y = ndr.Y;
            GraphicsDevice.Viewport = view;

            GameContainer.GuiRenderer.BeginLayout(gameTime);

            debugWindow.Draw(gameTime);

            //TODO: get a rendertarget

            // GraphicsDevice.SetRenderTarget(renderTarget);

            
            GameContainer._spriteBatch.Begin(SpriteSortMode.BackToFront, samplerState: SamplerState.PointClamp
                , transformMatrix: camera.ViewMatrix
                , blendState: BlendState.NonPremultiplied
                );

            world.Draw(gameTime);

            if(ShowEnergy || hardEnemiesSpawn)
                energy.Draw(GameContainer._spriteBatch, gameTime.GetElapsedSeconds() * (dtStatic / dt), ppm);

            if (mouse.WasButtonJustDown(MouseButton.Left))
                cursor.Set(mouseCoordinate);

            cursor.Update(mouseCoordinate, gameTime.GetElapsedSeconds(),
                    mouse.IsButtonDown(MouseButton.Left));

            cursor.headWidth /= camera.Zoom;
            cursor.Draw(GameContainer._spriteBatch, ppm, 2f);
            cursor.headWidth *= camera.Zoom;

            ImGui.End();

            GameContainer._spriteBatch.End();

            //GraphicsDevice.SetRenderTarget(null);

            GameContainer._spriteBatch.Begin(samplerState: SamplerState.PointClamp
                , transformMatrix: camera.ViewMatrix
                , blendState: BlendState.NonPremultiplied);



            //GameContainer._spriteBatch.Draw(renderTarget, ndr, Color.White);

            SpriteAligner.BatchDraw(camera.orthoCamera);

            GameContainer._spriteBatch.End();

            ///
            GameContainer._spriteBatch.Begin(samplerState: SamplerState.LinearWrap
              , transformMatrix: camera.ViewMatrix);
            TextDrawer.BatchDraw(camera.orthoCamera);
            GameContainer._spriteBatch.End();
            ///

            GameContainer._spriteBatch.Begin(samplerState: SamplerState.LinearWrap
                , blendState: BlendState.NonPremultiplied);

            scoreBoard.Draw(GameContainer._spriteBatch, gameTime.GetElapsedSeconds());
            narrator.Draw(GameContainer._spriteBatch, gameTime.GetElapsedSeconds());

            
            

            flash.Draw(GameContainer._spriteBatch, gameTime.GetElapsedSeconds());

            GameContainer._spriteBatch.End();


            debug.CustomRender();

            //ImGui.ShowDemoWindow();


            GameContainer.GuiRenderer.EndLayout();

        }

        float accum;
        readonly float dtStatic = 1f / ticks; // For game logic
        Wrapper<float> dt = new Wrapper<float>(1f / ticks); // change so rate at which game updates is different. Dramatic Slow Motion effect!
        static Wrapper<int> ticks = new Wrapper<int>(30);
        public float alpha;
        public static KeyboardStateExtended ks;

        public static Random random = new Random();

        EventHandler eh, stopSpawning;
        EventAction ev, stopSpawningA;
        bool once;
        public void StartHardMode(object sender, EventArgs e)
        {
            SpawnControl = false;
            ScoreWhenHardModeStarted = score;
            hardEnemiesSpawn = true;
            SpawnControl = true;
            MediaPlayer.Play(GameContainer.songs["Powerup"]);
            MediaPlayer.IsRepeating = true;
        }

        public void StartSpawning(object sender, EventArgs e)
        {
            SpawnControl = true;
        }
        public void StopSpawning(object sender, EventArgs e)
        {
            SpawnControl = false;
            ShowEnergy = true;
        }

        public void InitalizeEvents()
        {
            eh += StartHardMode;
            stopSpawning += StopSpawning;
            ev = new EventAction(narrator.actor, eh);
            stopSpawningA = new EventAction(narrator.actor, stopSpawning);
        }
        public override void Update(GameTime gameTime)
        {

            MouseState ms = Mouse.GetState();
            ks = KeyboardExtended.GetState();


            if (ks.WasKeyJustDown(Keys.F))
            {
                game.FullScreen();
            }

            mouseCoordinate = camera.orthoCamera.ScreenToWorld(ms.X, ms.Y);

            if (!once && score >= ScoreToStartHardMode)
            {
                once = true;
                float scale = 0.18f;
                float y = 0.88f;
                DebugLog.Print(this, "should only add once");
                narrator.AddAction(stopSpawningA);
                narrator.AddAction(Actions.Delay(narrator.actor, 1f));
                if(life < 3)
                {
                    narrator.AddMessage("Main Font", "Are you OK? You look a little hurt...",
                   duration: 1.5f, readDelay: 2f, endDelay: 0.5f, size: scale, Interpolation.swingOut, Interpolation.smooth,
                   0.5f, 1 + scale / 2, 0.5f, y);
                    narrator.AddMessage("Main Font", "Let's continue anyways.",
                        duration: 1.5f, readDelay: 2f, endDelay: 0.5f, size: scale * 0.8f, Interpolation.swingOut, Interpolation.smooth,
                        0.5f, 1 + scale / 2, 0.5f, y);
                }
                else
                {
                    narrator.AddMessage("Main Font", "Is this too easy?",
                   duration: 1.5f, readDelay: 2f, endDelay: 0.5f, size: scale, Interpolation.swingOut, Interpolation.smooth,
                   0.5f, 1 + scale / 2, 0.5f, y);
                }
                narrator.AddMessage("Main Font", "Hmm... How can I make this more interesting?",
                        duration: 1.5f, readDelay: 2f, endDelay: 0.5f, size: scale * 0.8f, Interpolation.swingOut, Interpolation.smooth,
                        0.5f, 1 + scale / 2, 0.5f, y);
                narrator.AddMessage("Main Font", "I know!",
                    duration: 0.4f, readDelay: 1f, endDelay: 0.2f, size: scale, Interpolation.swingOut, Interpolation.smooth,
                    0.5f, 1 + scale / 2, 0.5f, y);
                narrator.AddMessage("Main Font", "I've given you a fancy new toy! :)",
                    duration: 1.5f, readDelay: 2f, endDelay: 0.5f, size: scale, Interpolation.swingOut, Interpolation.smooth,
                    0.5f, 1 + scale / 2, 0.5f, y);
                narrator.AddMessage("Main Font", "Press [Right Click] to activate your shield. Try it!",
                    duration: 1.5f, readDelay: 2f, endDelay: 0.5f, size: scale * 0.75f, Interpolation.swingOut, Interpolation.smooth,
                    0.5f, 1 + scale / 2, 0.5f, y);
                narrator.AddMessage("Main Font", "Be careful not to use all the energy in one go!",
                    duration: 1.5f, readDelay: 2f, endDelay: 0.5f, size: scale * 0.75f, Interpolation.swingOut, Interpolation.smooth,
                    0.5f, 1 + scale / 2, 0.5f, y);
                narrator.AddMessage("Main Font", "Time it right to defend yourself.",
                    duration: 1.5f, readDelay: 2f, endDelay: 0.5f, size: scale, Interpolation.swingOut, Interpolation.smooth,
                    0.5f, 1 + scale / 2, 0.5f, y);
                narrator.AddMessage("Main Font", "How about you show off your gadget to some NEW friends?",
                    duration: 1.5f, readDelay: 2f, endDelay: 0.5f, size: scale * 0.6f, Interpolation.swingOut, Interpolation.smooth,
                    0.5f, 1 + scale / 2, 0.5f, y);
                narrator.AddMessage("Main Font", "I'm sending them your way now! Good luck!",
                    duration: 1.5f, readDelay: 2f, endDelay: 0.5f, size: scale * 0.8f, Interpolation.swingOut, Interpolation.smooth,
                    0.5f, 1 + scale / 2, 0.5f, y);

                //I'm sending some NEW friends your way. Good luck!
                narrator.AddAction(ev);
            }

            accum += gameTime.GetElapsedSeconds();

            while (accum >= dt)
            {
                world.Update(gameTime);
                accum -= dt;
            }

            alpha = accum / dt;

        }

        public void TestNarrator()
        {
            if (ks.WasKeyJustDown(Keys.K))
            {
                float scale = 0.2f;

                narrator.AddMessage("Main Font", "Hello World!",
                    duration: .4f, readDelay: .5f, endDelay: 0, size: scale, Interpolation.swingOut, Interpolation.smooth, 0.5f, 1 + scale / 2, 0.5f, 1 - scale / 2);
                // 1.5 2
            }
        }

       
    }
}
