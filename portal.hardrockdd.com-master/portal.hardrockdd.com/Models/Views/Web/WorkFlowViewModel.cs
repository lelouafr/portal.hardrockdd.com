//using DB.Infrastructure.ViewPointDB.Data;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace portal.Models.Views.Web
//{
//    public class WorkFlowListViewModel
//    {
//        public WorkFlowListViewModel()
//        {

//        }

//        public WorkFlowListViewModel(dynamic entity, string tableName)
//        {
//            if (entity == null)
//            {
//                throw new System.ArgumentNullException(nameof(entity));
//            }
//            var workFlows = (List<WorkFlow>)entity.WorkFlows.ToList();
//            var active = workFlows.FirstOrDefault(f => f.Active && f.TableName == TableName);

//            List = workFlows.Select(s => new WorkFlowViewModel(s)).ToList();
//            #region mapping
//            Co = active.Co;
//            Id = active.WorkFlowId;
//            TableName = active.TableName;
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
//        [Display(Name = "Id")]
//        public int Id { get; set; }

//        [Key]
//        [HiddenInput]
//        [Required]
//        [Display(Name = "TableName")]
//        public string TableName { get; set; }

//        public List<WorkFlowViewModel> List { get; }
//    }

//    public class WorkFlowViewModel
//    {
//        public WorkFlowViewModel()
//        {

//        }
//        public WorkFlowViewModel(DB.Infrastructure.ViewPointDB.Data.WorkFlow workFlow)
//        {
//            if (workFlow == null)
//            {
//                throw new System.ArgumentNullException(nameof(workFlow));
//            }
//            #region mapping
//            Co = workFlow.Co;
//            WorkFlowId = workFlow.WorkFlowId;
//            TableName = workFlow.TableName;
//            Id = workFlow.Id;
//            Status = workFlow.Status;
//            Active = workFlow.Active;

//            DateString = workFlow.StatusDate.Date == DateTime.Now.Date ? "Today" :
//                         workFlow.StatusDate.Date == DateTime.Now.Date.AddDays(-1) ? "Yesterday" :
//                         workFlow.StatusDate.Date.ToShortDateString();

//            TimeString = workFlow.StatusDate.ToShortTimeString();

//            AssignedUsers = workFlow.AsssignedUsers.Select(s => new Web.WebUserViewModel(s.AssignedUser)).ToList();
//            var completedWorkflow = workFlow.AsssignedUsers.Where(f => f.CompletedOn != null).FirstOrDefault();
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
//        [Display(Name = "Id")]
//        public int WorkFlowId { get; set; }

//        [HiddenInput]
//        [Required]
//        [Display(Name = "TableName")]
//        public string TableName { get; set; }

//        [HiddenInput]
//        [Required]
//        [Display(Name = "Id")]
//        public int Id { get; set; }

//        [Display(Name = "Active")]
//        public bool Active { get; set; }

//        [UIHint("EnumBox")]
//        [Display(Name = "Status")]
//        public int Status { get; set; }
        
//        [UIHint("DateBox")]
//        [DataType(DataType.Date)]
//        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
//        [Display(Name = "Date")]
//        public DateTime StatusDate { get; set; }

//        [Display(Name = "Comments")]
//        [UIHint("TextAreaBox")]
//        public string Comments { get; set; }

//        [ReadOnly(true)]
//        [UIHint("WebUserBox")]
//        [Display(Name = "Completed User")]
//        public Web.WebUserViewModel CompletedUser { get; set; }

//        [UIHint("DateBox")]
//        [DataType(DataType.Date)]
//        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
//        [Display(Name = "Completed Date")]
//        public DateTime CompletedDate { get; set; }

//        [Required]
//        [Display(Name = "Date")]
//        [UIHint("TextBox")]
//        public string DateString { get; set; }

//        [Required]
//        [Display(Name = "Time")]
//        [UIHint("TextBox")]
//        public string TimeString { get; set; }

//        [ReadOnly(true)]
//        [UIHint("WebUserBox")]
//        [Display(Name = "AssignedUsers")]
//        public List<Web.WebUserViewModel> AssignedUsers { get; }
//    }

//}