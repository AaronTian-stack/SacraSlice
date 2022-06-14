using System;
using System.Collections.Generic;
using System.Text;

namespace SacraSlice.Dependencies.Engine.Scene.ActionClasses
{
    public class EventAction : Action
    {
        EventHandler eventH;
        public EventAction(Actor a, EventHandler eventH) : base(a)
        {
            this.eventH = eventH;
        }
        public override bool Act(float elapsedTime)
        {
            if (elapsedTime != 0)
                eventH?.Invoke(this, EventArgs.Empty);
            return true;
        }
    }
}
