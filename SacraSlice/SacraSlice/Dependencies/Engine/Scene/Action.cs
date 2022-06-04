using MonoGame.Extended.Collections;

namespace SacraSlice.Dependencies.Engine.Scene
{
    public abstract class Action : IPoolable
    {
        public float timer;
        public Actor a;
        private ReturnToPoolDelegate _returnAction;

        public bool poolOverride;
        public Action(Actor a)
        {
            this.a = a;
        }

        public IPoolable NextNode { get; set; }
        public IPoolable PreviousNode { get; set; }

        public abstract bool Act(float elapsedTime);

        public void Initialize(ReturnToPoolDelegate returnDelegate)
        {
            _returnAction = returnDelegate;
        }

        public void Return()
        {
            if (_returnAction != null)
            {
                // not yet returned, return it now
                _returnAction.Invoke(this);
                // set the delegate instance reference to null, so we don't accidentally return it again
                _returnAction = null;
            }
        }
    }
}
