using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.SM.Request.Equipment.Forms.Line
{
    public class AssignmentListViewModel
    {
        public AssignmentListViewModel()
        {

        }

        public AssignmentListViewModel(DB.Infrastructure.ViewPointDB.Data.SMRequestLine requestLine)
        {
            if (requestLine == null) 
                throw new System.ArgumentNullException(nameof(requestLine));

            SMCo = requestLine.SMCo;
            RequestId = requestLine.RequestId;
            LineId = requestLine.LineId;
            if (requestLine.WorkFlow != null)
            {
                List = requestLine.WorkFlow.CurrentSequence().AssignedUsers.Select(s => new AssignmentViewModel(s)).ToList();

            }
            else
            {
                List = new List<AssignmentViewModel>();
            }
        }

        [Key]
        [HiddenInput]
        public byte SMCo { get; set; }

        [Key]
        [HiddenInput]
        public int RequestId { get; set; }

        [Key]
        [HiddenInput]
        public int LineId { get; set; }


        public List<AssignmentViewModel> List { get;  }
    }
    public class AssignmentViewModel
    {

        public AssignmentViewModel()
        {

        }

        public AssignmentViewModel(WorkFlowUser user)
        {
            if (user == null) 
                throw new System.ArgumentNullException(nameof(user));

            WFCo = user.WFCo;
            WorkFlowId = user.WorkFlowId;
            SeqId = user.SeqId;
            AssignedTo = user.AssignedTo;

            EmployeeId = user.AssignedUser.PREmployee.EmployeeId;
        }

        [Key]
        [HiddenInput]
        public byte WFCo { get; set; }

        [Key]
        [HiddenInput]
        public int WorkFlowId { get; set; }

        [Key]
        [HiddenInput]
        public int SeqId { get; set; }

        [Key]
        [HiddenInput]
        public string AssignedTo { get; set; }


        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PRCombo/Combo", ComboForeignKeys = "PRCo")]
        public int EmployeeId { get; set; }

    }
}