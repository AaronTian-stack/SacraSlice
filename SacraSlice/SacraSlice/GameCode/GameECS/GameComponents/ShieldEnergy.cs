using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Input;
using SacraSlice.Dependencies.Engine;
using SacraSlice.Dependencies.Engine.Scene;
using SacraSlice.Dependencies.Engine.Scene.ActionClasses;
using SacraSlice.GameCode.Screens;
using System;
using System.Collections.Generic;
using System.Text;

namespace SacraSlice.GameCode.GameECS.GameComponents
{
    public class ShieldEnergy
    {
        public float energyTimer;
        public float energyMax = .8f;
        GameCamera gd;
        public bool defending;
        RepeatAction ra;
        public ShieldEnergy(GameCamera cam)
        {
            this.gd = cam;
            energyTimer = energyMax;
            ra = new RepeatAction(a,
               new ColorAction(a, Color.White, Color.Red, 0.5f, Interpolation.smooth)
               , new ColorAction(a, Color.Red, Color.White, 0.5f, Interpolation.smooth));
        }
        NinePatch energy = new NinePatch(GameContainer.atlas.FindRegion("shieldbar"), 2, 2, 2, 2);
        Sprite pixel = new Sprite(GameContainer.atlas.FindRegion("whitepixel"));

        Color green = new Color(30, 188, 115);
        Color orange = new Color(251, 107, 29);
        Color red = new Color(179, 56, 49);


        public float clickTimer;
        public float realClick;
        float clickTime = 0.1f;

        float notclickTimer;
        float rechargeWait = 0.3f;
        float recharge;
        float rechargeWaitLong = 1f;
        Actor a = new Actor();
        public bool HasActions { get => a.actions.Count != 0; }

        public void Draw(SpriteBatch sb, float dt, float ppm)
        {
            var v = gd.orthoCamera.BoundingRectangle;
            energy.position = new Vector2(v.Center.X, v.Top + 22f);

            energy.Scale = new Vector2(80f, .5f);
            //DebugLog.Print(this, realClick);
            a.Act(dt);
            if (PlayScreen.mouse.IsButtonDown(MouseButton.Right))
            {
                clickTimer += dt; realClick += dt;
                
                notclickTimer = 0;
                if (energyTimer > 0)
                {
                    defending = true;
                    energyTimer -= dt;
                    recharge = rechargeWait;
                }
                else
                {
                    defending = false;
                    recharge = rechargeWaitLong;
                    if(a.actions.Count == 0)
                        a.AddAction(ra);
                }         
            }
            else
            {
                realClick = 0;
                notclickTimer += dt;

                if(notclickTimer > recharge)
                {
                    a.ClearActions();
                    if (energyTimer < energyMax)
                        energyTimer += dt;
                }
                else
                    clickTimer += dt;

                if (notclickTimer > recharge + 0.5f)
                {
                    clickTimer -= dt;
                    
                }
                    
                defending = false;
            }

            clickTimer = Math.Clamp(clickTimer, 0, clickTime);
            realClick = Math.Clamp(realClick, 0, 999);
            var opa = Interpolation.smooth.Apply(0, 255, clickTimer / clickTime);
            if (a.actions.Count != 0)
                energy.color = a.color;
            else
                energy.color = Color.White;

            energy.color.A = (byte)opa;
            energy.Draw(sb, ppm, 0.000002f);
            pixel.Position = energy.position;


            Color color;
            color = green;

            if (energyTimer < energyMax * 0.7f)
            {
                color = orange;
            }
                

            if (energyTimer < energyMax * 0.4f)
            {
                color = red;
            }

            float s = 0.3f;
            pixel.Color.R = (byte)Interpolation.smooth.Apply(pixel.Color.R, color.R, s * dt / (1/120f));
            pixel.Color.G = (byte)Interpolation.smooth.Apply(pixel.Color.G, color.G, s * dt / (1 / 120f));
            pixel.Color.B = (byte)Interpolation.smooth.Apply(pixel.Color.B, color.B, s * dt / (1 / 120f));



            pixel.Color.A = (byte)opa;


            pixel.Scale = new Vector2(42f * Math.Max(0, energyTimer / energyMax), 2.25f);
            pixel.Draw(sb, 0.000001f);



        }
    }
}
