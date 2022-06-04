namespace SacraSlice.Dependencies.Engine
{
    public class Wrapper<T> where T : struct
    {
        public static implicit operator T(Wrapper<T> w)
        {
            return w.Value;
        }

        public void ChangeFloatValue(Wrapper<float> w, int newValue)
        {
            w.Value = newValue;
        }


        public Wrapper(T t)
        {
            _t = t;
        }

        public override string ToString()
        {
            return _t.ToString();
        }

        private T _t;

        public T Value { get => _t; set => this._t = value; }
    }
}
