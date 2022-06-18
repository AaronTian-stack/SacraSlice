using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using SacraSlice.Dependencies.Engine;
using SacraSlice.Dependencies.Engine.ECS.Component;
using SacraSlice.Dependencies.Engine.States;
using System;
using System.Collections.Generic;
using System.Text;

namespace SacraSlice.GameCode.GameStates.Demon
{
    public class DemonState : State
    {
        public DemonState(StateManager sm, float dt, Position p, Timer t) : base(sm, dt, p, t)
        {
            name = "Demon";
        }
        public override void Act()
        {
           
        }

        public override void Draw(SpriteBatch sb, float dt)
        {
            
        }

        public override void OnEnter()
        {
            
        }

        public override void OnLeave()
        {
            
        }
    }
}
