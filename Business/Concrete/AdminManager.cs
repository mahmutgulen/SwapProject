using Business.Abstract;
using Core.Entities;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Contexts;
using Entities.Concrete;
using Entities.Dtos.CompanyVault;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;

namespace Business.Concrete
{
    public class AdminManager : IAdminService
    {
        private ICompanyVaultDal _companyVaultDal;
        private IUserAssetsDal _userAssetsDal;
        private IUserDal _userDal;
        private IWalletDal _walletDal;
        private ISuspendDal _suspendDal;
        private IOpenOrderDal _openOrderDal;
        private SwapContext _context;
        public AdminManager(ICompanyVaultDal companyVaultDal, IUserAssetsDal userAssetsDal, IUserDal userDal, IWalletDal walletDal, ISuspendDal suspendDal, IOpenOrderDal openOrderDal, SwapContext context)
        {
            _companyVaultDal = companyVaultDal;
            _userAssetsDal = userAssetsDal;
            _userDal = userDal;
            _walletDal = walletDal;
            _suspendDal = suspendDal;
            _openOrderDal = openOrderDal;
            _context = context;
        }


        public IDataResult<List<CompanyVault>> GetCompanyVault()
        {
            var companyVault = _companyVaultDal.GetList().ToList();

            return new SuccessDataResult<List<CompanyVault>>(companyVault);
        }
        public IDataResult<List<OpenOrder>> GetOpenOrders()
        {
            var openOrder = _openOrderDal.GetList().ToList();
            return new SuccessDataResult<List<OpenOrder>>(openOrder);
        }

        public IDataResult<List<SuspendUser>> GetSuspendUsers()
        {
            var suspend = _suspendDal.GetList().ToList();

            return new SuccessDataResult<List<SuspendUser>>(suspend);
        }

        public IDataResult<List<UserAssets>> GetUserAssets()
        {

            var userAssets = _userAssetsDal.GetList().ToList();
            return new SuccessDataResult<List<UserAssets>>(userAssets);
        }

        public IDataResult<List<User>> GetUsers()
        {

            var user = _userDal.GetList().ToList();
            return new SuccessDataResult<List<User>>(user);
        }

        public IDataResult<List<UserWallet>> GetWallets()
        {
            var wallets = _walletDal.GetList().ToList();
            return new SuccessDataResult<List<UserWallet>>(wallets);
        }

        public IResult RemoveSuspendAccount(int UserId)
        {
            var userDATA = _userDal.Get(x => x.UserId == UserId);
            if (userDATA != null)
            {
                if (userDATA.Status == true)
                {
                    return new ErrorResult("Hesap zaten askıda değil.");
                }
                var userUpdate = new User
                {
                    UserId = userDATA.UserId,
                    UserAddress = userDATA.UserAddress,
                    UserEmail = userDATA.UserEmail,
                    UserIdentityNumber = userDATA.UserIdentityNumber,
                    UserName = userDATA.UserName,
                    UserPasswordHash = userDATA.UserPasswordHash,
                    UserPasswordSalt = userDATA.UserPasswordSalt,
                    UserPhoneNumber = userDATA.UserPhoneNumber,
                    UserSurname = userDATA.UserSurname,
                    Status = true
                };
                _userDal.Update(userUpdate);
                //suspend
                var sus = _suspendDal.Get(x => x.UserId == UserId);
                var suspend = new SuspendUser
                {
                    DateTime = sus.DateTime,
                    Description = sus.Description,
                    UserId = UserId,
                    Status = false,
                    Id = sus.Id
                };
                _suspendDal.Update(suspend);
                return new SuccessResult("Hesap askıdan çıkarıldı.");
            }
            return new ErrorResult("Kullanıcı bulunamadı.");


        }

        public IResult SuspendAccount(int UserId, string description)
        {
            //users
            var userDATA = _userDal.Get(x => x.UserId == UserId);

            if (userDATA != null)
            {
                if (userDATA.Status == false)
                {
                    return new ErrorResult("Hesap zaten askıda.");
                }
                var userUpdate = new User
                {
                    UserId = userDATA.UserId,
                    UserAddress = userDATA.UserAddress,
                    UserEmail = userDATA.UserEmail,
                    UserIdentityNumber = userDATA.UserIdentityNumber,
                    UserName = userDATA.UserName,
                    UserPasswordHash = userDATA.UserPasswordHash,
                    UserPasswordSalt = userDATA.UserPasswordSalt,
                    UserPhoneNumber = userDATA.UserPhoneNumber,
                    UserSurname = userDATA.UserSurname,
                    Status = false
                };
                _userDal.Update(userUpdate);

                var suspend = new SuspendUser
                {
                    DateTime = DateTime.Now,
                    Description = description,
                    UserId = UserId,
                    Status = true
                };
                _suspendDal.Add(suspend);
                return new SuccessResult("Hesap askıya alındı.");
            }
            return new ErrorResult("Kullanıcı bulunamadı.");


        }


    }
}


