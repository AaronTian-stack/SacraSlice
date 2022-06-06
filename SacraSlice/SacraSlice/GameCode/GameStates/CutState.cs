using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using SacraSlice.Dependencies.Engine;
using SacraSlice.Dependencies.Engine.ECS.Component;
using SacraSlice.Dependencies.Engine.InterfaceLayout;
using SacraSlice.Dependencies.Engine.Scene;
using SacraSlice.Dependencies.Engine.States;
using SacraSlice.GameCode.GameECS.GameComponents;
using SacraSlice.GameCode.Screens;
using System;
using System.Collections.Generic;
using System.Text;

namespace SacraSlice.GameCode.GameStates
{
    public class CutState : State
    {
        CircleF leftHitbox, rightHitbox;
        HitBox hb;
        float ppm;
        public Sword s;
        int direction; // 0 is left to right 1 is right to left
        public CutState(StateManager sm, float dt, Position p, Timer t, HitBox hb, float ppm) : base(sm, dt, p, t)
        {
            name = "Cut";
            this.hb = hb;
            this.ppm = ppm;
            tolerance = radius;
        }
        bool begin;

        float radius = 6;
        float tolerance;
        public override void Act()
        {

        }
        Random random = new Random();
        float max;
        public override void OnEnter()
        {
            // generate the hitboxes
            begin = false;
            float off = 50;

            CircleF c = new CircleF(pos.currPosition, hb.rect.Width / 2 * 1.5f);

            Vector2 p = c.BoundaryPointAt(MathHelper.ToRadians(random.NextSingle(90 + off, 270 - off)));
            leftHitbox = new CircleF(p, radius);


            p = c.BoundaryPointAt(MathHelper.ToRadians(random.NextSingle(-90 + off, 90 - off)));
            rightHitbox = new CircleF(p, radius);


            // pick a direction
            direction = random.Next(0, 2);

            if (direction == 1) // swap if from right to left
            {
                
                var hold = leftHitbox;
                leftHitbox = rightHitbox;
                rightHitbox = hold;
            }
            timer.GetTimer("Life").Value = 2;
            max = timer.GetTimer("Life");

            timer.GetTimer("leeway").Value = 0.5f;

            a.ClearActions();
        }

        public void CutLogic()
        {
            
            if (leftHitbox.Contains(PlayScreen.mouseCoordinate))
            {
                begin = true;
            }

            if(!PlayScreen.mouse.IsButtonDown(MouseButton.Left))
                begin = false;

            if (begin)
            {
                if (CalculateDistanceFromLine(PlayScreen.mouseCoordinate) > tolerance)
                {
                    begin = false;
                }

                else if (rightHitbox.Contains(PlayScreen.mouseCoordinate))
                {
                    sm.SetStateUpdate("Shrink", timer);
                }

            }
        }

        public float CalculateDistanceFromLine(Vector2 point)
        {
            float x = rightHitbox.Center.X - leftHitbox.Center.X;
            float y = rightHitbox.Center.Y - leftHitbox.Center.Y;

            return MathF.Abs((x) *
                (leftHitbox.Center.Y - point.Y) -
                (leftHitbox.Center.X - point.X) * (y)) / MathF.Sqrt(x * x + y * y);
        }
        Actor a = new Actor();
        public void Explode(SpriteBatch sb, float dt)
        {
            a.Act(dt);
            
            float s = 0.2f;

            var ds = timer.GetTimer("Life").Value.ToString("0.0");
            float scale = 0.2f;
            var si = TextDrawer.MeasureFont("CandyBeans", ds);

            if (timer.GetTimer("Life") < max / 2)
            {
                a.AddAction(Actions.ColorAction(a, Color.White, Color.Red, 0.2f, Interpolation.smooth));
                a.AddAction(Actions.ColorAction(a, Color.Red, Color.White, 0.2f, Interpolation.smooth));
            } 

            TextDrawer.BatchQueue("CandyBeans", ds, 
                new Vector2(pos.renderPosition.X, pos.renderPosition.Y - si.Height * scale * 0.8f),
                a.color, Color.Black, scale, 1, 1, 1, 4);

            timer.GetTimer("Life").Value -= dt;
            if (timer.GetTimer("Life") <= 0)
            {
                timer.GetTimer("Life").Value = 0;
                if(timer.GetTimer("leeway") <= 0)
                    sm.SetStateUpdate("Shrink", timer);
                timer.GetTimer("leeway").Value -= dt;
            }

        }

        public override void Draw(SpriteBatch sb, float dt)
        {
            CutLogic();
            Explode(sb, dt);

            if(leftHitbox.Contains(PlayScreen.mouseCoordinate))
                sb.DrawCircle(leftHitbox, 12, Color.Green, ppm);
            else
                sb.DrawCircle(leftHitbox, 12, Color.Red, ppm);
            if (rightHitbox.Contains(PlayScreen.mouseCoordinate))
                sb.DrawCircle(rightHitbox, 12, Color.Green, ppm);
            else
                sb.DrawCircle(rightHitbox, 12, Color.Red, ppm);

            if(begin)
                sb.DrawLine(leftHitbox.Center, rightHitbox.Center, Color.HotPink, ppm * tolerance);
            else
                sb.DrawLine(leftHitbox.Center, rightHitbox.Center, Color.Yellow, ppm * tolerance);

            sb.DrawCircle(new CircleF(rightHitbox.Center, radius * 2), 12, Color.Red, ppm);


            
        }
    }
}
