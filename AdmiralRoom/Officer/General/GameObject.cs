using System;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public abstract class GameObject<T> : NotificationObject, IUpdatable<T>, IIdentifiable, IComparable<GameObject<T>>
    {
        public T rawdata { get; private set; }
        protected GameObject(T data)
        {
            Update(data);
        }
        public void Update(T data)
        {
            rawdata = data;
            UpdateProp();
            OnAllPropertyChanged();
        }
        protected virtual void UpdateProp() { }
        public void Update()
        {
            UpdateProp();
            OnAllPropertyChanged();
        }
        public abstract int Id { get; }
        public int CompareTo(GameObject<T> other) => Id - other.Id;
    }
}
