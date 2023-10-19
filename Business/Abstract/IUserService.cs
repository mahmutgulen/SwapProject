using Core.Entities.Concrete;
using Core.Utilities.Results;
using Entities.Concrete.Dtos.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IUserService
    {
        IDataResult<List<Role>> GetRoles(User user);
        IDataResult<User> GetByMail(string email);
    }
}
