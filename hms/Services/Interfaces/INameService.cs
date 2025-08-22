namespace hms.Services.Interfaces
{
    public interface INameService
    {
        public void ValidateName(string Name);
        public string Generate(string fullName);
    }
}
