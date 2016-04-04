using System;
using RecoverCertPassword.Utilities;
using RecoverCertPassword.Workers;

namespace RecoverCertPassword
{
    class Program
    {
        static void Main(string[] args)
        {
            AttackCriteria criteria = AttackCriteria.CurrentCriteria;
            ConsoleInteraction consoleInteraction = ConsoleInteraction.CurrentConsole;

            consoleInteraction.DisplayMessage("Starting...");
            consoleInteraction.DisplayMessage(criteria.ToString(), ConsoleColor.Cyan);

            ICertAttacker attacker = new CertAttacker(CertTester.Tester, new PasswordGenerator());
            attacker.SetConsole(consoleInteraction);
            attacker.SetCriteria(criteria);

            if (!attacker.AttackPassword())
                consoleInteraction.DisplayMessageAndExit("No eligible password found.");
        }
    }
}
