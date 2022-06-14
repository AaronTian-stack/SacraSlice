using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using SacraSlice.Dependencies.Engine.Animation;
using SacraSlice.Dependencies.Engine.States;
using System;
using System.Collections.Generic;
using System.Text;

namespace SacraSlice.Dependencies.Engine.ECS.Systems
{
    public class SquashAnimation : EntityDrawSystem
    {
        private ComponentMapper<SquashManager> squashMapper;
        private ComponentMapper<Sprite> spriteMapper;
        private ComponentMapper<StateManager> stateMapper;

        public SquashAnimation() : base(Aspect.All(typeof(SquashManager), typeof(Sprite), typeof(StateManager))) { }

        public override void Draw(GameTime gameTime)
        {
            foreach (var entity in ActiveEntities)
            {

                var sqm = squashMapper.Get(entity);
                var sprite = spriteMapper.Get(entity);
                var sm = stateMapper.Get(entity);

                SquashValue sv;
                float x = 1;
                float y = 1;

                if (sqm.lookup.TryGetValue(sm.currentState, out sv))
                {
                    x = sv.interpolationX.Apply(sv.startValues.X, sv.endValues.X, MathF.Min(1, sv.timeX.Item1.Value * sv.timeX.Item2));
                    y = sv.interpolationY.Apply(sv.startValues.Y, sv.endValues.Y, MathF.Min(1, sv.timeY.Item1.Value * sv.timeY.Item2));
                }


                sprite.Scale = new Vector2(x, y);
            }
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            squashMapper = mapperService.GetMapper<SquashManager>();
            spriteMapper = mapperService.GetMapper<Sprite>();
            stateMapper = mapperService.GetMapper<StateManager>();
        }

    }
}
