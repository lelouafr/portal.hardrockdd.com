using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class WorkFlow
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
                        _db = this.Company?.db;

                    if (_db == null)
                        throw new NullReferenceException("GetDbContextFromEntity is null");

                }
                return _db;
            }
        }
        public static string BaseTableName { get { return "budWFFH"; } }

        public static int GetNextWorkFlowId(byte co)
        {
            using var db = new VPEntities();
            return db.GetNextId(BaseTableName, 1);
        }

        public static int GetNextWorkFlowId(byte co, VPEntities db)
        {
            var outParm = new System.Data.Entity.Core.Objects.ObjectParameter("workFlowId", typeof(int));

            var result = db.udNextWorkFlowId(co, outParm);

            return (int)outParm.Value;
        }

        public void CompleteSequance(string userId = null)
        {
            var sequances = this.Sequances.Where(w => w.Active == true).ToList();
            userId ??= db.CurrentUserId;
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            foreach (var seq in sequances)
            {
                seq.Active = false;
                seq.CompletedBy = userId;
                seq.CompletedOn = DateTime.Now;
                seq.AssignedUsers.ToList().ForEach(e => e.CompletedOn = DateTime.Now);
                seq.CompletedUser = user;
            }
        }

        public void CompleteWorkFlow(string userId = null)
        {
            userId ??= StaticFunctions.GetUserId();
            CompleteSequance(userId);
            Active = false;
            CompletedOn = DateTime.Now;
        }

        public void CompleteWorkFlow(Employee employee)
        {
            if (employee == null)
            {
                CompleteWorkFlow();
                return;
            }
            var empUserId = employee.Resource.FirstOrDefault().WebUser.Id;
            CompleteWorkFlow(empUserId);
        }

        public bool IsUserInWorkFlow(string userId)
        {
            var result = CurrentSequance()?.AssignedUsers.Any(f => f.AssignedTo == userId) ?? false;
            
            return result;
        }

        public WorkFlowSequance AddSequance(int statusId)
        {
            this.Sequances.ToList().ForEach(e => e.Active = false);
            var sequance = new WorkFlowSequance
            {
                WFCo = WFCo,
                WorkFlowId = WorkFlowId,
                SeqId = Sequances.DefaultIfEmpty().Max(f => f == null ? 0 : f.SeqId) + 1,
                Status = statusId,
                Active = true,
                StatusDate = DateTime.Now,
                CreatedBy = db.CurrentUserId,

                CreatedUser = db.GetCurrenUser(),
                WorkFlow = this,
                
            };
            this.Sequances.Add(sequance);
            return sequance;

        }

        public WorkFlowSequance CreateSequance(byte statusId, bool copyUsers = false)
        {
            return CreateSequance((int)statusId, copyUsers);
        }

        public WorkFlowSequance CreateSequance(int statusId, bool copyUsers = false)
        {
            var currentSequance = CurrentSequance();

            if (currentSequance == null)
            {
                currentSequance = AddSequance(statusId);
            }

            if (currentSequance.Status != statusId || currentSequance.Active == false)
            {
               
                CompleteSequance();
                currentSequance = AddSequance(statusId);
                if (copyUsers)
                {
                    var users = currentSequance.AssignedUsers.ToList();
                    foreach (var user in users)
                    {
                        AddUser(user.AssignedUser);
                    }
                }
            }

            return CurrentSequance();
        }

        public WorkFlowSequance CurrentSequance()
        {
            var currentSequance = this.Sequances.FirstOrDefault(f => f.Active == true);
            if (currentSequance == null)
            {
                currentSequance = this.Sequances.OrderByDescending(o => o.CompletedOn).FirstOrDefault();
            }
            return currentSequance;
        }

        public void AddUser(string userId)
        {
            var webUser = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            AddUser(webUser);
        }

        public void AddUser(WebUser webUser)
        {
            if (webUser == null)
                return;
            var sequances = this.Sequances.Where(w => w.Active == true).ToList();
            foreach (var seq in sequances)
            {
                if (!seq.AssignedUsers.Any(f => f.AssignedTo == webUser.Id))
                {
                    var user = new WorkFlowUser
                    {
                        WFCo = WFCo,
                        WorkFlowId = WorkFlowId,
                        SeqId = seq.SeqId,
                        AssignedTo = webUser.Id,
                        AssignedOn = seq.StatusDate,

                        AssignedUser = webUser,
                    };
                    seq.AssignedUsers.Add(user);
                }
            }
        }

        public void AddByEmail(string email)
        {
           // using var db = new VPEntities();

            var user = db.WebUsers.FirstOrDefault(f => f.Email.ToLower() == email.ToLower());
            AddUser(user);
        }

        public void AddEmployee(Employee employee, bool includeSupervisor = false)
        {
            if (employee == null)
                return;

            AddUser(employee.Resource.FirstOrDefault().WebUser);
            if (includeSupervisor)
            {
                AddEmployee(employee.Supervisor, false);
            }
        }

        public void AddUsersByPosition(string positionCode)
        {
            //using var db = new VPEntities();
            var position = db.HRPositions.FirstOrDefault(f => f.PositionCodeId == positionCode);
            if (position != null)
            {
                foreach (var emp in position.Resources.Where(f => f.ActiveYN == "Y" && f.PortalAccountActive == "Y" && f.WebUser != null))
                {
                    AddUser(emp.WebUser);
                }

            }
        }
    }

    public interface IWorkFlow
    {
        public WorkFlow GetWorkFlow();

        public WorkFlow AddWorkFlow();

        public void AddWorkFlowAssignments(bool reset = false);

    }
}