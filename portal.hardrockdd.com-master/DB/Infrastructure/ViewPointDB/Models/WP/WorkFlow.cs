using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class WorkFlow
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
            using var db = new VPContext();
            return db.GetNextId(BaseTableName, 1);
        }

        public static int GetNextWorkFlowId(byte co, VPContext db)
        {
            var outParm = new System.Data.Entity.Core.Objects.ObjectParameter("workFlowId", typeof(int));

            var result = db.udNextWorkFlowId(co, outParm);

            return (int)outParm.Value;
        }

        public void CompleteSequence(string userId = null)
        {
            var sequences = this.Sequences.Where(w => w.Active == true).ToList();
            userId ??= db.CurrentUserId;
            var user = db.WebUsers.FirstOrDefault(f => f.Id == userId);
            foreach (var seq in sequences)
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
            userId ??= db.CurrentUserId;
            CompleteSequence(userId);
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
            var result = CurrentSequence()?.AssignedUsers.Any(f => f.AssignedTo == userId) ?? false;
            //var users = CurrentSequence()?.AssignedUsers.ToList();
            return result;
        }

        public WorkFlowSequence AddSequence(int statusId)
        {
            this.Sequences.ToList().ForEach(e => e.Active = false);
            var user = db.GetCurrentUser();
            if (user.Id == null)
				user = db.WebUsers.First(f => f.Id == "System");

			var sequence = new WorkFlowSequence
            {
                WFCo = WFCo,
                WorkFlowId = WorkFlowId,
                SeqId = Sequences.DefaultIfEmpty().Max(f => f == null ? 0 : f.SeqId) + 1,
                Status = statusId,
                Active = true,
                StatusDate = DateTime.Now,
                CreatedBy = user.Id,

                CreatedUser = user,
                WorkFlow = this,
                //db = this.db,
                
            };
            this.Sequences.Add(sequence);
            return sequence;

        }

        public WorkFlowSequence CreateSequence(byte statusId, bool copyUsers = false)
        {
            return CreateSequence((int)statusId, copyUsers);
        }

        public WorkFlowSequence CreateSequence(int statusId, bool copyUsers = false)
        {
            var currentSequence = CurrentSequence();

            if (currentSequence == null)
            {
                currentSequence = AddSequence(statusId);
            }

            if (currentSequence.Status != statusId || currentSequence.Active == false)
            {
               
                CompleteSequence();
                currentSequence = AddSequence(statusId);
                if (copyUsers)
                {
                    var users = currentSequence.AssignedUsers.ToList();
                    foreach (var user in users)
                    {
                        AddUser(user.AssignedUser);
                    }
                }
            }

            return CurrentSequence();
        }

        public WorkFlowSequence CurrentSequence()
        {
            var currentSequence = this.Sequences.FirstOrDefault(f => f.Active == true);
            if (currentSequence == null)
                currentSequence = this.Sequences.OrderByDescending(o => o.SeqId).FirstOrDefault();
            return currentSequence;
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
            var sequences = this.Sequences.Where(w => w.Active == true).ToList();
            if (!sequences.Any())
            {
                sequences.Add(this.CurrentSequence());
            }
            foreach (var seq in sequences)
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
           // using var db = new VPContext();

            var user = db.WebUsers.FirstOrDefault(f => f.Email.ToLower() == email.ToLower());
            AddUser(user);
        }

        public void AddEmployee(Employee employee, bool includeSupervisor = false)
        {
            if (employee == null)
                return;

            AddUser(employee.Resource.FirstOrDefault().WebUser);
            if (includeSupervisor)
                AddEmployee(employee.Supervisor, false);
        }


        public void AddUsersByPosition(string positionCode)
        {
            //using var db = new VPContext();
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
        public WorkFlow WorkFlow { get; }

        public WorkFlow GetWorkFlow();

        public WorkFlow AddWorkFlow();

        public void AddWorkFlowAssignments(bool reset = false);

    }
}