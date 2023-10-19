using Core.Utilities.Results;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IWalletService
    {
        IResult AddBalance(int balance, string token);
        IResult WithDrawBalance(int balance, string token);
        IDataResult<UserWallet> GetBalance(string token);

    }
}
