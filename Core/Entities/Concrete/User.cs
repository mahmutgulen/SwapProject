namespace Core.Entities.Concrete
{
    public class User : IEntity
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserSurname { get; set; }
        public string UserEmail { get; set; }
        public decimal UserPhoneNumber { get; set; }
        public string UserAddress { get; set; }
        public decimal UserIdentityNumber { get; set; }
        public byte[] UserPasswordSalt { get; set; }
        public byte[] UserPasswordHash { get; set; }
        public bool Status { get; set; }

    }
}
