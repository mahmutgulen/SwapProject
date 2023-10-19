using Business.Abstract;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using DataAccess.Abstract;

namespace Business.Concrete
{
    public class UserManager : IUserService
    {
        private readonly IUserDal _userDal;

        public UserManager(IUserDal userDal)
        {
            _userDal = userDal;
        }
        public IDataResult<User> GetByMail(string email)
        {
            var user = _userDal.Get(x => x.UserEmail == email);
            return new SuccessDataResult<User>(user);
        }

        public IDataResult<List<Role>> GetRoles(User user)
        {
            return new SuccessDataResult<List<Role>>(_userDal.GetRoles(user));
        }
    }
}
