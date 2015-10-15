namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public abstract class GameObject<T> : NotifyBase
    {
        protected T rawdata;
        public GameObject(T data)
        {
            rawdata = data;
            UpdateProp();
        }
        public void Update(T data)
        {
            rawdata = data;
            UpdateProp();
            OnPropertyChanged();
        }
        protected virtual void UpdateProp() { }
    }
}
