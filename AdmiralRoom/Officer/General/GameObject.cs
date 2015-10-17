namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public abstract class GameObject<T> : NotifyBase, IUpdatable<T>
    {
        protected T rawdata;
        public GameObject() { }
        public GameObject(T data)
        {
            rawdata = data;
            UpdateProp();
        }
        public void Update(T data)
        {
            rawdata = data;
            UpdateProp();
            OnPropertyChanged(null);
        }
        protected virtual void UpdateProp() { }
    }
}
