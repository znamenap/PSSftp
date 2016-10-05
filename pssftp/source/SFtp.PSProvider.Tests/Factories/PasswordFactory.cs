using System.Linq;
using System.Management.Automation;
using System.Security;

namespace SFtp.PSProvider.Tests.Factories
{
    public class PasswordFactory
    {
        public static SecureString CreateSecureString(string value)
        {
            var result = new SecureString();
            value.ToList().ForEach(x => result.AppendChar(x));
            return result;
        }

        public static PSCredential CreatePSCredential(string userName, SecureString password)
        {
            return new PSCredential(userName, password);
        }

        public static PSCredential CreateTestingCredential()
        {
            return CreatePSCredential("username", CreateSecureString("password"));
        }
    }
}