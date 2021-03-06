using System;
using designPatterns.Data;
using designPatterns.Domain;
using designPatterns.Domain.Entities;
using designPatterns.Domain.Exceptions;
using designPatterns.Domain.Services;
using designPatterns.Web.Api.Infrastructure.Authentication;
using Nancy.Security;

namespace designPatterns.Web.Api.Infrastructure.Configuration
{
    public class ApiUserMapper : IApiUserMapper<Guid>
    {
        readonly IReadOnlyRepository _readOnlyRepo;
        readonly ITimeProvider _timeProvider;

        public ApiUserMapper(IReadOnlyRepository readOnlyRepo, ITimeProvider timeProvider)
        {
            _readOnlyRepo = readOnlyRepo;
            _timeProvider = timeProvider;
        }

        #region IApiUserMapper<Guid> Members

        public IUserIdentity GetUserFromAccessToken(Guid token)
        {
            UserLoginSession userLoginSession = GetUserSessionFromToken(token);
            MakeSureTokenHasntExpiredYet(userLoginSession);
            return new LoggedInUserIdentity(userLoginSession);
        }

        #endregion

        UserLoginSession GetUserSessionFromToken(Guid token)
        {
            UserLoginSession userLoginSession;
            try
            {
                userLoginSession = _readOnlyRepo.First<UserLoginSession>(x => x.Id == token);
            }
            catch (ItemNotFoundException<UserLoginSession> e)
            {
                throw new TokenDoesNotExistException();
            }
            return userLoginSession;
        }

        void MakeSureTokenHasntExpiredYet(UserLoginSession userLoginSession)
        {
            DateTime expires = userLoginSession.Expires;
            DateTime now = _timeProvider.Now();
            if (expires < now)
            {
                throw new TokenExpiredException();
            }
        }
    }
}