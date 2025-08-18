using System.Text;

namespace hms.Utils
{
    public static class RandomPass
    {
        private static string Generate(string pool, int length)
        {
            StringBuilder password = new();
            Random random = new();
            for (int i = 0; i < length; i++)
            {
                password.Append(pool[random.Next(pool.Length)]);
            }
            return password.ToString();
        }

        public static string Password(int length = 20)
        {
            string pool = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM" +
                "1234567890" + "~!@#$%^&*()_+-=[]{};:,./<>?";
            return Generate(pool, length);
        }

        public static string OTP(int length = 10)
        {
            return Generate("0123456789", length);
        }
    }
}
