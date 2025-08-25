namespace hms.Utils
{
    public static class Consts
    {
        public const string ConnStr = "Host=127.0.0.1;Username=postgres;Password=abcd1234;Database=hms";
        public const int PageSizeMax = 50;
        public const double OtpValidityMinutes = 2;
        public const int OtpCleanupMinutes = 2;
        public const int ApptAutoCancelMinutes = 30;
        public const int CookieValidityMinutes = 60;
        public const int WebSockKeepAliveMinutes = 2;

        public const int UNameMinLen = 3;
    }
}
