namespace hms.Utils
{
    public static class Pagination
    {
        public static bool IsValid(int page, int pageSize)
        {
            return page >= 1 && pageSize >= 1 && pageSize <= Consts.PageSizeMax;
        }
    }
}
