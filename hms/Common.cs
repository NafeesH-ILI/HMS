namespace hms
{
    public class Common
    {
    }
    
    public class PaginatedResponse<T>
    {
        public required int Count { get; set; }
        public required T Value { get; set; }
    }
}
