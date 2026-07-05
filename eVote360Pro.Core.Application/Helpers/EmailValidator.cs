using System.Net.Mail;

namespace eVote360Pro.Core.Application.Helpers
{
    public static class EmailValidator
    {
        public static bool IsAValidEmail(string email)
        {
            try
            {
                var address = new MailAddress(email);
                return string.Equals(address.Address, email, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
