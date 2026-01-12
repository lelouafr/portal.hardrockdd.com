using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.WF
{
    public class WorkFlowUserListViewModel
    {
        public WorkFlowUserListViewModel()
        {

        }

		public WorkFlowUserListViewModel(DB.Infrastructure.ViewPointDB.Data.WorkFlow workFlow)
		{

			if (workFlow == null)
			{
				List = new List<WorkFlowUserViewModel>();
				AddUser = new WorkFlowUserViewModel();
				return;
			}
            var seq = workFlow.CurrentSequence();
            if (seq == null)
			{
				List = new List<WorkFlowUserViewModel>();
				AddUser = new WorkFlowUserViewModel();
				return;
			}
			List = seq.AssignedUsers.Select(s => new WorkFlowUserViewModel(s)).ToList();
			AddUser = new WorkFlowUserViewModel(seq);

		}

		public WorkFlowUserListViewModel(DB.Infrastructure.ViewPointDB.Data.WorkFlowSequence wfSequence)
		{

			if (wfSequence == null)
			{
				List = new List<WorkFlowUserViewModel>();
				AddUser = new WorkFlowUserViewModel();

				return;
			}
			List = wfSequence.AssignedUsers.Select(s => new WorkFlowUserViewModel(s)).ToList();
			AddUser = new WorkFlowUserViewModel(wfSequence);

		}

		public WorkFlowUserViewModel AddUser { get; set; }

        public List<WorkFlowUserViewModel> List { get; }
    }

    public class WorkFlowUserViewModel
    {
        public WorkFlowUserViewModel()
        {

        }

        public WorkFlowUserViewModel(DB.Infrastructure.ViewPointDB.Data.WorkFlowSequence wfSequence)
        {
            WFCo = wfSequence.WFCo;
            WorkFlowId = wfSequence.WorkFlowId;
            SeqId = wfSequence.SeqId;
        }

        public WorkFlowUserViewModel(DB.Infrastructure.ViewPointDB.Data.WorkFlowUser user)
        {
            WFCo = user.WFCo;
            WorkFlowId = user.WorkFlowId;
            SeqId = user.SeqId;
            AssignedTo = user.AssignedTo;
            AssignedUser = user.AssignedUser.FullName();
            AssignedOn = user.AssignedOn;

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
        [Display(Name = "Assigned To")]
        [Field(LabelSize = 2, TextSize = 12, ComboUrl = "/WebUsers/Combo", ComboForeignKeys = "")]
        public string AssignedUser { get; set; }

        [UIHint("DateBox")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "General", FormGroupRow = 1, Placeholder = "")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date")]
        public System.DateTime AssignedOn { get; set; }
    }
}