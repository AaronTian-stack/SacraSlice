using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SacraSlice.Dependencies.Engine
{
    public struct TextureRegion
    {
        public Texture2D texture;

        public int index;

        public Rectangle sourceRectangle;
        public float width { get => sourceRectangle.Width; }
        public float height { get => sourceRectangle.Height; }

        public TextureRegion(TextureRegion reg) // pass in Atlas.findRegion
        {
            this = reg;
        }

        public TextureRegion(Texture2D tex, Rectangle rect)
        {
            texture = tex;

            sourceRectangle = rect;

            index = 0;

        }



        public void draw(SpriteBatch sb, Vector2 position, Color color)
        {
            sb.Draw(texture, position, sourceRectangle, color);
        }


    }
}
