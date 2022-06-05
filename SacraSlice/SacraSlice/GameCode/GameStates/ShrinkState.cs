using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using SacraSlice.Dependencies.Engine;
using SacraSlice.Dependencies.Engine.ECS.Component;
using SacraSlice.Dependencies.Engine.States;
using SacraSlice.GameCode.Screens;
using System;
using System.Collections.Generic;
using System.Text;

namespace SacraSlice.GameCode.GameStates
{
    public class ShrinkState : State
    {
        Wrapper<bool> dead;
        Wrapper<int> p;
        public ShrinkState(StateManager sm, Position pos, Timer t, Wrapper<bool> dead, Wrapper<int> p) : base(sm: sm, p: pos, t: t)
        {
            name = "Shrink";
            this.dead = dead;
            this.p = p;

        }

        public override void OnEnter()
        {
            p.Value++;
        }

        public override void Act()
        {

        }

        public override void Draw(SpriteBatch sb)
        {
            if(timer.GetTimer("State Time") > 5)
            {
                dead.Value = true;
                //pos.gravity = false;
                //sm.SetStateUpdate("Fall", timer);
                //pos.ground = false;
            }
        }
    }
}
