using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class PRCrewRequest
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
                    _db = this.PRCompany?.db;

                    if (_db == null)
                        _db = VPContext.GetDbContextFromEntity(this);

                }
                return _db;
            }
        }

        public static string BaseTableName { get { return "budPRCR"; } }



        public string StatusComments { get; set; }

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
        public int? WPDivisionId
        {
            get
            {
                return this.tWPDivisionId;
            }
            set
            {
                if (tWPDivisionId != value)
                {
                    tWPDivisionId = value;
                }
            }
        }

        public PRCrewRequestStatusEnum Status
        {
            get
            {
                return (PRCrewRequestStatusEnum)(this.StatusId);
            }
            set
            {
                if (StatusId != (int)value || value == PRCrewRequestStatusEnum.New)
                    UpdateStatusChange((int)value);

            }
        }

        public void UpdateStatusChange(int newValue)
        {
            if (WorkFlow == null)
                GenerateWorkFlow();

            var status = (PRCrewRequestStatusEnum)newValue;
            if (newValue != StatusId || status == PRCrewRequestStatusEnum.New)
            {
                switch (status)
                {
                    case PRCrewRequestStatusEnum.New:
                        break;
                    case PRCrewRequestStatusEnum.Submitted:
                        break;
                    case PRCrewRequestStatusEnum.Approved:
                        break;
                    case PRCrewRequestStatusEnum.Completed:
                        break;
                    case PRCrewRequestStatusEnum.Canceled:
                        break;
                    default:
                        break;
                }
                tStatusId = newValue;
                WorkFlow.CreateSequence((int)newValue);
                WorkFlow.CurrentSequence().Comments = StatusComments;
                GenerateWorkFlowAssignments();
            }
        }

        #region Workflow
        public void GenerateWorkFlow()
        {
            if (WorkFlow == null)
            {
                var workflow = new WorkFlow
                {
                    WFCo = PRCo,
                    WorkFlowId = WorkFlow.GetNextWorkFlowId(PRCo),
                    TableName = HRTermRequest.BaseTableName,
                    Id = RequestId,
                    CreatedBy = RequestedBy,
                    CreatedOn = RequestedDate,
                    Active = true,

                    Company = PRCompany.HQCompanyParm,
                };
                //PRCompany.HQCompanyParm.WorkFlows.Add(workflow);
                db.WorkFlows.Add(workflow);
                WorkFlow = workflow;
                WorkFlowId = workflow.WorkFlowId;
            }
        }

        public void GenerateWorkFlowAssignments(bool reset = false)
        {
            if (WorkFlow == null)
                GenerateWorkFlow();
            if (reset)
            {
                var assignedusers = WorkFlow.CurrentSequence().AssignedUsers.ToList();
                foreach (var user in assignedusers)
                {
                    WorkFlow.CurrentSequence().AssignedUsers.Remove(user);
                }
            }
            switch (Status)
            {
                case PRCrewRequestStatusEnum.New:
                    WorkFlow.AddUser(RequestedBy);
                    break;
                case PRCrewRequestStatusEnum.Submitted:
                    //WorkFlow.GenerateUsersByPosition("HR-MGR");
                    WorkFlow.AddUsersByPosition("IT-DIR");
                    //EmailStatusUpdate();
                    break;
                case PRCrewRequestStatusEnum.Approved:

                    break;
                case PRCrewRequestStatusEnum.Completed:
                    WorkFlow.CompleteWorkFlow();
                    break;
                case PRCrewRequestStatusEnum.Canceled:
                    WorkFlow.CompleteWorkFlow();
                    break;
                default:
                    break;
            }
        }
        #endregion

    }
}