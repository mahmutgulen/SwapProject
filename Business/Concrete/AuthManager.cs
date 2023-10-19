using Business.Abstract;
using Business.Contants;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Core.Utilities.Security.Hashing;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.Concrete.Dtos.User;
using System.IdentityModel.Tokens.Jwt;

namespace Business.Concrete
{
    public class AuthManager : IAuthService
    {
        private IUserService _userService;
        private ITokenHelper _tokenHelper;
        private IToken _token;
        private IUserDal _userDal;
        private IUserRoleDal _userRoleDal;
        private IWalletDal _walletDal;
        private IUserAssetsDal _userAssetsDal;
        private IPendingCoinsDal _pendingCoinsDal;

        public AuthManager(IUserService userService, ITokenHelper tokenHelper, IUserDal userDal, IUserRoleDal userRoleDal, IToken token, IWalletDal walletDal, IUserAssetsDal userAssetsDal, IPendingCoinsDal pendingCoinsDal)
        {
            _userService = userService;
            _tokenHelper = tokenHelper;
            _userDal = userDal;
            _userRoleDal = userRoleDal;
            _token = token;
            _walletDal = walletDal;
            _userAssetsDal = userAssetsDal;
            _pendingCoinsDal = pendingCoinsDal;
        }

        public IDataResult<AccessToken> CreateAccessToken(User user)
        {
            var roles = _userService.GetRoles(user);
            var accessToken = _tokenHelper.CreateToken(user, roles.Data);
            return new SuccessDataResult<AccessToken>(accessToken, "Kayıt Başarılı.");
        }

        public IDataResult<User> Login(UserLoginDto userLoginDto)
        {
            var userToCheck = _userService.GetByMail(userLoginDto.UserEmail);
            if (userToCheck.Data == null)
            {
                return new ErrorDataResult<User>(Messages.UserNotFound);
            }
            if (!HashingHelper.VerifyPasswordHash(userLoginDto.Password, userToCheck.Data.UserPasswordHash, userToCheck.Data.UserPasswordSalt))
            {
                return new ErrorDataResult<User>(Messages.PasswordError);
            }
            return new SuccessDataResult<User>(userToCheck.Data, Messages.SuccessfulLogin);
        }

        public IResult PasswordChange(string token, string newPassword, string confirmPassword)
        {
            //token üzerinden userId getiriyorum
            var userId = _token.GetToken(token);
            var userDb = _userDal.Get(x => x.UserId == userId);

            if (!HashingHelper.VerifyPasswordHash(confirmPassword, userDb.UserPasswordHash, userDb.UserPasswordSalt))
            {
                return new ErrorDataResult<User>(Messages.PasswordsNotMatch);
            }

            byte[] passwordHash, passwordSalt;
            HashingHelper.CreatePasswordHash(newPassword, out passwordHash, out passwordSalt);

            var user = new User
            {
                UserId = userDb.UserId,
                UserName = userDb.UserName,
                Status = userDb.Status,
                UserAddress = userDb.UserAddress,
                UserEmail = userDb.UserEmail,
                UserIdentityNumber = userDb.UserIdentityNumber,
                UserPasswordHash = passwordHash,
                UserPasswordSalt = passwordSalt,
                UserPhoneNumber = userDb.UserPhoneNumber,
                UserSurname = userDb.UserSurname
            };
            _userDal.Update(user);
            return new SuccessResult(Messages.UserPasswordIsChanged);
        }

        public IDataResult<User> Register(UserRegisterDto userRegisterDto, string password)
        {
            //salt & hash oluşturma
            byte[] passwordHash, passwordSalt;
            HashingHelper.CreatePasswordHash(password, out passwordHash, out passwordSalt);

            //users ekliyor
            var user = new User
            {
                Status = true,
                UserAddress = userRegisterDto.UserAddress,
                UserEmail = userRegisterDto.UserEmail,
                UserIdentityNumber = userRegisterDto.UserIdentityNumber,
                UserName = userRegisterDto.UserName,
                UserPhoneNumber = userRegisterDto.UserPhoneNumber,
                UserSurname = userRegisterDto.UserSurname,
                UserPasswordHash = passwordHash,
                UserPasswordSalt = passwordSalt
            };
            _userDal.Add(user);
            //kullanıcıya rol atıyor
            var userRole = new UserRole
            {
                Status = true,
                RoleId = 2,
                UserId = user.UserId
            };
            _userRoleDal.Add(userRole);
            //wallet ekliyorum
            var wallet = new UserWallet
            {
                Balance = 0,
                UserId = user.UserId,
            };
            _walletDal.Add(wallet);
            //userAssets
            var userAssets = new UserAssets
            {
                UserId = user.UserId,
                USDT = 0,
                BTC = 0,
                ETH = 0,
                TRX = 0,
                status = true
            };
            _userAssetsDal.Add(userAssets);
            return new SuccessDataResult<User>(user, Messages.UserRegistered);
        }

        public IResult UserExists(string email)
        {
            var user = _userService.GetByMail(email).Data;
            if (user != null)
            {
                return new ErrorResult(Messages.UserAlreadyExists);
            }
            return new SuccessResult();
        }
    }
}
