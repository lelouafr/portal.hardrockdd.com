using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Linq;

namespace portal.Repository.VP.WP
{
    public partial class WebUserRepository : IDisposable
    {
        
        public WebUser GetUserbyEmail(string Email)
        {
            var qry = db.WebUsers.FirstOrDefault(user => user.Email.ToLower() == Email.ToLower());
            
            return qry;
        }

    }
}