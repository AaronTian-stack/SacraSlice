namespace SacraSlice.Dependencies.Engine.Scene
{
    public class ParallelAction : Action
    {
        Action[] list;
        public ParallelAction(Actor a, params Action[] list) : base(a)
        {
            this.list = list;
        }
        bool finished;
        public override bool Act(float elapsedTime)
        {
            finished = true;
            foreach (Action a in list)
            {
                if (!a.Act(elapsedTime))
                    finished = false;
            }
            return finished;
        }
    }
}
