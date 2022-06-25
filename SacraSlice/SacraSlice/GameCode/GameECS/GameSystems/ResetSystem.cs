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
    public class ResetSystem : EntityDrawSystem
    {

        private ComponentMapper<Position> posMapper;
        private ComponentMapper<Timer> timeMapper;
        Wrapper<bool> reset;
        EntityFactory ef;
        public ResetSystem(EntityFactory ef, Wrapper<bool> reset) : base(Aspect.All(typeof(string), typeof(Position),
            typeof(StateManager), typeof(Timer), typeof(AnimationStateManager)))
        {
            this.ef = ef;
            this.reset = reset;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            posMapper = mapperService.GetMapper<Position>();
            timeMapper = mapperService.GetMapper<Timer>();
        }

        public override void Draw(GameTime gameTime)
        {
            if (reset)
            {
                foreach (var entity in ActiveEntities)
                {
                    var time = timeMapper.Get(entity);
                    if (time.GetSwitch("IsDemon"))
                        continue;
                    var pos = posMapper.Get(entity);
                    ef.Free((int)pos.start);
                    DestroyEntity(entity);
                }
                reset.Value = false;
            }
           

        }

    }
}
