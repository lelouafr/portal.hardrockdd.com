using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Web;

namespace portal.Models.Views.Equipment.Audit
{
    public class EquipmentAuditListViewModel
    {
        public EquipmentAuditListViewModel()
        {
            List = new List<EquipmentAuditViewModel>();
        }

        public EquipmentAuditListViewModel(DB.Infrastructure.ViewPointDB.Data.WebUser user, DB.EMAuditStatusEnum? status = null)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var wf = new List<WorkFlow>();

            var db = user.db;
            var workFlows = db.WorkFlowUsers
                            .Include("Sequence")
                            .Include("Sequence.WorkFlow")
                            .Include("Sequence.WorkFlow.EquipmentAudits")
                            .Where(f => f.AssignedTo == user.Id && f.Sequence.Active && f.Sequence.WorkFlow.EquipmentAudits.Any());


            //var activeWorkFlowSeqs = user.WorkFlows.Where(f => f.Sequence.Active).ToList();
            var wfSeq = workFlows.Where(f => f.Sequence.WorkFlow.EquipmentAudits.Any()).Select(s => s.Sequence).Distinct().ToList();


            if (status != null)
                wf = wfSeq.Where(f => f.Status == (byte)status).Select(s => s.WorkFlow).Distinct().ToList();
            else
                wf = wfSeq.Select(s => s.WorkFlow).Distinct().ToList();
            var audits = wf.SelectMany(s => s.EquipmentAudits).ToList();

            List = audits.Select(s => new EquipmentAuditViewModel(s)).ToList();
            //var list = wf.GroupBy(g => g.EquipmentAudit).Select(s => s.Key).OrderByDescending(o => o.CreatedOn).ToList();
            //List = list.Select(s => new EquipmentAuditViewModel(s)).ToList();
        }


        public EquipmentAuditListViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company)
        {
            if (company == null)
                throw new ArgumentNullException(nameof(company));
            List = company.EMCompanyParm.EquipmentAudits.Select(s => new EquipmentAuditViewModel(s)).ToList();
        }

        public EquipmentAuditListViewModel(Crew crew)
        {
            if (crew == null)
                throw new ArgumentNullException(nameof(crew));
            var emp = crew.CrewLeader;
            var resource = emp.Resource.FirstOrDefault();
            var user = resource.WebUser;
            if (user == null)
            {
                using var db = new VPContext();
                user = db.WebUsers.FirstOrDefault(f => f.Email.ToLower() == emp.Email.ToLower());
                if (user == null )
                {
                    user = db.WebUsers.FirstOrDefault(f => f.Email.ToLower() == resource.CompanyEmail.ToLower());
                }
            }
            EMCo = crew.PRCo;
            CrewId = crew.CrewId;
            if (user == null)
            {
                List = new List<EquipmentAuditViewModel>();
            }
            else
            {
                List = user.AssignedEMAudits.ToList().Where(f => f.Status != DB.EMAuditStatusEnum.Canceled).Select(s => new EquipmentAuditViewModel(s)).ToList();
            }
            
        }

        [Key]
        [HiddenInput]
        public byte EMCo { get; set; }

        [Key]
        [UIHint("DropdownBox")]
        [Field( ComboUrl = "/PRCombo/CrewCombo", ComboForeignKeys = "EMCo")]
        [Display(Name = "Crew")]
        public string CrewId { get; set; }

        [Key]
        [HiddenInput]
        public string EmployeeId { get; set; }


        public List<EquipmentAuditViewModel> List { get; }
    }

    public class EquipmentAuditViewModel : AuditBaseViewModel
    {
        public EquipmentAuditViewModel()
        {

        }

        public EquipmentAuditViewModel(EMAudit audit) : base(audit)
        {
            if (audit == null)
                return;

            EMCo = audit.EMCo;
            AuditId = audit.AuditId;
            StatusId = audit.Status;
            AuditTypeId = (DB.EMAuditTypeEnum)audit.AuditTypeId;
            AuditForm = (DB.EMAuditFormEnum)audit.AuditFormId;
            CompletedOn = audit.CompletedOn;
            CreatedOn = audit.CreatedOn;
            Description = audit.Description;
            CreatedUser = new Web.WebUserViewModel(audit.CreatedUser);
            StatusIdString = StatusId.DisplayName();
            AuditTypeIdString = AuditTypeId.DisplayName();
            AuditFormIdString = AuditForm.DisplayName();
            Action = new EquipmentAuditActionViewModel(audit);
            PRCo = audit.EMParameter.HQCompanyParm.PRCo;
            CrewId = audit.ParmCrewId;
            EmployeeId = audit.ParmEmployeeId;
            LocationId = audit.ParmLocationId;
            IncludCrewLeaderEquipment = audit.ParmIncludeCrewLeaderEquipment ?? false;
            IncludeDirectReportEmployeeEquipment = audit.ParmIncludeDirectReports ?? false;
            IncludeSubEquipment = audit.ParmIncludeSubEquipment ?? false;


            //         EmployeeId LocationId IncludCrewLeaderEquipment IncludeDirectReportEmployeeEquipment IncludeSubEquipment
            //var assignedUsers = audit.XXX_WorkFlows.Where(f => f.TableName == "budEMAH").SelectMany(s => s.Sequences.Where(w => w.Active ).SelectMany(seq => seq.AssignedUsers).Where(w => w.AssignedUser != null).ToList());
            var currentSequence = audit.WorkFlow.CurrentSequence();
            if (currentSequence != null)
            {
                var assignedUsers = currentSequence.AssignedUsers.Where(w => w.AssignedUser != null).ToList();
                if (assignedUsers.Any())
                {
                    AssignedTo = string.Join(", ", assignedUsers.Select(s => string.Format(AppCultureInfo.CInfo(), "{0} {1}", s.AssignedUser.FirstName, s.AssignedUser.LastName)).ToArray());
                }
            }
        }

        [Key]
        [HiddenInput]
        public byte EMCo { get; set; }

        [Key]
        [HiddenInput]
        public int AuditId { get; set; }

        public byte? PRCo { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        public DB.EMAuditStatusEnum StatusId { get; set; }

        public string StatusIdString { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [UIHint("EnumBox")]
        [Display(Name = "Audit Type")]
        public DB.EMAuditTypeEnum AuditTypeId { get; set; }

        [Required]
        [UIHint("EnumBox")]
        [Display(Name = "Audit Form")]
        public DB.EMAuditFormEnum AuditForm { get; set; }

        public string AuditTypeIdString { get; set; }

        public string AuditFormIdString { get; set; }

        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Completed Date")]
        public DateTime? CompletedOn { get; set; }

        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Created Date")]
        public DateTime? CreatedOn { get; set; }


        [UIHint("WebUserBox")]
        [Display(Name = "Created By")]
        public WebUserViewModel CreatedUser { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Assigned To")]
        public string AssignedTo { get; set; }

        

        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/PRCombo/CrewCombo", ComboForeignKeys = "PRCo")]
        [Display(Name = "Crew")]
        public string CrewId { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/PRCombo/ActiveEmployeeCombo", ComboForeignKeys = "PRCo", InfoUrl = "/HumanResource/Resource/PopupForm", InfoForeignKeys = "HRCo=PRCo, EmployeeId")]
        [Display(Name = "Employee")]
        public int? EmployeeId { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/EquipmentLocation/Combo", ComboForeignKeys = "EMCo")]
        [Display(Name = "Location")]
        public string LocationId { get; set; }

        [UIHint("CheckBox")]
        [Display(Name = "Include Crew Leader Assigned Equipment")]
        [Field(LabelSize = 5, TextSize = 1)]
        public bool IncludCrewLeaderEquipment { get; set; }

        [UIHint("CheckBox")]
        [Display(Name = "Include Direct Report Employees Equipment")]
        [Field(LabelSize = 5, TextSize = 1)]
        public bool IncludeDirectReportEmployeeEquipment { get; set; }
        
        [UIHint("CheckBox")]
        [Display(Name = "Include Sub Equipment")]
        [Field(LabelSize = 5, TextSize = 1)]
        public bool IncludeSubEquipment { get; set; }

        public EquipmentAuditActionViewModel Action { get; set; }

    }
}