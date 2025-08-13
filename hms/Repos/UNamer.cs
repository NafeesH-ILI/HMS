using hms.Models;

namespace hms.Repos
{
    public class UNamer
    {
        private static readonly DbCtx ctx = new();
        private static readonly object _lock = new();
        //private readonly static HashSet<(string, string)> ongoing = [];
        public static string UNameOf(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return string.Empty;
            var parts = fullName.Trim().Split([' '], StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1)
                return parts[0].ToLower();
            var initials = "";
            for (int i = 0; i < parts.Length - 1; i++)
                initials += char.ToUpper(parts[i][0]);
            var last = parts[parts.Length - 1].ToLower();
            last = char.ToUpper(last[0]) + last.Substring(1).ToLower();
            return $"{initials}.{last}";
        }

        public static string Generate(string table, string fullName)
        {
            string baseName = UNameOf(fullName);
            int count = 0;
            lock (_lock) // TODO: lock the DB table as well
            {
                UName? uName = ctx.UNames.Where(u => u.Name == baseName && u.Table == table).FirstOrDefault();
                if (uName == null)
                {
                    uName = new UName() { Name = baseName, Table = table, Count = 1 };
                    ctx.UNames.Add(uName);
                }
                else
                {
                    count = uName.Count;
                    uName.Count++;
                    ctx.UNames.Update(uName);
                }
                ctx.SaveChanges(); // TODO: await on it?
            }
            if (count == 0)
                return baseName;
            return baseName + "_" + count;
        }
    }
}
