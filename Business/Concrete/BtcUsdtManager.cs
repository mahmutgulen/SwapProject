using Business.Abstract;
using Business.Contants;
using Core.Utilities.Results;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using DataAccess.Concrete.EntityFramework.Contexts;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics.Metrics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace Business.Concrete
{
    public class BtcUsdtManager : IBtcUsdtService
    {
        private IToken _token;
        private IUserDal _userDal;
        private SwapContext _context;
        private IOpenOrderDal _openOrderDal;
        private ICompanyVaultDal _companyVaultDal;
        private IUserAssetsDal _userAssetsDal;
        private IWalletDal _walletDal;
        private IPendingCoinsDal _pendingCoinsDal;

        public BtcUsdtManager(IToken token, IUserDal userDal, SwapContext context, IOpenOrderDal openOrderDal, ICompanyVaultDal companyVaultDal, IUserAssetsDal userAssetsDal, IWalletDal walletDal, IPendingCoinsDal pendingCoinsDal)
        {
            _token = token;
            _userDal = userDal;
            _context = context;
            _openOrderDal = openOrderDal;
            _companyVaultDal = companyVaultDal;
            _userAssetsDal = userAssetsDal;
            _walletDal = walletDal;
            _pendingCoinsDal = pendingCoinsDal;
        }
        [Precision(18, 8)]
        public decimal coinGain { get; set; }
        public IResult BuyLimiteOrder(string token, decimal buyLimit, decimal buyPrice)
        {
            //Token üzerinden user bilgileri
            var userId = _token.GetToken(token);
            var userDb = _userDal.Get(x => x.UserId == userId);
            //Alıcının USDT bilgileri
            var usdtInformations = _userAssetsDal.Get(x => x.UserId == userDb.UserId);
            //userwallet
            var userWalletFirst = _walletDal.Get(x => x.UserId == userDb.UserId);
            //Parite getiriyorum 
            HttpClient client = new HttpClient();
            var response = client.GetAsync($"https://api.binance.com/api/v3/ticker/price?symbol=BTCUSDT").Result;
            var responseBody = response.Content.ReadAsStringAsync();
            var BTC = JsonConvert.DeserializeObject<Parite>(responseBody.Result);
            //Hesaplamalar----------------------------------------------------------------------------------------------
            //Elde edeceği coini hesaplıyorum
            decimal coinGain = (Math.Round(buyPrice / buyLimit, 8));
            //Alıcının eline geçecek olan BTC'ye komisyon kesiyorum
            decimal buyerCommission = (Math.Round(coinGain / 1000, 8));
            decimal buyerGain = (Math.Round(coinGain - buyerCommission, 8));
            //satıcının eline geçecek olan USDT'ye komisyon kesiyorum
            decimal sellerCommission = (Math.Round(buyPrice / 1000, 8));
            decimal sellerGain = (Math.Round(buyPrice - sellerCommission, 8));
            //openOrders tablosundaki en düşük miktarları getir
            var minAmountBTC = _openOrderDal.GetList(x => x.Coin == "BTC" && x.Operation == "Sell").Where(x => x.Status == 2).OrderBy(x => x.Amount);
            //
            var firstMinAmountDATA = _context.OpenOrders.OrderBy(x => x.Amount).First(x => x.Operation == "Sell" && x.Coin == "BTC" && x.Status == 2);
            var firstMinAmount = firstMinAmountDATA.Amount;
            decimal requestAmount = (Math.Round(coinGain, 8));
            decimal counter = 0;
            //aralık kontorlü
            AmountCalculator amountCalculator = new();
            var minIn = amountCalculator.GetBuyAmount(buyLimit);
            if (buyLimit < minIn)
            {
                //para kontrolü
                if (userWalletFirst.Balance >= buyPrice)
                {
                    //minimum direkt olarak istekten büyükse parçalasın
                    if (firstMinAmount > requestAmount)
                    {
                        //minimum miktardan ihtiyacın kadar alıp alışı tamamla işlemi kapat ve minimum miktarı güncelle
                        //minimum miktardan kalanı hesaplıyorum
                        decimal remainderOfTheMinimum = (Math.Round(firstMinAmount - requestAmount, 8));
                        //bekleyen satışı güncelliyorum
                        var oOData = _openOrderDal.Get(x => x.Id == firstMinAmountDATA.Id);
                        var openOrderReminder = new OpenOrder
                        {
                            Coin = firstMinAmountDATA.Coin,
                            UserId = firstMinAmountDATA.UserId,
                            Operation = firstMinAmountDATA.Operation,
                            DateTime = firstMinAmountDATA.DateTime,
                            Price = firstMinAmountDATA.Price,
                            Amount = remainderOfTheMinimum,
                            Total = firstMinAmountDATA.Total,
                            Status = 2,
                            Id = firstMinAmountDATA.Id,
                        };
                        _openOrderDal.Update(openOrderReminder);
                        //satıcının balance'ını güncelliyorum 
                        var uAData = _userAssetsDal.Get(x => x.UserId == firstMinAmountDATA.UserId);
                        var userAssets = new UserAssets
                        {
                            UserId = uAData.UserId,
                            BTC = uAData.BTC,
                            ETH = uAData.ETH,
                            status = uAData.status,
                            Id = uAData.Id,
                            TRX = uAData.TRX,
                            USDT = uAData.USDT + sellerGain,
                        };
                        _userAssetsDal.Update(userAssets);
                        //satıcının walletini güncelliyorum
                        var uWData = _walletDal.Get(x => x.UserId == firstMinAmountDATA.UserId);
                        var userWallet = new UserWallet
                        {
                            UserId = uWData.UserId,
                            Balance = uWData.Balance + sellerGain
                        };
                        _walletDal.Update(userWallet);
                        //komisyonu şirket kasasına ekliyorum
                        var companyVault = new CompanyVault
                        {
                            BTC = buyerCommission,
                            USDT = sellerCommission,
                            DateTime = DateTime.UtcNow,
                            Status = 1,
                        };
                        _companyVaultDal.Add(companyVault);
                        //pendingden düşüyorum
                        var pendingDATA = _pendingCoinsDal.Get(x => x.OpenOrdersId == oOData.Id);
                        var pending = new Pending
                        {
                            Id = pendingDATA.Id,
                            OpenOrdersId = pendingDATA.OpenOrdersId,
                            BTC = pendingDATA.BTC,
                            CanceledDateTime = pendingDATA.CanceledDateTime,
                            ClosedDateTime = DateTime.UtcNow,
                            Commission = pendingDATA.Commission,
                            ETH = pendingDATA.ETH,
                            OpenedDateTime = pendingDATA.OpenedDateTime,
                            Operation = pendingDATA.Operation,
                            Status = 1,
                            TRX = pendingDATA.Status,
                            USDT = pendingDATA.USDT,
                            UserId = pendingDATA.UserId
                        };
                        _pendingCoinsDal.Update(pending);
                        //counter kontrolü için gerekli
                        counter = requestAmount;
                    }
                    //minimum satış kücük ise istenen miktardan, bu satışları yiyiyor.
                    if (firstMinAmount < requestAmount)
                    {
                        foreach (var sells in minAmountBTC)
                        {
                            decimal minSellAmount = (Math.Round(sells.Amount, 8));
                            //openOrders'da gerçekleşen satış işlemini güncelliyorum
                            var oOData = _openOrderDal.Get(x => x.Id == sells.Id);
                            var openOrderSeller = new OpenOrder
                            {
                                Id = oOData.Id,
                                Amount = oOData.Amount,
                                Coin = oOData.Coin,
                                DateTime = oOData.DateTime,
                                Operation = oOData.Operation,
                                Price = oOData.Price,
                                Total = oOData.Total,
                                UserId = oOData.UserId,
                                Status = 1
                            };
                            _openOrderDal.Update(openOrderSeller);
                            //satıcının balance'ını güncelliyorum assets-wallet
                            var uAData = _userAssetsDal.Get(x => x.UserId == sells.UserId);
                            var userAssets = new UserAssets
                            {
                                UserId = uAData.UserId,
                                BTC = uAData.BTC,
                                ETH = uAData.ETH,
                                status = uAData.status,
                                Id = uAData.Id,
                                TRX = uAData.TRX,
                                USDT = uAData.USDT + sellerGain,
                            };
                            _userAssetsDal.Update(userAssets);
                            //userWallet
                            var uWData = _walletDal.Get(x => x.UserId == sells.UserId);
                            var userWallet = new UserWallet
                            {
                                UserId = uWData.UserId,
                                Balance = uWData.Balance + sellerGain
                            };
                            _walletDal.Update(userWallet);

                            //şirketkasasına ekliyorum
                            var companyVault = new CompanyVault
                            {
                                BTC = buyerCommission,
                                DateTime = DateTime.UtcNow,
                                USDT = sellerCommission,
                                Status = 1
                            };
                            _companyVaultDal.Add(companyVault);
                            //eksik kalan miktarı belirle---------------------------------------------------------??????????????????????????????????????????????????????????----------------------------------
                            counter = counter + minSellAmount;

                            if (counter > requestAmount)
                            {
                                //fazla aldığı miktarı burada geri veriyorum
                                //geri ödenecek miktarı hesaplıyorum
                                var payBack = counter - requestAmount;
                                //openOrders düzeltiyorum
                                var openOrderDT = _openOrderDal.Get(x => x.Id == sells.Id);
                                var openOrderPayback = new OpenOrder
                                {
                                    Amount = payBack,
                                    Coin = openOrderDT.Coin,
                                    DateTime = openOrderDT.DateTime,
                                    Id = openOrderDT.Id,
                                    Operation = openOrderDT.Operation,
                                    Price = openOrderDT.Price,
                                    Status = openOrderDT.Status,
                                    Total = openOrderDT.Total,
                                    UserId = openOrderDT.UserId
                                };
                                _openOrderDal.Update(openOrderPayback);
                                //counter düzeltiyorum
                                counter = counter - payBack;
                                break;
                            }
                        }
                    }
                    //İstenen miktarı karşılayamadıysa islem beklemeye alınıyor
                    if (counter != requestAmount)
                    {
                        var openOrderPending = new OpenOrder
                        {
                            UserId = userDb.UserId,
                            Coin = "BTC",
                            DateTime = DateTime.Now,
                            Amount = buyerGain,
                            Operation = "Buy",
                            Price = buyLimit,
                            Total = buyPrice,
                            Status = 2,
                        };
                        _openOrderDal.Add(openOrderPending);
                        //pendinge ekliyorum
                        var pending = new Pending
                        {
                            USDT = buyPrice,
                            BTC = buyerGain,
                            Commission = buyerCommission,
                            OpenedDateTime = DateTime.UtcNow,
                            Operation = "Buy",
                            Status = 2,
                            UserId = userDb.UserId,
                            OpenOrdersId = openOrderPending.Id
                        };
                        _pendingCoinsDal.Add(pending);
                        return new SuccessResult($"{buyPrice}$ karşılığında, {buyerGain} miktarında BTC emiri alış emri beklemeye alındı.");
                    }
                    //işlem complete olarak openorderse ekleniyor.
                    var openOrder = new OpenOrder
                    {
                        UserId = userDb.UserId,
                        Coin = "BTC",
                        DateTime = DateTime.Now,
                        Amount = buyerGain,
                        Operation = "Buy",
                        Price = buyLimit,
                        Total = buyPrice,
                        Status = 1,
                    };
                    _openOrderDal.Add(openOrder);
                    //alıcı varlıklarını güncelliyrum
                    var uA = _userAssetsDal.Get(x => x.UserId == userDb.UserId);
                    var userAssetsFinito = new UserAssets
                    {
                        BTC = uA.BTC + buyerGain,
                        Id = uA.Id,
                        ETH = uA.ETH,
                        status = uA.status,
                        TRX = uA.TRX,
                        USDT = uA.USDT - buyPrice,
                        UserId = uA.UserId,
                    };
                    _userAssetsDal.Update(userAssetsFinito);
                    //userwallets güncelliyorum
                    var uWComplete = _walletDal.Get(x => x.UserId == userDb.UserId);
                    var walletComplete = new UserWallet
                    {
                        UserId = uWComplete.UserId,
                        Balance = uWComplete.Balance - buyPrice,
                    };
                    _walletDal.Update(walletComplete);
                    //komisyonu ekliyorum "alıcı"
                    var CompanyVaultClosed = new CompanyVault
                    {
                        BTC = buyerCommission,
                        DateTime = DateTime.UtcNow,
                        Status = 1,
                    };
                    _companyVaultDal.Add(CompanyVaultClosed);
                    return new SuccessResult($"{buyPrice}$ karşılığında, {buyerGain} miktarında BTC alış emri başarılı bir şekilde tamamlandı.");
                }
                return new ErrorResult($"Yetersiz Bakiye, mevcut bakiyeinz {userWalletFirst.Balance}$");
            }
            return new ErrorResult($"Alış emri oluşturabilmek için girebileceğiniz minimum limit fiyatı {minIn}$ olmalıdır.");

        }

        public IResult SellLimiteOrder(string token, decimal sellLimit, decimal sellAmount)
        {
            //Token üzerinden user bilgileri
            var userId = _token.GetToken(token);
            var userDb = _userDal.Get(x => x.UserId == userId);
            //Satıcının BTC bilgileri
            var btcInformations = _userAssetsDal.Get(x => x.UserId == userDb.UserId);
            //Parite getiriyorum 
            HttpClient client = new HttpClient();
            var response = client.GetAsync($"https://api.binance.com/api/v3/ticker/price?symbol=BTCUSDT").Result;
            var responseBody = response.Content.ReadAsStringAsync();
            var BTC = JsonConvert.DeserializeObject<Parite>(responseBody.Result);
            //Hesaplamalar----------------------------------------------------------------------------------------------
            //satıcının Elde edeceği ücreti hesaplıyorum
            decimal gain = sellAmount * sellLimit;
            //satıcının eline geçecek olan USDT'ye komisyon kesiyorum
            decimal sellerCommission = (Math.Round(gain / 1000, 8));
            decimal sellerGain = (Math.Round(gain - sellerCommission, 8));
            //alıcının eline geçecek olan BTC'ye komisyon kesiyorum 
            decimal buyerCommission = (Math.Round(sellAmount / 1000, 8));
            decimal buyerGain = (Math.Round(sellAmount - buyerCommission, 8));
            //openOrders tablosundaki en düşük alışları getir
            var maxAmountBTC = _openOrderDal.GetList(x => x.Coin == "BTC" && x.Operation == "Buy").Where(x => x.Status == 2).OrderBy(x => x.Amount);
            var firstMaxAmountDATA = _context.OpenOrders.OrderBy(x => x.Amount).First(x => x.Operation == "Buy" && x.Coin == "BTC" && x.Status == 2);
            decimal firstMaxAmount = firstMaxAmountDATA.Amount;
            decimal requestSellAmount = sellAmount;
            decimal sellCounter = 0;
            decimal counter = 0;
            //giriş kontrolü
            AmountCalculator amountCalculator = new();
            var maxIn = amountCalculator.GetSellAmount(sellLimit);
            if (sellLimit < maxIn)
            {
                //para kontrolü
                if (btcInformations.BTC >= sellAmount)
                {
                    //minimum alış, satış miktarından büyükse direkt parçalasın
                    if (firstMaxAmount > requestSellAmount)
                    {
                        decimal remainderOfTheMaximum = (Math.Round(firstMaxAmount - requestSellAmount, 8));
                        //bekleyen satışı güncelliyorum
                        var oOData = _openOrderDal.Get(x => x.Id == firstMaxAmountDATA.Id);
                        var openOrderReminder = new OpenOrder
                        {
                            Coin = firstMaxAmountDATA.Coin,
                            UserId = firstMaxAmountDATA.UserId,
                            Operation = firstMaxAmountDATA.Operation,
                            DateTime = firstMaxAmountDATA.DateTime,
                            Price = firstMaxAmountDATA.Price,
                            Amount = remainderOfTheMaximum,
                            Total = firstMaxAmountDATA.Total,
                            Status = 2,
                            Id = firstMaxAmountDATA.Id,
                        };
                        _openOrderDal.Update(openOrderReminder);
                        //alıcının balance'ını güncelliyorum 
                        var uAData = _userAssetsDal.Get(x => x.UserId == firstMaxAmountDATA.UserId);
                        var userAssets = new UserAssets
                        {
                            UserId = uAData.UserId,
                            BTC = uAData.BTC + buyerGain,
                            ETH = uAData.ETH,
                            status = uAData.status,
                            Id = uAData.Id,
                            TRX = uAData.TRX,
                            USDT = uAData.USDT - gain,
                        };
                        _userAssetsDal.Update(userAssets);
                        //satıcının walletini güncelliyorum
                        var uWData = _walletDal.Get(x => x.UserId == firstMaxAmountDATA.UserId);
                        var userWallet = new UserWallet
                        {
                            UserId = uWData.UserId,
                            Balance = uWData.Balance - gain
                        };
                        _walletDal.Update(userWallet);
                        //komisyonu şirket kasasına ekliyorum
                        var companyVault = new CompanyVault
                        {
                            BTC = buyerCommission,
                            USDT = sellerCommission,
                            DateTime = DateTime.UtcNow,
                            Status = 1,
                        };
                        _companyVaultDal.Add(companyVault);
                        //pendingden düşüyorum
                        var pendingDATA = _pendingCoinsDal.Get(x => x.OpenOrdersId == oOData.Id);
                        var pending = new Pending
                        {
                            Id = pendingDATA.Id,
                            OpenOrdersId = pendingDATA.OpenOrdersId,
                            BTC = pendingDATA.BTC,
                            CanceledDateTime = pendingDATA.CanceledDateTime,
                            ClosedDateTime = DateTime.UtcNow,
                            Commission = pendingDATA.Commission,
                            ETH = pendingDATA.ETH,
                            OpenedDateTime = pendingDATA.OpenedDateTime,
                            Operation = pendingDATA.Operation,
                            Status = 1,
                            TRX = pendingDATA.Status,
                            USDT = pendingDATA.USDT,
                            UserId = pendingDATA.UserId
                        };
                        _pendingCoinsDal.Update(pending);

                        //counter kontrolü için gerekli
                        sellCounter = requestSellAmount;
                    }
                    if (firstMaxAmount < requestSellAmount)
                    {
                        foreach (var buys in maxAmountBTC)
                        {
                            decimal minBuyAmount = (Math.Round(buys.Amount, 8));
                            //openOrders'da gerçekleşen satış işlemini güncelliyorum
                            var oOData = _openOrderDal.Get(x => x.Id == buys.Id);
                            var openOrderBuyer = new OpenOrder
                            {
                                Id = oOData.Id,
                                Amount = oOData.Amount,
                                Coin = oOData.Coin,
                                DateTime = oOData.DateTime,
                                Operation = oOData.Operation,
                                Price = oOData.Price,
                                Total = oOData.Total,
                                UserId = oOData.UserId,
                                Status = 1
                            };
                            _openOrderDal.Update(openOrderBuyer);
                            //alıcını balance'ını güncelliyorum assets-wallet
                            var uAData = _userAssetsDal.Get(x => x.UserId == buys.UserId);
                            var userAssets = new UserAssets
                            {
                                UserId = uAData.UserId,
                                BTC = uAData.BTC + sellAmount,
                                ETH = uAData.ETH,
                                status = uAData.status,
                                Id = uAData.Id,
                                TRX = uAData.TRX,
                                USDT = uAData.USDT - gain,
                            };
                            _userAssetsDal.Update(userAssets);
                            //userWallet buyer
                            var uWData = _walletDal.Get(x => x.UserId == buys.UserId);
                            var userWallet = new UserWallet
                            {
                                UserId = uWData.UserId,
                                Balance = uWData.Balance - gain
                            };
                            _walletDal.Update(userWallet);
                            //komisyon kesiyorum
                            var cV = new CompanyVault
                            {
                                Status = 1,
                                DateTime = DateTime.UtcNow,
                                BTC = sellerCommission,
                                USDT = buyerCommission,
                            };
                            _companyVaultDal.Add(cV);
                            //pendingden düşüyorum
                            var pendingDATA = _pendingCoinsDal.Get(x => x.OpenOrdersId == oOData.Id);
                            var pending = new Pending
                            {
                                Id = pendingDATA.Id,
                                OpenOrdersId = pendingDATA.OpenOrdersId,
                                BTC = pendingDATA.BTC,
                                CanceledDateTime = pendingDATA.CanceledDateTime,
                                ClosedDateTime = DateTime.UtcNow,
                                Commission = pendingDATA.Commission,
                                ETH = pendingDATA.ETH,
                                OpenedDateTime = pendingDATA.OpenedDateTime,
                                Operation = pendingDATA.Operation,
                                Status = 1,
                                TRX = pendingDATA.Status,
                                USDT = pendingDATA.USDT,
                                UserId = pendingDATA.UserId
                            };
                            _pendingCoinsDal.Update(pending);
                            //
                            sellCounter = sellCounter + minBuyAmount;
                            if (sellCounter > requestSellAmount)
                            {
                                //fazla sattığı miktarı burada geri alıyorumm 
                                //geri miktarı hesaplıyorum
                                var coinBack = sellCounter - requestSellAmount;
                                //openorders düzeltiyorum
                                var openOrderDT = _openOrderDal.Get(x => x.Id == buys.Id);
                                var openOrderPayback = new OpenOrder
                                {
                                    Amount = coinBack,
                                    Coin = openOrderDT.Coin,
                                    DateTime = openOrderDT.DateTime,
                                    Id = openOrderDT.Id,
                                    Operation = openOrderDT.Operation,
                                    Price = openOrderDT.Price,
                                    Status = openOrderDT.Status,
                                    Total = openOrderDT.Total,
                                    UserId = openOrderDT.UserId
                                };
                                _openOrderDal.Update(openOrderPayback);
                                //counter düzeltiyorum
                                sellCounter = sellCounter - coinBack;
                                break;
                            }
                        }
                    }
                    //İstenen miktarı karşılayamadıysa islem beklemeye alınıyor
                    if (sellCounter != requestSellAmount)
                    {
                        var openOrderPending = new OpenOrder
                        {
                            UserId = userDb.UserId,
                            Coin = "BTC",
                            DateTime = DateTime.Now,
                            Amount = sellAmount,
                            Operation = "Sell",
                            Price = sellLimit,
                            Total = gain,
                            Status = 2,
                        };
                        _openOrderDal.Add(openOrderPending);
                        return new SuccessResult($"{buyerGain} miktarında BTC satış emri beklemeye alındı.");
                    }
                    //işlem complete olarak openorderse ekleniyor.
                    var openOrder = new OpenOrder
                    {
                        UserId = userDb.UserId,
                        Coin = "BTC",
                        DateTime = DateTime.Now,
                        Amount = sellAmount,
                        Operation = "Sell",
                        Price = sellLimit,
                        Total = gain,
                        Status = 1,
                    };
                    _openOrderDal.Add(openOrder);
                    //satıcı varlıklarını güncelliyrum
                    var uA = _userAssetsDal.Get(x => x.UserId == userDb.UserId);
                    var userAssetsFinito = new UserAssets
                    {
                        BTC = uA.BTC - buyerGain,
                        Id = uA.Id,
                        ETH = uA.ETH,
                        status = uA.status,
                        TRX = uA.TRX,
                        USDT = uA.USDT + gain,
                        UserId = uA.UserId,
                    };
                    _userAssetsDal.Update(userAssetsFinito);
                    //userwallets güncelliyorum
                    var uWComplete = _walletDal.Get(x => x.UserId == userDb.UserId);
                    var walletComplete = new UserWallet
                    {
                        UserId = uWComplete.UserId,
                        Balance = uWComplete.Balance + gain,
                    };
                    _walletDal.Update(walletComplete);
                    //komisyonu ekliyorum "alıcı"

                    return new SuccessResult($"{buyerGain} miktarında BTC satış emri başarılı bir şekilde tamamlandı.");
                }
                return new ErrorResult($"Yetersiz miktar, mevcut sahip olduğunuz miktar {btcInformations.BTC} BTC");
            }
            return new ErrorResult($"Satış emri oluşturabilmek için girebileceğiniz maksimum limit fiyatı {maxIn}$ olmalıdır.");
        }


        //public IResult SellLimiteOrder(string token, decimal sellLimit, decimal sellAmount)
        //{
        //    //Token üzerinden user bilgileri
        //    var userId = _token.GetToken(token);
        //    var userDb = _userDal.Get(x => x.UserId == userId);
        //    //Satıcının coin bilgileri
        //    var coinInformations = _userAssetsDal.Get(x => x.UserId == userDb.UserId);
        //    //Parite getiriyorum 
        //    HttpClient client = new HttpClient();
        //    var response = client.GetAsync($"https://api.binance.com/api/v3/ticker/price?symbol=BTCUSDT").Result;
        //    var responseBody = response.Content.ReadAsStringAsync();
        //    var BTC = JsonConvert.DeserializeObject<Parite>(responseBody.Result);
        //    //Hesaplamalar----------------------------------------------------------------------------------------------
        //    //Elde edeceği ücreti hesaplıyorum
        //    var gain = sellAmount * sellLimit;
        //    //satıcının eline geçecek olan USDT'ye komisyon kesiyorum
        //    var sellerCommission = gain / 1000;
        //    var sellerGain = gain - sellerCommission;
        //    //alıcının eline geçecek olan BTC'ye komisyon kesiyorum 
        //    var buyerCommission = sellAmount / 1000;
        //    var buyerGain = sellAmount - buyerCommission;
        //    //openOrders tablosundaki denk gelen miktarı getir
        //    var equalAmountBTC = _context.OpenOrders.First(x => x.Operation == "Buy" && x.Coin == "BTC" && x.Status == 2 && x.Amount == sellAmount);
        //    //maks emir limiti hesaplama
        //    AmountCalculator amountCalculator = new AmountCalculator();
        //    var maksSellLimit = amountCalculator.GetSellAmount(sellLimit);
        //    if (sellLimit <= maksSellLimit)
        //    {
        //        if (coinInformations.BTC >= sellAmount)
        //        {
        //            //-------------Emir karşılıyor ise-------------
        //            if (sellLimit == BTC.price && equalAmountBTC.Amount == sellAmount)
        //            {
        //                //OpenOrders tablosuna ekliyorum
        //                var openOrder = new OpenOrder
        //                {
        //                    UserId = userDb.UserId,
        //                    DateTime = DateTime.UtcNow,
        //                    Coin = "BTC",
        //                    Operation = "Sell",
        //                    Amount = sellAmount,
        //                    Price = sellLimit,
        //                    Total = gain,
        //                    Status = 1
        //                };
        //                _openOrderDal.Add(openOrder);
        //                //komisyonu şirket kasasına ekliyorum
        //                var cV = _companyVaultDal.Get(x => x.BTC == BTC.price);
        //                var companyVault = new CompanyVault
        //                {
        //                    BTC = cV.BTC + sellerCommission,
        //                    USDT = cV.USDT + buyerCommission,
        //                    ETH = cV.ETH,
        //                    TRX = cV.TRX,
        //                    Id = cV.Id
        //                };
        //                _companyVaultDal.Update(companyVault);
        //                //satıcının varlıklarını güncelliyorum
        //                var uA = _userAssetsDal.Get(x => x.UserId == userDb.UserId);
        //                var userAssets = new UserAssets
        //                {
        //                    Id = uA.Id,
        //                    ETH = uA.ETH,
        //                    TRX = uA.TRX,
        //                    BTC = uA.BTC - sellAmount,
        //                    UserId = uA.UserId,
        //                    USDT = uA.USDT + sellerGain,
        //                    status = uA.status
        //                };
        //                _userAssetsDal.Update(userAssets);
        //                //satıcının balance'ını güncelliyorum
        //                var uW = _walletDal.Get(x => x.UserId == userDb.UserId);
        //                var userWallet = new UserWallet
        //                {
        //                    UserId = uW.UserId,
        //                    Balance = uW.Balance + sellerGain
        //                };
        //                _walletDal.Update(userWallet);
        //                //alıcının varlıklarını güncelliyorum
        //                var buyerAssetsData = _userAssetsDal.Get(x => x.UserId == equalAmountBTC.UserId);
        //                var newBuyerUSDT = BTC.price / buyerGain;
        //                var buyerAssets = new UserAssets
        //                {
        //                    Id = buyerAssetsData.Id,
        //                    status = buyerAssetsData.status,
        //                    TRX = buyerAssetsData.TRX,
        //                    ETH = buyerAssetsData.ETH,
        //                    UserId = buyerAssetsData.UserId,
        //                    BTC = buyerAssetsData.BTC + buyerGain,
        //                    USDT = buyerAssetsData.USDT - gain
        //                };
        //                _userAssetsDal.Update(buyerAssets);
        //                //alıcının balance'ını güncelliyorum
        //                var buyerUW = _walletDal.Get(x => x.UserId == equalAmountBTC.UserId);
        //                var buyerWallet = new UserWallet
        //                {
        //                    UserId = buyerUW.UserId,
        //                    Balance = buyerUW.Balance - gain
        //                };
        //                _walletDal.Update(buyerWallet);
        //                return new SuccessResult($"{sellerGain}$ karşılığında, {sellAmount} miktarında BTC satışı gerçekleştirildi. [Alt Bilgi] {sellerCommission}$ satıcıdan komisyon kesildi, {buyerCommission} miktarında BTC alıcıdan komisyon kesildi. Güncel şirket kasası BTC= {companyVault.BTC} USDT= {companyVault.USDT}$");
        //            }
        //            //-------------Emir karşılamıyor ise----------------------------------------------------------------------------------------------------------------------------------------
        //            //Satıcının alacağı usdtye komisyon kesiyorum
        //            var usdtCommission = gain / 1000;
        //            //openorders ekliyorum
        //            var openOrders = new OpenOrder
        //            {
        //                UserId = userDb.UserId,
        //                Coin = "BTC",
        //                Operation = "Sell",
        //                Price = sellLimit,
        //                Amount = sellAmount,
        //                DateTime = DateTime.UtcNow,
        //                Total = gain,
        //                Status = 2
        //            };
        //            _openOrderDal.Add(openOrders);
        //            //komisyonu ve coini askıya alıyorum
        //            var pending = new Pending
        //            {
        //                BTC = sellAmount,
        //                Commission = usdtCommission,
        //                OpenedDateTime = DateTime.UtcNow,
        //                ETH = 0,
        //                TRX = 0,
        //                Status = 1,
        //                USDT = 0,
        //                UserId = userDb.UserId,
        //                OpenOrdersId = openOrders.Id,
        //                Operation = "Sell"
        //            };
        //            _pendingCoinsDal.Add(pending);
        //            //komisyonu kasaya ekliyorum
        //            var companyV = _companyVaultDal.Get(x => x.BTC == BTC.price);
        //            var companyVaultUpdate = new CompanyVault
        //            {
        //                BTC = companyV.BTC,
        //                ETH = companyV.ETH,
        //                TRX = companyV.TRX,
        //                Id = companyV.Id,
        //                USDT = companyV.USDT + usdtCommission
        //            };
        //            _companyVaultDal.Update(companyVaultUpdate);
        //            //satıcının varlıklarından coini düşüyorum
        //            var sellerAssets = _userAssetsDal.Get(x => x.UserId == userDb.UserId);
        //            var newAssets = new UserAssets
        //            {
        //                UserId = sellerAssets.UserId,
        //                ETH = sellerAssets.ETH,
        //                Id = sellerAssets.Id,
        //                status = sellerAssets.status,
        //                TRX = sellerAssets.TRX,
        //                USDT = sellerAssets.USDT,
        //                BTC = sellerAssets.BTC - sellAmount
        //            };
        //            _userAssetsDal.Update(newAssets);
        //            return new SuccessResult($"{sellAmount} miktarında BTC satış emri işleme alındı.");
        //        }
        //        return new ErrorResult($"Bu miktarda BTC'ye sahip değilsiniz. {coinInformations.BTC} BTC'ye sahipsiniz.");
        //    }
        //    return new SuccessResult($"{maksSellLimit}$ üzerinde işlem açamazsınız.");
        //}
    }

}

