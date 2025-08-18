using System.Text;

namespace hms.Utils
{
    public static class RandomPass
    {
        public static string RandomPassword(int length = 20)
        {
            string pool = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM" +
                "1234567890" + "~!@#$%^&*()_+-=[]{};:,./<>?";
            StringBuilder password = new();
            Random random = new();
            for (int i = 0; i < length; i++)
            {
                password.Append(pool[random.Next(pool.Length)]);
            }
            return password.ToString();
        }
    }
}
