using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using SacraSlice.Dependencies.Engine;
using SacraSlice.Dependencies.Engine.ECS.Component;
using SacraSlice.Dependencies.Engine.Scene;
using System;
using System.Collections.Generic;
using System.Text;

namespace SacraSlice.GameCode.GameECS.GameSystems
{
    public class RandomCube 
    {
        public Sprite sprite = new Sprite(GameContainer.atlas.FindRegion("randomcube"));
        public Actor a = new Actor();
        float rotation;
        public RandomCube()
        {

        }

        public void Draw(SpriteBatch sb, float dt)
        {
            a.Act(dt);
            
            sprite.Position.X = a.x;
            sprite.Position.Y = a.y;
            sprite.Rotation += rotation;
            sprite.Color = BackGroundSystem.blueDark;

            sprite.Draw(sb, 1);
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
        public BackGroundSystem(SpriteBatch sb, Timer slope, float ppm) : base(Aspect.All(typeof(GameCamera)))
        {
            this.sb = sb;
            this.slope = slope;
            this.ppm = ppm;
        }
        // Random Rectangles floating up

        Bag<RandomCube> activeCubes = new Bag<RandomCube>();
        Bag<RandomCube> bag = new Bag<RandomCube>();
        private ComponentMapper<GameCamera> camMapper;

        Color blueLight = new Color(72 / 255f, 74 / 255f, 119 / 255f);
        public static Color blueDark = new Color(50 / 255f, 51 / 255f, 83 / 255f);
        Vector2 lastpos;

        Actor a = new Actor();
        bool bruh;
        FastRandom rnd = new FastRandom(69);
        float timer = 0;
        float change;

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
        public override void Draw(GameTime gameTime)
        {

            // TODO: render the floor sprite
            float slope = this.slope.GetTimer("slope");
            int number = (int)(10 * (1 / Math.Abs(slope)));
            float drawoffset = 10f;

            var thickness = 6f;

            a.Act(gameTime.GetElapsedSeconds());

            foreach (var entity in ActiveEntities)
            {
                var cam = camMapper.Get(entity);
                var b = cam.orthoCamera.BoundingRectangle;

                var b1 = cam.orthoCamera.BoundingRectangle;
                b1.Position = new Vector2();

                // if camera is not shaking
                if (!cam.IsShaking)
                    b1.Position = b.Position;
                else
                    b1.Position = lastpos;

                var offset = b1.Width / number;

                if (!bruh)
                {
                    a.AddAction(new RepeatAction(a,
                Actions.MoveFrom(a, 0, 0, -offset, 0, 2, Interpolation.linear)
                , Actions.MoveFrom(a, 0, 0, 0, 0, 0, Interpolation.linear)));
                    bruh = true;
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

                        ResetCube(cube, b1);

                        cube.sprite.Position.Y = b1.Bottom;
                    }
                    else
                    {
                        var cube = new RandomCube();
                        activeCubes.Add(cube);
                        ResetCube(cube, b1);
                        cube.sprite.Position.Y = b1.Bottom;
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

                //sb.DrawLine(b1.BottomLeft, b1.TopRight, Color.Red, 1, 1);


                var xRight = b1.Width * 2;

                var yRight = xRight * slope + b1.Bottom;

                var bl = b1.BottomLeft;
                bl.X -= drawoffset;
                bl.Y += drawoffset;

                var tr = new Vector2(xRight, yRight);
                tr.X -= drawoffset;
                tr.Y += drawoffset;

                for(int i = -number; i < number; i++)
                {
                    Vector2 BL = bl;
                    Vector2 TR = tr;

                    BL.X += i * offset + a.x;
                    TR.X += i * offset + a.x;

                    sb.DrawLine(BL, TR, blueDark, ppm * thickness, 0.99f);
                }

                //sb.DrawLine(bl, tr, Color.Red, 1, 1);

                lastpos = b1.Position;
            }
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            camMapper = mapperService.GetMapper<GameCamera>();
        }

        
    }
}
