using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using SacraSlice.Dependencies.Engine.ECS.Component;

namespace SacraSlice.Dependencies.Engine.ECS.Systems
{
    public class PositionModifier : EntityUpdateSystem
    {
        float globalGravity;
        private ComponentMapper<Position> positionMapper;

        public PositionModifier(float globalGravity) : base(Aspect.All(typeof(Position)))
        {
            this.globalGravity = globalGravity;
        }
        public override void Initialize(IComponentMapperService mapperService)
        {
            positionMapper = mapperService.GetMapper<Position>();
        }
        public override void Update(GameTime gameTime)
        {
            foreach (var entity in ActiveEntities)
            {


                var pos = positionMapper.Get(entity);

                pos.prevPosition = pos.currPosition;

                if (pos.gravity && !pos.ground)
                    pos.velocity.Y += globalGravity;

                pos.currPosition += pos.velocity;



            }
        }
    }

    public class PositionClamp : EntityUpdateSystem
    {
        private ComponentMapper<Position> positionMapper;
        private ComponentMapper<HitBox> hitBoxMapper;

        public PositionClamp() : base(Aspect.All(typeof(Position), typeof(HitBox))) { }
        public override void Initialize(IComponentMapperService mapperService)
        {
            positionMapper = mapperService.GetMapper<Position>();
            hitBoxMapper = mapperService.GetMapper<HitBox>();
        }
        public override void Update(GameTime gameTime)
        {
            foreach (var entity in ActiveEntities)
            {


                var pos = positionMapper.Get(entity);
                var hb = hitBoxMapper.Get(entity);

                
                if (pos.currPosition.Y + hb.rect.Height / 2 > 0)
                {
                    pos.velocity.Y = 0;
                    pos.currPosition.Y = -hb.rect.Height / 2;
                    pos.ground = true;
                }

                /*if (pos.currPosition.X - hb.rect.Width / 2 < 0)
                {
                    pos.currPosition.X = hb.rect.Width / 2;
                }*/
               

            }
        }
    }
}
