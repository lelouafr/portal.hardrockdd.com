using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class PRCrewRequest
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
                    _db = this.PRCompany?.db;

                    if (_db == null)
                        _db = VPEntities.GetDbContextFromEntity(this);

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

        public DB.PRCrewRequestStatusEnum Status
        {
            get
            {
                return (DB.PRCrewRequestStatusEnum)(this.StatusId);
            }
            set
            {
                if (StatusId != (int)value || value == DB.PRCrewRequestStatusEnum.New)
                    UpdateStatusChange((int)value);

            }
        }

        public void UpdateStatusChange(int newValue)
        {
            if (WorkFlow == null)
                GenerateWorkFlow();

            var status = (DB.PRCrewRequestStatusEnum)newValue;
            if (newValue != StatusId || status == DB.PRCrewRequestStatusEnum.New)
            {
                switch (status)
                {
                    case DB.PRCrewRequestStatusEnum.New:
                        break;
                    case DB.PRCrewRequestStatusEnum.Submitted:
                        break;
                    case DB.PRCrewRequestStatusEnum.Approved:
                        break;
                    case DB.PRCrewRequestStatusEnum.Completed:
                        break;
                    case DB.PRCrewRequestStatusEnum.Canceled:
                        break;
                    default:
                        break;
                }
                tStatusId = newValue;
                WorkFlow.CreateSequance((int)newValue);
                WorkFlow.CurrentSequance().Comments = StatusComments;
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
                PRCompany.HQCompanyParm.WorkFlows.Add(workflow);
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
                var assignedusers = WorkFlow.CurrentSequance().AssignedUsers.ToList();
                foreach (var user in assignedusers)
                {
                    WorkFlow.CurrentSequance().AssignedUsers.Remove(user);
                }
            }
            switch (Status)
            {
                case DB.PRCrewRequestStatusEnum.New:
                    WorkFlow.AddUser(RequestedBy);
                    break;
                case DB.PRCrewRequestStatusEnum.Submitted:
                    //WorkFlow.GenerateUsersByPosition("HR-MGR");
                    WorkFlow.AddUsersByPosition("IT-DIR");
                    //EmailStatusUpdate();
                    break;
                case DB.PRCrewRequestStatusEnum.Approved:

                    break;
                case DB.PRCrewRequestStatusEnum.Completed:
                    WorkFlow.CompleteWorkFlow();
                    break;
                case DB.PRCrewRequestStatusEnum.Canceled:
                    WorkFlow.CompleteWorkFlow();
                    break;
                default:
                    break;
            }
        }
        #endregion

    }
}