using Business.Abstract;
using Business.Contants;
using Core.Utilities.Results;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class HistoryManager : IHistoryService
    {
        private IUserDal _userDal;
        private IToken _token;
        private IOpenOrderDal _openOrderDal;

        public HistoryManager(IUserDal userDal, IToken token, IOpenOrderDal openOrderDal)
        {
            _userDal = userDal;
            _token = token;
            _openOrderDal = openOrderDal;
        }

        public IDataResult<List<OpenOrder>> GetMyTransactions(string token)
        {
            //Token üzerinden user bilgileri
            var userId = _token.GetToken(token);
            var userDb = _userDal.Get(x => x.UserId == userId);
            //openOrder
            var openOrder = _openOrderDal.GetList(x => x.UserId == userDb.UserId).ToList();
            return new SuccessDataResult<List<OpenOrder>>(openOrder);
        }
    }
}
