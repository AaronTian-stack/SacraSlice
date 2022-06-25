using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collections;
using SacraSlice.Dependencies.Engine;
using SacraSlice.Dependencies.Engine.InterfaceLayout;
using SacraSlice.GameCode.Screens;
using System;
using System.Collections.Generic;
using System.Text;

namespace SacraSlice.GameCode.UserInterface
{
    public struct Number 
    {
        public Number(float y, float alpha)
        {
            this.Y = y;
            this.alpha = alpha;
        }
        public float Y;
        public float alpha;
    }

    public class ScoreBoard
    {

        public ScoreBoard()
        {
            old = PlayScreen.life;
            oldScore = PlayScreen.score;
        }
        Sprite X = new Sprite(GameContainer.atlas.FindRegion("xBlank"));
        Sprite XRed = new Sprite(GameContainer.atlas.FindRegion("xRed"));

        float shakeTimer = 999;
        float shakeAmount = 0.005f;
        float shakeDuration = 2;

        float old, oldScore;
        FastRandom fastRandom = new FastRandom();

        Bag<Number> numbers = new Bag<Number>();
        
        public void Reset()
        {
            numbers.Clear();
            old = PlayScreen.life;
            oldScore = PlayScreen.score;
            shakeTimer = 999f;
        }
        public void Draw(SpriteBatch sb, float dt)
        {

            var s = "Score: " + PlayScreen.score;

            TextDrawer.DrawTextStatic("Main Font", s, new Vector2(.5f + CalculateShake(),
                .04f + CalculateShake()), .2f, Color.GhostWhite, Color.Black,
                1, 1, 1, 5);

            for(int i = numbers.Count - 1; i >= 0; i--)
            {

                var r = numbers[i];
                r.alpha -= 2 * (dt * 120f);
                r.Y -= 0.0005f;
                numbers[i] = r;
                Color c = Color.GhostWhite;
                c.A = (byte)Math.Clamp(numbers[i].alpha, 0, 255);

                TextDrawer.DrawTextStatic("Main Font", "+"+PlayScreen.justAdded, new Vector2(.66f + CalculateShake(),
               .06f + CalculateShake() + numbers[i].Y), .12f, c, c,
                0, 0, 0, 0, i * 0.00001f);

                if (numbers[i].alpha <= 0)
                   numbers.RemoveAt(i);

            }

            X.Rotation = 45;
            XRed.Rotation = 45;

            float pOff = 0.055f;

            if (old != PlayScreen.life)
            {
                GameContainer.sounds["LoseHealth"].Play();
                shakeTimer = 0;
                PlayScreen.camera.AddShake(0.2f, 0.1f, 2);
            }

            if(oldScore != PlayScreen.score)
            {
                // point adder
                numbers.Add(new Number(0, 255));
            }
                
            shakeTimer += dt;

            var scale = 0.1f;

            float y = 0.16f;
            if (PlayScreen.life < 3)
                SpriteAligner.DrawSprite(XRed, 0.5f + pOff + CalculateShake(), y + CalculateShake(), scale);
            else
                SpriteAligner.DrawSprite(X, 0.5f + pOff + CalculateShake(), y + CalculateShake(), scale);


            if (PlayScreen.life < 2)
                SpriteAligner.DrawSprite(XRed, 0.5f + CalculateShake(), y + CalculateShake(), scale);
            else
                SpriteAligner.DrawSprite(X, 0.5f + CalculateShake(), y + CalculateShake(), scale);

            if (PlayScreen.life < 1)
                SpriteAligner.DrawSprite(XRed, 0.5f - pOff + CalculateShake(), y + CalculateShake(), scale);
            else
                SpriteAligner.DrawSprite(X, 0.5f - pOff + CalculateShake(), y + CalculateShake(), scale);

            old = PlayScreen.life;
            oldScore = PlayScreen.score;
            
        }

        public float CalculateShake()
        {
            return fastRandom.NextSingle(-1, 1) * shakeAmount * Math.Clamp((shakeDuration - shakeTimer) / shakeDuration, 0, 1);
        }

    }
}
