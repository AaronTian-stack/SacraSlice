using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.ViewportAdapters;
using SacraSlice.Dependencies.Engine;
using SacraSlice.Dependencies.Engine.Animation;
using SacraSlice.Dependencies.Engine.ECS.Component;
using SacraSlice.Dependencies.Engine.States;
using SacraSlice.GameCode.GameStates;
using SacraSlice.GameCode.Screens;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SacraSlice.GameCode.GameECS
{
    public class EntityFactory
    {
        World world;
        //GraphicsDevice graphics;
        float dt;

        public EntityFactory()
        {

        }
        public void Initialize(World world, GraphicsDevice graphics, float dt)
        {
            this.world = world;
            //this.graphics = graphics;
            this.dt = dt;
        }
        public void CreateCamera(GameCamera camera, BoxingViewportAdapter viewportAdapter, Position target = null)
        {
            var entity = world.CreateEntity();

            if (target != null)
            {
                camera.targets.Push(target);
                camera.LookAt(new Vector2(target.renderPosition.X, target.renderPosition.Y));
            }
            else
                camera.LookAt(new Vector2(0, 0));
               

            camera.interpolationP.Push(Interpolation.linear);
            camera.interpolationZ.Push(Interpolation.linear);

            //camera.targetZooms.Push(1);

            camera.speedP.Push(0.06f);
            camera.speedZ.Push(0.125f);

            entity.Attach(camera);
            entity.Attach(viewportAdapter);

            var input = new InputManager(new List<string> { "Left", "Right", "Down", "Up", "ZoomIn", "ZoomOut", "RotateLeft", "RotateRight" },
            new List<Keys> { Keys.A, Keys.D, Keys.S, Keys.W, Keys.I, Keys.K, Keys.G, Keys.H });

            entity.Attach(input);
            entity.Attach("Game Camera");

        }

        public static void ResetPosition(Position p)
        {
            p.SetAllPosition(new Vector2(rand.NextSingle(-40, 40), -100), rand.NextSingle(0, 1));
            //p.velocity.Y = rand.NextSingle(0, 10);
        }

        public static void ResetVelocity(Position p)
        {
            //p.SetAllPosition(new Vector2(rand.NextSingle(-40, 40), -100), rand.NextSingle(0, 1));
            p.velocity.Y = rand.NextSingle(0, 10);
        }


        static Random rand = new Random();
        public Entity CreateDropper(float ppm)
        {
            var entity = world.CreateEntity();

            entity.Attach("Dropping Item");

            Position p = new Position();
            p.gravity = true;


            ResetPosition(p);
            ResetVelocity(p);

            // maybe give random velocity

            entity.Attach(p);


            StateManager sm = new StateManager();

            Timer t = new Timer();

            HitBox hb = new HitBox();

            FallState f = new FallState(sm, dt, p, t);

            Wrapper<bool> dead = new Wrapper<bool>(false);

            CutState c = new CutState(sm, dt, p, t, hb, ppm);

            ShrinkState s = new ShrinkState(sm, p, t, dead, PlayScreen.score);

            sm.AddState(f);
            sm.AddState(c);
            sm.AddState(s);
            sm.currentState = f;

            AnimationStateManager asm = new AnimationStateManager();

            switch (rand.Next(0, 2)) 
            {
                case 0:
                    asm.AddState(f, new Animation<TextureRegion>("Sphere", 0.5f, GameContainer.atlas.FindRegions("sphere"), PlayMode.LOOP));
                    asm.AddState(c, new Animation<TextureRegion>("Sphere", 0.5f, GameContainer.atlas.FindRegions("sphere"), PlayMode.LOOP));
                    asm.AddState(s, new Animation<TextureRegion>("Sphere", 0.5f, GameContainer.atlas.FindRegions("sphere"), PlayMode.LOOP));
                    break;
                case 1:
                    asm.AddState(f, new Animation<TextureRegion>("Cube", 0.5f, GameContainer.atlas.FindRegions("cube"), PlayMode.LOOP));
                    asm.AddState(c, new Animation<TextureRegion>("Cube", 0.5f, GameContainer.atlas.FindRegions("cube"), PlayMode.LOOP));
                    asm.AddState(s, new Animation<TextureRegion>("Cube", 0.5f, GameContainer.atlas.FindRegions("cube"), PlayMode.LOOP));
                    break;
            }

            


            

            hb.AddState(f, new RectangleF(0,0,25,25));
            hb.AddState(c, new RectangleF(0,0,25,25));
            hb.AddState(s, new RectangleF(0, 0, 25, 25));

            (Wrapper<float>, float) idleT = (t.GetTimer("Ground Time"), 3.5f);
            SquashValue idleSQ = new SquashValue(c, Interpolation.swingOut, 
                Interpolation.swingOut, new Vector2(2, 0.25f), new Vector2(1f, 1f), idleT, idleT);

            (Wrapper<float>, float) fallT = (t.GetTimer("State Time"), 0.7f);
            SquashValue fallSQ = new SquashValue(f, Interpolation.pow2In, 
                Interpolation.pow2In, new Vector2(1f, 1f), new Vector2(0.5f, 1.2f), fallT, fallT);

            (Wrapper<float>, float) death = (t.GetTimer("State Time"), 6f);
            (Wrapper<float>, float) death2 = (t.GetTimer("State Time"), 5.8f);
            SquashValue dieSQ = new SquashValue(s, Interpolation.swingIn,
                Interpolation.pow2In, new Vector2(1f, 1f), new Vector2(0, 0f), death, death2);

            SquashManager sqm = new SquashManager(idleSQ, fallSQ, dieSQ);

            entity.Attach(new Sprite());
            entity.Attach(t);
            entity.Attach(hb);
            entity.Attach(asm);
            entity.Attach(sm);

            entity.Attach(sqm);

            entity.Attach(dead);

            return entity;
        }


    }
}
