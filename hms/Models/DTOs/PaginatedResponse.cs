namespace hms.Models.DTOs
{
    public class PaginatedResponse<T>
    {
        public required int Count { get; set; }
        public required T Value { get; set; }
    }
}
