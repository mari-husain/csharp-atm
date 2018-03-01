using System;
namespace ATMWeb
{
    [Serializable]
    public class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal Balance { get; set; }
        public string PINHash { get; set; }

        public User()
        {
            FirstName = "John";
            LastName = "Doe";
            Balance = 1000;
        }

        public User(string _firstName, string _lastName, decimal _balance)
        {
            FirstName = _firstName;
            LastName = _lastName;
            Balance = _balance;
        }
    }
}
