using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using SacraSlice.Dependencies.Engine.ECS.Component;

namespace SacraSlice.Dependencies.Engine.ECS.Systems
{
    public class PositionInterpolator : EntityDrawSystem
    {
        private ComponentMapper<Position> positionMapper;

        public PositionInterpolator() : base(Aspect.All(typeof(Position))) { }

        public override void Draw(GameTime gameTime)
        {
            foreach (var entity in ActiveEntities)
            {
                var pos = positionMapper.Get(entity);

                pos.renderPosition = pos.currPosition * GameContainer.alpha + pos.prevPosition * (1 - GameContainer.alpha);

            }
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            positionMapper = mapperService.GetMapper<Position>();
        }

    }
}
