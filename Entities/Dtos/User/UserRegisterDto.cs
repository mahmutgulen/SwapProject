using Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace Entities.Concrete.Dtos.User
{
    public class UserRegisterDto : IDto
    {
        [EmailAddress]
        [Required]
        public string UserEmail { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string UserSurname { get; set; }
        [Required]
        public decimal UserIdentityNumber { get; set; }
        [Required]
        public decimal UserPhoneNumber { get; set; }
        [Required]
        public string UserAddress { get; set; }
        [Required]
        public string Password { get; set; }


    }
}
