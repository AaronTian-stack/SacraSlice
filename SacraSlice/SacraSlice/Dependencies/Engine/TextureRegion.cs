using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SacraSlice.Dependencies.Engine
{
    public struct TextureRegion
    {
        public Texture2D texture;

        public int index;

        public Rectangle sourceRectangle;
        public int width { get => sourceRectangle.Width; }
        public int height { get => sourceRectangle.Height; }

        public Vector2 Scale;
        public TextureRegion(TextureRegion reg) // pass in Atlas.findRegion
        {
            this = reg;
            Scale = new Vector2(1);
        }

        public TextureRegion(Texture2D tex, Rectangle rect)
        {
            texture = tex;
            sourceRectangle = rect;
            index = 0;
            Scale = new Vector2(1);
        }



        public void Draw(SpriteBatch sb, Vector2 Position, Color color, float layerDepth)
        {
            sb.Draw(texture, Position, sourceRectangle, color, 0
                , new Vector2(sourceRectangle.Width / 2, sourceRectangle.Height / 2), Scale, SpriteEffects.None, layerDepth);
        }


    }
}
