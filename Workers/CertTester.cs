using System.Security.Cryptography.X509Certificates;

namespace RecoverCertPassword.Workers
{
    interface ICertTester
    {
        bool TestPassword(string certPath, string certPassword);
    }


    class CertTester : ICertTester
    {
        private CertTester() { }
        private static CertTester _tester = new CertTester();

        public static CertTester Tester { get { return _tester; } }

        public bool TestPassword(string certPath, string certPassword)
        {
            try
            {
                var _certificate = new X509Certificate2(certPath, certPassword);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
