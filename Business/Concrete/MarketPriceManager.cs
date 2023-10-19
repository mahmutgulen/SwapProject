using Business.Abstract;
using Core.Utilities.Results;
using Entities.Concrete;
using Entities.Dtos.MarketPrice;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class MarketPriceManager : IMarketPriceService
    {

        public IDataResult<MarketPriceListDto> GetCoins()
        {
            HttpClient client = new HttpClient();
            //BTC
            var responseBTC = client.GetAsync($"https://api.binance.com/api/v3/ticker/price?symbol=BTCUSDT").Result;
            var responseBodyBTC = responseBTC.Content.ReadAsStringAsync();
            var BTC = JsonConvert.DeserializeObject<Parite>(responseBodyBTC.Result);
            //ETH
            var responseETH = client.GetAsync($"https://api.binance.com/api/v3/ticker/price?symbol=ETHUSDT").Result;
            var responseBodyETH = responseETH.Content.ReadAsStringAsync();
            var ETH = JsonConvert.DeserializeObject<Parite>(responseBodyETH.Result);
            //ADA
            var responseADA = client.GetAsync($"https://api.binance.com/api/v3/ticker/price?symbol=ADAUSDT").Result;
            var responseBodyADA = responseADA.Content.ReadAsStringAsync();
            var ADA = JsonConvert.DeserializeObject<Parite>(responseBodyADA.Result);
            //TRX
            var responseTRX = client.GetAsync($"https://api.binance.com/api/v3/ticker/price?symbol=TRXUSDT").Result;
            var responseBodyTRX = responseTRX.Content.ReadAsStringAsync();
            var TRX = JsonConvert.DeserializeObject<Parite>(responseBodyTRX.Result);
            //BNB
            var responseBNB = client.GetAsync($"https://api.binance.com/api/v3/ticker/price?symbol=BNBUSDT").Result;
            var responseBodyBNB = responseBNB.Content.ReadAsStringAsync();
            var BNB = JsonConvert.DeserializeObject<Parite>(responseBodyBNB.Result);
            //DASH
            var responseDASH = client.GetAsync($"https://api.binance.com/api/v3/ticker/price?symbol=DASHUSDT").Result;
            var responseBodyDASH = responseDASH.Content.ReadAsStringAsync();
            var DASH = JsonConvert.DeserializeObject<Parite>(responseBodyDASH.Result);
            //DOGE
            var responseDOGE = client.GetAsync($"https://api.binance.com/api/v3/ticker/price?symbol=DOGEUSDT").Result;
            var responseBodyDOGE = responseDOGE.Content.ReadAsStringAsync();
            var DOGE = JsonConvert.DeserializeObject<Parite>(responseBodyDOGE.Result);
            //XRP
            var responseXRP = client.GetAsync($"https://api.binance.com/api/v3/ticker/price?symbol=XRPUSDT").Result;
            var responseBodyXRP = responseXRP.Content.ReadAsStringAsync();
            var XRP = JsonConvert.DeserializeObject<Parite>(responseBodyXRP.Result);
            //SHIB
            var responseSHIB = client.GetAsync($"https://api.binance.com/api/v3/ticker/price?symbol=SHIBUSDT").Result;
            var responseBodySHIB = responseSHIB.Content.ReadAsStringAsync();
            var SHIB = JsonConvert.DeserializeObject<Parite>(responseBodySHIB.Result);
            //UNI
            var responseUNI = client.GetAsync($"https://api.binance.com/api/v3/ticker/price?symbol=UNIUSDT").Result;
            var responseBodyUNI = responseUNI.Content.ReadAsStringAsync();
            var UNI = JsonConvert.DeserializeObject<Parite>(responseBodyUNI.Result);

            var marketPriceDto = new MarketPriceListDto
            {
                BTC = BTC.price,
                ETH = ETH.price,
                ADA = ADA.price,
                TRX = TRX.price,
                BNB = BNB.price,
                DASH = DASH.price,
                DOGE = DOGE.price,
                XRP = XRP.price,
                SHIB = SHIB.price,
                UNI = UNI.price
            };
            return new SuccessDataResult<MarketPriceListDto>(marketPriceDto);
        }
    }
}
