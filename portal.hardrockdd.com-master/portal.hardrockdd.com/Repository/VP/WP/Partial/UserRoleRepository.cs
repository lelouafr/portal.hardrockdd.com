using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace portal.Repository.VP.WP
{
    public partial class WebUserRoleRepository : IDisposable
    {
        public List<WebRole> GetAssignedRoles(string userId)
        {
            var qry = db.WebRoles.Where(f => db.WebUserRoles.Where(r => r.RoleId == f.Id && r.UserId == userId).Any());
            var results = qry.ToList();
            return results;
        }

        public List<WebRole> GetUnassignedRoles(string userId)
        {
            var qry = db.WebRoles.Where(f => !db.WebUserRoles.Where(r => r.RoleId == f.Id && r.UserId == userId).Any());
            var results = qry.ToList();
            return results;

        }

        public List<WebUser> GetAssignedUsers(string roleId)
        {
            var qry = db.WebUsers.Where(f => db.WebUserRoles.Where(r => r.RoleId == roleId && r.UserId == f.Id).Any());
            var results = qry.ToList();
            return results;
        }

        public List<WebUser> GetUnassignedUsers(string roleId)
        {
            var qry = db.WebUsers.Where(f => !db.WebUserRoles.Where(r => r.RoleId == roleId && r.UserId == f.Id).Any());
            var results = qry.ToList();
            return results;

        }

    }
}