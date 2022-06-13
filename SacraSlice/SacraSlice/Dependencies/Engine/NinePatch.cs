using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SacraSlice.Dependencies.Engine
{
    public class NinePatch
    {

        /// <summary>
        /// 
        /// 0 1 2
        /// 3 4 5
        /// 6 7 8
        /// 
        /// </summary>

        TextureRegion texture;
        int left, right, top, bottom;
        public Vector2 position;
        public Color color = Color.White;
        public Vector2 Scale = new Vector2(1);

        TextureRegion[] textureRegions = new TextureRegion[9];
        public NinePatch(TextureRegion texture, int left, int right, int top, int bottom)
        {

            this.texture = texture;
            this.left = left;
            this.right = right;
            this.top = top;
            this.bottom = bottom;

            int middleWidth = texture.width - left - right;
            int middleHeight = texture.height - top - bottom;

            // generate sub texture regions

            var r = texture.sourceRectangle;
            var s = texture.texture;

            textureRegions[0] = new TextureRegion(s, new Rectangle(r.Left, r.Top, left, top));
            textureRegions[1] = new TextureRegion(s, new Rectangle(r.Left + left, r.Top, middleWidth, top));
            textureRegions[2] = new TextureRegion(s, new Rectangle(r.Left + left + middleWidth, r.Top, right, top));


            textureRegions[3] = new TextureRegion(s, new Rectangle(r.Left, r.Top + top, left, middleHeight));
            textureRegions[4] = new TextureRegion(s, new Rectangle(r.Left + left, r.Top + top, middleWidth, middleHeight));
            textureRegions[5] = new TextureRegion(s, new Rectangle(r.Left + left + middleWidth, r.Top + top, right, middleHeight));


            textureRegions[6] = new TextureRegion(s, new Rectangle(r.Left, r.Top + top + middleHeight, left, bottom));
            textureRegions[7] = new TextureRegion(s, new Rectangle(r.Left + left, r.Top + top + middleHeight, middleWidth, bottom));
            textureRegions[8] = new TextureRegion(s, new Rectangle(r.Left + left + middleWidth, r.Top + top + middleHeight, right, bottom));

        }

        public void Draw(SpriteBatch sb, float ppm, float depth)
        {
            Scale *= ppm;
            textureRegions[4].Scale = Scale;
            textureRegions[4].Draw(sb, position, color, depth);

            textureRegions[0].Draw(sb, position + new Vector2(-textureRegions[4].width * Scale.X / 2 - textureRegions[0].width / 2, 
                -textureRegions[4].height * Scale.Y / 2 - textureRegions[0].height / 2), color, depth);

            textureRegions[2].Draw(sb, position + new Vector2(textureRegions[4].width * Scale.X / 2 + textureRegions[2].width / 2,
                -textureRegions[4].height * Scale.Y / 2 - textureRegions[2].height / 2), color, depth);

            textureRegions[6].Draw(sb, position + new Vector2(-textureRegions[4].width * Scale.X / 2 - textureRegions[6].width / 2,
               textureRegions[4].height * Scale.Y / 2 + textureRegions[6].height / 2), color, depth);

            textureRegions[8].Draw(sb, position + new Vector2(textureRegions[4].width * Scale.X / 2 + textureRegions[8].width / 2,
               textureRegions[4].height * Scale.Y / 2 + textureRegions[8].height / 2), color, depth);

            textureRegions[5].Scale.Y = Scale.Y;
            textureRegions[5].Draw(sb, position + new Vector2(textureRegions[4].width * Scale.X / 2 + textureRegions[5].width / 2, 0), 
                color, depth);

            textureRegions[3].Scale.Y = Scale.Y;
            textureRegions[3].Draw(sb, position - new Vector2(textureRegions[4].width * Scale.X / 2 + textureRegions[3].width / 2, 0), 
                color, depth);

            textureRegions[1].Scale.X = Scale.X;
            textureRegions[1].Draw(sb, position - new Vector2(0, textureRegions[4].width * Scale.Y / 2 + textureRegions[1].height / 2), 
                color, depth);

            textureRegions[7].Scale.X = Scale.X;
            textureRegions[7].Draw(sb, position + new Vector2(0, textureRegions[4].width * Scale.Y / 2 + textureRegions[7].height / 2), 
                color, depth);
            Scale /= ppm;
        }
    }
}
