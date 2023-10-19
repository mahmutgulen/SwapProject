using Business.Abstract;
using Business.Contants;
using Core.Utilities.Results;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class CancelManager : ICancelService
    {
        private IOpenOrderDal _openOrderDal;
        private IToken _token;
        private IUserDal _userDal;
        private IWalletDal _walletDal;
        private IPendingCoinsDal _pendingCoinsDal;
        private IUserAssetsDal _userAssetsDal;
        private ICompanyVaultDal _companyVaultDal;

        public CancelManager(IOpenOrderDal openOrderDal, IToken token, IUserDal userDal, IWalletDal walletDal, IPendingCoinsDal pendingCoinsDal, IUserAssetsDal userAssetsDal, ICompanyVaultDal companyVaultDal)
        {
            _openOrderDal = openOrderDal;
            _token = token;
            _userDal = userDal;
            _walletDal = walletDal;
            _pendingCoinsDal = pendingCoinsDal;
            _userAssetsDal = userAssetsDal;
            _companyVaultDal = companyVaultDal;
        }

        public IResult CancelProcess(string token, int openOrderId)
        {
            //Token üzerinden user bilgileri
            var userId = _token.GetToken(token);
            var userDb = _userDal.Get(x => x.UserId == userId);
            //openorder
            var openOrderData = _openOrderDal.Get(x => x.Id == openOrderId);
            if (openOrderData != null)
            {
                if (openOrderData.Status == 3)
                {
                    return new ErrorResult(Messages.ProcessAlreadyCancel);
                }
                var openOrder = new OpenOrder
                {
                    Id = openOrderId,
                    Amount = openOrderData.Amount,
                    Coin = openOrderData.Coin,
                    DateTime = openOrderData.DateTime,
                    Operation = openOrderData.Operation,
                    Price = openOrderData.Price,
                    Total = openOrderData.Total,
                    UserId = openOrderData.UserId,
                    Status = 3
                };
                _openOrderDal.Update(openOrder);
                //pendingden düşüyorum
                var pendingDATA = _pendingCoinsDal.Get(x => x.OpenOrdersId == openOrderId);
                var pending = new Pending
                {
                    Id = pendingDATA.Id,
                    OpenOrdersId = pendingDATA.OpenOrdersId,
                    BTC = pendingDATA.BTC,
                    CanceledDateTime = DateTime.UtcNow,
                    ClosedDateTime = pendingDATA.ClosedDateTime,
                    Commission = pendingDATA.Commission,
                    ETH = pendingDATA.ETH,
                    OpenedDateTime = pendingDATA.OpenedDateTime,
                    Operation = pendingDATA.Operation,
                    Status = 3,
                    TRX = pendingDATA.Status,
                    USDT = pendingDATA.USDT,
                    UserId = pendingDATA.UserId
                };
                _pendingCoinsDal.Update(pending);
                //kullanıcıya geri ödüyorum
                var userassetsDATA = _userAssetsDal.Get(x => x.UserId == userDb.UserId);
                var userAssets = new UserAssets
                {
                    BTC = pendingDATA.BTC + userassetsDATA.BTC,
                    ETH = pendingDATA.ETH + userassetsDATA.ETH,
                    Id = userassetsDATA.Id,
                    status = userassetsDATA.status,
                    TRX = pendingDATA.TRX + userassetsDATA.TRX,
                    USDT = pendingDATA.USDT + userassetsDATA.USDT,
                    UserId = userassetsDATA.UserId
                };
                _userAssetsDal.Update(userAssets);
                //userWallet
                var walletDATa = _walletDal.Get(x => x.UserId == userDb.UserId);
                var userWallet = new UserWallet
                {
                    UserId = walletDATa.UserId,
                    Balance = walletDATa.Balance + pending.USDT,
                };
                _walletDal.Update(userWallet);
                //şirket kasasından komisyonu geri alıyorum
                var cV = _companyVaultDal.Get(x => x.PendingId == pendingDATA.Id);
                var companyVault = new CompanyVault
                {
                    Id = cV.Id,
                    PendingId = cV.PendingId,
                    BTC = cV.BTC - pendingDATA.BTC,
                    ETH = cV.ETH - pendingDATA.ETH,
                    TRX = cV.TRX - pendingDATA.TRX,
                    USDT = cV.USDT - pendingDATA.USDT,
                    DateTime = cV.DateTime,
                    Status = 3
                };
                _companyVaultDal.Update(companyVault);
                return new SuccessResult(Messages.CanselIsComplete);
            }
            return new ErrorResult(Messages.ProcessIsNotExists);
        }
    }
}
