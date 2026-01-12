using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.PM.Project.New
{
    public class ProjectViewModel
    {
        public ProjectViewModel()
        {

        }

        public ProjectViewModel(DB.Infrastructure.ViewPointDB.Data.PMTempJob tempJob)
        {
            PMCo = tempJob.PMCo;
            Id = tempJob.Id;
            Description = tempJob.Description;
            StartDate = tempJob.StartDate;
            OwnerId = tempJob.OwnerId;
            PipeSize = tempJob.PipeSize;
            Footage = tempJob.Footage;
            RigCategoryId = tempJob.RigCategoryId;
            EndMarketId = tempJob.EndMarketId;
            DivisionId = tempJob.DivisionId;
            IntersectJobId = tempJob.IntersectJobId;
            ParentId = tempJob.ParentId;
            ProjectMgrId = tempJob.ProjectMgrId;
            JobId = tempJob.JobId;
            CrewId = tempJob.CrewId;
        }

        [Key]
        public byte PMCo { get; set; }
        [Key]
        public int Id { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 0, TextSize = 7)]
        [Display(Name = "Description")]
        public string Description { get; set; }


        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }


        [HiddenInput]
        public string FirmType { get; set; }

        [Required]
        [Display(Name = "Owner")]
        [UIHint("DropdownBox")]
        [Field(FormGroup = "Project Info", FormGroupRow = 3, Placeholder = "Select Owner", ComboUrl = "/Firm/OwnerCombo", ComboForeignKeys = "vendGroupId", AddUrl = "/Firm/Add", AddForeignKeys = "PMCo, FirmType", EditUrl = "/Firm/Form", EditForeignKeys = "PMCo, FirmNumber")]
        public int? OwnerId { get; set; }

        [Display(Name = "Pipe Size")]
        [UIHint("IntegerBox")]
        [TableField(Width = "15")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public byte? PipeSize { get; set; }

        [Required]
        [Display(Name = "Length")]
        [UIHint("IntegerBox")]
        [TableField(Width = "15")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public short? Footage { get; set; }

        [Display(Name = "Rig Type")]
        [UIHint("DropdownBox")]
        [TableField(Width = "15")]
        [Field(Placeholder = "Select Size", ComboUrl = "/EMCombo/RigCatCombo", ComboForeignKeys = "EMCo")]
        public string RigCategoryId { get; set; }

        public int? EndMarketId { get; set; }

        [Required]
        [Display(Name = "Division")]
        [UIHint("DropdownBox")]
        [Field(FormGroup = "Project Info", FormGroupRow = 3, Placeholder = "Select Division", ComboUrl = "/Division/Combo", ComboForeignKeys = "PMCo")]
        public byte? DivisionId { get; set; }

        public int? IntersectJobId { get; set; }

        public int? ParentId { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "Project Mgr")]
        [Field(LabelSize = 2, TextSize = 10, ComboUrl = "/PRCombo/ProjectMangerCombo", ComboForeignKeys = "PMCo", InfoUrl = "/HumanResource/Resource/PopupForm", InfoForeignKeys = "HRCo=PMCo, EmployeeId=ProjectMgrId")]
        public int? ProjectMgrId { get; set; }

        public string JobId { get; set; }

        public string CrewId { get; set; }

    }
}