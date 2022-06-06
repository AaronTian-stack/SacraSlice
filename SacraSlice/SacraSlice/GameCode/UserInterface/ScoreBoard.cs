using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using SacraSlice.Dependencies.Engine;
using SacraSlice.Dependencies.Engine.InterfaceLayout;
using System;
using System.Collections.Generic;
using System.Text;

namespace SacraSlice.GameCode.UserInterface
{
    public class ScoreBoard
    {
        Wrapper<int> score, life;

        // TODO: add the x's
        public ScoreBoard(Wrapper<int> score, Wrapper<int> life)
        {
            this.score = score; this.life = life; old = life;
        }
        Sprite X = new Sprite(GameContainer.atlas.FindRegion("xBlank"));
        Sprite XRed = new Sprite(GameContainer.atlas.FindRegion("xRed"));

        float shakeTimer = 999;
        float shakeAmount = 0.005f;
        float shakeDuration = 2;

        float old;
        FastRandom fastRandom = new FastRandom();
        public void Draw(SpriteBatch sb, float dt)
        {

            var s = "Score: " + score;

            TextDrawer.DrawTextStatic("CandyBeans", s, new Vector2(.5f + CalculateShake(),
                .07f + CalculateShake()), .15f, Color.GhostWhite, Color.Black,
                1, 1, 1, 5);

            X.Color = Color.White;
            X.Scale = new Vector2(0.8f);
            X.Rotation = 45;
            XRed.Color = Color.White;
            XRed.Scale = new Vector2(0.8f);
            XRed.Rotation = 45;

            float pOff = 0.075f;

            if (old != life)
                shakeTimer = 0;

            shakeTimer += dt;

            if (life < 3)
                SpriteAligner.DrawSprite(XRed, 0.5f + pOff + CalculateShake(), 0.2f + CalculateShake());
            else
                SpriteAligner.DrawSprite(X, 0.5f + pOff + CalculateShake(), 0.2f + CalculateShake());


            if (life < 2)
                SpriteAligner.DrawSprite(XRed, 0.5f + CalculateShake(), 0.2f + CalculateShake());
            else
                SpriteAligner.DrawSprite(X, 0.5f + CalculateShake(), 0.2f + CalculateShake());

            if (life < 1)
                SpriteAligner.DrawSprite(XRed, 0.5f - pOff + CalculateShake(), 0.2f + CalculateShake());
            else
                SpriteAligner.DrawSprite(X, 0.5f - pOff + CalculateShake(), 0.2f + CalculateShake());

            old = life;
            

        }

        public float CalculateShake()
        {
            return fastRandom.NextSingle(-1, 1) * shakeAmount * Math.Clamp((shakeDuration - shakeTimer) / shakeDuration, 0, 1);
        }

    }
}
