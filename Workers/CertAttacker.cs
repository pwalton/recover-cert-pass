using System;
using System.Collections.Generic;
using System.Linq;
using RecoverCertPassword.Utilities;

namespace RecoverCertPassword.Workers
{
    interface ICertAttacker
    {
        bool AttackPassword();
        void SetCriteria(AttackCriteria criteria);
        void SetConsole(ConsoleInteraction console);
    }

    class CertAttacker : ICertAttacker
    {
        private AttackCriteria _criteria;
        private ConsoleInteraction _console;
        private ICertTester _certTester;
        private IPasswordGenerator _passwordGenerator;

        private UInt64 passwordsRejected = 0;
        private int maxDepth;
        private int currentDepth;
        private bool passwordFound;

        private System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

        public CertAttacker(ICertTester certTester, IPasswordGenerator passwordGenerator)
        {
            _certTester = certTester;
            _passwordGenerator = passwordGenerator;
        }

        public bool AttackPassword()
        {
            passwordFound = false;
            currentDepth = 0;

            for (int i = 1; i <= _criteria.MaxDictionaryItemsToUse; i++)
            {
                maxDepth = i;
                CrunchDictionaryItems("", _criteria.DictionaryItems, new List<string>());
                if (passwordFound)
                    return true;
            }
            return false;
        }

        private void CrunchDictionaryItems(string prefix, IEnumerable<string> wordsToUse, IEnumerable<string> wordsToSkip)
        {
            if (++currentDepth <= maxDepth)
            {
                foreach (string pass in _passwordGenerator.GeneratePasswords(prefix, wordsToUse,
                    wordsToSkip, _criteria.MaximumPasswordLength))
                {
                    if (passwordFound)
                        return;

                    if (currentDepth == 1 && _criteria.DictionaryPrefixesToSkip.Contains(pass))
                        continue;

                    if (currentDepth == maxDepth && pass.Length >= _criteria.MimimumPasswordLength
                        && _certTester.TestPassword(_criteria.CertificatePath, pass))
                    {
                        _console.DisplayMessage(String.Format("Password found: {0}", pass), ConsoleColor.Green);
                        _console.DisplayChallenge("Enter password to finish: ", pass);
                        passwordFound = true;
                        return;
                    }
                    else if (currentDepth != maxDepth)
                        CrunchDictionaryItems(pass, wordsToUse,
                            (
                                _criteria.DictionaryItemsMayRepeat ?
                                    new List<string>() :    
                                    wordsToSkip.Concat(new List<string>() { pass.Remove(0, prefix.Length) })
                                    
                            )
                        );

                    if ((++passwordsRejected) % 1000 == 0)
                        _console.DisplayMessage(String.Format("Current Depth: {0}, Passwords Rejected: {1}",
                            currentDepth, passwordsRejected));
                }
            }
            --currentDepth;
        }

        public void SetCriteria(AttackCriteria criteria)
        {
            _criteria = criteria;
        }

        public void SetConsole(ConsoleInteraction console)
        {
            _console = console;
        }
    }
}
