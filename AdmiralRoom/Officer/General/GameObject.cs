using System;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public abstract class GameObject<T> : NotifyBase, IUpdatable<T>, IIdentifiable, IComparable<GameObject<T>>
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
            OnAllPropertyChanged();
        }
        protected virtual void UpdateProp() { }
        public void Update()
        {
            UpdateProp();
            OnAllPropertyChanged();
        }
        public abstract int Id { get; }
        public int CompareTo(GameObject<T> other)
        {
            return Id - other.Id;
        }
    }
}
