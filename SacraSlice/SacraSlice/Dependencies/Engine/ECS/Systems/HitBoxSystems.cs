using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using SacraSlice.Dependencies.Engine.ECS.Component;
using SacraSlice.Dependencies.Engine.States;

namespace SacraSlice.Dependencies.Engine.ECS.Systems
{
    public class HitBoxChange : EntityUpdateSystem
    {
        private ComponentMapper<HitBox> hitboxMapper;
        private ComponentMapper<StateManager> stateMapper;

        public HitBoxChange() : base(Aspect.All(typeof(HitBox), typeof(StateManager))) { }
        public override void Initialize(IComponentMapperService mapperService)
        {
            hitboxMapper = mapperService.GetMapper<HitBox>();
            stateMapper = mapperService.GetMapper<StateManager>();
        }
        public override void Update(GameTime gameTime)
        {
            foreach (var entity in ActiveEntities)
            {


                var hb = hitboxMapper.Get(entity);
                var sm = stateMapper.Get(entity);

                hb.SetRect(sm.currentState);
                hb.SetRect(sm.currentState);

            }
        }
    }

    public class HitBoxRenderer : EntityDrawSystem
    {
        SpriteBatch sb;
        float ppm;
        private ComponentMapper<HitBox> hitboxMapper;
        private ComponentMapper<Position> positionMapper;

        public HitBoxRenderer(SpriteBatch sb, float ppm) : base(Aspect.All(typeof(HitBox), typeof(Position)))
        {
            this.sb = sb;
            this.ppm = ppm;
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var entity in ActiveEntities)
            {
                var hb = hitboxMapper.Get(entity);
                var pos = positionMapper.Get(entity);

                hb.rect.X = pos.renderPosition.X - hb.rect.Width / 2;
                hb.rect.Y = pos.renderPosition.Y - hb.rect.Height / 2;

                if (hb.debug)
                    sb.DrawRectangle(hb.rect, hb.Color, ppm, 0);

            }
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            hitboxMapper = mapperService.GetMapper<HitBox>();
            positionMapper = mapperService.GetMapper<Position>();
        }

    }
}
