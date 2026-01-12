using DB.Infrastructure.ViewPointDB.Data;
using portal.Repository.VP.PR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.DailyTicket
{
    public class DailyCrewEmployeeListViewModel: DailyEmployeeEntryListViewModel
    {
        public DailyCrewEmployeeListViewModel()
        {

        }

        public DailyCrewEmployeeListViewModel(DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket)
        {
            if (ticket == null)
            {
                throw new System.ArgumentNullException(nameof(ticket));
            }
            DTCo = ticket.DTCo;
            TicketId = ticket.TicketId;

            //Used for Table View
            List = ticket.DailyEmployeeEntries.Select(s => new DailyCrewEmployeeViewModel(s)).ToList();

            //Used for Panel View
            EmployeeForms = ticket.DailyEmployeeEntries.GroupBy(grp => grp.PREmployee).Select(s => new DailyCrewEmployeeFormViewModel(ticket, s.Select(e => e).ToList())).ToList();

        }

        public new List<DailyCrewEmployeeViewModel> List { get; }

        public List<DailyCrewEmployeeFormViewModel> EmployeeForms { get; }
    }

    public class DailyCrewEmployeeViewModel : Models.Views.DT.Forms.DailyEmployeeEntryAbstract
    {
        public DailyCrewEmployeeViewModel()
        {

        }

        public DailyCrewEmployeeViewModel(DB.Infrastructure.ViewPointDB.Data.DailyEmployeeEntry entry):base(entry)
        {
        }

        //public DailyCrewEmployeeViewModel(DB.Infrastructure.ViewPointDB.Data.DailyEmployeeEntry entry, DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket) : base(entry, ticket)
        //{
        //}

        

        [UIHint("DropdownBox")]
        [Field(ComboUrl = "/JCCombo/ActiveCombo", ComboForeignKeys = "JCCo")]
        [Display(Name = "Job")]
        public override string JobId { get; set; }

        public override string DayDescription { get; set; }
        public override int? EmployeeId { get; set; }
        public override short? EarnCodeId { get; set; }
        public override string EquipmentId { get; set; }
        public override byte PhaseGroupId { get; set; }
        public override string PhaseId { get; set; }


        internal DailyCrewEmployeeViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));
            var updObj = db.DailyEmployeeEntries.FirstOrDefault(f => f.DTCo == DTCo && f.TicketId == TicketId && f.LineNum == LineNum);

            if (updObj != null)
            {
                /****Write the changes to object****/

                if (updObj.EntryType != EntryTypeId)
                {
                    updObj.EntryType = EntryTypeId ?? DB.EntryTypeEnum.Admin;
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
                    return new DailyCrewEmployeeViewModel(updObj);
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

    public class DailyCrewEmployeeFormViewModel
    {
        public DailyCrewEmployeeFormViewModel()
        {

        }

        public DailyCrewEmployeeFormViewModel(DB.Infrastructure.ViewPointDB.Data.DailyTicket ticket, List<DB.Infrastructure.ViewPointDB.Data.DailyEmployeeEntry> entries)
        {
            if (ticket == null)
            {
                throw new System.ArgumentNullException(nameof(ticket));
            }
            if (entries == null)
            {
                throw new System.ArgumentNullException(nameof(entries));
            }
            DTCo = ticket.DTCo;
            TicketId = ticket.TicketId;

            if (entries.Count != 0) 
            {
                var employee = entries.FirstOrDefault().PREmployee;
                if (employee != null)
                {
                    EmployeeName = EmployeeRepository.FullName(employee); 
                    var perdiem = ticket.DailyEmployeePerdiems.FirstOrDefault(f => f.EmployeeId == employee.EmployeeId);
                    if (perdiem != null)
                    {
                        Perdiem = new DailyCrewEmployeePerdiemViewModel(perdiem);
                    }

                }
            }
            Entries = entries.Select(s => new DailyCrewEmployeeEntryViewModel(s)).ToList();
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

        public string EmployeeName { get; set; }

        public DailyCrewEmployeePerdiemViewModel Perdiem { get; set; }


        public List<DailyCrewEmployeeEntryViewModel> Entries { get; }
    }

    public class DailyCrewEmployeePerdiemViewModel: DailyEmployeePerdiemViewModel
    {
        public DailyCrewEmployeePerdiemViewModel()
        {

        }

        public DailyCrewEmployeePerdiemViewModel(DB.Infrastructure.ViewPointDB.Data.DailyEmployeePerdiem entry): base(entry)
        {
           
        }

    }

    public class DailyCrewEmployeeEntryViewModel: DailyEmployeeEntryViewModel
    {
        public DailyCrewEmployeeEntryViewModel():base()
        {

        }

        public DailyCrewEmployeeEntryViewModel(DB.Infrastructure.ViewPointDB.Data.DailyEmployeeEntry entry):base(entry)
        {
            
        }
                

        [UIHint("DropdownBox")]
        [Field(LabelSize = 4, TextSize = 8, FormGroup = "", FormGroupRow = 3, ComboUrl = "/JCCombo/ActiveCombo", ComboForeignKeys = "JCCo")]
        [Display(Name = "Job")]
        public new string JobId { get; set; }




        //public void UpdateFromModel(Models.Views.DailyTicket.DailyCrewEmployeeViewModel model)
        //{
        //    if (model == null)
        //        return;

        //    /****Write the changes to object****/
        //    if (EntryType != model.EntryTypeId)
        //    {
        //        EntryType = model.EntryTypeId;
        //    }
        //    else
        //    {
        //        switch (EntryType)
        //        {
        //            case DB.EntryTypeEnum.Admin:
        //                break;
        //            case DB.EntryTypeEnum.Job:
        //                JobId = model.JobId;
        //                PhaseId = model.PhaseId;
        //                break;
        //            case DB.EntryTypeEnum.Equipment:
        //                EquipmentId = model.EquipmentId;
        //                break;
        //            default:
        //                break;
        //        }
        //        EmployeeId = model.EmployeeId;
        //        Value = model.Value;
        //        Comments = model.Comments;
        //    }


        //    //Find Perdiem Line for update
        //    var perdiem = GetPerdiem();
        //    perdiem.UpdateFromModel(model);

        //}


    }

}