using Microsoft.Xna.Framework;
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

namespace SacraSlice.GameCode.GameStates
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
            sprite.Rotation = 45;
        }
        bool begin;

        float radius = 5;
        public override void Act()
        {

        }
        Random random = new Random();
        float max;
        RectangleF bounds;
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

            bounds = RectangleF.CreateFrom(leftHitbox.Center - new Vector2(radius), 
                rightHitbox.Center + new Vector2(radius));

            // pick a direction
            direction = random.Next(0, 2);

            // positive is clockwise
            timer.GetTimer("Angle").Value = MathHelper.ToDegrees(MathF.Atan2(rightHitbox.Center.Y - leftHitbox.Center.Y,
                rightHitbox.Center.X - leftHitbox.Center.X));

            if (direction == 1) // swap if from right to left
            {
                
                var hold = leftHitbox;
                leftHitbox = rightHitbox;
                rightHitbox = hold;
                timer.GetTimer("Angle").Value += 180;
            }

            rightHitbox.Radius *= 2;
            timer.GetTimer("Life").Value = PlayScreen.lifeTime;
            max = timer.GetTimer("Life");

            timer.GetTimer("leeway").Value = 0.5f;

            timer.GetSwitch("Sword Active").Value = true;

            a.ClearActions();
        }
        bool reset;
        public void CutLogic()
        {

            if (leftHitbox.Contains(PlayScreen.mouseCoordinate) && reset)
            {
                begin = true;
            }

            if (!PlayScreen.mouse.IsButtonDown(MouseButton.Left))
            {
                begin = false;
                reset = true;
            }

            if (timer.GetSwitch("Protect"))
            {
                if (begin)
                {
                    // swing sword
                    var dur = 0.25f;
                    var sc = 1.5f;
                    a1.AddAction(Actions.MoveFrom(a1, 0, 0, sc, sc, dur, Interpolation.swingOut));
                    a1.AddAction(Actions.Delay(a1, 0.5f));
                    a1.AddAction(Actions.MoveFrom(a1, sc, sc, 0, 0, dur, Interpolation.swingIn));
                    a1.scaleX = leftHitbox.Center.X; 
                    a1.scaleY = leftHitbox.Center.Y;
                    

                    timer.GetSwitch("Attack").Value = true;
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

            return MathF.Abs((x) *
                (leftHitbox.Center.Y - point.Y) -
                (leftHitbox.Center.X - point.X) * (y)) / MathF.Sqrt(x * x + y * y);
        }
        Actor a = new Actor();
        Actor a1 = new Actor();
        public void Explode(SpriteBatch sb, float dt)
        {

            var ds = timer.GetTimer("Life").Value.ToString("0.0");
            float scale = 0.2f;
            //var si = TextDrawer.MeasureFont("Main Font", ds);

            if (!timer.GetSwitch("Protect"))
            {
                timer.GetTimer("Life").Value -= dt;
                a.color = Color.White;
            }
                

            if (timer.GetTimer("Life") < max / 2)
            {
                a.AddAction(Actions.ColorAction(a, Color.White, Color.Red, 0.2f, Interpolation.smooth));
                a.AddAction(Actions.ColorAction(a, Color.Red, Color.White, 0.2f, Interpolation.smooth));
            }
            a.Act(dt);
            a1.Act(dt);

            if (timer.GetSwitch("Protect"))
                a.color = Color.Gold;


            if(timer.GetSwitch("sword"))
                TextDrawer.BatchQueue("Main Font", ds, 
                    new Vector2(pos.renderPosition.X, pos.renderPosition.Y - hb.rect.Height),
                    a.color, Color.Black, scale, 2, 2, 2, 4);
            else
                TextDrawer.BatchQueue("Main Font", ds,
                    new Vector2(pos.renderPosition.X, pos.renderPosition.Y - hb.rect.Height * 0.6f),
                    a.color, Color.Black, scale, 2, 2, 2, 4);


            if (timer.GetTimer("Life") <= 0)
            {
                timer.GetTimer("Life").Value = 0;
                if(timer.GetTimer("leeway") < 0)
                {
                    sm.SetStateUpdate("Shrink", timer);
                    timer.GetSwitch("Sword Active").Value = false;
                }
                timer.GetTimer("leeway").Value -= dt;
            }

        }
        Sprite sprite = new Sprite(GameContainer.atlas.FindRegion("xRed"));
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

            SpriteAligner.BatchQueue(sprite, new Vector2(a1.scaleX, a1.scaleY), new Vector2(a1.x, a1.y), 0);

            arrow.Origin = new Vector2(0, arrow.Textureregion.height / 2);
            arrow.Rotation = timer.GetTimer("Angle");

            arrow.Scale = new Vector2(1, 1f); ;

            if(begin)
                arrow.Color = new Color(0, 1, 0, 0.8f);
            else if (timer.GetSwitch("Protect") && leftHitbox.Contains(PlayScreen.mouseCoordinate))
                arrow.Color = new Color(1, 0, 0, 0.8f);
            else
                arrow.Color = new Color(1, 1, 1, 0.8f);

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
    }
}
