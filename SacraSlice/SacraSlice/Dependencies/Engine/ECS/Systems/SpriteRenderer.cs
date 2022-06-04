using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using SacraSlice.Dependencies.Engine.ECS.Component;
using System;

namespace SacraSlice.Dependencies.Engine.ECS.Systems
{
    public class SpriteRenderer : EntityDrawSystem
    {
        SpriteBatch sb;
        float ppm;
        private ComponentMapper<Position> positionMapper;
        private ComponentMapper<Sprite> spriteMapper;
        private ComponentMapper<HitBox> hitboxMapper;

        public SpriteRenderer(SpriteBatch sb, float ppm) : base(Aspect.All(typeof(Sprite), typeof(Position)))
        {
            this.sb = sb;
            this.ppm = ppm;
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var entity in ActiveEntities)
            {


                var sprite = spriteMapper.Get(entity);
                var pos = positionMapper.Get(entity);

                sprite.FlipX = pos.orientation;

                sprite.Position = new Vector2(pos.renderPosition.X, pos.renderPosition.Y);

                sprite.Scale *= ppm;

                sprite.Origin = new Vector2(sprite.Textureregion.width / 2, sprite.Textureregion.height / 2);


                var hb = hitboxMapper.Get(entity);

                if (hb != null)
                {
                    switch (hb.renderFlag)
                    {
                        case HitBoxFlag.Right:
                            float f = MathF.Abs(sprite.BoundingBox.Width - hb.rect.Width) / -2f;
                            ChangeX(f, sprite, hb);
                            break;
                        case HitBoxFlag.Left:
                            f = MathF.Abs(sprite.BoundingBox.Width - hb.rect.Width) / 2f;
                            ChangeX(f, sprite, hb);
                            break;
                        case HitBoxFlag.Bottom:
                            f = MathF.Abs(sprite.BoundingBox.Height - hb.rect.Height) / 2f;
                            ChangeY(f, sprite, hb);
                            break;
                        case HitBoxFlag.Top:
                            f = MathF.Abs(sprite.BoundingBox.Height - hb.rect.Height) / -2f;
                            ChangeY(f, sprite, hb);
                            break;
                    }

                }


                var bb = sprite.BoundingBox;
                bb.X = sprite.Position.X - sprite.BoundingBox.Width / 2;
                bb.Y = sprite.Position.Y - sprite.BoundingBox.Height / 2;

                sprite.BoundingBox = bb;

                //DebugLog.Print("Dept", pos.Depth.ToString());

                sprite.Draw(sb, pos.depth);

                if (sprite.debug)
                    sb.DrawRectangle(sprite.BoundingBox, sprite.BoundColor, ppm, pos.depth);

                sprite.Scale /= ppm;

            }




        }

        public void ChangeY(float f, Sprite sprite, HitBox hb)
        {
            var a = sprite.Position;
            if (sprite.BoundingBox.Height > hb.rect.Height)
            {
                a.Y -= f;
            }
            else
            {
                a.Y += f;
            }
            sprite.Position = a;
        }

        public void ChangeX(float f, Sprite sprite, HitBox hb)
        {
            var a = sprite.Position;
            if (sprite.BoundingBox.Width > hb.rect.Width)
            {
                a.X += f;
            }
            else
            {
                a.X -= f;
            }
            sprite.Position = a;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            positionMapper = mapperService.GetMapper<Position>();
            spriteMapper = mapperService.GetMapper<Sprite>();
            hitboxMapper = mapperService.GetMapper<HitBox>();
        }
    }

}
