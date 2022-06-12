using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using SacraSlice.Dependencies.Engine;
using SacraSlice.Dependencies.Engine.ECS.Component;
using System;
using System.Collections.Generic;
using System.Text;

namespace SacraSlice.GameCode.GameECS.GameSystems
{
    public class ShadowSystem : EntityDrawSystem
    {
        SpriteBatch sb;
        float ppm;
        public ShadowSystem(SpriteBatch sb, float ppm) : base(Aspect.All(typeof(Position), typeof(Sprite), typeof(Timer)))
        {
            this.sb = sb;
            this.ppm = ppm;
        }
        private ComponentMapper<Position> posMapper;
        private ComponentMapper<Timer> tMapper;
        private ComponentMapper<Sprite> sMapper;

        public override void Draw(GameTime gameTime)
        {
            foreach (var entity in ActiveEntities)
            {
                var pos = posMapper.Get(entity);
                var time = tMapper.Get(entity);
                //var sprite = sMapper.Get(entity);

                float w = 100 / 2;
                float h = 32 / 2;

                var sc = new Vector2(w * ppm, h * ppm);

                var y = -1f;

                var ba = -10;
                Color c = new Color((46 + ba) / 255f, (34 + ba) / 255f, (47 + ba) / 255f);

                if (time.GetSwitch("dead")) continue;

                if (time.GetSwitch("Shrink"))
                {
                    sc *= Interpolation.smooth.apply(1, 0, Math.Clamp(time.GetTimer("State Time") / 0.2f, 0, 1));
                    DrawEllipses(sc, pos, y, c, ppm * 4);
                }
                else 
                {
                    sc *= Interpolation.smooth.apply(0, 1, Math.Clamp((100 + pos.renderPosition.Y) / 100f, 0, 1));
                    DrawEllipses(sc, pos, y, c, ppm * 4);
                }
            }
        }

        public void DrawEllipses(Vector2 sc, Position pos, float y, Color c, float ppm)
        {
            var bruh = (int)(sc.X / ppm);
            var huh = sc / bruh;
            for (int i = 0; i < bruh; i++)
            {
                sb.DrawEllipse(new Vector2(pos.renderPosition.X, y), sc
                        , 24, c, ppm, pos.depth + 0.0001f);
                sc -= huh;
            }
            
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            posMapper = mapperService.GetMapper<Position>();
            tMapper = mapperService.GetMapper<Timer>();
            sMapper = mapperService.GetMapper<Sprite>();
        }


    }
}
