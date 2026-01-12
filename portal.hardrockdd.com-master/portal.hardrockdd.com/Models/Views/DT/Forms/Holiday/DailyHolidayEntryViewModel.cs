using DB.Infrastructure.ViewPointDB.Data;
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
    public class DailyHolidayEntryListViewModel
    {
        public DailyHolidayEntryListViewModel()
        {

        }
        public DailyHolidayEntryListViewModel(DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket)
        {
            if (ticket == null)
            {
                throw new System.ArgumentNullException(nameof(ticket));
            }
            DTCo = ticket.DTCo;
            TicketId = ticket.TicketId;

            List = ticket.DailyEmployeeEntries
                            .OrderBy(o => o.PREmployee?.LastName)
                            .ThenBy(o => o.PREmployee?.FirstName)
                            .Select(s => new DailyHolidayEntryViewModel(s, ticket)).ToList();
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

        public List<DailyHolidayEntryViewModel> List { get; }
    }

    public class DailyHolidayEntryViewModel: DT.Forms.DailyEmployeeEntryAbstract
    {
        public DailyHolidayEntryViewModel()
        {

        }

        public DailyHolidayEntryViewModel(DB.Infrastructure.ViewPointDB.Data.DailyEmployeeEntry entry):base(entry)
        {
        }

        public DailyHolidayEntryViewModel(DB.Infrastructure.ViewPointDB.Data.DailyEmployeeEntry entry, DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket): base(entry)
        {
            EmployeeName = entry.PREmployee.FullName;
        }

        public override string DayDescription { get; set; }
        public override int? EmployeeId { get; set; }
        public override short? EarnCodeId { get; set; }
        public override string JobId { get; set; }
        public override string EquipmentId { get; set; }
        public override byte PhaseGroupId { get; set; }
        public override string PhaseId { get; set; }

        public new bool Validate(ModelStateDictionary modelState)
        {
            if (modelState == null)
            {
                throw new System.ArgumentNullException(nameof(modelState));
            }
            var ok = true;
            return ok;
        }

        public string EmployeeName { get; set; }

        internal DailyHolidayEntryViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            var updObj = db.DailyEmployeeEntries.FirstOrDefault(f => f.DTCo == DTCo && f.TicketId == TicketId && f.LineNum == LineNum);

            if (updObj != null)
            {
                /****Write the changes to object****/
                updObj.EntryType = EntryTypeId ?? DB.EntryTypeEnum.Admin;
                updObj.EmployeeId = EmployeeId;

                updObj.Value = Value;
                updObj.Comments = Comments;
                updObj.EarnCodeId = (short)(updObj.PREmployee.EarnCodeId != 1 ? 18 : 17);

                try
                {
                    db.BulkSaveChanges();
                    return new DailyHolidayEntryViewModel(updObj);
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