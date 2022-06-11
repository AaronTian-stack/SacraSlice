using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using SacraSlice.Dependencies.Engine;
using SacraSlice.Dependencies.Engine.ECS.Component;
using SacraSlice.Dependencies.Engine.Scene;
using SacraSlice.Dependencies.Engine.Scene.ActionClasses;
using SacraSlice.Dependencies.Engine.States;
using SacraSlice.GameCode.GameECS;
using SacraSlice.GameCode.Screens;
using System;
using System.Collections.Generic;
using System.Text;

namespace SacraSlice.GameCode.GameStates
{
    public class ShrinkState : State
    {
        Wrapper<int> p, l;
        Sprite sprite;
        public ShrinkState(StateManager sm, Position pos, Timer t, Wrapper<int> p,
            Wrapper<int> l, Sprite sprite, EntityFactory ef) : base(sm: sm, p: pos, t: t)
        {
            name = "Shrink";
            this.p = p;
            this.l = l;
            this.sprite = sprite;
            ra = new RepeatAction(a,
                new ColorAction(a, Color.White, Color.Red, 0.1f, Interpolation.smooth)
                , new ColorAction(a, Color.White, Color.Red, 0.1f, Interpolation.smooth));
            this.ef = ef;
        }
        RepeatAction ra;
        EntityFactory ef;

        public override void OnEnter()
        {
            PlayScreen.enemiesOnScreen.Value--;
            
            a.actions.Clear();
            if (timer.GetTimer("leeway") > 0)
                p.Value++;
            else
                l.Value--;

            ef.Free((int)pos.start);
            if (timer.GetSwitch("Killed"))
            {

                a.AddAction(ra);
                timer.GetSwitch("Killed").Value = false;
            }
        }

        public override void OnLeave()
        {
            a.actions.Clear();
            sprite.Color = Color.White;

        }

        public override void Act()
        {

        }
        Actor a = new Actor();
        public override void Draw(SpriteBatch sb, float dt)
        {
            a.Act(dt);
            sprite.Color = a.color;
            timer.GetSwitch("dead").Value = true;
        }
    }
}
