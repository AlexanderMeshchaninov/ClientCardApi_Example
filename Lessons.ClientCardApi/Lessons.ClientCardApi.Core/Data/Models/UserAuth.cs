using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Lessons.ClientCardApi.Abstraction.Data.Models
{
    public sealed class UserAuth : IdentityUser
    {
        [Key]
        public override string Id { get; set; }
    }
}