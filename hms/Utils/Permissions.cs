using hms.Models;

namespace hms.Utils
{
    public class Permissions
    {
        public static bool CanAffect(User.Types actor, User.Types subject)
        {
            if (actor == User.Types.Receptionist)
            {
                if (subject != User.Types.Patient)
                {
                    return false;
                }
            }
            else if (actor == User.Types.Admin)
            {
                if (subject == User.Types.SuperAdmin)
                {
                    return false;
                }
            }
            else if (subject != User.Types.SuperAdmin)
            {
                return false;
            }
            return true;
        }
    }
}
