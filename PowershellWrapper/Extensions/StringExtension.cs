using System.Security;

namespace Powershell.Extensions
{
    public static class StringExtension
    {
        public static SecureString ToSecureString(this string password)
        {
            var securePass = new SecureString();
            if (!string.IsNullOrEmpty(password))
            {
                foreach (var secureChar in password.ToCharArray())
                {
                    securePass.AppendChar(secureChar);
                }
            }

            securePass.MakeReadOnly();
            return securePass;
        }
    }
}
