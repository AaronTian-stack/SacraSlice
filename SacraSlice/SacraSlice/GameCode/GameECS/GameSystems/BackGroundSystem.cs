using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using SacraSlice.Dependencies.Engine;
using SacraSlice.Dependencies.Engine.ECS.Component;
using SacraSlice.Dependencies.Engine.InterfaceLayout;
using SacraSlice.Dependencies.Engine.Scene;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SacraSlice.GameCode.GameECS.GameSystems
{
    public class RandomCube 
    {
        public Sprite sprite = new Sprite(GameContainer.atlas.FindRegion("randomcube"));
        public Actor a = new Actor();
        float rotation;
        GameCamera cam;
        public RandomCube(GameCamera cam)
        {
            this.cam = cam;
        }

        public void Draw(SpriteBatch sb, float dt)
        {
            a.Act(dt);

            sprite.Position.X = a.x / cam.Zoom;
            sprite.Position.Y = a.y / cam.Zoom;
            sprite.Rotation += rotation;
            sprite.Color = BackGroundSystem.blueDark;
            sprite.Scale /= cam.Zoom;
            sprite.Draw(sb, 0.9999f);
            sprite.Scale *= cam.Zoom;
        }

        public void Reset(float duration, float rotation, Vector2 start, Vector2 finish, Vector2 scale)
        {
            this.rotation = rotation;
            a.ClearActions();
            a.AddAction(Actions.MoveFrom(a, start.X, start.Y, finish.X, finish.Y, duration, Interpolation.linear));
            sprite.Scale = scale;
        }
    }

    public class BackGroundSystem : EntityDrawSystem
    {
        SpriteBatch sb;
        Timer slope;
        float ppm;
        bool floor;
        public BackGroundSystem(SpriteBatch sb, Timer slope, float ppm, bool floor) : base(Aspect.All(typeof(GameCamera)))
        {
            this.sb = sb;
            this.slope = slope;
            this.ppm = ppm;
            this.floor = floor;
        }
        // Random Rectangles floating up

        Bag<RandomCube> activeCubes = new Bag<RandomCube>();
        Bag<RandomCube> bag = new Bag<RandomCube>();
        private ComponentMapper<GameCamera> camMapper;

        Color blueLight = new Color(72, 74, 119);
        public static Color blueDark = new Color(50, 51, 83);
        Vector2 lastpos;

        FastRandom rnd = new FastRandom(69);
        float timer = 0;
        float change;

        Sprite pixel = new Sprite(GameContainer.atlas.FindRegion("whitepixel"));
        public void ResetCube(RandomCube cube, RectangleF b1)
        {
            float x = rnd.NextSingle(b1.Left, b1.Right);

            cube.Reset(
                rnd.NextSingle(8f, 15f)
                , rnd.NextSingle(0, 1f)
                , new Vector2(x, b1.Bottom + cube.sprite.BoundingBox.Height / 2)
                , new Vector2(x, b1.Top - cube.sprite.BoundingBox.Height / 2)
                , new Vector2(rnd.NextSingle(0.1f, 0.2f)));
        }
        float cubeSpawnFrequency = 2f;
        float moveDuration = 2f;
        float timer2;
        RectangleF lastRect;
        public override void Draw(GameTime gameTime)
        {

            // TODO: render the floor sprite
            float slope = this.slope.GetTimer("slope");
            int number = (int)(10 * (1 / Math.Abs(slope)));
            float drawoffset = 14f;

            var thickness = 6f;
            var direction = -1;
            //float oldZoom = 0;

            timer2 += gameTime.GetElapsedSeconds();

            foreach (var entity in ActiveEntities)
            {
                var cam = camMapper.Get(entity);
                var b = cam.orthoCamera.BoundingRectangle;

                //var b1 = cam.orthoCamera.BoundingRectangle;
                //b1.Position = new Vector2();

                // if camera is not shaking
                if (cam.IsShaking)
                    //b1.Position = b.Position;
                //else
                {
                    b.Position = lastpos;
                }
                   

                var offset = b.Width / number;

                var X = timer2 / moveDuration * offset * direction;
                if(timer2 > moveDuration)
                {
                    timer2 = 0;
                }

                timer += gameTime.GetElapsedSeconds();
                if(timer > change)
                {
                    timer = 0;
                    change = rnd.NextSingle(0, cubeSpawnFrequency);
                    // spawn a cube
                    if(bag.Count != 0)
                    {
                        var cube = bag[0];
                        activeCubes.Add(cube);
                        bag.RemoveAt(0);

                        ResetCube(cube, b);

                        cube.sprite.Position.Y = b.Bottom;
                    }
                    else
                    {
                        var cube = new RandomCube(cam);
                        activeCubes.Add(cube);
                        ResetCube(cube, b);
                        cube.sprite.Position.Y = b.Bottom;
                    }

                }

                int j = 0;
                while (j < activeCubes.Count)
                {
                    activeCubes[j].Draw(sb, gameTime.GetElapsedSeconds());
                    if (activeCubes[j].a.actions.Count == 0)
                    {
                        activeCubes.RemoveAt(j);
                        continue;
                    }
                    j++;
                }

                var bCopy = b;
                bCopy.Inflate(10, 10);

                sb.FillRectangle(bCopy, blueLight, 0.999999f);

                var xRight = b.Width * 2;

                var yRight = xRight * slope + b.Bottom;

                var bl = b.BottomLeft;
                bl.X -= drawoffset;
                bl.Y += drawoffset;

                var tr = new Vector2(xRight, yRight);
                tr.X -= drawoffset;
                tr.Y += drawoffset;

                for(int i = -number; i < number; i++)
                {
                    Vector2 BL = bl;
                    Vector2 TR = tr;

                    BL.X += i * offset + X;
                    TR.X += i * offset + X;

                    sb.DrawLine(BL, TR, blueDark, ppm * thickness / cam.Zoom, 0.99f);
                }

                //sb.DrawLine(bl, tr, Color.Red, 1, 1);

                lastpos = b.Position;
                lastRect = b;
                //oldZoom = cam.Zoom;
            }

            if (floor)
            {
                // draw the floor
                pixel.Color = Color.White;

                pixel.Color = new Color(155, 171, 178);
                pixel.Origin = new Vector2(0.5f, 0.5f);


                //var h = lastRect.Bottom - (lastRect.Bottom - lastRect.Top) * 0.25f * 0.5f;

                var h = 3.75f;

                pixel.Position = new Vector2(0, h);
                pixel.Scale = new Vector2(lastRect.Width * 1.1f, 40);
                pixel.Draw(sb, 0.97f);

                pixel.Color = Color.Black;

                pixel.Position = new Vector2(0, h - 1);
                pixel.Draw(sb, 0.971f);
            }

        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            camMapper = mapperService.GetMapper<GameCamera>();
        }

        
    }
}
