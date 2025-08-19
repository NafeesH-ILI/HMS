namespace hms.Utils
{
    public class PaginatedResponse<T>
    {
        public required int Count { get; set; }
        public required T Value { get; set; }
    }
}
