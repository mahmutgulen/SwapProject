using Core.Entities.Concrete;
using Core.Utilities.Results;
using DataAccess.Abstract;
using System.IdentityModel.Tokens.Jwt;

namespace Business.Contants
{
    public class Token : IToken
    {
        private IUserDal _userDal;

        public Token(IUserDal userDal)
        {
            _userDal = userDal;
        }


        public int GetToken(string token)
        {
            var stream = token;
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(stream);
            var tokenS = jsonToken as JwtSecurityToken;
            var userId = Convert.ToInt32(tokenS.Claims.First(claim => claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value);
            var userDb = _userDal.Get(x => x.UserId == userId);

             return userDb.UserId;
        }

    }
}
