using DB.Infrastructure.ViewPointDB.Data;
using portal.Models;
using portal.Models.Views.DailyTicket;
using portal.Models.Views.DailyTicket.Form;
using portal.Models.Views.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.DailyTicket.Equipment
{
    public class DailyEquipmentUsageListViewModel
    {       

        public DailyEquipmentUsageListViewModel()
        {
            List = new List<DailyEquipmentUsageViewModel>();
        }

        public DailyEquipmentUsageListViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company, int weekId, VPContext db)
        {
            if (company == null) throw new System.ArgumentNullException(nameof(company));
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            DTCo = company.HQCo;
            WeekId = weekId;

            List = db.DailyEquipmentUsages.Where(f => f.DTCo == company.HQCo &&
                                                      f.Calendar.Week == weekId)
                                          .AsEnumerable()
                                          .Select(s => new DailyEquipmentUsageViewModel(s))
                                          .ToList();
        }

        public DailyEquipmentUsageListViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company, int weekId, string equipmentId,  VPContext db)
        {
            if (db == null) throw new System.ArgumentNullException(nameof(db));
            if (company == null) throw new System.ArgumentNullException(nameof(company));

            DTCo = company.HQCo;
            WeekId = weekId;
            EquipmentId = equipmentId;

            List = db.DailyEquipmentUsages.Where(f => f.DTCo == company.HQCo &&
                                                      f.Calendar.Week == weekId &&
                                                      f.EquipmentId == equipmentId )
                                          .AsEnumerable()
                                          .Select(s => new DailyEquipmentUsageViewModel(s))
                                          .ToList();
        }

        public DailyEquipmentUsageListViewModel(byte dtco, int weekId, string equipmentId, VPContext db)
        {
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            DTCo = dtco;
            WeekId = weekId;
            EquipmentId = equipmentId;

            List = db.DailyEquipmentUsages.Where(f => f.DTCo == dtco &&
                                                      f.Calendar.Week == weekId &&
                                                      f.EquipmentId == equipmentId)
                                          .AsEnumerable()
                                          .Select(s => new DailyEquipmentUsageViewModel(s))
                                          .ToList();
        }


        [Key]
        [HiddenInput]
        [Required]
        [Display(Name = "Co")]
        public byte DTCo { get; set; }

        [Required]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 3, TextSize = 4, ComboUrl = "/Calendar/WeekCombo", ComboForeignKeys = "")]
        [Display(Name = "Week")]
        public int WeekId { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 3, TextSize = 4, ComboUrl = "/EMCombo/Combo", ComboForeignKeys = "EMCo")]
        [Display(Name = "Equipment Id")]
        public string EquipmentId { get; set; }

        public List<DailyEquipmentUsageViewModel> List { get; }
    }

    public class DailyEquipmentUsageViewModel
    {

        public DailyEquipmentUsageViewModel()
        {

        }


        public DailyEquipmentUsageViewModel(DailyEquipmentUsage entity)
        {
            if (entity == null)
            {
                throw new System.ArgumentNullException(nameof(entity));
            }
            ContainsErrors = false;
            ErrorMsg = string.Empty;
            DTCo = entity.DTCo;
            TicketId = entity.TicketId;
            ActualDate = (DateTime)entity.ActualDate;
            SeqId = entity.SeqId;
            BatchTransType = entity.BatchTransType;

            EMCo = entity.EMCo ?? entity.Company.EMCo;
            EquipmentId = entity.EquipmentId;
            EquipmentName = entity.Equipment.Description.ToCamelCase();
            EMGroupId = entity.EMGroupId ?? entity.Company.EMGroupId;
            RevCode = entity.RevCodeId;

            JCCo = entity.JCCo ?? entity.Company.JCCo;
            JobId = entity.JobId;
            if (entity.Job != null)
            {
                JobDescription = entity.Job.Description;
            }
            PRCo = entity.PRCo ?? entity.DailyTicket.DailyJobTicket.Crew.PRCo;
            Crew = entity.DailyTicket.DailyJobTicket.Crew.Description.ToCamelCase();
            PhaseGroupId = entity.PhaseGroupId ?? entity.Company.PhaseGroupId;
            PhaseId = entity.JCPhaseId;
            JCCostType = entity.JCCostType;

            Status = (DB.EMUsageTransStatusEnum)entity.Status;
            RevRate = entity.RevRate;
            RevWorkUnits = entity.RevWorkUnits;
            RevTimeUnits = entity.RevTimeUnits;
            GLCo = entity.GLCo ?? entity.Company.GLCo;
            GLOffsetAcct = entity.GLOffsetAcct;
            RevDollars = entity.RevDollars;
            CanProcess = true;
        }

        public bool ContainsErrors { get; set; }

        [UIHint("SwitchBoxGreen")]
        public bool CanProcess { get; set; }

        public string ErrorMsg { get; set; }


        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Co")]
        public byte DTCo { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "TicketId")]
        public int TicketId { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "SeqId")]
        public int SeqId { get; set; }


        public string BatchTransType { get; set; }

        public byte? EMCo { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 3, TextSize = 9, ComboUrl = "/EMCombo/Combo", ComboForeignKeys = "EMCo")]
        [Display(Name = "Equipment Id")]
        public string EquipmentId { get; set; }


        [Display(Name = "Name")]
        public string EquipmentName { get; set; }

        public byte? EMGroupId { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 3, TextSize = 9, ComboUrl = "/EMCombo/RevCodeCombo", ComboForeignKeys = "EMGroupId")]
        [Display(Name = "RevCode")]
        public string RevCode { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 3, TextSize = 9, ComboUrl = "/EMCombo/UsageTransTypeCombo", ComboForeignKeys = "Co")]
        [Display(Name = "EMTransType")]
        public string EMTransType { get; set; }

        [UIHint("DateBox")]
        [Field(LabelSize = 3, TextSize = 9, FormGroup = "General", FormGroupRow = 1, Placeholder = "")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Actual Date")]
        public DateTime ActualDate { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 3, TextSize = 9, FormGroup = "General", FormGroupRow = 3, Placeholder = "")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        public byte? GLCo { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 3, TextSize = 9, ComboUrl = "/GLAccount/Combo", ComboForeignKeys = "GLCo")]
        [Display(Name = "Acct")]
        public string GLTransAcct { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 3, TextSize = 9, ComboUrl = "/GLAccount/Combo", ComboForeignKeys = "GLCo")]
        [Display(Name = "Offset Acct")]
        public string GLOffsetAcct { get; set; }

        //[Required]
        public byte? PRCo { get; set; }

        [UIHint("DropdownBox")]
        [Display(Name = "Employee")]
        [Field(LabelSize = 3, TextSize = 9, ComboUrl = "/PRCombo/ActiveEmployeeCombo", ComboForeignKeys = "PRCo")]
        public int? PREmployee { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 0, TextSize = 9, ComboUrl = "/HQCombo/UMCombo", ComboForeignKeys = "HQCo=DTCo")]
        [Display(Name = "Time UM")]
        public string UM { get; set; }

        public byte? JCCo { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 3, TextSize = 9, ComboUrl = "/JCCombo/ActiveCombo", ComboForeignKeys = "JCCo")]
        [Display(Name = "Job")]
        public string JobId { get; set; }

        [Field(LabelSize = 3, TextSize = 9)]
        [Display(Name = "Job")]
        public string JobDescription { get; set; }

        [Field(LabelSize = 3, TextSize = 9)]
        [Display(Name = "Crew")]
        public string Crew { get; set; }

        public byte? PhaseGroupId { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 3, TextSize = 9, ComboUrl = "/PhaseMaster/Combo", ComboForeignKeys = "PhaseGroupId", SearchUrl = "/PhaseMaster/Search", SearchForeignKeys = "PhaseGroupId")]
        [Display(Name = "Phase")]
        public string PhaseId { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 3, TextSize = 9, ComboUrl = "/PhaseMasterCostType/Combo", ComboForeignKeys = "PhaseGroupId, PhaseId")]
        [Display(Name = "CostType")]
        public byte? JCCostType { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Rate")]
        [Field(LabelSize = 3, TextSize = 9)]
        public decimal? RevRate { get; set; }

        [UIHint("IntegerBox")]
        [Display(Name = "Work Units")]
        [Field(LabelSize = 3, TextSize = 9)]
        public decimal? RevWorkUnits { get; set; }

        [UIHint("IntegerBox")]
        [Display(Name = "Time Units")]
        [Field(LabelSize = 3, TextSize = 9)]
        public decimal? RevTimeUnits { get; set; }

        [UIHint("CurrencyBox")]
        [Display(Name = "Amount")]
        [Field(LabelSize = 3, TextSize = 9)]
        public decimal? RevDollars { get; set; }

        [UIHint("IntegerBox")]
        [Display(Name = "Odometer")]
        [Field(LabelSize = 3, TextSize = 9)]
        public decimal? PreviousOdometer { get; set; }

        [UIHint("IntegerBox")]
        [Display(Name = "Odometer")]
        [Field(LabelSize = 3, TextSize = 9)]
        public decimal? CurrentOdometer { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 0, TextSize = 9, ComboUrl = "/HQCombo/UMCombo", ComboForeignKeys = "HQCo=DTCo")]
        [Display(Name = "Time UM")]
        public string TimeUM { get; set; }

        [Required]
        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        public DB.EMUsageTransStatusEnum Status { get; set; }
    }


}
