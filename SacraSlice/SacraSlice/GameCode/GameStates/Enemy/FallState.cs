using MonoGame.Extended;
using SacraSlice.Dependencies.Engine;
using SacraSlice.Dependencies.Engine.ECS.Component;
using SacraSlice.Dependencies.Engine.States;
using System;
using System.Collections.Generic;
using System.Text;

namespace SacraSlice.GameCode.GameStates.Enemy
{
    public class FallState : State
    {

        public FallState(StateManager sm, float dt, Position p, Timer t) : base(sm, dt, p, t)
        {
            name = "Fall";
        }
        public override void Act()
        {
            if (pos.ground)
                sm.SetStateUpdate("Cut", timer);
        }
    }
}
