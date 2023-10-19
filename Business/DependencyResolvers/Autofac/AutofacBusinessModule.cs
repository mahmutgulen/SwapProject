using Autofac;
using Business.Abstract;
using Business.Concrete;
using Business.Contants;
using Core.Entities.Concrete;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using DataAccess.Concrete.EntityFramework.Contexts;

namespace Business.DependencyResolvers.Autofac
{
    public class AutofacBusinessModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UserManager>().As<IUserService>();
            builder.RegisterType<EfUserDal>().As<IUserDal>();
            builder.RegisterType<EfSuspendDal>().As<ISuspendDal>();

            builder.RegisterType<SwapContext>().As<SwapContext>();
            builder.RegisterType<AmountCalculator>().As<AmountCalculator>();
            builder.RegisterType<UserRole>().As<UserRole>();
            builder.RegisterType<EfUserRoleDal>().As<IUserRoleDal>();

            builder.RegisterType<AuthManager>().As<IAuthService>();
            builder.RegisterType<CancelManager>().As<ICancelService>();
            builder.RegisterType<JwtHelper>().As<ITokenHelper>();

            builder.RegisterType<Token>().As<IToken>();

            builder.RegisterType<WalletManager>().As<IWalletService>();
            builder.RegisterType<EfWalletDal>().As<IWalletDal>();

            builder.RegisterType<MarketPriceManager>().As<IMarketPriceService>();
            builder.RegisterType<EfMarketPriceDal>().As<IMarketPriceDal>();


            builder.RegisterType<AdminManager>().As<IAdminService>();

            builder.RegisterType<EfUserAssetsDal>().As<IUserAssetsDal>();

            builder.RegisterType<EfCompanyVaultDal>().As<ICompanyVaultDal>();


            builder.RegisterType<BtcUsdtManager>().As<IBtcUsdtService>();
            builder.RegisterType<HistoryManager>().As<IHistoryService>();


            builder.RegisterType<EfPendingCoinsDal>().As<IPendingCoinsDal>();
            builder.RegisterType<EfOpenOrderDal>().As<IOpenOrderDal>();

        }
    }
}
