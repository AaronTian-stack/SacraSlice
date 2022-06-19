using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SacraSlice.Dependencies.Engine;
using SacraSlice.Dependencies.Engine.Scene;
using System;
using System.Collections.Generic;
using System.Text;

namespace SacraSlice.GameCode.UserInterface
{
    public class ScreenFlash
    {
        GraphicsDevice gd;
        public ScreenFlash(GraphicsDevice gd)
        {
            this.gd = gd;
            normal.actions.Add(Actions.ColorAction(normal, 
                new Color(), new Color(), 0, Interpolation.linear));
        }

        public Actor normal = new Actor();
        public Actor priority = new Actor();
        Sprite s = new Sprite(GameContainer.atlas.FindRegion("whitepixel"));
        public void Draw(SpriteBatch sb, float dt)
        {
            var viewW = gd.Adapter.CurrentDisplayMode.Width;
            var viewH = gd.Adapter.CurrentDisplayMode.Height;

            s.Position = new Vector2(viewW / 2, viewH / 2);
            s.Scale = new Vector2(viewW + 2, viewH);

            s.Color = new Color();

            if (priority.actions.Count != 0)
            {
                s.Color = priority.color;
                priority.Act(dt);
            }
            else if (normal.actions.Count != 0)
            {
                s.Color = normal.color;
                normal.Act(dt);
            }
            
            if(priority.actions.Count != 0 || normal.actions.Count != 0)
                s.Draw(sb);
        }

    }
}
