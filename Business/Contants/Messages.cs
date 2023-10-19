using Core.Entities.Concrete;

namespace Business.Contants
{
    public static class Messages
    {
        public static string UserNotFound = "Kullanıcı bulunamadı.";

        public static string UserAlreadyExists = "Email zaten kullanılıyor.";

        public static string UserPasswordIsChanged = "Kullanıcı şifresi başarıyla değiştirildi.";

        public static string AccessTokenCreated = "Token başarıyla oluşturuldu.";

        public static string PasswordError = "Şifre hatalı.";

        public static string SuccessfulLogin = "Giriş başarılı.";

        public static string UserRegistered = "Kayıt Başarılı.";

        public static string PasswordsCannotBeTheSame = "Yeni şifre eski şifreyle aynı olamaz.";

        public static string PasswordsNotMatch = "Şifreler Aynı Olmalıdır.";

        public static string InsufficientBalance = "Yetersiz Bakiye.";

        public static string CoinNotExists = "Bu coine sahip değilsiniz.";

        public static string CanselIsComplete = "İşlem iptal edildi.";

        public static string ProcessIsNotExists = "İşlem bulunamadı.";

        public static string ProcessAlreadyCancel = "İşlem zaten iptal edildi.";
    }
}