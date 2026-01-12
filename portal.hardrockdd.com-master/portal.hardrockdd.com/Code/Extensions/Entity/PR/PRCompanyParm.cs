using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class PRCompanyParm
    {
        private VPEntities _db;

        public VPEntities db
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
                        _db = VPEntities.GetDbContextFromEntity(this);

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
                RequestedUser = db.GetCurrenUser(),
            };
            request.GenerateWorkFlow();
            request.Status = DB.PRCrewRequestStatusEnum.New;
            this.PRCrewRequests.Add(request);

            return request;
        }
    }
}