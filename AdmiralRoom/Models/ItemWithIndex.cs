namespace Huoyaoyuan.AdmiralRoom.Models
{
    public class ItemWithIndex<T>
    {
        public int Index { get; set; }
        public T Item { get; set; }
        public static ItemWithIndex<T> Generator(T item, int index) => new ItemWithIndex<T> { Index = index + 1, Item = item };
    }
}
