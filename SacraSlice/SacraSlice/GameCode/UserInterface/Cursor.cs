using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using SacraSlice.Dependencies.Engine;
using System.Diagnostics;

namespace SacraSlice.GameCode.UserInterface
{
    public class Cursor
    {
        Vector2[] vectors = new Vector2[10];
        float u;
        float h;
        public Cursor(float updateFrequency, float headWidth)
        {
            u = updateFrequency;
            h = headWidth;
        }
        float timer;
        public void Update(Vector2 pos, float elapsed, bool updateHead)
        {
            timer += elapsed;
            if(timer > u)
            {
                timer = 0;
                for (int i = vectors.Length - 1; i > 0; i--)
                {
                    vectors[i] = vectors[i - 1];
                }
                if(updateHead) vectors[0] = pos;
            }
        }

        public void Set(Vector2 v)
        {
            for (int i = 0; i < vectors.Length; i++)
            {
                vectors[i] = v;
            }
        }

        public void Draw(SpriteBatch sb, float ppm, float outline)
        {
            //var head = h;
            for(int i = 0; i < vectors.Length - 1; i++)
            {

                var head = h * (1 - (i / (vectors.Length - 1f)));
                sb.DrawLine(vectors[i], vectors[i+1], Color.Black, ppm * (head + outline), 0.01f);
                if (vectors[i] != vectors[i + 1])
                {
                    sb.DrawCircle(new CircleF(vectors[i], ppm / 2 * (head + outline)), 12, Color.Black, ppm / 2 * (head + outline), 0.00001f);
                }
                    
            }

            for (int i = 0; i < vectors.Length - 1; i++)
            {

                var head = h * (1 - (i / (vectors.Length - 1f)));
                sb.DrawLine(vectors[i], vectors[i + 1], Color.WhiteSmoke, head * ppm);
                if (vectors[i] != vectors[i + 1])
                {
                    sb.DrawCircle(new CircleF(vectors[i], head * ppm / 2), 12, Color.WhiteSmoke, head * ppm / 2);
                }

            }

        }
    }
}
