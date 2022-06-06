using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using SacraSlice.Dependencies.Engine;
using SacraSlice.Dependencies.Engine.ECS.Component;
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
                    //dead.Value = true;
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

        public override void Draw(SpriteBatch sb)
        {
            CutLogic();

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
