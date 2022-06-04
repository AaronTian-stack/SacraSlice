using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using SacraSlice.Dependencies.Engine.ECS.Component;
using System;
using System.Collections.Generic;
using System.Text;

namespace SacraSlice.Dependencies.Engine.ECS.Systems
{
    public class TimerUpdater : EntityDrawSystem
    {
        Wrapper<float> dt;
        float dtStatic;
        private ComponentMapper<Position> positionMapper;
        private ComponentMapper<Timer> timerMapper;

        public TimerUpdater(Wrapper<float> dt, float dtStatic) : base(Aspect.All(typeof(Timer), typeof(Position)))
        {
            this.dt = dt;
            this.dtStatic = dtStatic;
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var entity in ActiveEntities)
            {

                var pos = positionMapper.Get(entity);
                var t = timerMapper.Get(entity);

                if (pos.ground)
                {
                    t.GetTimer("Air Time").Value = 0;
                    t.GetTimer("Ground Time").Value += gameTime.GetElapsedSeconds() * (dtStatic / dt);

                }
                else
                {

                    t.GetTimer("Ground Time").Value = 0;
                    t.GetTimer("Air Time").Value += gameTime.GetElapsedSeconds() * (dtStatic / dt);
                }

            }
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            positionMapper = mapperService.GetMapper<Position>();
            timerMapper = mapperService.GetMapper<Timer>();
        }

    }
}
