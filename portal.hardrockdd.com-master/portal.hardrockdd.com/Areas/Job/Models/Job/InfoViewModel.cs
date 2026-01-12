using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.Job.Models.Job
{
    public class InfoViewModel
    {
        public InfoViewModel()
        {

        }

        public InfoViewModel(DB.Infrastructure.ViewPointDB.Data.Job job)
        {
            if (job == null)
                return;

            JCCo = job.JCCo;
            PRCo = job.PRCo ?? job.JCCompanyParm.HQCompanyParm.PRCo;
            EMCo = job.EMCo ?? job.JCCompanyParm.HQCompanyParm.EMCo;

            JobId = job.JobId;
            Description = job.Description;
            ProjectOwner = job.Owner;
            CrewId = job.CrewId;
            RigId = job.RigId;
            PipeSize = job.PipeSize;
            Footage = job.Footage;
            DivisionId = (int)(job.DivisionId ?? 0);
            EndMarketId = job.EndMarket;
            StatusId = job.StatusId;
            BidId = job.Bid()?.BidId;

            HasAttachment = job.HasAttachments();

        }

        [Key]
        [HiddenInput]
        public byte JCCo { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Job")]
        public string JobId { get; set; }

        public byte? PRCo { get; set; }

        public byte? EMCo { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 0, TextSize = 6)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Status")]
        [UIHint("DropdownBox")]
        [Field( ComboUrl = "/JCCombo/StatusCombo", ComboForeignKeys = "")]
        public string StatusId { get; set; }

        [Display(Name = "Project Owner")]
        [UIHint("TextBox")]
        public string ProjectOwner { get; set; }

        [Display(Name = "Crew")]
        [UIHint("DropdownBox")]
        [Field( ComboUrl = "/PRCombo/CrewCombo", ComboForeignKeys = "PRCo")]
        public string CrewId { get; set; }

        [Display(Name = "Rig")]
        [UIHint("DropdownBox")]
        [Field( ComboUrl = "/EMCombo/Combo", ComboForeignKeys = "EMCo")]
        public string RigId { get; set; }

        [Display(Name = "Footage")]
        [UIHint("LongBox")]
        [Field()]
        public short? Footage { get; set; }

        [Display(Name = "PipeSize")]
        [UIHint("LongBox")]
        [Field()]
        public byte? PipeSize { get; set; }

        [Display(Name = "Division")]
        [UIHint("DropdownBox")]
        [Field( ComboUrl = "/Division/Combo", ComboForeignKeys = "PMCo=JCCo")]
        public int DivisionId { get; set; }

        [Display(Name = "End Market")]
        [UIHint("TextBox")]
        public string EndMarketId { get; set; }

        [ReadOnly(true)]
        [Display(Name = "Bid Id")]
        [UIHint("LongBox")]
        [Field(InfoUrl = "/BidForm/BidPopupForm", InfoForeignKeys = "BDCo='1',BidId")]
        public int? BidId { get; set; }

        [HiddenInput]
        public bool HasAttachment { get; set; }


        internal InfoViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            var updObj = db.Jobs.FirstOrDefault(f => f.JCCo == this.JCCo && f.JobId == this.JobId);

            if (updObj != null)
            {
                updObj.Description = this.Description;
                updObj.StatusId = this.StatusId;
                updObj.EndMarket = this.EndMarketId;
                updObj.DivisionId = (byte)this.DivisionId;
                updObj.DivisionDesc = db.ProjectDivisions.FirstOrDefault(f => f.PMCo == this.JCCo && f.DivisionId == this.DivisionId)?.Description;

                updObj.CrewId = this.CrewId;
                updObj.RigId = this.RigId;
                updObj.PipeSize = this.PipeSize;
                updObj.Footage = this.Footage;
                updObj.Owner = this.ProjectOwner;

                //Todo : Update status of parent, Update division information
                try
                {
                    db.BulkSaveChanges();
                    return new InfoViewModel(updObj);
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