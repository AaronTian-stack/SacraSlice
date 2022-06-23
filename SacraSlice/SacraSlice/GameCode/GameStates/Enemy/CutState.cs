using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using SacraSlice.Dependencies.Engine;
using SacraSlice.Dependencies.Engine.ECS.Component;
using SacraSlice.Dependencies.Engine.InterfaceLayout;
using SacraSlice.Dependencies.Engine.Scene;
using SacraSlice.Dependencies.Engine.Scene.ActionClasses;
using SacraSlice.Dependencies.Engine.States;
using SacraSlice.GameCode.GameECS.GameComponents;
using SacraSlice.GameCode.Screens;
using System;
using System.Collections.Generic;
using System.Text;

namespace SacraSlice.GameCode.GameStates.Enemy
{
    public class CutState : State
    {
        CircleF leftHitbox, rightHitbox;
        HitBox hb;
        float ppm;
        int direction; // 0 is left to right 1 is right to left
        public CutState(StateManager sm, float dt, Position p, Timer t, HitBox hb, float ppm) : base(sm, dt, p, t)
        {
            name = "Cut";
            this.hb = hb;
            this.ppm = ppm;
            XRed.Rotation = 45;
            so = GameContainer.sounds["Wrong"].CreateInstance();
            so.Pitch = 1;
        }
        bool begin;
        SoundEffectInstance so;
        float radius = 5;
        public override void Act()
        {

        }
        Random random = new Random();
        float max;
        RectangleF bounds;
        public override void OnEnter()
        {
            GameContainer.sounds["FloorImpact"].Play();
            // generate the hitboxes
            begin = false;
            float off = 50;

            CircleF c = new CircleF(pos.currPosition, hb.rect.Width / 2 * 1.5f);

            Vector2 p = c.BoundaryPointAt(MathHelper.ToRadians(random.NextSingle(90 + off, 270 - off)));
            leftHitbox = new CircleF(p, radius);

            p = c.BoundaryPointAt(MathHelper.ToRadians(random.NextSingle(-90 + off, 90 - off)));
            rightHitbox = new CircleF(p, radius);

            bounds = RectangleF.CreateFrom(leftHitbox.Center - new Vector2(radius),
                rightHitbox.Center + new Vector2(radius));

            // pick a direction
            direction = random.Next(0, 2);

            // positive is clockwise
            timer.GetTimer("Angle").Value = MathHelper.ToDegrees(MathF.Atan2(rightHitbox.Center.Y - leftHitbox.Center.Y,
                rightHitbox.Center.X - leftHitbox.Center.X));

            timer.GetTimer("DrawAngle").Value = timer.GetTimer("Angle");

            if (timer.GetTimer("Angle") > 0)
            {
                timer.GetTimer("Angle").Value = 180 - timer.GetTimer("Angle").Value;
            }
            else
                timer.GetTimer("Angle").Value *= -1;





            if (direction == 1) // swap if from right to left
            {
                var hold = leftHitbox;
                leftHitbox = rightHitbox;
                rightHitbox = hold;
                timer.GetTimer("DrawAngle").Value += 180;
                //timer.GetTimer("Angle").Value += 180;
            }

            rightHitbox.Radius *= 2;


            if (PlayScreen.hardEnemiesSpawn)
                timer.GetTimer("Life").Value = PlayScreen.LifeTimeEnemyVariable;
            else
                timer.GetTimer("Life").Value = PlayScreen.lifeTimeBasic;


            max = timer.GetTimer("Life");

            timer.GetTimer("leeway").Value = 0.5f;

            timer.GetSwitch("Sword Active").Value = true;
            timer.GetSwitch("Attack").Value = false;
            timer.GetSwitch("Attacking").Value = false;

            colorFlashActor.ClearActions();
            xActor.ClearActions();
            exclamationActor.ClearActions();

            timer.GetSwitch("RESET").Value = true;

            timer.GetTimer("random attack timer").Value = 0;
            timer.GetTimer("random attack").Value
                = random.NextSingle(attackInterval.X, attackInterval.Y);
        }
        /// <summary>
        /// Swing the sword
        /// </summary>
        /// <param name="x"> If true then X sprite will show </param>
        
        public void Swing(bool x, float time)
        {
            if (timer.GetSwitch("Attacking"))
                return;
            timer.GetSwitch("Attacking").Value = true;
            DebugLog.Print("ATTACK", "EXECUTED");
            var dur = 0.25f;
            var sc = 0.1f;
            if (x)
            {
                GameContainer.sounds["Wrong"].Play();
                xActor.ClearActions();
                //DebugLog.Print("CutState", "CounterAttacked");
                xActor.x = 0; xActor.y = 0;
                xActor.AddAction(Actions.MoveFrom(xActor, 0, 0, sc, sc, dur, Interpolation.swingOut));
                xActor.AddAction(Actions.Delay(xActor, 0.5f));
                xActor.AddAction(Actions.MoveFrom(xActor, sc, sc, 0, 0, dur, Interpolation.swingIn));
                xActor.scaleX = leftHitbox.Center.X;
                xActor.scaleY = leftHitbox.Center.Y;
            }
            else
            {
                
                so.Play();
                exclamationActor.ClearActions();
                // do another telegraph
                var sc2 = 0.2f;
                exclamationActor.x = 0; exclamationActor.y = 0;
                exclamationActor.AddAction(Actions.MoveFrom(exclamationActor, 0, 0, sc2, sc2, dur, Interpolation.swingOut));
                exclamationActor.AddAction(Actions.Delay(exclamationActor, 0.5f));
                exclamationActor.AddAction(Actions.MoveFrom(exclamationActor, sc2, sc2, 0, 0, dur, Interpolation.swingIn));
                exclamationActor.scaleX = pos.renderPosition.X;
                exclamationActor.scaleY = pos.renderPosition.Y - hb.rect.Height * 0.3f;
            }
            timer.GetSwitch("Attack").Value = true;
            timer.GetTimer("Attack Time").Value = time;
        }

        bool reset;
        public void CutLogic()
        {
            if (PlayScreen.life <= 0)
                return;

            if (leftHitbox.Contains(PlayScreen.mouseCoordinate) && reset)
            {
                begin = true;
            }

            if (!PlayScreen.mouse.IsButtonDown(MouseButton.Left))
            {
                begin = false;
                reset = true;
            }


            if (timer.GetSwitch("Protect") && !timer.GetSwitch("Attacking"))
            {
                if (begin)
                {
                    GameContainer.sounds["SwordHit"].Play();
                    // also X sound


                    // swing sword
                    Swing(true, 0.6f);
                    //PlayScreen.life.Value--;
                    begin = false;
                    reset = false;
                }
                return;
            }

            if (begin)
            {
                if (CalculateDistanceFromLine(PlayScreen.mouseCoordinate) > radius)
                {
                    begin = false;
                }

                else if (rightHitbox.Contains(PlayScreen.mouseCoordinate))
                {
                    timer.GetSwitch("Killed").Value = true;
                    sm.SetStateUpdate("Shrink", timer);
                }

            }
        }

        public float CalculateDistanceFromLine(Vector2 point)
        {
            float x = rightHitbox.Center.X - leftHitbox.Center.X;
            float y = rightHitbox.Center.Y - leftHitbox.Center.Y;

            return MathF.Abs(x *
                (leftHitbox.Center.Y - point.Y) -
                (leftHitbox.Center.X - point.X) * y) / MathF.Sqrt(x * x + y * y);
        }
        Actor colorFlashActor = new Actor();
        Actor xActor = new Actor();
        Actor exclamationActor = new Actor();
        //float randomAttack;
        //float randomAttackTimer;
        Vector2 attackInterval = new Vector2(3f, 5f);
        public void Explode(SpriteBatch sb, float dt)
        {

            //dt *= (PlayScreen.dtStatic / PlayScreen.dt);


            var ds = timer.GetTimer("Life").Value.ToString("0.0");
            float scale = 0.05f;
            //var si = TextDrawer.MeasureFont("Main Font", ds);

            if (!timer.GetSwitch("Protect"))
            {
                timer.GetTimer("Life").Value -= dt;
                colorFlashActor.color = Color.White;
                if(timer.GetSwitch("sword"))
                    timer.GetTimer("random attack timer").Value += dt;
                if (timer.GetTimer("random attack timer") > timer.GetTimer("random attack"))
                {
                    timer.GetTimer("random attack timer").Value = 0;
                    Swing(false, 1f);
                    timer.GetTimer("random attack").Value = 
                        random.NextSingle(attackInterval.X, attackInterval.Y);
                }
            }

            if (timer.GetTimer("Life") < max / 2)
            {
                colorFlashActor.AddAction(Actions.ColorAction(colorFlashActor, Color.White, Color.Red, 0.2f, Interpolation.smooth));
                colorFlashActor.AddAction(Actions.ColorAction(colorFlashActor, Color.Red, Color.White, 0.2f, Interpolation.smooth));
            }
            colorFlashActor.Act(dt);
            xActor.Act(dt);
            exclamationActor.Act(dt);

            if (timer.GetSwitch("Protect"))
                colorFlashActor.color = Color.Gold;


            if (timer.GetSwitch("sword"))
                TextDrawer.BatchQueue("Main Font", ds,
                    new Vector2(pos.renderPosition.X, pos.renderPosition.Y - hb.rect.Height),
                    colorFlashActor.color, Color.Black, scale, PlayScreen.ppm, 1, 1, 1, 2);
            else
                TextDrawer.BatchQueue("Main Font", ds,
                    new Vector2(pos.renderPosition.X, pos.renderPosition.Y - hb.rect.Height * 0.6f),
                    colorFlashActor.color, Color.Black, scale, PlayScreen.ppm, 1, 1, 1, 2);


            if (timer.GetTimer("Life") <= 0)
            {
                timer.GetTimer("Life").Value = 0;
                if (timer.GetTimer("leeway") < 0)
                {
                    sm.SetStateUpdate("Shrink", timer);
                    timer.GetSwitch("Sword Active").Value = false;
                }
                timer.GetTimer("leeway").Value -= dt;
            }




        }
        Sprite XRed = new Sprite(GameContainer.atlas.FindRegion("xRed"));
        Sprite warning = new Sprite(GameContainer.atlas.FindRegion("warning"));
        Sprite arrow = new Sprite(GameContainer.atlas.FindRegion("arrow"));
        Sprite shield = new Sprite(GameContainer.atlas.FindRegion("shield"));
        public override void Draw(SpriteBatch sb, float dt)
        {
            CutLogic();
            Explode(sb, dt);

            //sb.DrawRectangle(bounds, Color.BlueViolet, ppm);

            /*if(leftHitbox.Contains(PlayScreen.mouseCoordinate))
                sb.DrawCircle(leftHitbox, 12, Color.Green, ppm);
            else
                sb.DrawCircle(leftHitbox, 12, Color.Red, ppm);
            if (rightHitbox.Contains(PlayScreen.mouseCoordinate))
                sb.DrawCircle(rightHitbox, 12, Color.Green, ppm);
            else
                sb.DrawCircle(rightHitbox, 12, Color.Red, ppm);

            //sb.DrawCircle(new CircleF(rightHitbox.Center, radius * 2), 12, Color.Red, ppm);

            if (begin)
                sb.DrawLine(leftHitbox.Center, rightHitbox.Center, Color.HotPink, ppm * radius);
            else
                sb.DrawLine(leftHitbox.Center, rightHitbox.Center, Color.Yellow, ppm * radius);*/

           
            SpriteAligner.BatchQueue(XRed, new Vector2(xActor.scaleX, xActor.scaleY), new Vector2(xActor.x, xActor.y), 0);
            SpriteAligner.BatchQueue(warning, new Vector2(exclamationActor.scaleX, exclamationActor.scaleY), new Vector2(exclamationActor.x, exclamationActor.y), 0);
            
            arrow.Origin = new Vector2(0, arrow.Textureregion.height / 2);
            arrow.Rotation = timer.GetTimer("DrawAngle");

            arrow.Scale = new Vector2(1, 1f); ;


            if (begin)
            {
                // make arrow color interpolate to green based off how close you are to finish
                //arrow.Color = new Color(0, 1, 0, 0.8f);
                arrow.Color = Interpolation.sineOut.Apply(Color.Yellow, Color.Green,

                    1 - Math.Clamp(

                        (PlayScreen.mouseCoordinate - new Vector2(rightHitbox.Center.X, rightHitbox.Center.Y)).Length()

                    / (rightHitbox.Center - leftHitbox.Center).Length()

                    , 0, 1)

                    );

                arrow.Color.A = (byte)(0.8f * 255);

            }
            else if (timer.GetSwitch("Protect") && leftHitbox.Contains(PlayScreen.mouseCoordinate))
                arrow.Color = new Color(1, 0, 0, 0.8f);
            else
                arrow.Color = new Color(1, 1, 1, 0.8f);

            if (PlayScreen.life <= 0)
                arrow.Color = Color.White;

            arrow.Scale *= ppm;
            arrow.Position = leftHitbox.Center;
            arrow.Draw(sb, 0.001f);


            arrow.Scale /= ppm;

            if (timer.GetSwitch("Protect"))
            {
                shield.Scale = new Vector2(0.8f);
                shield.Scale *= ppm;
                shield.Position = new Vector2(pos.renderPosition.X, pos.renderPosition.Y - hb.rect.Height * 0.3f);
                shield.Draw(sb, 0.0011f);
                shield.Scale /= ppm;
            }


        }

        public override void OnLeave()
        {
            colorFlashActor.ClearActions();
            xActor.ClearActions();
            exclamationActor.ClearActions();
            SoundEffectInstance s;

            if(timer.GetSwitch("pien"))
                s = GameContainer.sounds["Emoji"].CreateInstance();
            else if (timer.GetSwitch("cardboard"))
                s = GameContainer.sounds["Cardboard"].CreateInstance();
            else if (timer.GetSwitch("banana"))
                s = GameContainer.sounds["Banana"].CreateInstance();
            else if (timer.GetSwitch("wood"))
                s = GameContainer.sounds["Wood"].CreateInstance();
            else
                s = GameContainer.sounds["Death"].CreateInstance();

            s.Pitch = random.NextSingle(-1f, 1f);
            s.Play();
        }
    }
}
