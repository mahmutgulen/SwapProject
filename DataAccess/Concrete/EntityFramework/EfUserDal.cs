using Core.DataAccess.EntityFramework;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Contexts;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfUserDal : EfEntityRepositoryBase<User, SwapContext>, IUserDal
    {
        public List<Role> GetRoles(User user)
        {
            using (var context = new SwapContext())
            {
                var result = from role in context.Roles
                             join userrole in context.UserRoles
                             on role.Id equals userrole.RoleId
                             where userrole.UserId == user.UserId
                             select new Role { Id = role.Id, Name = role.Name };
                return result.ToList();
            }
        }
    }
}
