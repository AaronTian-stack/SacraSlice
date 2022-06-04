using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using SacraSlice.Dependencies.Engine;
using SacraSlice.Dependencies.Engine.States;
using System;
using System.Collections.Generic;
using System.Text;

namespace SacraSlice.GameCode.GameECS.GameSystems
{
    public class ResizerSystem : EntityDrawSystem
    {
        private ComponentMapper<string> stringMapper;
        private ComponentMapper<Sprite> spriteMapper;
        private ComponentMapper<HitBox> hitboxMapper;
        private ComponentMapper<StateManager> stateMapper;
        float ppm;
        public ResizerSystem(float ppm) : base(Aspect.All(typeof(string), typeof(Sprite), typeof(HitBox), typeof(StateManager)))
        {
            this.ppm = ppm;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            stringMapper = mapperService.GetMapper<string>();
            spriteMapper = mapperService.GetMapper<Sprite>();
            hitboxMapper = mapperService.GetMapper<HitBox>();
            stateMapper = mapperService.GetMapper<StateManager>();
        }
        public override void Draw(GameTime gameTime)
        {
            foreach (var entity in ActiveEntities)
            {
                
                var name = stringMapper.Get(entity);
                var sprite = spriteMapper.Get(entity);
                var hb = hitboxMapper.Get(entity);
                var sm = stateMapper.Get(entity);

                if (!name.Equals("Dropping Item"))
                    return;

                RectangleF r = sprite.BoundingBox;
                r.Width *= sprite.Scale.X * ppm;
                r.Height *= sprite.Scale.Y * ppm;

                hb.ChangeRect(sm.currentState, r);
                

            }
        }

    }
}
