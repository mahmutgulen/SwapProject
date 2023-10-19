using Entities.Concrete;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Contants
{
    public class AmountCalculator
    {
        public decimal GetBuyAmount(decimal amount)
        {
            HttpClient client = new HttpClient();
            var response = client.GetAsync($"https://api.binance.com/api/v3/ticker/price?symbol=BTCUSDT").Result;
            var responseBody = response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<Parite>(responseBody.Result);

            if (amount >= 0 && amount <= 100)
            {

                var girilecekMaksFiyat = data.price - 5;
                return girilecekMaksFiyat;
            }
            else if (amount >= 100 && amount <= 1000)
            {

                var girilecekMaksFiyat = data.price - 20;
                return girilecekMaksFiyat;
            }
            else if (amount >= 1000 && amount <= 10000)
            {

                var girilecekMaksFiyat = data.price - 80;
                return girilecekMaksFiyat;
            }
            else if (amount >= 10000 && amount <= 100000)
            {

                var girilecekMaksFiyat = data.price - 320;
                return girilecekMaksFiyat;
            }
            else if (amount >= 100000 && amount <= 1000000)
            {

                var girilecekMaksFiyat = data.price - 1240;
                return girilecekMaksFiyat;
            }
            return amount;
        }

        public decimal GetSellAmount(decimal amount)
        {
            HttpClient client = new HttpClient();
            var response = client.GetAsync($"https://api.binance.com/api/v3/ticker/price?symbol=BTCUSDT").Result;
            var responseBody = response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<Parite>(responseBody.Result);

            if (amount >= 0 && amount <= 100)
            {
                var girilecekMinFiyat = data.price + 5;
                return girilecekMinFiyat;
            }
            else if (amount >= 100 && amount <= 1000)
            {
                var girilecekMinFiyat = data.price + 20;
                return girilecekMinFiyat;
            }
            else if (amount >= 1000 && amount <= 10000)
            {
                var girilecekMinFiyat = data.price + 80;
                return girilecekMinFiyat;
            }
            else if (amount >= 10000 && amount <= 100000)
            {
                var girilecekMinFiyat = data.price + 320;
                return girilecekMinFiyat;
            }
            else if (amount >= 100000 && amount <= 1000000)
            {
                var girilecekMinFiyat = data.price + 1240;
                return girilecekMinFiyat;
            }
            return amount;
        }
    }
}
