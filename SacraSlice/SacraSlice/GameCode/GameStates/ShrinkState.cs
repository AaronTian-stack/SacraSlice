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
        //Wrapper<bool> dead;
        Wrapper<int> p, l;
        public ShrinkState(StateManager sm, Position pos, Timer t, Wrapper<int> p,
            Wrapper<int> l) : base(sm: sm, p: pos, t: t)
        {
            name = "Shrink";
            this.p = p;
            this.l = l;
        }

        public override void OnEnter()
        {
            if (timer.GetTimer("Life") > 0)
                p.Value++;
            else
                l.Value--;
        }

        public override void Act()
        {

        }

        public override void Draw(SpriteBatch sb, float dt)
        {
            if(timer.GetTimer("State Time") > 5)
            {
                //dead.Value = true;
                timer.GetSwitch("dead").Value = true;
            }
        }
    }
}
