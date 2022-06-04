using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using SacraSlice.Dependencies.Engine.Animation;
using SacraSlice.Dependencies.Engine.ECS.Component;
using SacraSlice.Dependencies.Engine.States;
using System;
using System.Collections.Generic;
using System.Text;

namespace SacraSlice.Dependencies.Engine.ECS.Systems
{
    public class AnimationUpdater : EntityDrawSystem
    {
        private ComponentMapper<AnimationStateManager> animationMapper;
        private ComponentMapper<StateManager> stateMapper;

        public AnimationUpdater() : base(Aspect.All(typeof(AnimationStateManager), typeof(StateManager))) { }

        public override void Draw(GameTime gameTime)
        {
            foreach (var entity in ActiveEntities)
            {

                var asm = animationMapper.Get(entity);
                var sm = stateMapper.Get(entity);

                asm.SetAnimation(sm.currentState);
            }
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            animationMapper = mapperService.GetMapper<AnimationStateManager>();
            stateMapper = mapperService.GetMapper<StateManager>();
        }

    }

    public class SpriteAnimation : EntityDrawSystem
    {
        Wrapper<float> dt;
        float dtStatic;
        private ComponentMapper<AnimationStateManager> animationMapper;
        private ComponentMapper<Sprite> spriteMapper;
        private ComponentMapper<Timer> timerMapper;

        public SpriteAnimation(Wrapper<float> dt, float dtStatic) : base(Aspect.All(typeof(AnimationStateManager), typeof(Sprite), typeof(Timer)))
        {
            this.dt = dt;
            this.dtStatic = dtStatic;
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var entity in ActiveEntities)
            {

                var asm = animationMapper.Get(entity);
                var sprite = spriteMapper.Get(entity);
                var timer = timerMapper.Get(entity);

                sprite.Textureregion = asm.currAnim.getKeyFrame(timer.GetTimer("State Time"));

                timer.GetTimer("State Time").Value += gameTime.GetElapsedSeconds() * (dtStatic / dt);


            }
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            animationMapper = mapperService.GetMapper<AnimationStateManager>();
            spriteMapper = mapperService.GetMapper<Sprite>();
            timerMapper = mapperService.GetMapper<Timer>();
        }

    }
}
