//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace portal.Models.Views.Bid
//{
//    public class BidWorkFlowListViewModel
//    {
//        public BidWorkFlowListViewModel()
//        {

//        }

//        public BidWorkFlowListViewModel(DB.Infrastructure.ViewPointDB.Data.Bid bid)
//        {
//            if (bid == null)
//            {
//                throw new System.ArgumentNullException(nameof(bid));
//            }
//            #region mapping
//            Co = bid.Co;
//            BidId = bid.BidId;
//            #endregion
//            var workFlows = bid.WorkFlows.GroupBy(g => new { g.Status, g.StatusDate }).Select(s => new { s.Key.StatusDate, WorkFlow = s.FirstOrDefault() }). OrderByDescending(o => o.StatusDate).ToList();
//            List = workFlows.Select(s => new BidWorkFlowViewModel(s.WorkFlow)).ToList();
//        }

//        [Key]
//        [HiddenInput]
//        [Required]
//        [Display(Name = "Co")]
//        public byte Co { get; set; }

//        [Key]
//        [HiddenInput]
//        [Required]
//        [Display(Name = "Bid Id")]
//        public int BidId { get; set; }

//        public List<BidWorkFlowViewModel> List { get; }
//    }

//    public class BidWorkFlowViewModel
//    {
//        public BidWorkFlowViewModel()
//        {

//        }
//        public BidWorkFlowViewModel(DB.Infrastructure.ViewPointDB.Data.BidWorkFlowAssignment workflow)
//        {
//            if (workflow == null)
//            {
//                throw new System.ArgumentNullException(nameof(workflow));
//            }
//            #region mapping
//            Co = workflow.Co;
//            BidId = workflow.BidId;
//            Status = (DB.BidStatusEnum)(workflow.Status);
//            Comments = workflow.Comments;
//            // AssignedUser= workflow.AssignedTo

//            DateString = workflow.StatusDate.Date == DateTime.Now.Date ? "Today" :
//                         workflow.StatusDate.Date == DateTime.Now.Date.AddDays(-1) ? "Yesterday" : 
//                         workflow.StatusDate.Date.ToShortDateString();

//            TimeString = workflow.StatusDate.ToShortTimeString();

//            var list = workflow.Bid.WorkFlows.Where(f => f.StatusDate == workflow.StatusDate).ToList();
//            AssignedUsers = list.Select(s => new Web.WebUserViewModel(s.AssignedUser)).ToList();
//            var completedWorkflow = list.Where(f => f.CompletedOn != null).FirstOrDefault();
//            if (completedWorkflow?.AssignedUser != null)
//            {
//                Comments = completedWorkflow.Comments;
//                CompletedUser = new Web.WebUserViewModel(completedWorkflow.AssignedUser);
//            }
//            #endregion
//        }

//        [Key]
//        [HiddenInput]
//        [Required]
//        [Display(Name = "Co")]
//        public byte Co { get; set; }

//        [Key]
//        [HiddenInput]
//        [Required]
//        [Display(Name = "Bid Id")]
//        public int BidId { get; set; }

//        [UIHint("EnumBox")]
//        [Field()]
//        [Display(Name = "Status")]
//        public DB.BidStatusEnum Status { get; set; }
        
//        [UIHint("DateBox")]
//        [Field()]
//        [DataType(DataType.Date)]
//        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
//        [Display(Name = "Date")]
//        public DateTime StatusDate { get; set; }

//        [Display(Name = "Comments")]
//        [UIHint("TextAreaBox")]
//        [Field()]
//        public string Comments { get; set; }

//        [ReadOnly(true)]
//        [UIHint("WebUserBox")]
//        [Display(Name = "Completed User")]
//        [Field()]
//        public Web.WebUserViewModel CompletedUser { get; set; }

//        [ReadOnly(true)]
//        [UIHint("WebUserBox")]
//        [Display(Name = "AssignedUsers")]
//        [Field()]
//        public List<Web.WebUserViewModel> AssignedUsers { get; }

//        [UIHint("DateBox")]
//        [Field()]
//        [DataType(DataType.Date)]
//        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
//        [Display(Name = "Completed Date")]
//        public DateTime CompletedDate { get; set; }

//        [Required]
//        [Display(Name = "Date")]
//        [UIHint("TextBox")]
//        [Field()]
//        public string DateString { get; set; }

//        [Required]
//        [Display(Name = "Time")]
//        [UIHint("TextBox")]
//        [Field()]
//        public string TimeString { get; set; }
//    }
//    public class BidWorkFlowSubmitViewModel
//    {
//        public BidWorkFlowSubmitViewModel()
//        {

//        }
//        public BidWorkFlowSubmitViewModel(DB.Infrastructure.ViewPointDB.Data.BidWorkFlowAssignment workflow)
//        {
//            if (workflow == null)
//            {
//                throw new System.ArgumentNullException(nameof(workflow));
//            }
//            #region mapping
//            Co = workflow.Co;
//            BidId = workflow.BidId;
//            LineId = workflow.LineId;
//            Status = (DB.BidStatusEnum)(workflow.Status);
//            Comments = workflow.Comments;
//            #endregion
//        }

//        [Key]
//        [HiddenInput]
//        [Required]
//        [Display(Name = "Co")]
//        public byte Co { get; set; }

//        [Key]
//        [HiddenInput]
//        [Required]
//        [Display(Name = "Bid Id")]
//        public int BidId { get; set; }

//        [Key]
//        [HiddenInput]
//        [Required]
//        [Display(Name = "Line Id")]
//        public int LineId { get; set; }
        
//        [HiddenInput]
//        [Display(Name = "Status")]
//        public DB.BidStatusEnum Status { get; set; }

//        [Display(Name = "Comments")]
//        [UIHint("TextAreaBox")]
//        [Field(LabelSize = 2, TextSize = 10, FormGroup = "Project Info", FormGroupRow = 6)]
//        public string Comments { get; set; }
//    }
//}