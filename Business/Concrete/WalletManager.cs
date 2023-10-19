using Business.Abstract;
using Business.Contants;
using Core.Utilities.Results;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class WalletManager : IWalletService
    {
        private IToken _token;
        private IWalletDal _walletDal;
        private IUserDal _userDal;
        private IUserAssetsDal _userAssetsDal;

        public WalletManager(IToken token, IWalletDal walletDal, IUserDal userDal, IUserAssetsDal userAssetsDal)
        {
            _token = token;
            _walletDal = walletDal;
            _userDal = userDal;
            _userAssetsDal = userAssetsDal;
        }

        public IResult AddBalance(int balance, string token)
        {
            //token üzerinden userId getiriyorum
            var userId = _token.GetToken(token);
            var userDb = _userDal.Get(x => x.UserId == userId);
            var userWallet = _walletDal.Get(x => x.UserId == userId);

            if (balance < 100)
            {
                return new ErrorResult($"Yatırımınız 100$'dan az olmamalıdır.{100 - balance}$ tutarında eksiğiniz var.");
            }
            var newBalance = userWallet.Balance + balance;
            var wallet = new UserWallet
            {
                UserId = userDb.UserId,
                Balance = newBalance,
            };
            _walletDal.Update(wallet);
            //userAssets USDT güncelliyorum
            var uA = _userAssetsDal.Get(x => x.UserId == userDb.UserId);
            var assets = new UserAssets
            {
                BTC = uA.BTC,
                ETH = uA.ETH,
                TRX = uA.TRX,
                Id = uA.Id,
                status = uA.status,
                UserId = uA.UserId,
                USDT = newBalance

            };
            _userAssetsDal.Update(assets);
            return new SuccessResult($"{balance}$ tutarındaki yatırımınız gerçekleşmiştir.");
        }

        public IDataResult<UserWallet> GetBalance(string token)
        {
            //token üzerinden userId getiriyorum
            var userId = _token.GetToken(token);
            var userDb = _userDal.Get(x => x.UserId == userId);
            var userWallet = _walletDal.Get(x => x.UserId == userId);

            return new SuccessDataResult<UserWallet>($"Bakiyeniz: {userWallet.Balance}$");
        }

        public IResult WithDrawBalance(int balance, string token)
        {
            //token üzerinden userId getiriyorum
            var userId = _token.GetToken(token);
            var userDb = _userDal.Get(x => x.UserId == userId);
            var userWallet = _walletDal.Get(x => x.UserId == userId);
            if (balance > userWallet.Balance)
            {
                return new ErrorResult("Bakiyeniz yetersiz.");
            }
            if (balance < 100)
            {
                return new ErrorResult($"Çekim tutarınız 100$'dan az olmamalıdır.");
            }
            var newBalance = userWallet.Balance - balance;
            var wallet = new UserWallet
            {
                UserId = userDb.UserId,
                Balance = newBalance,
            };
            _walletDal.Update(wallet);
            //userAssets USDT güncelliyorum
            var uA = _userAssetsDal.Get(x => x.UserId == userDb.UserId);
            var assets = new UserAssets
            {
                BTC = uA.BTC,
                ETH = uA.ETH,
                TRX = uA.TRX,
                Id = uA.Id,
                status = uA.status,
                UserId = uA.UserId,
                USDT = newBalance

            };
            _userAssetsDal.Update(assets);
            return new SuccessResult($"{balance}$ tutarındaki çekiminiz gerçekleşmiştir.");


        }
    }
}
