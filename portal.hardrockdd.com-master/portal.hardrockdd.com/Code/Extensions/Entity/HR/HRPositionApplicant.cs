using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class HRPositionApplicant
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
                    _db = VPEntities.GetDbContextFromEntity(this);

                    if (_db == null)
                        _db = this.Request.db;

                    if (_db == null)
                        throw new NullReferenceException("GetDbContextFromEntity is null");

                }
                return _db;
            }
        }

        public int StatusId
        {
            get
            {
                return tStatusId;
            }
            set
            {
                if (tStatusId != value)
                {
                    UpdateStatusChange(value);
                }
            }
        }

        public DB.HRPositionApplicantStatusEnum Status
        {
            get
            {
                return (DB.HRPositionApplicantStatusEnum)(this.StatusId);
            }
            set
            {
                if (StatusId != (int)value || value == DB.HRPositionApplicantStatusEnum.Pending)
                    UpdateStatusChange((int)value);

            }
        }

        public void UpdateStatusChange(int newValue)
        {
            var status = (DB.HRPositionApplicantStatusEnum)newValue;
            if (newValue != StatusId || status == DB.HRPositionApplicantStatusEnum.Pending)
            {
                switch (status)
                {
                    case DB.HRPositionApplicantStatusEnum.Pending:
                        this.ApprovedOn = null;
                        this.ApprovedBy = null;
                        break;
                    case DB.HRPositionApplicantStatusEnum.Approved:
                        this.ApprovedOn = DateTime.Now;
                        this.ApprovedBy = this.db.CurrentUserId;
                        break;
                    case DB.HRPositionApplicantStatusEnum.Denied:
                        this.ApprovedOn = null;
                        this.ApprovedBy = null;
                        break;
                    default:
                        break;
                }

                tStatusId = newValue;
                //WorkFlow.CreateSequance((int)newValue);
                //WorkFlow.CurrentSequance().Comments = StatusComments;
                //GenerateWorkFlowAssignments();
            }
        }
    }
}