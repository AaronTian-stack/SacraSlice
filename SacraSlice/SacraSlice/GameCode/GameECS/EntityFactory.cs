using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Entities;
using MonoGame.Extended.ViewportAdapters;
using SacraSlice.Dependencies.Engine;
using SacraSlice.Dependencies.Engine.Animation;
using SacraSlice.Dependencies.Engine.ECS.Component;
using SacraSlice.Dependencies.Engine.States;
using SacraSlice.GameCode.GameECS.GameComponents;
using SacraSlice.GameCode.GameStates.Demon;
using SacraSlice.GameCode.GameStates.Enemy;
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
            occupy = new bool[3];
        }
        public void Initialize(World world, GraphicsDevice graphics, float dt)
        {
            this.world = world;
            //this.graphics = graphics;
            this.dt = dt;
        }
        public void CreateCamera(GameCamera camera, ViewportAdapter viewportAdapter, Position target = null)
        {
            var entity = world.CreateEntity();

            if (target != null)
            {
                camera.targets.Push(target);
                camera.LookAt(new Vector2(target.renderPosition.X, target.renderPosition.Y));
            }
            else
                camera.LookAt(new Vector2(0, 0));
               

            camera.interpolationPosition.Push(Interpolation.linear);
            camera.interpolationZoom.Push(Interpolation.linear);
            camera.interpolationRotation.Push(Interpolation.linear);

            camera.targetRotations.Push(0);


            camera.speedPosition.Push(0.06f);
            camera.speedZoom.Push(0.125f);
            camera.speedRotation.Push(0.125f);

            entity.Attach(camera);
            entity.Attach(viewportAdapter);

            var input = new InputManager(new List<string> { "Left", "Right", "Down", "Up", "ZoomIn", "ZoomOut", "RotateLeft", "RotateRight" },
            new List<Keys> { Keys.A, Keys.D, Keys.S, Keys.W, Keys.I, Keys.K, Keys.G, Keys.H });

            entity.Attach(input);
            entity.Attach("Game Camera");

        }

        float ra = 48f;
        bool[] occupy;
        public void Free(int t)
        {
            occupy[t] = false;
        }
        public bool ResetPosition(Position p, int counter)
        {

            var bruh2 = rand.Next(0, 3);

            p.start = bruh2;

            if (occupy[bruh2])
            {
                ResetPosition(p, 0);
                return false;
            }
            occupy[bruh2] = true;

            bruh2 -= 1;
            Vector2 pos = new Vector2(bruh2 * ra, -100);

            p.SetAllPosition(pos, rand.NextSingle(0.004f, .8f));

            return true;
        }

        public void ResetVelocity(Position p)
        {
            p.velocity.Y = rand.NextSingle(0, 10);
        }

        public void RandomAnimation(AnimationStateManager asm)
        {
            List<TextureRegion> frames = null;
            var bruh = rand.Next(0, 2);
            string sphere, cube;
            bool b = false;
            if(PlayScreen.hardEnemiesSpawn)
            {
                sphere = "sphereLegs";
                cube = "cubeLegs";
                b = true;
            }
            else
            {
                sphere = "sphere";
                cube = "cube";
            }
          

            switch (bruh)
            {
                case 0:
                    frames = GameContainer.atlas.FindRegions(sphere);
                    break;
                case 1:
                    frames = GameContainer.atlas.FindRegions(cube);
                    break;
            }

            foreach (var s in asm.states)
            {
                asm.GetAnimation(s).KeyFrames = frames;
                if(b)
                    asm.GetAnimation(s).SetFrameDuration(0.2f);
            }
        }

        Random rand = new Random();
        float boxSize = 20f;
        float height = 30f;
        int bruh;
        public Entity CreateDropper(float ppm, int score)
        {
            var entity = world.CreateEntity();
            PlayScreen.enemiesOnScreen.Value++;
            entity.Attach("Dropping Item "+bruh);
            bruh++;
            Position p = new Position();
            p.gravity = true;

            ResetPosition(p, 0);
            ResetVelocity(p);

            entity.Attach(p);


            StateManager sm = new StateManager();

            Timer t = new Timer();

            HitBox hb = new HitBox();

            FallState f = new FallState(sm, dt, p, t);

            CutState c = new CutState(sm, dt, p, t, hb, ppm);

            var sprite = new Sprite();
            ShrinkState s = new ShrinkState(sm, p, t, PlayScreen.score, PlayScreen.life, sprite, this);

            sm.AddState(f);
            sm.AddState(c);
            sm.AddState(s);

            sm.currentState = f;

            AnimationStateManager asm = new AnimationStateManager();

            //string sphere = "sphereLegs";
            //string cube = "cubeLegs";

            switch (rand.Next(0, 2)) 
            {
                case 0:
                    asm.AddAnimation(f, new Animation<TextureRegion>("Sphere", 0.5f, null, PlayMode.LOOP));
                    asm.AddAnimation(c, new Animation<TextureRegion>("Sphere", 0.5f, null, PlayMode.LOOP));
                    asm.AddAnimation(s, new Animation<TextureRegion>("Sphere", 0.5f, null, PlayMode.LOOP));
                    break;
                case 1:
                    asm.AddAnimation(f, new Animation<TextureRegion>("Cube", 0.5f, null, PlayMode.LOOP));
                    asm.AddAnimation(c, new Animation<TextureRegion>("Cube", 0.5f, null, PlayMode.LOOP));
                    asm.AddAnimation(s, new Animation<TextureRegion>("Cube", 0.5f, null, PlayMode.LOOP));
                    break;
            }
            RandomAnimation(asm);

            hb.AddState(f, new RectangleF(0, 0, boxSize, height));
            hb.AddState(c, new RectangleF(0, 0, boxSize, height));
            hb.AddState(s, new RectangleF(0, 0, boxSize, height));
            hb.renderFlag = HitBoxFlag.Bottom;

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

            entity.Attach(sprite);
            entity.Attach(t);
            entity.Attach(hb);
            entity.Attach(asm);
            entity.Attach(sm);

            entity.Attach(sqm);

            if(PlayScreen.hardEnemiesSpawn)
            {
                t.GetSwitch("sword").Value = true;
                Sword sword = new Sword(rand.Next(1000), t);
                entity.Attach(sword);
            }

            return entity;
        }

        public Entity CreateDemon()
        {
            var entity = world.CreateEntity();

            Position p = new Position();

            p.gravity = false;

            p.SetAllPosition(new Vector2(0, 200), 0.000003f);

            entity.Attach(p);
            entity.Attach("Demon");

            Timer t = new Timer();
            entity.Attach(t);
            t.GetSwitch("no shadow").Value = true;
            t.GetSwitch("IsDemon").Value = true;

            StateManager sm = new StateManager();
            DemonState state = new DemonState(sm, dt, p, t);
            sm.currentState = state;

            entity.Attach(sm);

            AnimationStateManager asm = new AnimationStateManager();

            asm.AddAnimation(state, new Animation<TextureRegion>("spook", 0.2f, GameContainer.atlas.FindRegions("spook"), PlayMode.LOOP));

            entity.Attach(asm);

            entity.Attach(new Sprite());

            return entity;
        }


    }
}
