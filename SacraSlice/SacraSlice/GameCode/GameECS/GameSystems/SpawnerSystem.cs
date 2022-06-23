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
using SacraSlice.Dependencies.Engine.Animation;
using SacraSlice.GameCode.GameECS.GameComponents;
using SacraSlice.GameCode.Screens;
using Microsoft.Xna.Framework.Input;

namespace SacraSlice.GameCode.GameECS.GameSystems
{
    public class SpawnerSystem : EntityDrawSystem
    {
        private ComponentMapper<string> stringMapper;
        private ComponentMapper<Position> posMapper;
        private ComponentMapper<StateManager> smMapper;
        private ComponentMapper<Timer> timeMapper;
        private ComponentMapper<AnimationStateManager> aMapper;
        private ComponentMapper<Sword> swordMapper;
        EntityFactory ef;
        float ppm;
        Wrapper<int> score;

        Wrapper<float> dt;
        float dtStatic;
        public SpawnerSystem(EntityFactory ef, float ppm, Wrapper<int> score, Wrapper<float> dt, float dtStatic) : base(Aspect.All(typeof(string), typeof(Position),
            typeof(StateManager), typeof(Timer), typeof(AnimationStateManager)))
        {
            this.ef = ef;
            this.ppm = ppm;
            this.score = score;
            this.dt = dt;
            this.dtStatic = dtStatic;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            stringMapper = mapperService.GetMapper<string>();
            posMapper = mapperService.GetMapper<Position>();
            smMapper = mapperService.GetMapper<StateManager>();
            timeMapper = mapperService.GetMapper<Timer>();
            aMapper = mapperService.GetMapper<AnimationStateManager>();
            swordMapper = mapperService.GetMapper<Sword>();
        }
        float timer = 0;
        public override void Draw(GameTime gameTime)
        {
            bool created = false;

            if (PlayScreen.enemiesOnScreen < PlayScreen.MaxEnemiesOnScreen && PlayScreen.SpawnControl)
                timer += gameTime.GetElapsedSeconds() * (dtStatic / dt);
            else
                timer = 0;


            if (timer > PlayScreen.SpawnGapVariable && PlayScreen.SpawnControl) // timer > gap
            {
                timer = 0;
                //DebugLog.Print("Spawner", "trying to spawn");

                foreach (var entity in ActiveEntities)
                {

                    var t = timeMapper.Get(entity);

                    if (!t.GetSwitch("dead")) 
                    {
                        //DebugLog.Print("Spawner", "still alive");
                        continue;
                    } 
                      

                    var s = stringMapper.Get(entity);

                    //if(s.Equals("Dropping Item"))
                    //{

                        var p = posMapper.Get(entity);
                        var sm = smMapper.Get(entity);
                        //DebugLog.Print("Spawner", "FOUND ONE! "+entity);

                        t.GetSwitch("dead").Value = false;
                        t.GetSwitch("Shrink").Value = false;
                        ef.ResetPosition(p, 0);
                            
                        ef.ResetVelocity(p);

                        ef.RandomAnimation(aMapper.Get(entity), t);

                        if (swordMapper.Get(entity) == null && PlayScreen.hardEnemiesSpawn)
                        {
                            t.GetSwitch("sword").Value = true;
                            GetEntity(entity).Attach(new Sword(PlayScreen.random.Next(1000), t));
                        }
                        else if (swordMapper.Get(entity) != null)
                            swordMapper.Get(entity).Reset();


                        p.ground = false;
                        sm.SetStateUpdate("Fall", timeMapper.Get(entity));
                        created = true;
                        PlayScreen.enemiesOnScreen.Value++;
                        break;

                    //}
                }
                if(!created && PlayScreen.enemiesOnScreen < PlayScreen.MaxEnemiesOnScreen)
                    ef.CreateDropper(ppm, score);


            }

           
        }

    }
}
