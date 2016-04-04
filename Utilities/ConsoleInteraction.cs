using System;

namespace RecoverCertPassword.Utilities
{
    class ConsoleInteraction
    {
        private static ConsoleInteraction _consoleInteraction = null;
        private ConsoleInteraction() {}

        public static ConsoleInteraction CurrentConsole
        {
            get
            {
                if (_consoleInteraction == null)
                    _consoleInteraction = new ConsoleInteraction();

                return _consoleInteraction;
            }
        }


        public void DisplayMessageAndExit(string message, int systemErrorCode = 0)
        {
            if (systemErrorCode != 0)
                Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine(message);
            Console.ResetColor();
            Console.WriteLine("press any key to exit");
            Console.ReadKey();
            Environment.Exit(systemErrorCode);
        }

        public void DisplayMessage(string message, ConsoleColor messageColor = ConsoleColor.White)
        {
            Console.ForegroundColor = messageColor;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public void DisplayChallenge(string message, string expectedResponse, ConsoleColor messageColor = ConsoleColor.White)
        {
            string userResponse = "";
            while (userResponse != expectedResponse)
            {
                DisplayMessage(message, messageColor);
                userResponse = Console.ReadLine();
            }
        }
    }
}
