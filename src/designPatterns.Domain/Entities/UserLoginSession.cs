using System;
using System.Linq;
using System.Security.Claims;
using AcklenAvenue.Commands;
using designPatterns.Domain.Services;


namespace designPatterns.Domain.Entities
{
    public class UserLoginSession : Entity, IUserSession
    {
        protected UserLoginSession()
        {            
        }

        public UserLoginSession(Guid token, User user, DateTime expires)
        {
            Id = token;
            User = user;
            Expires = expires;

            Claims = string.Join(",", user.UserRoles.Select(x => x.Description));
        }

        public virtual User User { get; protected set; }
        public virtual DateTime Expires { get; protected set; }
        public virtual string Claims { get; protected set; }

        public virtual string[] GetClaimsAsArray()
        {

            return Claims.Split(',');
        }

    }
}