using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SacraSlice.Dependencies.Engine;
using SacraSlice.Dependencies.Engine.Scene;
using System;
using System.Collections.Generic;
using System.Text;

namespace SacraSlice.GameCode.GameECS.GameComponents
{
    public class Sword
    {
        // has a sword sprite and some methods for picking random floating position offset and rotation angle

        public Sprite s;
        public Actor a = new Actor();
        public Sword()
        {
            s = new Sprite(GameContainer.atlas.FindRegion("sword"));
        }

        Random random = new Random();
        public void Update()
        {

        }

        public void Draw(SpriteBatch sb)
        {
            
            s.Draw(sb);
        }


    }
}
