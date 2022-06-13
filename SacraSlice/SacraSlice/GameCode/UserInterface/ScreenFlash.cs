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
        }

        public Actor normal = new Actor();
        public Actor priority = new Actor();
        Sprite s = new Sprite(GameContainer.atlas.FindRegion("whitepixel"));
        public void Draw(SpriteBatch sb, float dt)
        {
            var view = gd.Viewport;
            s.Position = new Vector2(view.Width / 2, view.Height / 2);
            s.Scale = new Vector2(view.Width, view.Height);

            normal.Act(dt);
            priority.Act(dt);

            s.Color = new Color(0, 0, 0, 0);

            if (priority.actions.Count != 0)
                s. Color = priority.color;
            else if (normal.actions.Count != 0)
                s.Color = normal.color;

            s.Draw(sb);
        }

    }
}
