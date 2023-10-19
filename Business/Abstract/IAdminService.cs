using Core.Entities.Concrete;
using Core.Utilities.Results;
using Entities.Concrete;
using Entities.Dtos.CompanyVault;

namespace Business.Abstract
{
    public interface IAdminService
    {
        //Get
        IDataResult<List<User>> GetUsers();
        IDataResult<List<UserWallet>> GetWallets();
        IDataResult<List<OpenOrder>> GetOpenOrders();
        IDataResult<List<CompanyVault>> GetCompanyVault();
        IDataResult<List<UserAssets>> GetUserAssets();

        //Suspend Users
        IResult SuspendAccount(int UserId, string description);
        IResult RemoveSuspendAccount(int UserId);
        IDataResult<List<SuspendUser>> GetSuspendUsers();

    }
}
