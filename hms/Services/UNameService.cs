using hms.Models;
using hms.Services.Interfaces;
using hms.Utils;

namespace hms.Services
{
    public class UNameService(DbCtx ctx) : IUNameService
    {
        private readonly DbCtx _ctx = ctx;
        private static readonly object _lock = new();
        private static string UNameOf(string fullName)
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

        public string Generate(string fullName)
        {
            string baseName = UNameOf(fullName);
            int count = 0;
            lock (_lock) // TODO: lock the DB table as well
            {
                UName? uName = _ctx.UNames.Find(baseName);
                if (uName == null)
                {
                    uName = new UName() { Name = baseName, Count = 1 };
                    _ctx.UNames.Add(uName);
                }
                else
                {
                    count = uName.Count;
                    uName.Count++;
                    _ctx.UNames.Update(uName);
                }
                _ctx.SaveChanges();
            }
            if (count == 0)
                return baseName;
            return baseName + "_" + count;
        }
    }
}
