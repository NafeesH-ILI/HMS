namespace hms.Utils
{
    public static class Roles
    {
        public const string SuperAdmin = "SuperAdmin";
        public const string Admin = "Admin,SuperAdmin";
        public const string Receptionist = "Receptionist,Admin,SuperAdmin";
        public const string Anyone = "Patient,Doctor,Receptionist,Admin,SuperAdmin";
        public const string AnyoneButPatient = "Doctor,Receptionist,Admin,SuperAdmin";
    }
}
