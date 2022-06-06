using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SacraSlice.Dependencies.Engine
{
    public class TextDrawer
    {
        static Dictionary<string, SpriteFont> lookup = new Dictionary<string, SpriteFont>();
        public static void AddSpriteFont(string name, SpriteFont font)
        {
            lookup.Add(name, font);
        }

        public static SpriteFont GetFont(string s)
        {
            return lookup[s];
        }

        public static void DrawText(SpriteBatch sb, string font, string text, Vector2 pos, float scale, Color color,
            Color outlineColor, float outlineWidth = 0)
        {
            DrawText(sb, font, text, pos, scale, color, outlineColor, outlineWidth, outlineWidth, outlineWidth, outlineWidth);
        }

        public static void DrawText(SpriteBatch sb, string font, string text, Vector2 pos, float scale, Color color, Color outlineColor,
           float outlineLeft = 0, float outlineRight = 0, float outlineUp = 0, float outlineDown = 0
           )
        {
            var sf = lookup[font];
            
            if(outlineDown != 0)
                sb.DrawString(sf, text, new Vector2(pos.X, pos.Y + outlineDown), outlineColor, 0, new Vector2(), 1f, SpriteEffects.None, 0);
            if (outlineUp != 0)
                sb.DrawString(sf, text, new Vector2(pos.X, pos.Y - outlineUp), outlineColor, 0, new Vector2(), 1f, SpriteEffects.None, 0);
            if (outlineRight != 0)
                sb.DrawString(sf, text, new Vector2(pos.X + outlineRight, pos.Y), outlineColor, 0, new Vector2(), 1f, SpriteEffects.None, 0);
            if (outlineLeft != 0)
                sb.DrawString(sf, text, new Vector2(pos.X - outlineLeft, pos.Y), outlineColor, 0, new Vector2(), 1f, SpriteEffects.None, 0);

            if (outlineDown != 0 && outlineRight != 0)
                sb.DrawString(sf, text, new Vector2(pos.X + outlineRight, pos.Y + outlineDown), outlineColor, 0, new Vector2(), 1f, SpriteEffects.None, 0);
            if (outlineUp != 0 && outlineRight != 0)
                sb.DrawString(sf, text, new Vector2(pos.X + outlineRight, pos.Y - outlineUp), outlineColor, 0, new Vector2(), 1f, SpriteEffects.None, 0);
            if (outlineDown != 0 && outlineLeft != 0)
                sb.DrawString(sf, text, new Vector2(pos.X - outlineLeft, pos.Y + outlineDown), outlineColor, 0, new Vector2(), 1f, SpriteEffects.None, 0);
            if (outlineUp != 0 && outlineLeft != 0)
                sb.DrawString(sf, text, new Vector2(pos.X - outlineLeft, pos.Y - outlineUp), outlineColor, 0, new Vector2(), 1f, SpriteEffects.None, 0);
            
            sb.DrawString(sf, text, pos, color, 0, new Vector2(), scale, SpriteEffects.None, 0);
        }
    }
}
