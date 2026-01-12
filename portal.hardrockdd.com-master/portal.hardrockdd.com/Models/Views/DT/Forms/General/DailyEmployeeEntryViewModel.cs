using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.DT.Forms;
using portal.Repository.VP.DT;
using portal.Repository.VP.PR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.DailyTicket
{
    public class DailyEmployeeEntryListViewModel
    {
        public DailyEmployeeEntryListViewModel()
        {

        }
        public DailyEmployeeEntryListViewModel(DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket)
        {
            if (ticket == null)
            {
                throw new System.ArgumentNullException(nameof(ticket));
            }
            DTCo = ticket.DTCo;
            TicketId = ticket.TicketId;

            List = ticket.DailyEmployeeEntries.Select(s => new DailyEmployeeEntryViewModel(s, ticket)).ToList();
        }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Co")]
        [Field(LabelSize = 2, TextSize = 10)]
        public byte DTCo { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Ticket Id")]
        [Field(LabelSize = 2, TextSize = 10)]
        public int TicketId { get; set; }

        public List<DailyEmployeeEntryViewModel> List { get; }
    }

    public class DailyEmployeeEntryViewModel : DailyEmployeeEntryAbstract
    {
        public DailyEmployeeEntryViewModel()
        {

        }

        public DailyEmployeeEntryViewModel(DB.Infrastructure.ViewPointDB.Data.DailyEmployeeEntry entry) : base(entry)
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
            EmployeeId = entry.EmployeeId;
            EntryTypeId = (DB.EntryTypeEnum)entry.EntryTypeId;

            JobId = entry.JobId;
            EquipmentId = entry.EquipmentId;
            PhaseId = entry.PhaseId;
            Value = entry.ValueAdj ?? entry.Value;
            var perdiem = entry.DailyTicket.DailyEmployeePerdiems.FirstOrDefault(f => f.LineNum == entry.PerdiemLineNum);
            if (perdiem == null)
            {
                perdiem = entry.DailyTicket.DailyEmployeePerdiems.FirstOrDefault(f => f.tEmployeeId == entry.tEmployeeId && f.WorkDate == entry.WorkDate);
                if (perdiem == null)
                {
                    perdiem = entry.DailyTicket.AddPerdiem(entry.PREmployee, entry.WorkDate);
                    entry.PerdiemLineNum = perdiem.LineNum;
                    entry.db.SaveChanges();
                }
                else
                {
                    entry.PerdiemLineNum = perdiem.LineNum;
                    entry.db.SaveChanges();
                }
            }
            if (perdiem != null)
            {
                PerdiemLineNum = perdiem.LineNum;
                Perdiem = (DB.PerdiemEnum)(perdiem.PerDiemIdAdj ?? perdiem.PerDiemId);
            }
            Comments = entry.Comments;
        }

        public DailyEmployeeEntryViewModel(DB.Infrastructure.ViewPointDB.Data.DailyEmployeeEntry entry, DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket) : base(entry)
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
            EmployeeId = entry.EmployeeId;
            EntryTypeId = (DB.EntryTypeEnum)entry.EntryTypeId;
            JobId = entry.JobId;
            EquipmentId = entry.EquipmentId;
            PhaseGroupId = (byte)entry.DailyTicket.HQCompanyParm.PhaseGroupId;
            PhaseId = entry.PhaseId;
            Value = entry.ValueAdj ?? entry.Value;
            var perdiem = entry.DailyTicket.DailyEmployeePerdiems.Where(f => f.LineNum == entry.PerdiemLineNum).FirstOrDefault();
            if (perdiem != null)
            {
                PerdiemLineNum = perdiem.LineNum;
                Perdiem = (DB.PerdiemEnum)(perdiem.PerDiemIdAdj ?? perdiem.PerDiemId);
            }
            Comments = entry.Comments;
        }

        //public bool Validate(ModelStateDictionary modelState)
        //{
        //    if (modelState == null)
        //    {
        //        throw new System.ArgumentNullException(nameof(modelState));
        //    }
        //    var ok = true;
        //    switch (EntryTypeId)
        //    {
        //        case DB.EntryTypeEnum.Admin:

        //            break;
        //        case DB.EntryTypeEnum.Equipment:
        //            if (EquipmentId == null)
        //            {

        //                modelState.AddModelError("EquipmentId", "Equipment Missing");
        //                ok = false;
        //            }
        //            break;
        //        case DB.EntryTypeEnum.Job:
        //            if (JobId == null)
        //            {
        //                modelState.AddModelError("JobId", "Job Missing");
        //                ok = false;
        //            }
        //            if (JobId != null)
        //            {
        //                if (PhaseId == null)
        //                {
        //                    modelState.AddModelError("PhaseId", "Task Missing");
        //                    ok = false;
        //                }
        //            }
        //            break;
        //        default:
        //            break;
        //    }

        //    return ok;
        //}

        [Required]
        [UIHint("DropdownBox")]
        [Display(Name = "Employee")]
        [Field(ForeignKeys = "PRCo,EmployeeId", ComboUrl = "/PRCombo/ActiveEmployeeCombo", ComboForeignKeys = "PRCo")]
        public override int? EmployeeId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PRCombo/PRCodeCombo", ComboForeignKeys = "PRCo")]
        [Display(Name = "Type")]
        public override short? EarnCodeId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/JCCombo/ActiveCombo", ComboForeignKeys = "JCCo")]
        [Display(Name = "Job")]
        public override string JobId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/EMCombo/Combo", ComboForeignKeys = "EMCo")]
        [Display(Name = "Equipment Id")]
        public override string EquipmentId { get; set; }

        [HiddenInput]
        [Display(Name = "Phase Group")]
        public override byte PhaseGroupId { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PhaseMaster/ProductionCombo", ComboForeignKeys = "PhaseGroupId")]
        [Display(Name = "Task")]
        public override string PhaseId { get; set; }

        public override string DayDescription { get; set; }

        internal DailyEmployeeEntryViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            var updObj = db.DailyEmployeeEntries.FirstOrDefault(f => f.DTCo == DTCo && f.TicketId == TicketId && f.LineNum == LineNum);

            if (updObj != null)
            {
                /****Write the changes to object****/
                updObj.EntryType = EntryTypeId ?? DB.EntryTypeEnum.Admin;
                updObj.EmployeeId = EmployeeId;
                updObj.JobId = JobId;
                updObj.PhaseId = PhaseId;
                updObj.EquipmentId = EquipmentId;

                updObj.WorkDate = WorkDate;
                updObj.Value = Value;
                updObj.Comments = Comments;

                try
                {
                    db.BulkSaveChanges();
                    return new DailyEmployeeEntryViewModel(updObj);
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