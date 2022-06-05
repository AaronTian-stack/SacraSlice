using Microsoft.Xna.Framework.Graphics;
using SacraSlice.Dependencies.Engine.ECS.Component;
using System;

namespace SacraSlice.Dependencies.Engine.States
{
    public abstract class State // A state is a class that contains a method for a certain action eg Run Crouch Fly ex.
    {
        public string name;
        public StateManager sm;

        public Timer timer;
        public InputManager im;
        public Position pos;
        public float dt;

        public State(StateManager sm)
        {
            this.sm = sm;
            if (name == null)
                throw new ArgumentNullException("State must have a name");
        }

        public State(StateManager sm = null, float dt = 0, Position p = null, Timer t = null, InputManager im = null)
        {
            this.sm = sm;
            timer = t;
            this.im = im;
            pos = p;
            this.dt = dt;
        }
        public abstract void Act();
        public virtual void Draw(SpriteBatch sb) { }
        public virtual void OnEnter() { }
        public virtual void OnLeave() { }
        public override string ToString()
        {
            return name;
        }
    }
}
