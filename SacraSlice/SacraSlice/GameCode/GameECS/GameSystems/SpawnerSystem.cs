using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Text;
using MonoGame.Extended;
using SacraSlice.Dependencies.Engine.ECS.Component;
using SacraSlice.Dependencies.Engine.States;
using SacraSlice.Dependencies.Engine;

namespace SacraSlice.GameCode.GameECS.GameSystems
{
    public class SpawnerSystem : EntityDrawSystem
    {
        private ComponentMapper<string> stringMapper;
        private ComponentMapper<Position> posMapper;
        private ComponentMapper<Wrapper<bool>> boolMapper;
        private ComponentMapper<StateManager> smMapper;
        private ComponentMapper<Timer> timeMapper;
        EntityFactory ef;
        float ppm;
        public SpawnerSystem(EntityFactory ef, float ppm) : base(Aspect.All(typeof(string), typeof(Position), typeof(Wrapper<bool>),
            typeof(StateManager), typeof(Timer)))
        {
            this.ef = ef;
            this.ppm = ppm;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            stringMapper = mapperService.GetMapper<string>();
            posMapper = mapperService.GetMapper<Position>();
            boolMapper = mapperService.GetMapper<Wrapper<bool>>();
            smMapper = mapperService.GetMapper<StateManager>();
            timeMapper = mapperService.GetMapper<Timer>();
        }
        float timer;
        float gap = 2;
        public override void Draw(GameTime gameTime)
        {
            bool created = false;
            timer += gameTime.GetElapsedSeconds();

            if(timer > gap)
            {
                timer = 0;
                DebugLog.Print("Spawner", "trying to spawn");

                foreach (var entity in ActiveEntities)
                {

                    var b = boolMapper.Get(entity);

                    if (!b) 
                    {
                        DebugLog.Print("Spawner", "still alive");
                        continue;
                    } 
                      

                    var s = stringMapper.Get(entity);

                    if(s.Equals("Dropping Item"))
                    {

                        var p = posMapper.Get(entity);
                        var sm = smMapper.Get(entity);
                        DebugLog.Print("Spawner", "FOUND ONE! "+entity);

                        EntityFactory.ResetPosition(p);
                        EntityFactory.ResetVelocity(p);
                        p.ground = false;
                        sm.SetStateUpdate("Fall", timeMapper.Get(entity));
                        created = true;
                        break;

                    }
                }
                if(!created)
                    ef.CreateDropper(ppm);


            }

           
        }

    }
}
