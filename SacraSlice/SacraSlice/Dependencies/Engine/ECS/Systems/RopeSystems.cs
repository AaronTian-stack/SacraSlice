using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using SacraSlice.Dependencies.Engine;
using SacraSlice.Dependencies.Engine.Animation;
using SacraSlice.Dependencies.Engine.ECS.Component;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SacraSlice.Dependencies.Engine.ECS.Systems
{
    public class RopeUpdate : EntityUpdateSystem
    {
        private ComponentMapper<VerletRope> ropeMapper;
        public RopeUpdate() : base(Aspect.All(typeof(VerletRope))) { }
        public override void Initialize(IComponentMapperService mapperService)
        {
            ropeMapper = mapperService.GetMapper<VerletRope>();
        }
        public override void Update(GameTime gameTime)
        {
            foreach (var entity in ActiveEntities)
            {
                var rope = ropeMapper.Get(entity);

                rope.Update();

            }
        }
    }

    public class RopeRenderer : EntityDrawSystem
    {
        SpriteBatch sb;
        float ppm;
        private ComponentMapper<VerletRope> ropeMapper;

        public RopeRenderer(SpriteBatch sb, float ppm) : base(Aspect.All(typeof(VerletRope)))
        {
            this.sb = sb;
            this.ppm = ppm;
        }

        public override void Draw(GameTime gameTime)
        {

            foreach (var entity in ActiveEntities)
            {
                var rope = ropeMapper.Get(entity);

                Vector2 ren = Interpolate(rope.ropeSegments[0]);

                Vector2 diff = rope.target - ren;

                /// TODO: render circles for rounded edges

                int edges = 12;

                if (rope.outlineWidth > 0)
                {
                    var seg = rope.ropeSegments[0];
                    var ins = Interpolate(seg) + diff;
                    sb.DrawCircle(new CircleF(ins, rope.width * ppm / 2), edges, rope.color, (rope.width - 1) * ppm, rope.depth + 0.0002f);

                    sb.DrawCircle(new CircleF(ins, rope.outlineWidth * ppm / 2), edges, rope.outlineColor, (rope.outlineWidth - 1) * ppm, rope.depth + 0.0003f);

                    seg = rope.ropeSegments[rope.ropeSegments.Count - 1];
                    ins = Interpolate(seg) + diff;
                    sb.DrawCircle(new CircleF(ins, rope.width * ppm / 2), edges, rope.color, (rope.width - 1) * ppm, rope.depth + 0.0002f);

                    sb.DrawCircle(new CircleF(ins, rope.outlineWidth * ppm / 2), edges, rope.outlineColor, (rope.outlineWidth - 1) * ppm, rope.depth + 0.0003f);
                }

                for (int i = 0; i < rope.segments - 1; i++)
                {
                    // get seg and one in front then draw line

                    var seg1 = rope.ropeSegments[i];
                    var seg2 = rope.ropeSegments[i + 1];

                    var ins1 = Interpolate(seg1);
                    var ins2 = Interpolate(seg2);

                    if (rope.clamp) // Flag check?
                    {
                        ins1 += diff;
                        ins2 += diff;
                    }

                    sb.DrawLine(ins1, ins2, rope.color, ppm * rope.width, rope.depth);

                    sb.DrawCircle(new CircleF(ins1, rope.width * ppm / 2), edges, rope.color, (rope.width - 1) * ppm, rope.depth);

                    // add variable outline
                    if (rope.outlineWidth > 0)
                    {
                        sb.DrawLine(ins1, ins2, rope.outlineColor, ppm * rope.outlineWidth, rope.depth + 0.0001f);
                        sb.DrawCircle(new CircleF(ins1, rope.outlineWidth * ppm / 2), edges, rope.outlineColor, (rope.outlineWidth - 1) * ppm, rope.depth + 0.0001f);
                    }

                }

            }
        }

        private Vector2 Interpolate(RopeSegment seg)
        {
            return seg.posNow * GameContainer.alpha + seg.posOld * (1 - GameContainer.alpha);
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            ropeMapper = mapperService.GetMapper<VerletRope>();
        }

    }

    // TODO: maybe you can turn the locking rope to a system?
    // TODO: add rope outline

    public class RopePositioner : EntityDrawSystem
    {
        private ComponentMapper<VerletRope> ropeMapper;
        private ComponentMapper<Position> positionMapper;

        public RopePositioner() : base(Aspect.All(typeof(VerletRope), typeof(Position)))
        {

        }

        public override void Draw(GameTime gameTime)
        {

            foreach (var entity in ActiveEntities)
            {
                var rope = ropeMapper.Get(entity);
                var position = positionMapper.Get(entity);

                rope.target = new Vector2(position.currPosition.X, position.currPosition.Y);
                rope.depth = position.depth + rope.depthOffset;

            }
        }

        private Vector2 Interpolate(RopeSegment seg)
        {
            return seg.posNow * GameContainer.alpha + seg.posOld * (1 - GameContainer.alpha);
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            ropeMapper = mapperService.GetMapper<VerletRope>();
            positionMapper = mapperService.GetMapper<Position>();
        }

    }

    public class RopeClamperSystem : EntityDrawSystem
    {
        private ComponentMapper<VerletRope> ropeMapper;
        private ComponentMapper<RopeClamper> ropeclamperMapper;
        private ComponentMapper<Sprite> spriteMapper;
        private ComponentMapper<Timer> timerMapper;
        private ComponentMapper<AnimationStateManager> animationMapper;

        float ppm;
        public RopeClamperSystem(float ppm) : base(Aspect.All(typeof(VerletRope), typeof(RopeClamper),
            typeof(Sprite), typeof(Timer), typeof(AnimationStateManager)))
        {
            this.ppm = ppm;
        }

        public override void Draw(GameTime gameTime)
        {

            foreach (var entity in ActiveEntities)
            {
                var rope = ropeMapper.Get(entity);
                var ropeClamper = ropeclamperMapper.Get(entity);
                var sprite = spriteMapper.Get(entity);
                var timer = timerMapper.Get(entity);
                var asm = animationMapper.Get(entity);

                sprite.Scale *= ppm;

                // get array based off rope name and current animation (name)
                var array = ropeClamper.GetArray(rope.name, asm.currAnim.name);

                // get frame number based off State Timer and current animation
                int frame = asm.currAnim.getKeyFrameIndex(timer.GetTimer("State Time"));

                // find the new target position based off sprite bounding box and Array[frame] X and Y

                Vector2 offset = new Vector2(array[frame, 0], array[frame, 1]);

                Vector2 right = sprite.BoundingBox.TopRight;

                if (sprite.FlipX)
                {
                    rope.target.X = right.X - (offset.X * ppm);
                    rope.target.Y = right.Y + (offset.Y * ppm);
                }
                else
                {
                    rope.target = sprite.BoundingBox.TopLeft + (offset * ppm);
                }


                sprite.Scale /= ppm;

            }
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            ropeMapper = mapperService.GetMapper<VerletRope>();
            ropeclamperMapper = mapperService.GetMapper<RopeClamper>();
            spriteMapper = mapperService.GetMapper<Sprite>();
            timerMapper = mapperService.GetMapper<Timer>();
            animationMapper = mapperService.GetMapper<AnimationStateManager>();
        }

    }


}
