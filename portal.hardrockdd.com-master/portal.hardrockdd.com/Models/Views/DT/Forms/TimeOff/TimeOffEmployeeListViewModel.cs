using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.DailyTicket
{
    public class DailyEmployeeTimeOffListViewModel
    {
        public DailyEmployeeTimeOffListViewModel()
        {

        }
        public DailyEmployeeTimeOffListViewModel(DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket)
        {
            if (ticket == null)
            {
                throw new System.ArgumentNullException(nameof(ticket));
            }
            DTCo = ticket.DTCo;
            TicketId = ticket.TicketId;
            Entries = ticket.DailyEmployeeEntries.Select(s => new DailyEmployeeTimeOffViewModel(s)).OrderBy(o => o.WorkDate).ToList();

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

        public List<DailyEmployeeTimeOffViewModel> Entries { get; }

    }

    public class DailyEmployeeTimeOffViewModel : DT.Forms.DailyEmployeeEntryAbstract
    {

        public DailyEmployeeTimeOffViewModel()
        {

        }

        public DailyEmployeeTimeOffViewModel(DB.Infrastructure.ViewPointDB.Data.DailyEmployeeEntry entry):base(entry)
        {
            if (entry == null)
            {
                throw new System.ArgumentNullException(nameof(entry));
            }
            EarnCodeId = entry.EarnCodeId;
            EmployeeEarnCodeId = entry.PREmployee.EarnCodeId;
        }

        public new bool Validate(ModelStateDictionary modelState)
        {
            if (modelState == null)
            {
                throw new System.ArgumentNullException(nameof(modelState));
            }
            using var db = new VPContext();
            var ticket = db.DailyTickets.FirstOrDefault(f => f.DTCo == DTCo && f.TicketId == TicketId);
            var maxDate = ticket.WorkDate.Value.WeekEndDate(DayOfWeek.Saturday);
            var ok = true;
            var futureDate = WorkDate > maxDate;
            if (futureDate)
            {
                modelState.AddModelError("WorkDate", "You may not enter Time off for a date outside of current week");
                ok &= false;
            }
            DayDescription = string.Format(AppCultureInfo.CInfo(), "{0}", WorkDate.DayOfWeek.ToString());
            return ok;
        }

        [HiddenInput]
        [ReadOnly(true)]
        [Display(Name = "Type")]
        public short? EmployeeEarnCodeId { get; set; }

        [UIHint("TextBox")]
        [ReadOnly(true)]
        [Display(Name = "Day")]
        public override string DayDescription { get; set; }

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/PRCombo/TimeOffEarnCodeCombo", ComboForeignKeys = "PRCo, EmployeeEarnCodeId")]
        [Display(Name = "Type")]
        public override short? EarnCodeId { get; set; }


        public override int? EmployeeId { get; set; }
        public override string JobId { get; set; }
        public override string EquipmentId { get; set; }
        public override byte PhaseGroupId { get; set; }
        public override string PhaseId { get; set; }


        internal DailyEmployeeTimeOffViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            var baseModel = (Models.Views.DT.Forms.DailyEmployeeEntryAbstract)this;
            var updObj = db.DailyEmployeeEntries.FirstOrDefault(f => f.DTCo == DTCo && f.TicketId == TicketId && f.LineNum == LineNum);

            if (updObj != null)
            {
                if (Comments == null && EmployeeId != null)
                {
                    var workDateChanged = updObj.WorkDate != WorkDate;

                    updObj.EntryType = EntryTypeId ?? DB.EntryTypeEnum.Admin;
                    updObj.EmployeeId = EmployeeId;

                    updObj.WorkDate = WorkDate;
                    updObj.EarnCodeId = EarnCodeId;
                    updObj.Value = Value;
                    updObj.PhaseGroupId = updObj.DailyTicket.HQCompanyParm.PhaseGroupId;

                    if (workDateChanged)
                    {
                        var perdeim = updObj.DailyTicket.DailyEmployeePerdiems.FirstOrDefault(f => f.WorkDate == updObj.WorkDate);
                        updObj.PerdiemLineNum = perdeim?.LineNum;
                    }
                }
                else if (EmployeeId == null && updObj.Comments != Comments)
                {
                    updObj.Comments = Comments;
                }
                try
                {
                    db.BulkSaveChanges();
                    return new DailyEmployeeTimeOffViewModel(updObj);
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