using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using SacraSlice.Dependencies.Engine;
using SacraSlice.Dependencies.Engine.Animation;
using SacraSlice.Dependencies.Engine.ECS.Component;
using SacraSlice.Dependencies.Engine.Scene;
using SacraSlice.Dependencies.Engine.Scene.ActionClasses;
using SacraSlice.Dependencies.Engine.States;
using SacraSlice.GameCode.GameECS.GameComponents;
using SacraSlice.GameCode.Screens;
using System;
using System.Collections.Generic;
using System.Text;

namespace SacraSlice.GameCode.GameECS.GameSystems
{
    public class SwordDraw : EntityDrawSystem
    {
        private ComponentMapper<Sword> sMapper;
        private ComponentMapper<Position> pMapper;
        private ComponentMapper<HitBox> hMapper;
        private ComponentMapper<Timer> tMapper;
        SpriteBatch sb;
        float ppm;
        public SwordDraw(SpriteBatch sb, float ppm) : base(Aspect.All(typeof(Sword), typeof(Position), typeof(HitBox), typeof(Timer)))
        {
            this.sb = sb;
            this.ppm = ppm;

            
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            sMapper = mapperService.GetMapper<Sword>();
            pMapper = mapperService.GetMapper<Position>();
            hMapper = mapperService.GetMapper<HitBox>();
            tMapper = mapperService.GetMapper<Timer>();
        }
        public override void Draw(GameTime gameTime)
        {
            foreach (var entity in ActiveEntities)
            {

                var sword = sMapper.Get(entity);
                var pos = pMapper.Get(entity);
                var hb = hMapper.Get(entity);
                var timer = tMapper.Get(entity);

                sword.a.Act(gameTime.GetElapsedSeconds());
                sword.s.Position.X = pos.renderPosition.X - hb.rect.Width / 2 - (sword.s.BoundingBox.Width * 0.3f * sword.s.Scale.X * ppm);
                sword.s.Position.Y = pos.renderPosition.Y + sword.a.y;

                sword.hand.Position = sword.s.Position;

                sword.ChangeAngle(gameTime.GetElapsedSeconds());

                // the sword rotation starts upright (0 = 90)
                // sword counter clockwise is negative

                if (timer.GetSwitch("Attack"))
                {
                    sword.Swing();
                    timer.GetSwitch("Attack").Value = false;
                    PlayScreen.camera.AddShake(0.2f, 0.1f, 2);
                }
                   

                var swordR = sword.s.Rotation - 90;

                var angle = MathF.Min(360 - MathF.Abs(swordR - timer.GetTimer("Angle")),
                    MathF.Abs(swordR - timer.GetTimer("Angle")));

                //DebugLog.Print("SWORD SYSTEM", angle);

                float tolerance = 25;

                if (timer.GetSwitch("Sword Active") && angle > 90 - tolerance && angle < 90 + tolerance)
                {
                    sword.s.Color = Color.Gold;
                    timer.GetSwitch("Protect").Value = true;
                }
                else
                {
                    sword.s.Color = Color.White;
                    timer.GetSwitch("Protect").Value = false;
                }

                sword.Draw(sb, gameTime.GetElapsedSeconds(), ppm);
                

            }
        }

    }

    public class SwordSquash : EntityDrawSystem
    {
        private ComponentMapper<Sword> sMapper;
        private ComponentMapper<SquashManager> sqMapper;
        private ComponentMapper<StateManager> stMapper;

        public SwordSquash() : base(Aspect.All(typeof(Sword), typeof(SquashManager), typeof(StateManager)))
        {

        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            sMapper = mapperService.GetMapper<Sword>();
            sqMapper = mapperService.GetMapper<SquashManager>();
            stMapper = mapperService.GetMapper<StateManager>();
        }
        public override void Draw(GameTime gameTime)
        {
            foreach (var entity in ActiveEntities)
            {

                var sword = sMapper.Get(entity);
                var sqm = sqMapper.Get(entity);
                var sm = stMapper.Get(entity);

                SquashValue sv;
                float x = 1;
                float y = 1;
                if (sqm.lookup.TryGetValue(sm.currentState, out sv))
                {
                    x = sv.interpolationX.apply(sv.startValues.X, sv.endValues.X, MathF.Min(1, sv.timeX.Item1.Value * sv.timeX.Item2));
                    y = sv.interpolationY.apply(sv.startValues.Y, sv.endValues.Y, MathF.Min(1, sv.timeY.Item1.Value * sv.timeY.Item2));
                }

                sword.s.Scale = new Vector2(x, y);
                sword.hand.Scale = new Vector2(x, y);

            }
        }

    }
}
