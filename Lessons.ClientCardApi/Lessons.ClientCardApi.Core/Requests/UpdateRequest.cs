using System;
using Lessons.ClientCardApi.Abstraction.Data;

namespace Lessons.ClientCardApi.Abstraction.Requests
{
    public sealed class UpdateRequest : BaseCardInfoModel
    {
        public long SearchId { get; set; }
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