using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Areas.Payroll.Models.Ticket
{
    public class EmployeeEntryListViewModel
    {
        public EmployeeEntryListViewModel()
        {

        }

        public EmployeeEntryListViewModel(DailyTicket ticket)
        {
            if (ticket == null)
            {
                throw new System.ArgumentNullException(nameof(ticket));
            }
            DTCo = ticket.DTCo;
            TicketId = ticket.TicketId;

            List = ticket.DailyEmployeeEntries.Select(s => new EmployeeEntryViewModel(s)).ToList();

        }

		[Key]
		[Display(Name = "Co")]
		public byte DTCo { get; set; }

		[Key]
		[Display(Name = "Ticket Id")]
		public int TicketId { get; set; }

		public List<EmployeeEntryViewModel> List { get; }
    }

    public class EmployeeEntryViewModel
    {
        public EmployeeEntryViewModel()
        {

        }

        public EmployeeEntryViewModel(DailyEmployeeEntry entry)
        {
			if (entry == null)
				return;

			DTCo = entry.DTCo;
			TicketId = entry.TicketId;
			LineNum = entry.LineNum;
			WorkDate = entry.WorkDate ?? DateTime.Now.Date;
			DayDescription = string.Format(AppCultureInfo.CInfo(), "{0}", WorkDate.DayOfWeek.ToString());
			PRCo = entry.PRCo ?? entry.DailyTicket.HQCompanyParm.PRCo;
			EmployeeId = entry.EmployeeId;
			EntryType = entry.EntryType;

			if (EntryType == DB.EntryTypeEnum.Job)
			{
				JCCo = entry.JCCo ?? entry.DailyTicket.HQCompanyParm.JCCo;
				JobId = entry.JobId;
				PhaseGroupId = (entry.PhaseGroupId ?? entry.Job?.JCCompanyParm?.HQCompanyParm?.PhaseGroupId) ?? 0;
				PhaseId = entry.PhaseId;
			}
			else if (EntryType == DB.EntryTypeEnum.Equipment)
			{
				EMCo = entry.EMCo ?? entry.DailyTicket.HQCompanyParm.EMCo;
				EquipmentId = entry.EquipmentId;
			}

			Value = entry.Value;
			Comments = entry.Comments;

			var perDiem = entry.GetPerdiem();
			if (perDiem.IsSaveRequired)
				entry.db.BulkSaveChanges();

			if (perDiem != null)
			{
				PerdiemLineNum = perDiem.LineNum;
				Perdiem = (DB.PerdiemEnum?)perDiem.PayrolPerdiem;
			}
		}


		[Key]
		[Display(Name = "Co")]
		public byte DTCo { get; set; }

		[Key]
		[Display(Name = "Ticket Id")]
		public int TicketId { get; set; }

		[Key]
		[Display(Name = "Line Num")]
		public int LineNum { get; set; }

		public int PerdiemLineNum { get; set; }


		[UIHint("DateBox")]
		[Display(Name = "Work Date")]
		public DateTime WorkDate { get; set; }


		[UIHint("TextBox")]
		[ReadOnly(true)]
		[Display(Name = "Day")]
		public string DayDescription { get; set; }//{ get { return string.Format(AppCultureInfo.CInfo(), "{0}", WorkDate.DayOfWeek.ToString()); } }

		public byte? PRCo { get; set; }

		[Required]
		[UIHint("DropdownBox")]
		[Display(Name = "Employee")]
		[Field(ComboUrl = "/PRCombo/ActiveEmployeeCombo", ComboForeignKeys = "PRCo")]
		public int? EmployeeId { get; set; }

		[UIHint("DropdownBox")]
		[Field(ComboUrl = "/PRCombo/PRCodeCombo", ComboForeignKeys = "PRCo")]
		[Display(Name = "Type")]
		public short? EarnCodeId { get; set; }

		[Required]
		[UIHint("EnumBox")]
		[Display(Name = "PerDiem")]
		public DB.PerdiemEnum? Perdiem { get; set; }

		[Required]
		[UIHint("IntegerBox")]
		[Display(Name = "Hours")]
		public decimal? Value { get; set; }

		[Required]
		[UIHint("EnumBox")]
		[Display(Name = "Entry Type")]
		[Field(Placeholder = "Entry Type")]
		public DB.EntryTypeEnum EntryType { get; set; }

		public byte? JCCo { get; set; }

		[UIHint("DropdownBox")]
		[Field(ComboUrl = "/JCCombo/Combo", ComboForeignKeys = "JCCo")]
		[Display(Name = "Job")]
		public string JobId { get; set; }

		public byte? EMCo { get; set; }

		[UIHint("DropdownBox")]
		[Field(ComboUrl = "/EMCombo/Combo", ComboForeignKeys = "EMCo")]
		[Display(Name = "Equipment Id")]
		public string EquipmentId { get; set; }

		[HiddenInput]
		[Display(Name = "Phase Group")]
		public byte PhaseGroupId { get; set; }

		[UIHint("DropdownBox")]
		[Field(ComboUrl = "/PhaseMaster/ProductionCombo", ComboForeignKeys = "PhaseGroupId")]
		[Display(Name = "Task")]
		public string PhaseId { get; set; }

		[UIHint("TextBox")]
		[Display(Name = "Comments")]
		[Field(LabelSize = 2, TextSize = 10)]
		[TableField(InternalTableRow = 2)]
		public string Comments { get; set; }


		internal EmployeeEntryViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));
            var updObj = db.DailyEmployeeEntries.FirstOrDefault(f => f.DTCo == DTCo && f.TicketId == TicketId && f.LineNum == LineNum);

            if (updObj != null)
            {
                /****Write the changes to object****/

                if (updObj.EntryType != EntryType)
                {
                    updObj.EntryType = EntryType;
                }
                else
                {
                    switch (updObj.EntryType)
                    {
                        case DB.EntryTypeEnum.Admin:
                            break;
                        case DB.EntryTypeEnum.Job:
                            updObj.JobId = JobId;
                            updObj.PhaseId = PhaseId;
                            break;
                        case DB.EntryTypeEnum.Equipment:
                            updObj.EquipmentId = EquipmentId;
                            break;
                        default:
                            break;
                    }
                    updObj.EmployeeId = EmployeeId;
                    updObj.Value = Value;
                    updObj.Comments = Comments;
                }

                //Find Perdiem Line for update
                var perdiem = updObj.GetPerdiem();
                perdiem.EmployeeId = EmployeeId;
                perdiem.PerDiemId = (int)Perdiem;
                perdiem.PerDiemIdAdj = null;
                perdiem.Comments = Comments;

                try
                {
                    db.SaveChanges(modelState);
                    return new EmployeeEntryViewModel(updObj);
                }
                catch (Exception ex)
                {
                    modelState.AddModelError("", ex.Message);
                    return this;
                }
            }
            modelState.AddModelError("", "Object Doesn't Exist For Update!");
            return this;
        }
    }


}