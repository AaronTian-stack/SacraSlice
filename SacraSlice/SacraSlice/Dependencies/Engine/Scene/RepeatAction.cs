using System.Collections.Generic;

namespace SacraSlice.Dependencies.Engine.Scene
{
    public class RepeatAction : Action
    {
        List<Action> list;

        /// <summary>
        /// Sequence of actions that repeats forever
        /// </summary>
        public RepeatAction(Actor a, params Action[] list) : base(a)
        {
            this.list = new List<Action>(list);

            foreach (Action an in list)
            {
                an.poolOverride = true;
            }
        }
        public override bool Act(float elapsedTime)
        {

            if (list[0].Act(elapsedTime))
            {
                list[0].timer = 0;
                list.Add(list[0]);
                list.RemoveAt(0);
            }

            return false;
        }
    }
}
