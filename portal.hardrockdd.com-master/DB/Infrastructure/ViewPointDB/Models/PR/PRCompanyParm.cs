using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class PRCompanyParm
    {
        private VPContext _db;

        public VPContext db
        {
            set
            {
                _db = value;
            }
            get
            {
                if (_db == null)
                {
                    _db = this.HQCompanyParm?.db;

                    if (_db == null)
                        _db = VPContext.GetDbContextFromEntity(this);

                }
                return _db;
            }
        }

        public PRCrewRequest AddCrewRequest()
        {
            var request = new PRCrewRequest
            {
                PRCo = PRCo,
                RequestId = db.GetNextId(PRCrewRequest.BaseTableName),
                RequestedDate = DateTime.Now,
                RequestedBy = db.CurrentUserId,

                PRCompany = this,
                RequestedUser = db.GetCurrentUser(),
            };
            request.GenerateWorkFlow();
            request.Status = PRCrewRequestStatusEnum.New;
            this.PRCrewRequests.Add(request);

            return request;
        }
    }
}