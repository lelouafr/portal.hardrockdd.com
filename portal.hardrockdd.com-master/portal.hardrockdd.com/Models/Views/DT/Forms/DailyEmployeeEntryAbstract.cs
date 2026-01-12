using DB.Infrastructure.ViewPointDB.Data;
using portal.Repository.VP.DT;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.DT.Forms
{

    public abstract class DailyEmployeeEntryAbstract : AuditBaseViewModel
    {
        public DailyEmployeeEntryAbstract()
        {

        }

        public DailyEmployeeEntryAbstract(DailyEmployeeEntry entry) : base(entry)
        {
            if (entry == null)
            {
                throw new System.ArgumentNullException(nameof(entry));
            }
            DTCo = entry.DTCo;
            TicketId = entry.TicketId;
            LineNum = entry.LineNum;
            WorkDate = entry.WorkDate ?? DateTime.Now.Date;
            DayDescription = string.Format(AppCultureInfo.CInfo(), "{0}", WorkDate.DayOfWeek.ToString());
            PRCo = entry.PRCo ?? entry.DailyTicket.HQCompanyParm.PRCo;
            EmployeeId = entry.EmployeeId;
            EntryTypeId = (DB.EntryTypeEnum)entry.EntryTypeId;

            JCCo = entry.JCCo ?? entry.DailyTicket.HQCompanyParm.JCCo;
            JobId = entry.JobId;

            EMCo = entry.EMCo ?? entry.DailyTicket.HQCompanyParm.EMCo;

            EquipmentId = entry.EquipmentId;
            PhaseGroupId = (entry.PhaseGroupId ?? entry.Job?.JCCompanyParm?.HQCompanyParm?.PhaseGroupId) ?? 0;
            PhaseId = entry.PhaseId ;
            Value = entry.ValueAdj ?? entry.Value;
            var perdiem = entry.GetPerdiem();
            if (perdiem.IsSaveRequired)
            {
                entry.db.BulkSaveChanges();
            }
            //if (perdiem == null)
            //{
            //    perdiem = entry.DailyTicket.DailyEmployeePerdiems.Where(f => f.tEmployeeId == entry.tEmployeeId && f.WorkDate == entry.WorkDate).FirstOrDefault();
            //    if (perdiem == null)
            //    {
            //        using var db = new VPContext();
            //        var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == entry.DTCo && f.TicketId == entry.TicketId);
            //        var emp = ticket.DailyEmployeeEntries.FirstOrDefault(f => f.LineNum == entry.LineNum);
            //        perdiem = DailyEmployeePerdiemRepository.Init(ticket, emp);
            //        ticket.DailyEmployeePerdiems.Add(perdiem);
            //        emp.PerdiemLineNum = perdiem.LineNum;
            //        db.SaveChanges();
            //    }
            //    else
            //    {
            //        using var db = new VPContext();
            //        var emp = db.DailyEmployeeEntries.FirstOrDefault(f => f.DTCo == entry.DTCo && f.TicketId == entry.TicketId && f.LineNum == entry.LineNum);
            //        emp.PerdiemLineNum = perdiem.LineNum;
            //        db.SaveChanges();
            //    }
            //    //throw new System.ArgumentNullException(nameof(perdiem));
            //}
            if (perdiem != null)
            {
                PerdiemLineNum = perdiem.LineNum;
                //Perdiem = (DB.PerdiemEnum?)perdiem.PerDiemId;
                Perdiem = (DB.PerdiemEnum?)perdiem.PayrolPerdiem;
            }
            Comments = entry.Comments;
        }

        public DailyEmployeeEntryAbstract(DB.Infrastructure.ViewPointDB.Data.DailyEmployeeEntry entry, DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket) : base(entry)
        {
            if (entry == null)
            {
                throw new System.ArgumentNullException(nameof(entry));
            }
            if (ticket == null)
            {
                throw new System.ArgumentNullException(nameof(ticket));
            }
            DTCo = entry.DTCo;
            TicketId = entry.TicketId;
            LineNum = entry.LineNum;
            WorkDate = entry.WorkDate ?? DateTime.Now.Date;
            DayDescription = string.Format(AppCultureInfo.CInfo(), "{0}", WorkDate.DayOfWeek.ToString());
            EntryTypeId = (DB.EntryTypeEnum)entry.EntryTypeId;

            PRCo = entry.PRCo ?? entry.DailyTicket.HQCompanyParm.PRCo;
            EmployeeId = entry.EmployeeId;

            JCCo = entry.JCCo ?? entry.DailyTicket.HQCompanyParm.JCCo;
            JobId = entry.JobId;

            EMCo = entry.EMCo ?? entry.DailyTicket.HQCompanyParm.EMCo;
            EquipmentId = entry.EquipmentId;

            PhaseGroupId = (byte)entry.DailyTicket.HQCompanyParm.PhaseGroupId;
            PhaseId = entry.PhaseId;
            Value = entry.ValueAdj ?? entry.Value;

            var perdiem = entry.GetPerdiem();
            if (perdiem.IsSaveRequired)
                entry.db.BulkSaveChanges();

            if (perdiem != null)
            {
                PerdiemLineNum = perdiem.LineNum;
                Perdiem = (DB.PerdiemEnum)perdiem.PayrolPerdiem;
            }
            Comments = entry.Comments;
        }

        public bool Validate(ModelStateDictionary modelState)
        {
            if (modelState == null)
            {
                throw new System.ArgumentNullException(nameof(modelState));
            }
            var ok = true;
            switch (EntryTypeId)
            {
                case DB.EntryTypeEnum.Admin:

                    break;
                case DB.EntryTypeEnum.Equipment:
                    if (EquipmentId == null)
                    {

                        modelState.AddModelError("EquipmentId", "Equipment Missing");
                        ok = false;
                    }
                    break;
                case DB.EntryTypeEnum.Job:
                    if (JobId == null)
                    {
                        modelState.AddModelError("JobId", "Job Missing");
                        ok = false;
                    }

                    if (JobId != null)
                    {
                        if (PhaseId == null)
                        {
                            modelState.AddModelError("PhaseId", "Task Missing");
                            ok = false;
                        }
                    }
                    break;
                default:
                    break;
            }
            return ok;
        }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Co")]
        public byte DTCo { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Ticket Id")]
        public int TicketId { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Line Num")]
        public int LineNum { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Perdiem Line Num")]
        public int PerdiemLineNum { get; set; }


        [UIHint("DateBox")]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "Ticket", FormGroupRow = 1, Placeholder = "")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Work Date")]
        public DateTime WorkDate { get; set; }


        [UIHint("TextBox")]
        [ReadOnly(true)]
        [Display(Name = "Day")]
        abstract public string DayDescription { get; set; }//{ get { return string.Format(AppCultureInfo.CInfo(), "{0}", WorkDate.DayOfWeek.ToString()); } }

        public byte? PRCo { get; set; }

        [Required]
        [UIHint("DropdownBox")]
        [Display(Name = "Employee")]
        [Field(ForeignKeys = "Co=PRCo,EmployeeId", ComboUrl = "/PRCombo/ActiveEmployeeCombo", ComboForeignKeys = "PRCo")]
        abstract public int? EmployeeId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PRCombo/PRCodeCombo", ComboForeignKeys = "PRCo")]
        [Display(Name = "Type")]
        abstract public short? EarnCodeId { get; set; }

        [Required]
        [UIHint("EnumBox")]
        [Display(Name = "Perdiem")]
        [Field(LabelSize = 3, TextSize = 9, FormGroup = "", FormGroupRow = 1, IconClass = "", Placeholder = "")]
        public DB.PerdiemEnum? Perdiem { get; set; }

        [Required]
        [UIHint("IntegerBox")]
        [Display(Name = "Hours")]
        [Field(LabelSize = 3, TextSize = 9, FormGroup = "", FormGroupRow = 1, IconClass = "", Placeholder = "")]
        public decimal? Value { get; set; }

        [Required]
        [UIHint("EnumBox")]
        [Display(Name = "Entry Type")]
        [Field(Placeholder = "Entry Type")]
        public DB.EntryTypeEnum? EntryTypeId { get; set; }

        public byte? JCCo { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/JCCombo/Combo", ComboForeignKeys = "JCCo")]
        [Display(Name = "Job")]
        abstract public string JobId { get; set; }

        public byte? EMCo { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/EMCombo/Combo", ComboForeignKeys = "EMCo")]
        [Display(Name = "Equipment Id")]
        abstract public string EquipmentId { get; set; }

        [HiddenInput]
        [Display(Name = "Phase Group")]
        abstract public byte PhaseGroupId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PhaseMaster/ProductionCombo", ComboForeignKeys = "PhaseGroupId")]
        [Display(Name = "Task")]
        abstract public string PhaseId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Comments")]
        [Field(LabelSize = 2, TextSize = 10)]
        [TableField(InternalTableRow = 2)]
        public string Comments { get; set; }
    }
}