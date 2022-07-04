using System;

namespace Lessons.ClientCardApi.Abstraction.Data.Models
{
    
    public sealed class CreditCardInfoModel : BaseCardInfoModel
    {
        public long Id { get; set; }
        public override string FirstName { get; set; }
        public override string LastName { get; set; }
        public override string Patronymic { get; set; }
        public override DateTime BirthDate { get; set; }
        public override long PassportNumber { get; set; }
        public override long PhoneNumber { get; set; }
        public override string Email { get; set; }
        public override long CreditCardNumber { get; set; }
    }
}