using System;

namespace LegacyApp
{
    public class UserService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IUserCreditService _userCreditService;
        
        public UserService() : this(new ClientRepository(), new UserCreditService()) { }
        
        public UserService(IClientRepository clientRepository, IUserCreditService userCreditService)
        {
            _clientRepository = clientRepository;
            _userCreditService = userCreditService;
        }
        
        public bool AddUser(string firstName, string lastName, string email, DateTime dateOfBirth, int clientId)
        {
            if (!IsValidInput(firstName, lastName, email, dateOfBirth))
                return false;
            
            var client = _clientRepository.GetById(clientId);
            var user = CreateUser(firstName, lastName, email, dateOfBirth, client);
            
            if (!SetUserCreditInfo(user))
                return false;
            
            UserDataAccess.AddUser(user);
            return true;
        }
        
        private bool IsValidInput(string firstName, string lastName, string email, DateTime dateOfBirth)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
                return false;
            if (!email.Contains("@") || !email.Contains("."))
                return false;
            if (CalculateAge(dateOfBirth) < 21)
                return false;
            return true;
        }
        
        private int CalculateAge(DateTime dateOfBirth)
        {
            var now = DateTime.Now;
            int age = now.Year - dateOfBirth.Year;
            if (now.Month < dateOfBirth.Month || (now.Month == dateOfBirth.Month && now.Day < dateOfBirth.Day))
                age--;
            return age;
        }
        
        private User CreateUser(string firstName, string lastName, string email, DateTime dateOfBirth, Client client)
        {
            return new User
            {
                FirstName = firstName,
                LastName = lastName,
                EmailAddress = email,
                DateOfBirth = dateOfBirth,
                Client = client
            };
        }
        
        private bool SetUserCreditInfo(User user)
        {
            var client = (Client)user.Client;
            if (client.Type == ClientType.VeryImportantClient)
            {
                user.HasCreditLimit = false;
                return true;
            }
            
            int creditLimit = _userCreditService.GetCreditLimit(user.LastName, user.DateOfBirth);
            
            if (client.Type == ClientType.ImportantClient)
            {
                creditLimit *= 2;
                user.HasCreditLimit = true;
            }
            else
            {
                user.HasCreditLimit = true;
            }
            
            user.CreditLimit = creditLimit;
            
            if (user.HasCreditLimit && user.CreditLimit < 500)
                return false;
            return true;
        }
    }
}
