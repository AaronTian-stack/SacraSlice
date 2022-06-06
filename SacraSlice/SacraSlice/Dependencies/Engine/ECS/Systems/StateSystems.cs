using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using SacraSlice.Dependencies.Engine.Animation;
using SacraSlice.Dependencies.Engine.ECS.Component;
using SacraSlice.Dependencies.Engine.States;
using System;
using System.Diagnostics;

namespace SacraSlice.Dependencies.Engine.ECS.Systems
{
    public class StateUpdater : EntityUpdateSystem
    {
        Wrapper<bool> overRide;
        bool once, once2;
        private ComponentMapper<StateManager> managerMapper;

        public StateUpdater(Wrapper<bool> overRide) : base(Aspect.All(typeof(StateManager)))
        {
            this.overRide = overRide;
        }
        public override void Initialize(IComponentMapperService mapperService)
        {
            managerMapper = mapperService.GetMapper<StateManager>();
        }
        public override void Update(GameTime gameTime)
        {
            foreach (var entity in ActiveEntities)
            {


                var sm = managerMapper.Get(entity);

                if (overRide)
                {
                    once2 = false;
                    if (!once)
                    {
                        sm.currentState = sm.defaultState;
                        once = true;
                    }
                }
                else
                {
                    once = false;
                    if (!once2 && sm.resetState != null)
                    {
                        sm.currentState = sm.resetState;
                        sm.currentState.pos.gravity = true;
                        once2 = true;
                    }
                }


                sm.Update();

            }
        }
    }

    public class StateDrawUpdate : EntityDrawSystem
    {
        SpriteBatch sb;
        private ComponentMapper<StateManager> managerMapper;

        public StateDrawUpdate(SpriteBatch sb) : base(Aspect.All(typeof(StateManager)))
        {
            this.sb = sb;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            managerMapper = mapperService.GetMapper<StateManager>();
        }
        public override void Draw(GameTime gameTime)
        {
            foreach (var entity in ActiveEntities)
            {

                var sm = managerMapper.Get(entity);
                sm.currentState.Draw(sb, gameTime.GetElapsedSeconds());
            }
        }

    }


}
