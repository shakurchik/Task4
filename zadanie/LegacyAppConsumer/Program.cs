using LegacyApp;
using System;

namespace LegacyAppConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var clientRepository = new ClientRepository();
            var userCreditService = new UserCreditService();
            var userService = new UserService(clientRepository, userCreditService);
        }
    }
}
