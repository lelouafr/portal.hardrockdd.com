using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace portal.Models.Views.Employee
{
    public class EmployeeViewModel
    {
        public EmployeeViewModel()
        {

        }

        public EmployeeViewModel(DB.Infrastructure.ViewPointDB.Data.Employee employee)
        {
            if (employee == null)
            {
                throw new System.ArgumentNullException(nameof(employee));
            }
            PRCo = employee.PRCo;
            EmployeeId = employee.EmployeeId;
            LastName = employee.LastName;
            FirstName = employee.FirstName;
            MidName = employee.MidName;
            EmployeeName = employee.FullName();

            Phone = employee.Phone;
            Email = employee.Email;
            var res = employee.Resource.FirstOrDefault();

            var attchId = employee.UniqueAttchID ?? employee.Resource.FirstOrDefault().UniqueAttchID;
            if (attchId != null)
            {
                using var fileRepo = new Repository.VP.HQ.FileRepository();
                var file = fileRepo.GetFiles((Guid)attchId).Where(f => f.AttachmentTypeID == 50005).FirstOrDefault();// && f.OrigFileName == fileName

                if (file != null)
                {
                    using var attchRepo = new Repository.VP.HQ.AttachmentRepository();
                    var attchment = attchRepo.GetAttachment(file.AttachmentId);

                    ProfileImage = attchment.AttachmentData;
                }
            }
        }

        public EmployeeViewModel(DB.Infrastructure.ViewPointDB.Data.Employee employee, DateTime startDate, DateTime endDate):this(employee)
        {            

            Hours = employee.DailyEmployeeEntries.Where(f => f.DailyTicket.FormId != 1 && f.DailyTicket.FormId != 6 && f.WorkDate > startDate && f.WorkDate <= endDate).Select(s => new EmployeeWeeklyHoursViewModel(s)).ToList();
            Hours.AddRange(employee.DailyJobEmployees.Where(f => f.WorkDate > startDate && f.WorkDate <= endDate).Select(s => new EmployeeWeeklyHoursViewModel(s)).ToList());

            Perdiems = employee.DTEmployeePerdiems.Where(f => f.DailyTicket.FormId != 1 && f.DailyTicket.FormId != 6 && f.WorkDate > startDate && f.WorkDate <= endDate).Select(s => new EmployeeWeeklyPerdiemViewModel(s)).ToList();
            Perdiems.AddRange(employee.DailyJobEmployees.Where(f => f.WorkDate > startDate && f.WorkDate <= endDate).Select(s => new EmployeeWeeklyPerdiemViewModel(s)).ToList());

            Calendar = new List<DateTime>();
            for (int i = 1; i <= (endDate - startDate).TotalDays; i++)
            {
                Calendar.Add(startDate.AddDays(i));
            }
        }

        [Required]
        [Display(Name = "Co")]
        public byte PRCo { get; set; }

        [Required]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }

        [Required]
        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Mid Name")]
        public string MidName { get; set; }

        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }
        
        public string ProfileUri
        {
            get
            {
                if (ProfileImage == null)
                {
                    return null;
                }
                return "data:image/png;base64," + Convert.ToBase64String(ProfileImage);
            }
            set
            {
                var dataUri = value;
                if (dataUri != null)
                {
                    var encodedImage = dataUri.Split(',')[1];
                    var decodedImage = Convert.FromBase64String(encodedImage);
                    ProfileImage = decodedImage;
                }
                else
                {
                    ProfileImage = null;
                }
            }
        }

        [HiddenInput]
        public byte[] ProfileImage { get; set; }

        public List<EmployeeWeeklyPerdiemViewModel> Perdiems { get; }

        public List<EmployeeWeeklyHoursViewModel> Hours { get; }

        public List<DateTime> Calendar { get; }

    }

    public class EmployeeWeeklyHoursViewModel
    {
        public EmployeeWeeklyHoursViewModel()
        {

        }

        public EmployeeWeeklyHoursViewModel(DailyJobEmployee hour)
        {
            if (hour == null)
            {
                throw new System.ArgumentNullException(nameof(hour));
            }
            DTCo = hour.DTCo;
            TicketId = hour.TicketId;
            LineNum = hour.LineNum;
            FormId = (int)hour.DailyTicket.FormId;

            PRCo = hour.PRCo;
            EmployeeId = hour.EmployeeId;
            WorkDate = (DateTime)hour.WorkDate;
            Hours = hour.Hours;
            switch (hour.DailyTicket.FormId)
            {
                case 1:
                    Crew = hour.DailyTicket.DailyJobTicket.CrewId;
                    Job = hour.DailyTicket.DailyJobTicket.JobId;
                    break;
                case 6:
                    Crew = hour.DailyTicket.DailyShopTicket.CrewId;
                    Job = hour.DailyTicket.DailyShopTicket.JobId;
                    break;
                default:
                    break;
            }
        }

        public EmployeeWeeklyHoursViewModel(DailyEmployeeEntry hour)
        {
            if (hour == null)
            {
                throw new System.ArgumentNullException(nameof(hour));
            }
            DTCo = hour.DTCo;
            TicketId = hour.TicketId;
            LineNum = hour.LineNum;
            FormId = (int)hour.DailyTicket.FormId;
            PRCo = hour.PRCo;
            EmployeeId = hour.EmployeeId;
            WorkDate = (DateTime)hour.WorkDate;
            Hours = hour.Value;
            Crew = "";
            Job = hour.JobId;
        }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Co")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "", FormGroupRow = 0, IconClass = "")]
        public byte DTCo { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Ticket Id")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "", FormGroupRow = 0, IconClass = "")]
        public int TicketId { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Line Num")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "", FormGroupRow = 0, IconClass = "")]
        public int LineNum { get; set; }


        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Form")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "", FormGroupRow = 0, IconClass = "")]
        public int FormId { get; set; }

        [UIHint("DateBox")]
        [ReadOnly(true)]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "Ticket", FormGroupRow = 1)]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Work Date")]
        public DateTime WorkDate { get; set; }

        public byte? PRCo { get; set; }

        [HiddenInput]
        [UIHint("Textbox")]
        [Display(Name = "Employee")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "", FormGroupRow = 0, IconClass = "", ComboUrl = "/PRCombo/ActiveEmployeeCombo", ComboForeignKeys = "PRCo", InfoUrl = "/HumanResource/Resource/PopupForm", InfoForeignKeys = "HRCo=PRCo, EmployeeId")]
        public int? EmployeeId { get; set; }


        [Required]
        [UIHint("IntegerBox")]
        [Display(Name = "Hours")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "", FormGroupRow = 0, IconClass = "")]
        public decimal? Hours { get; set; }

        [UIHint("Textbox")]
        [Display(Name = "Crew")]
        public string Crew { get; set; }

        [UIHint("Textbox")]
        [Display(Name = "Job")]
        public string Job { get; set; }

    }
   
    public class EmployeeWeeklyPerdiemViewModel
    {
        public EmployeeWeeklyPerdiemViewModel()
        {

        }

        public EmployeeWeeklyPerdiemViewModel(DailyJobEmployee perdiem)
        {
            if (perdiem == null)
            {
                throw new System.ArgumentNullException(nameof(perdiem));
            }
            DTCo = perdiem.DTCo;
            TicketId = perdiem.TicketId;
            LineNum = perdiem.LineNum;
            FormId = (int)perdiem.DailyTicket.FormId;
            PRCo = perdiem.PRCo;
            EmployeeId = perdiem.EmployeeId;
            WorkDate = (DateTime)perdiem.WorkDate;
            Perdiem = (DB.PerdiemEnum)perdiem.Perdiem;
            switch (perdiem.DailyTicket.FormId)
            {
                case 1:
                    Crew = perdiem.DailyTicket.DailyJobTicket.CrewId;
                    Job = perdiem.DailyTicket.DailyJobTicket.JobId;
                    break;
                case 6:
                    Crew = perdiem.DailyTicket.DailyShopTicket.CrewId;
                    Job = perdiem.DailyTicket.DailyShopTicket.JobId;
                    break;
                default:
                    break;
            }
        }

        public EmployeeWeeklyPerdiemViewModel(DailyEmployeePerdiem perdiem)
        {
            if (perdiem == null)
            {
                throw new System.ArgumentNullException(nameof(perdiem));
            }
            DTCo = perdiem.DTCo;
            TicketId = perdiem.TicketId;
            LineNum = perdiem.LineNum;
            FormId = (int)perdiem.DailyTicket.FormId;
            PRCo = perdiem.PRCo;
            EmployeeId = perdiem.EmployeeId;
            WorkDate = (DateTime)perdiem.WorkDate;
            Perdiem = (DB.PerdiemEnum)perdiem.PerDiemId;
            Crew = "";
            Job = "";
        }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Co")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "", FormGroupRow = 0, IconClass = "")]
        public byte DTCo { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Ticket Id")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "", FormGroupRow = 0, IconClass = "")]
        public int TicketId { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Line Num")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "", FormGroupRow = 0, IconClass = "")]
        public int LineNum { get; set; }


        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Form")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "", FormGroupRow = 0, IconClass = "")]
        public int FormId { get; set; }

        [UIHint("DateBox")]
        [ReadOnly(true)]
        [Field(LabelSize = 2, TextSize = 4, FormGroup = "Ticket", FormGroupRow = 1)]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Work Date")]
        public DateTime WorkDate { get; set; }


        public byte? PRCo { get; set; }

        [HiddenInput]
        [UIHint("Textbox")]
        [Display(Name = "Employee")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "", FormGroupRow = 0, IconClass = "", ComboUrl = "/PRCombo/ActiveEmployeeCombo", ComboForeignKeys = "Co", InfoUrl = "/HumanResource/Resource/PopupForm", InfoForeignKeys = "HRCo=PRCo, EmployeeId")]
        public int? EmployeeId { get; set; }


        [UIHint("EnumBox")]
        [Display(Name = "Perdiem")]
        [Field(LabelSize = 2, TextSize = 10, FormGroup = "", FormGroupRow = 0, IconClass = "")]
        public DB.PerdiemEnum? Perdiem { get; set; }

        [UIHint("Textbox")]
        [Display(Name = "Crew")]
        public string Crew { get; set; }

        [UIHint("Textbox")]
        [Display(Name = "Job")]
        public string Job { get; set; }

    }
}