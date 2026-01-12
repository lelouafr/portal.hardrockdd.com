using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.PM.Project.Form
{
    public class ProjectInfoViewModel : AuditBaseViewModel
    {
        public ProjectInfoViewModel()
        {

        }

        public ProjectInfoViewModel(DB.Infrastructure.ViewPointDB.Data.Job job) : base(job)
        {
            if (job == null)
            {
                throw new System.ArgumentNullException(nameof(job));
            }

            JCCo = job.JCCo;
            JobId = job.JobId;
            StatusId = job.Status;
            Description = job.Description;
            CustGroupId = job.Contract?.CustGroupId;
            CustomerId = job.Contract?.CustomerId;
            ProjectOwner = job.Owner;
            CrewId = job.CrewId;
            PipeSize = job.PipeSize;
            DivisionId = (int)(job.DivisionId ?? 0);
            EndMarketId = job.EndMarket;

            StartDate = job.StartDate;

        }

        [Key]
        [HiddenInput]
        public byte JCCo { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "Job")]
        public string JobId { get; set; }

        [Display(Name = "Status")]
        [UIHint("EnumBox")]
        public DB.JCJobStatusEnum? StatusId { get; set; }

        public byte? CustGroupId { get; set; }

        [Display(Name = "Customer")]
        [UIHint("DropDownBox")]
        [Field(ComboUrl = "/ARCombo/CustomerCombo", ComboForeignKeys = "CustGroupId")]
        public int? CustomerId { get; set; }

        [Display(Name = "Progress")]
        [UIHint("PercentBox")]
        public decimal? Progress { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 0, TextSize = 6)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Project Owner")]
        [UIHint("TextBox")]
        public string ProjectOwner { get; set; }

        [Display(Name = "Crew")]
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/Crew/Combo", ComboForeignKeys = "PRCo")]
        public string CrewId { get; set; }

        [Display(Name = "PipeSize")]
        [UIHint("LongBox")]
        public decimal? PipeSize { get; set; }

        [Display(Name = "Division")]
        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/Division/Combo", ComboForeignKeys = "PMCo")]
        public int DivisionId { get; set; }

        [Display(Name = "End Market")]
        [UIHint("TextBox")]
        public string EndMarketId { get; set; }

        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        internal ProjectInfoViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            var updObj = db.Jobs.FirstOrDefault(f => f.JCCo == this.JCCo && f.JobId == this.JobId);

            if (updObj != null)
            {
                //updObj.Description = this.Description;
                //updObj.StatusId = this.StatusId;
                //updObj.EndMarket = this.EndMarketId;
                //updObj.DivisionId = (byte)this.DivisionId;

                //updObj.CrewId = this.CrewId;
                //updObj.PipeSize = this.PipeSize;
                //updObj.Owner = this.ProjectOwner;

                updObj.StartDate = StartDate;
                if (updObj.ProjectGanttTask != null)
                {
                    updObj.ProjectGanttTask.StartDate = updObj.CalculatedStartDate();
                    //JobProgressRepository.UpdateJobPhaseTracks(updObj, db);
                }
                //Todo : Update status of parent, Update division information
                try
                {
                    db.BulkSaveChanges();
                    return new ProjectInfoViewModel(updObj);
                }
                catch (Exception ex)
                {
                    modelState.AddModelError("", ex.Message);
                    return this;
                }
            }
            else
            {
                modelState.AddModelError("", "Object Doesn't Exist For Update!");
                return this;
            }
        }

    }
}