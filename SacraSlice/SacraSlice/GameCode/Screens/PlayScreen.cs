﻿using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.ViewportAdapters;
using SacraSlice.Dependencies.Engine;
using SacraSlice.Dependencies.Engine.ECS.Component;
using SacraSlice.Dependencies.Engine.ECS.Systems;
using SacraSlice.Dependencies.Engine.Scene;
using SacraSlice.GameCode.GameECS;
using SacraSlice.GameCode.GameECS.GameSystems;
using SacraSlice.GameCode.UserInterface;
using System;
using System.Collections.Generic;

namespace SacraSlice.GameCode.Screens
{
    public class PlayScreen : GameScreen
    {

        World world;

        EntityFactory entityFactory;

        GameCamera camera;

        Stack<InputManager> inputStack = new Stack<InputManager>();

        public DebugLog debug = new DebugLog();

        public static float gravity = 0.12f;

        public static float ppm = 0.25f;

        private RenderTarget2D renderTarget;

        public static Wrapper<bool> editMode = new Wrapper<bool>(false);

        public static Vector2 mouseCoordinate;

        public ScoreBoard scoreBoard;

        public Wrapper<int> score = new Wrapper<int>(0);

        //Bag<Entity> bag = new Bag<Entity>();
        public PlayScreen(GameContainer game) : base(game)
        {
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
                .AddSystem(new StateDrawUpdate(GameContainer._spriteBatch))




                // Drawing Systems

                .AddSystem(new PositionInterpolator())
                .AddSystem(new ImGuiEntityDraw())
                .AddSystem(new SpriteRenderer(GameContainer._spriteBatch, ppm))
                .AddSystem(new HitBoxRenderer(GameContainer._spriteBatch, ppm))

                // Rope

                .AddSystem(new RopePositioner())
                .AddSystem(new RopeClamperSystem(ppm))
                .AddSystem(new RopeRenderer(GameContainer._spriteBatch, ppm))

                .AddSystem(new CameraTracker())


                // Game Systems

                .AddSystem(new ResizerSystem(ppm))
                .AddSystem(new DropperSystem(score))

                .Build();

            entityFactory = new EntityFactory(world, GraphicsDevice, dtStatic);

            var viewportAdapter = new BoxingViewportAdapter(game.Window, GraphicsDevice, (int)(512 * ppm), (int)(288 * ppm));

            camera = new GameCamera(viewportAdapter);

            renderTarget = new RenderTarget2D(GraphicsDevice, 512, 288);

            //inputStack.Push(player.Get<InputManager>());

            //entityFactory.CreateDropper(ppm);

            entityFactory.CreateCamera(camera, viewportAdapter);

            camera.Position = new Vector2(0, -20);

            scoreBoard = new ScoreBoard(game, score);
        }


        public override void Draw(GameTime gameTime)
        {
            if(inputStack.Count > 0)
                inputStack.Peek().Poll();

            GameContainer.GuiRenderer.BeginLayout(gameTime);

            ImGui.Begin("Debug Window", ImGuiWindowFlags.MenuBar);

            if (ImGui.BeginMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("Open")) ; // TODO: fill these in
                    if (ImGui.MenuItem("Save")) ;
                    if (ImGui.MenuItem("Quit")) ;
                    ImGui.EndMenu();
                }
                if (ImGui.BeginMenu("Windows"))
                {
                    if (ImGui.MenuItem("Debug Log", null, debug.showWindow))
                    {
                        debug.showWindow = !debug.showWindow;

                    }
                    ImGui.EndMenu();
                }
                ImGui.EndMenuBar();
            }

            ImGui.Text("FPS: " + MathF.Round(ImGui.GetIO().Framerate));

            if (ImGui.Button("Reset Tickrate"))
            {
                ChangeTickRate(30, 0f, Interpolation.linear);
            }

            if (ImGui.Button("Slow Down Time"))
            {
                ChangeTickRate(5, 1f, Interpolation.fastSlow);
            }

            if (ImGui.Button("Speed Up Time To Normal"))
            {
                ChangeTickRate(30, 1f, Interpolation.slowFast);
            }

            actor.Act(gameTime.GetElapsedSeconds());
            ticks = (int)actor.x;

            ImGui.DragInt("Ticks per second", ref ticks, 1, 1, 30, null, ImGuiSliderFlags.AlwaysClamp);
            dt.Value = 1f / ticks;
            //TODO: get a rendertarget

            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GameContainer._spriteBatch.Begin(SpriteSortMode.BackToFront, samplerState: SamplerState.PointClamp
                , transformMatrix: camera.ViewMatrix
                , blendState: BlendState.NonPremultiplied
                );

            world.Draw(gameTime);

            ImGui.End();

            GameContainer._spriteBatch.End();

            

            GraphicsDevice.SetRenderTarget(null);

            GameContainer._spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.NonPremultiplied);
            GameContainer._spriteBatch.Draw(renderTarget, GraphicsDevice.PresentationParameters.Bounds, Color.White);

            scoreBoard.Draw(GameContainer._spriteBatch);


            GameContainer._spriteBatch.End();


            debug.CustomRender();

            //ImGui.ShowDemoWindow();


            GameContainer.GuiRenderer.EndLayout();

        }

        float accum;
        readonly float dtStatic = 1f / ticks; // For game logic
        Wrapper<float> dt = new Wrapper<float>(1f / ticks); // change so rate at which game updates is different. Dramatic Slow Motion effect!
        static int ticks = 30;
        public float alpha;

        public override void Update(GameTime gameTime)
        {

            MouseState ms = Mouse.GetState();
            mouseCoordinate = camera.orthoCamera.ScreenToWorld(ms.X, ms.Y);

            if (KeyboardExtended.GetState().WasKeyJustDown(Keys.K))
                entityFactory.CreateDropper(ppm);

            accum += gameTime.GetElapsedSeconds();

            while (accum >= dt)
            {
                world.Update(gameTime);
                accum -= dt;
            }

            alpha = accum / dt;

        }

        static Actor actor = new Actor(ticks, 0);

        public static void ChangeTickRate(int newTickRate, float time, Interpolation interpolation)
        // Adds an action to change the tick rate.
        {
            actor.x = ticks;
            actor.AddAction(Actions.MoveTo(actor, newTickRate, 0, time, interpolation));
        }
    }
}