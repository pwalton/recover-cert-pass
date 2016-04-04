using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecoverCertPassword.Workers
{
    interface IPasswordGenerator
    {
        IEnumerable<string> GeneratePasswords(string prefix, IEnumerable<string> dictionary,
            IEnumerable<string> termsToAvoid, int maxLength);
    }

    class PasswordGenerator : IPasswordGenerator
    {
        public IEnumerable<string> GeneratePasswords(string prefix, IEnumerable<string> dictionary,
            IEnumerable<string> termsToAvoid, int maxLength)
        {
            foreach (string word in dictionary)
            {
                if (!termsToAvoid.Contains(word))
                {
                    var sb = new StringBuilder(prefix);
                    sb.Append(word);
                    if (sb.Length <= maxLength)
                        yield return sb.ToString();
                }
            }
        }
    }
}
