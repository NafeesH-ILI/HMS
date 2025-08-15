namespace hms.Services.Interfaces
{
    public interface IUNameService
    {
        public string UNameOf(string fullName);

        public string Generate(string table, string fullName);
    }
}
