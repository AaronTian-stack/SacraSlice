using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using SacraSlice.Dependencies.Engine;
using SacraSlice.Dependencies.Engine.ECS.Component;
using SacraSlice.Dependencies.Engine.Scene;
using SacraSlice.GameCode.Screens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SacraSlice.GameCode.GameECS.GameSystems
{
    public class DemonSystem : EntityDrawSystem
    {
        SpriteBatch sb;
        float ppm;
        private ComponentMapper<Position> posMapper;
        private ComponentMapper<Timer> tMapper;
        private ComponentMapper<Sprite> sMapper;
        Wrapper<bool> demonReset;
        public DemonSystem(SpriteBatch sb, float ppm, Wrapper<bool> demonReset) : base(Aspect.All(typeof(Position), typeof(Timer),  typeof(Sprite)))
        {
            this.demonReset = demonReset;
            this.sb = sb;
            this.ppm = ppm;
            spawn = random.Next(range.X, range.Y);
            a.x = -1000; a.y = 1000;
            so = GameContainer.sounds["Waves"].CreateInstance();
            so.IsLooped = true;
            robo = GameContainer.sounds["RoboNoise"].CreateInstance();
            robo.IsLooped = true;
            jump = GameContainer.sounds["Jumpscare"].CreateInstance();
        }

        float timer;
        float spawn;
        
        /// <summary>
        /// Range of when it will spawn
        /// </summary>
        Point range = new Point(10, 20);
        Vector2 scaleRange = new Vector2(1.8f, 2.2f); // max: 2.2

        Vector2 delayRange = new Vector2(2, 3);

        Vector2 scale = new Vector2(1);
        Random random = new Random();

        Actor a = new Actor();

        // x: -60 to 32 y: -15 to 20

        Vector2 waitRange = new Vector2(1, 2);
        Vector2 durationRange = new Vector2(0.1f, 0.4f);

        Vector2 xRange = new Vector2(-60, 32);
        Vector2 yRange = new Vector2(-15, 20);

        bool b = true;
        SoundEffectInstance so, robo, jump;
        public override void Draw(GameTime gameTime)
        {
            foreach (var entity in ActiveEntities)
            {
                var pos = posMapper.Get(entity);
                var time = tMapper.Get(entity);
                var sprite = sMapper.Get(entity);

               
                if (!time.GetSwitch("IsDemon") || PlayScreen.score < PlayScreen.ScoreToSpawnDemon)
                    continue;

                if (demonReset)
                {
                    a.ClearActions();
                    a.x = -1000; a.y = 1000;
                    pos.currPosition.X = a.x;
                    pos.currPosition.Y = a.y;
                    robo.Stop();
                    so.Stop();
                    jump.Stop();
                    timer = 0;
                    spawn = random.Next(range.X, range.Y);
                    demonReset.Value = false;
                }
                   

                time.GetTimer("Spawn Timer").Value = timer;
                time.GetTimer("Time when spawn").Value = spawn;

                sprite.Color.A = (byte)(0.94f * 255);
                if (timer > spawn)
                {
                    
                    timer = 0;
                    spawn = random.Next(range.X, range.Y);

                    var s = random.NextSingle(scaleRange.X, scaleRange.Y);

                    scale = new Vector2(s);

                    var startduration = 0.25f;

                    // Generate Path

                    var startX = random.NextSingle(xRange.X, xRange.Y);

                    a.AddAction(Actions.MoveFrom(a, startX, 200, startX, 0, startduration, Interpolation.linear));
                    a.AddAction(Actions.Delay(a, random.NextSingle(delayRange.X, delayRange.Y)));

                    Vector2 currP = new Vector2(startX, 0);

                    for(int i = 0; i < 5; i++)
                    {
                        Vector2 nextPoint = new Vector2(random.NextSingle(xRange.X, xRange.Y), random.NextSingle(yRange.X, yRange.Y));
                        var duration = random.NextSingle(durationRange.X, durationRange.Y);
                        a.AddAction(Actions.MoveFrom(a, currP.X, currP.Y, nextPoint.X, nextPoint.Y, duration, Interpolation.linear));
                        a.AddAction(Actions.Delay(a, random.NextSingle(waitRange.X, waitRange.Y)));
                        currP = nextPoint;
                    }

                    a.AddAction(Actions.MoveFrom(a, currP.X, currP.Y, currP.X, 200, startduration, Interpolation.linear));

                    robo.Play();
                    robo.Volume = 1;
                    so.Play();
                    so.Volume = 0.6f;
                    jump.Play();
                }

                a.Act(gameTime.GetElapsedSeconds());
                if (a.actions.Count == 0)
                {
                    if(robo.Volume - 0.01f > 0)
                        robo.Volume -= 0.01f;
                    else
                    {
                        robo.Stop();
                        robo.Volume = 0;
                    }

                    if (so.Volume - 0.01f > 0)
                        so.Volume -= 0.01f;
                    else
                    {
                        so.Stop();
                        so.Volume = 0;
                    }   
                    
                    timer += gameTime.GetElapsedSeconds();
                    if (b)
                    {
                        PlayScreen.debugWindow.ChangeTickRate(30, 2f, Interpolation.fastSlow);
                        b = false;
                    }
                    
                }
                else
                {
                    sprite.Scale = scale;
                    pos.currPosition.X = a.x;
                    pos.currPosition.Y = a.y;

                    if (!b)
                    {
                        PlayScreen.debugWindow.ChangeTickRate(20, 2f, Interpolation.fastSlow);
                        b = true;
                    }
                        
                    if(PlayScreen.flash.normal.actions.Count == 0)
                    {
                        float dur = 0.4f;
                        Color ob = new Color(0, 0, 0, .9f);
                        PlayScreen.flash.normal.AddAction(Actions.ColorAction(
                            PlayScreen.flash.normal,
                            new Color(0, 0, 0, 0), ob, dur, Interpolation.smooth));
                        PlayScreen.flash.normal.AddAction(Actions.ColorAction(
                            PlayScreen.flash.normal,
                            ob, new Color(0, 0, 0, 0), dur, Interpolation.smooth));
                        PlayScreen.flash.normal.AddAction(Actions.Delay(
                            PlayScreen.flash.normal, 0.2f));

                    }
                }
                    
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
