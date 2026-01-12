using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class HRPositionRequest
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
                        _db = this.HRCompanyParm?.db;

                }
                return _db;
            }
        }

        public static string BaseTableName { get { return "budHRPR"; } }

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

        public DB.HRPositionRequestStatusEnum Status
        {
            get
            {
                return (DB.HRPositionRequestStatusEnum)(this.StatusId);
            }
            set
            {
                if (StatusId != (int)value || value == DB.HRPositionRequestStatusEnum.New)
                    UpdateStatusChange((int)value);

            }
        }

        public bool CreateCrew
        {
            get
            {
                return tCreateCrew ?? false;
            }
            set
            {
                if (tCreateCrew != value)
                {
                    UpdateCreateCrew(value);
                }
            }
        }

        public string ForCrewId
        {
            get
            {
                return tForCrewId;
            }
            set
            {
                if (value != tForCrewId)
                {
                    UpdateForCrew(value);
                }
            }
        }

        public int? PriorEmployeeId
        {
            get
            {
                return tPriorEmployeeId;
            }
            set
            {
                if (value != tPriorEmployeeId)
                {
                    UpdatePriorEmployee(value);
                }
            }
        }

        public int? NewEmployeeId
        {
            get
            {
                return tNewEmployeeId;
            }
            set
            {
                if (value != tNewEmployeeId)
                {
                    UpdateNewEmployee(value);
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
                    if (this.PRCrewRequest != null)
                        this.PRCrewRequest.WPDivisionId = value;
                    tWPDivisionId = value;

                    if (WPDivision?.DivisionId != value)
                        WPDivision = db.CompanyDivisions.FirstOrDefault(f => f.DivisionId == value);
                }
            }
        }

        public int? WAApplicantId
        {
            get
            {
                return tWAApplicantId;
            }
            set
            {
                if (tWAApplicantId != value)
                {
                    UpdateApplicant(value);
                }
            }
        }

        public int? WAApplicationId
        {
            get
            {
                return tWAApplicationId;
            }
            set
            {
                if (tWAApplicationId != value)
                {
                    UpdateApplication(value);
                }
            }
        }

        private void UpdateApplicant(int? value)
        {
            if (WAApplicant == null && tWAApplicantId != null)
            {
                var obj = db.WebApplicants.FirstOrDefault(f => f.ApplicantId == tWAApplicantId);
                tWAApplicantId = obj.ApplicantId;
                WAApplicant = obj;
                WAApplicationId = obj.CurrentApplication().ApplicationId;

            }

            if (tWAApplicantId != value)
            {
                var obj = db.WebApplicants.FirstOrDefault(f => f.ApplicantId == value);
                if (obj != null)
                {
                    tWAApplicantId = obj.ApplicantId;
                    WAApplicant = obj;
                    WAApplicationId = obj.CurrentApplication().ApplicationId;
                }
                else
                {
                    tWAApplicantId = null;
                    WAApplicant = null;
                }
            }

            if (WAApplicant != null)
            {
                var currentApp =  WAApplicant.CurrentApplication();
                if (currentApp != null)
                {
                    UpdateApplication(currentApp.ApplicationId);
                }

            }

        }

        private void UpdateApplication(int? value)
        {
            if (WAApplicant == null)
                return;

            if (WAApplicant == null)
            {
                tWAApplicationId = null;
                WAApplication = null;
                //return;
            }

            if (WAApplication == null && tWAApplicationId != null)
            {
                var obj = db.WebApplications.FirstOrDefault(f => f.ApplicantId == WAApplicantId && f.ApplicationId == tWAApplicationId);
                tWAApplicantId = obj.ApplicantId;
                tWAApplicationId = obj.ApplicationId;
                WAApplication = obj;
            }

            if (tWAApplicationId != value)
            {
                var obj = db.WebApplications.FirstOrDefault(f => f.ApplicantId == WAApplicantId && f.ApplicationId == value);
                if (obj != null)
                {
                    tWAApplicantId = obj.ApplicantId;
                    tWAApplicationId = obj.ApplicationId;
                    WAApplication = obj;


                }
                else
                {
                    tWAApplicationId = null;
                    WAApplication = null;
                }
            }

            if (WAApplication != null && ForCrew != null)
            {
                WAApplication.PRReportsToId ??= ForCrew.CrewLeaderID;
                WAApplication.WPDivisionId ??= ForCrew.CrewLeader?.DivisionId;
            }

        }

        public void UpdateStatusChange(int newValue)
        {
            if (WorkFlow == null)
                GenerateWorkFlow();

            var status = (DB.HRPositionRequestStatusEnum)newValue;
            if (newValue != StatusId || status == DB.HRPositionRequestStatusEnum.New)
            {
                switch (status)
                {
                    case DB.HRPositionRequestStatusEnum.New:
                        break;
                    case DB.HRPositionRequestStatusEnum.Submitted:
                        break;
                    case DB.HRPositionRequestStatusEnum.HRApproved:
                        break;
                    case DB.HRPositionRequestStatusEnum.ManagementReviewed:
                        break;
                    case DB.HRPositionRequestStatusEnum.HRReview:
                        break;
                    case DB.HRPositionRequestStatusEnum.Hire:
                        if (this.NewEmployeeId != null)
                            return;
                        else if (this.WAApplicant != null && this.WAApplication != null)
                        {
                            this.WAApplicant.CurrentApplication().HireDate ??= DateTime.Now.Date;
                            this.WAApplicant.CurrentApplication().CreateEmployee(this);
                        }
                        else
                        {
                            return;
                        }
                        break;
                    case DB.HRPositionRequestStatusEnum.Canceled:
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
        
        private void UpdateForCrew(string newCrewId)
        {

            if (ForCrew == null && tForCrewId != null)
            {
                var crew = db.Crews.FirstOrDefault(f => f.CrewId == tForCrewId);
                tForCrewId = crew.CrewId;
                PRCo = crew.PRCo;
                ForCrew = crew;
            }

            if (tForCrewId != newCrewId)
            {
                var crew = db.Crews.FirstOrDefault(f => f.CrewId == newCrewId);
                if (crew != null)
                {
                    tForCrewId = crew.CrewId;
                    PRCo = crew.PRCo;
                    ForCrew = crew;
                }
                else
                {
                    tForCrewId = null;
                    PRCo = null;
                    ForCrew = null;
                }
            }


            if (WAApplication != null && ForCrew != null)
            {
                WAApplication.PRReportsToId ??= ForCrew.CrewLeaderID;
                WAApplication.WPDivisionId ??= ForCrew.CrewLeader?.DivisionId;
            }
        }

        private void UpdateCreateCrew(bool value)
        {
            if (value)
            {
                if (PRCrewRequestId == null)
                {
                    var crewRequest = this.HRCompanyParm.HQCompanyParm.PRCompanyParm.AddCrewRequest();
                    crewRequest.WPDivisionId = this.WPDivisionId;

                    PRCrewRequestId = crewRequest.RequestId;
                    PRCrewRequest = crewRequest;
                }
                else
                {
                    PRCrewRequest.Status = DB.PRCrewRequestStatusEnum.New;
                }
            }
            else
            {
                if (PRCrewRequest != null)
                {
                    PRCrewRequest.Status = DB.PRCrewRequestStatusEnum.Canceled;
                }
            }

            tCreateCrew = value;

        }

        private void UpdatePriorEmployee(int? newVal)
        {
            var origEmployeeId = tPriorEmployeeId;


            if (PriorPREmployee == null && tPriorEmployeeId != null)
            {
                var employee = db.Employees.FirstOrDefault(f => f.EmployeeId == tPriorEmployeeId);
                tPriorEmployeeId = employee.EmployeeId;
                PRCo = employee.PRCo;
                PriorPREmployee = employee;
                WPDivisionId = employee.DivisionId;
            }

            if (tPriorEmployeeId != newVal)
            {
                var employee = db.Employees.FirstOrDefault(f => f.EmployeeId == newVal);
                if (employee != null)
                {
                    PRCo = employee.PRCo;
                    tPriorEmployeeId = employee.EmployeeId;
                    PriorPREmployee = employee;
                    WPDivisionId = employee.DivisionId;
                }
                else
                {
                    PRCo = null;
                    tPriorEmployeeId = null;
                    PriorPREmployee = null;
                    //WPDivisionId = employee.DivisionId;
                }

                if (PRCrewRequest != null && PriorPREmployee != null)
                {
                    PRCrewRequest.PriorCrewId = this.PriorPREmployee.CrewId;
                }
            }
        }

        private void UpdateNewEmployee(int? newVal)
        {
            var origEmployeeId = tNewEmployeeId;


            if (NewPREmployee == null && tNewEmployeeId != null)
            {
                var employee = db.Employees.FirstOrDefault(f => f.EmployeeId == tNewEmployeeId);
                tNewEmployeeId = employee.EmployeeId;
                PRCo = employee.PRCo;
                NewPREmployee = employee;
            }

            if (tNewEmployeeId != newVal)
            {
                var employee = db.Employees.FirstOrDefault(f => f.EmployeeId == newVal);
                if (employee != null)
                {
                    PRCo = employee.PRCo;
                    tNewEmployeeId = employee.EmployeeId;
                    NewPREmployee = employee;
                }
                else
                {
                    PRCo = null;
                    tNewEmployeeId = null;
                    NewPREmployee = null;
                }
            }
        }
        

        #region Workflow
        public void GenerateWorkFlow()
        {
            if (WorkFlow == null)
            {
                var workflow = new WorkFlow
                {
                    WFCo = HRCo,
                    WorkFlowId = WorkFlow.GetNextWorkFlowId(HRCo),
                    TableName = HRPositionRequest.BaseTableName,
                    Id = RequestId,
                    CreatedBy = RequestedBy,
                    CreatedOn = RequestedDate,
                    Active = true,

                    Company = HRCompanyParm.HQCompanyParm,
                };
                HRCompanyParm.HQCompanyParm.WorkFlows.Add(workflow);
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
                case DB.HRPositionRequestStatusEnum.New:
                    WorkFlow.AddUser(RequestedBy);
                    break;
                case DB.HRPositionRequestStatusEnum.Submitted:
                    WorkFlow.AddUsersByPosition("HR-MGR");
                    WorkFlow.AddUsersByPosition("IT-DIR");
                    EmailStatusUpdate();
                    break;
                case DB.HRPositionRequestStatusEnum.HRApproved:
                    WorkFlow.AddUser(RequestedBy);
                    if (this.PriorPREmployee != null && this.PriorPREmployee?.Supervisor != null)
                    {
                        WorkFlow.AddEmployee(this.PriorPREmployee?.Supervisor);
                        WorkFlow.AddEmployee(this.PriorPREmployee?.Supervisor?.Division?.DivisionManger);
                    }
                    WorkFlow.AddUsersByPosition("HR-MGR");
                    WorkFlow.AddUsersByPosition("IT-DIR");
                    break;
                case DB.HRPositionRequestStatusEnum.ManagementReviewed:
                    WorkFlow.AddUsersByPosition("HR-MGR");
                    WorkFlow.AddUsersByPosition("IT-DIR");
                    break;
                case DB.HRPositionRequestStatusEnum.Hire:
                    WorkFlow.AddUsersByPosition("HR-PRMGR");
                    WorkFlow.AddUsersByPosition("IT-DIR");
                    EmailStatusUpdate();
                    //WorkFlow.CompleteWorkFlow();
                    break;
                case DB.HRPositionRequestStatusEnum.PRReview:
                    WorkFlow.AddUsersByPosition("IT-DIR");
                    //WorkFlow.CompleteWorkFlow();
                    break;
                case DB.HRPositionRequestStatusEnum.AssetReview:
                    //WorkFlow.AddEmployee(this.NewPREmployee.Supervisor);
                    WorkFlow.CompleteWorkFlow();
                    break;
                case DB.HRPositionRequestStatusEnum.Canceled:
                    WorkFlow.CompleteWorkFlow();
                    break;
                case DB.HRPositionRequestStatusEnum.Complete:
                    WorkFlow.CompleteWorkFlow();
                    break;
                default:
                    break;
            }
        }
        #endregion

        public void EmailStatusUpdate()
        {
            //if (HttpContext.Current == null)
            //    return;

            //var result = new portal.Areas.HumanResource.Models.PositionRequest.RequestEmailViewModel(this);
            //var viewPath = "";
            //var subject = "";
            //using var db = new VPEntities();
            //switch (Status)
            //{
            //    case DB.HRPositionRequestStatusEnum.New:
            //        break;
            //    case DB.HRPositionRequestStatusEnum.Submitted:
            //        viewPath = "Email/Submit";
            //        subject = string.Format(AppCultureInfo.CInfo(), "Employee Position Requested By: {0} For: {1} ", result.RequestedUser, result.PositionName);
            //        break;
            //    case DB.HRPositionRequestStatusEnum.HRApproved:
            //        break;
            //    case DB.HRPositionRequestStatusEnum.Hire:
            //        viewPath = "Email/Hired";
            //        subject = string.Format(AppCultureInfo.CInfo(), "Employee Position Hired By: {0} For: {1} ", db.GetCurrenUser().FullName(), result.PositionName);
            //        break;
            //        break;
            //    case DB.HRPositionRequestStatusEnum.Canceled:
            //        break;
            //    default:
            //        break;
            //}

            //if (!string.IsNullOrEmpty(viewPath))
            //{
            //    try
            //    {
            //        using System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage()
            //        {
            //            Body = Code.EmailHelper.RenderViewToString("PositionRequest", "Index", "HumanResource", viewPath, result, false),
            //            IsBodyHtml = true,
            //            Subject = subject,
            //        };

            //        foreach (var workFlow in WorkFlow.CurrentSequance().AssignedUsers.ToList())
            //        {
            //            var user = db.WebUsers.FirstOrDefault(f => f.Id == workFlow.AssignedTo);
            //            msg.To.Add(new System.Net.Mail.MailAddress(user.Email));
            //        }

            //        if (RequestedUser != null && RequestedUser?.Email != StaticFunctions.GetCurrentUser().Email)
            //        {
            //            msg.CC.Add(new System.Net.Mail.MailAddress(StaticFunctions.GetCurrentUser().Email));
            //        }
            //        Code.EmailHelper.Send(msg);
            //    }
            //    catch (Exception)
            //    {
            //        //throw new Exception(ex.Message);
            //    }
            //}
        }

        public void UpdateStatusFromTermRequest(HRTermRequest request)
        {
            if (request == null)
                return;

            switch (request.Status)
            {
                case DB.HRTermRequestStatusEnum.New:
                    Status = DB.HRPositionRequestStatusEnum.New;
                    break;
                case DB.HRTermRequestStatusEnum.Submitted:
                    Status = DB.HRPositionRequestStatusEnum.Submitted;
                    break;
                case DB.HRTermRequestStatusEnum.Approved:
                    Status = DB.HRPositionRequestStatusEnum.HRApproved;
                    break;
                case DB.HRTermRequestStatusEnum.PRReview:
                    break;
                case DB.HRTermRequestStatusEnum.HRReview:
                    break;
                case DB.HRTermRequestStatusEnum.Completed:
                    //Status = DB.HRPositionRequestStatusEnum.Hire;
                    break;
                case DB.HRTermRequestStatusEnum.Canceled:
                    Status = DB.HRPositionRequestStatusEnum.Canceled;
                    break;
                default:
                    break;
            }
        }

        public HRPositionApplicant? AddApplication(WebApplication application)
        {
            if (application == null)
                return null;

            var posApplication = this.WAApplicants.FirstOrDefault(f => f.ApplicantId == application.ApplicantId && f.ApplicationId == application.ApplicationId);
            if (posApplication == null)
            {
                posApplication = new HRPositionApplicant()
                {
                    HRCo = this.HRCo,
                    RequestId = RequestId,
                    SeqId = 0,
                    ApplicantId = application.ApplicantId,
                    ApplicationId = application.ApplicationId,
                    Status = DB.HRPositionApplicantStatusEnum.Pending,

                    ApprovedBy = null,
                    ApprovedOn = null,

                    Request = this,
                    Application = application,
                };

                this.WAApplicants.Add(posApplication);
            }

            return posApplication;
        }

        
    }
}