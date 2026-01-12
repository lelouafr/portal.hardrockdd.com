using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class HRPositionApplicant
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
                    _db = VPContext.GetDbContextFromEntity(this);

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

        public HRPositionApplicantStatusEnum Status
        {
            get
            {
                return (HRPositionApplicantStatusEnum)(this.StatusId);
            }
            set
            {
                if (StatusId != (int)value || value == HRPositionApplicantStatusEnum.Pending)
                    UpdateStatusChange((int)value);

            }
        }

        public void UpdateStatusChange(int newValue)
        {
            var status = (HRPositionApplicantStatusEnum)newValue;
            if (newValue != StatusId || status == HRPositionApplicantStatusEnum.Pending)
            {
                switch (status)
                {
                    case HRPositionApplicantStatusEnum.Pending:
                        this.ApprovedOn = null;
                        this.ApprovedBy = null;
                        break;
                    case HRPositionApplicantStatusEnum.Approved:
                        this.ApprovedOn = DateTime.Now;
                        this.ApprovedBy = this.db.CurrentUserId;
                        break;
                    case HRPositionApplicantStatusEnum.Denied:
                        this.ApprovedOn = null;
                        this.ApprovedBy = null;
                        break;
                    default:
                        break;
                }

                tStatusId = newValue;
                //WorkFlow.CreateSequence((int)newValue);
                //WorkFlow.CurrentSequence().Comments = StatusComments;
                //GenerateWorkFlowAssignments();
            }
        }
    }
}