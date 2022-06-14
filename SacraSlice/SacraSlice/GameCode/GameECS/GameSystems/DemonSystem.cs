using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using SacraSlice.Dependencies.Engine;
using SacraSlice.Dependencies.Engine.ECS.Component;
using SacraSlice.Dependencies.Engine.Scene;
using SacraSlice.GameCode.Screens;
using System;
using System.Collections.Generic;
using System.Text;

namespace SacraSlice.GameCode.GameECS.GameSystems
{
    public class DemonSystem : EntityDrawSystem
    {
        SpriteBatch sb;
        float ppm;
        private ComponentMapper<Position> posMapper;
        private ComponentMapper<Timer> tMapper;

        public DemonSystem(SpriteBatch sb, float ppm) : base(Aspect.All(typeof(Position), typeof(Timer)))
        {
            this.sb = sb;
            this.ppm = ppm;
            spawn = random.Next(range.X, range.Y);
        }

        float timer;
        float spawn;
        Point range = new Point(10, 30);
        Random random = new Random();

        Actor a = new Actor();
        public override void Draw(GameTime gameTime)
        {
            foreach (var entity in ActiveEntities)
            {
                var pos = posMapper.Get(entity);
                var time = tMapper.Get(entity);

                if (!time.GetSwitch("IsDemon") || PlayScreen.score < PlayScreen.ScoreToSpawnDemon) continue;

                if(timer > spawn)
                {
                    timer = 0;
                    spawn = random.Next(range.X, range.Y);

                }


            }
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            posMapper = mapperService.GetMapper<Position>();
            tMapper = mapperService.GetMapper<Timer>();
        }


    }
}
