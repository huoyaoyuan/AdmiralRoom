using System;
using Meowtrix;
using Meowtrix.ComponentModel;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public abstract class GameObject : NotificationObject, IIdentifiable<int>, IComparable<GameObject>, IEquatable<GameObject>
    {
        protected virtual void UpdateProp() { }
        public void Update()
        {
            UpdateProp();
            OnAllPropertyChanged();
        }
        public abstract int Id { get; }
        public override int GetHashCode() => Id.GetHashCode();
        public int CompareTo(GameObject other) => Id - other.Id;
        public bool Equals(GameObject other) => Id == other?.Id;
    }
    public abstract class GameObject<T> : GameObject, IUpdatable<T>, IIdentifiable<int>
    {
        protected T rawdata;
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
    }
}
