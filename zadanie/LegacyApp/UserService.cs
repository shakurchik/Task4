using System;

namespace LegacyApp
{
    public class UserService
    {
        private readonly ClientRepository _clientRepository=new();
        private readonly UserCreditService _userCreditService=new();

        
        public bool AddUser(string firstName, string lastName, string email, DateTime dateOfBirth, int clientId)
        {
            if (!IsValidUserInput(firstName, lastName, email, dateOfBirth)) return false;

            var client = _clientRepository.GetById(clientId);
            var user = CreateUser(firstName, lastName, email, dateOfBirth, client);

            if (ShouldAssignCreditLimit(client.Type))
            {
                int creditLimit = CalculateCreditLimit(client.Type, lastName, dateOfBirth);
                if (creditLimit < 500) return false;

                user.HasCreditLimit = true;
                user.CreditLimit = creditLimit;
            }

            UserDataAccess.AddUser(user);
            return true;
        }

        private User CreateUser(string firstName, string lastName, string email, DateTime dateOfBirth, Client client)
        {
            return new User
            {
                FirstName = firstName,
                LastName = lastName,
                EmailAddress = email,
                DateOfBirth = dateOfBirth,
                Client = client,
                // Initialize other properties as necessary
            };
        }

        private bool ShouldAssignCreditLimit(string clientType)
        {
            return clientType != "VeryImportantClient";
        }

        private bool IsValidUserInput(string firstName, string lastName, string email, DateTime dateOfBirth)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                return false;
            }

            if (!email.Contains("@") || !email.Contains("."))
            {
                return false;
            }

            var age = DateTime.Now.Year - dateOfBirth.Year;
            if (dateOfBirth > DateTime.Now.AddYears(-age)) age--;

            if (age < 21)
            {
                return false;
            }

            return true;
        }
        private int CalculateCreditLimit(string clientType, string lastName, DateTime dateOfBirth)
        {
            var baseCreditLimit = _userCreditService.GetCreditLimit(lastName, dateOfBirth);

            if (clientType == "ImportantClient")
            {
                return baseCreditLimit * 2;
            }

            return baseCreditLimit;
        }

    }
}
