using System;

namespace Lessons.ClientCardApi.Abstraction.Data
{
    public abstract class BaseCardInfoModel
    {
        public abstract string FirstName { get; set; }
        public abstract string LastName { get; set; }
        public abstract string Patronymic { get; set; }
        public abstract DateTime BirthDate { get; set; }
        public abstract long PassportNumber { get; set; }
        public abstract long PhoneNumber { get; set; }
        public abstract string Email { get; set; }
        public abstract long CreditCardNumber { get; set; }
    }
}