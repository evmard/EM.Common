
namespace MicroServiceBase.Contract
{
    public class PagingParam<T> : JsonSerializable<T>
    {
        public int Count { get; set; }
        public int From { get; set; }
        public int SortType { get; set; }
        public string SortValue { get; set; }
    }
}
