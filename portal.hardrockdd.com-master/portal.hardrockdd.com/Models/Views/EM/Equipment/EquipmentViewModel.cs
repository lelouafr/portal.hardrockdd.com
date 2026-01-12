using Newtonsoft.Json;
using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Equipment.Audit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Mvc;

namespace portal.Models.Views.Equipment
{
    public class EquipmentListViewModel
    {
        public EquipmentListViewModel()
        {
            List = new List<EquipmentViewModel>();
        }

        public EquipmentListViewModel(List<DB.Infrastructure.ViewPointDB.Data.Equipment> list)
        {
            if (list == null)
            {
                throw new System.ArgumentNullException(nameof(list));
            }

            List = list.Select(s => new EquipmentViewModel(s)).ToList();
            ListJSON = JsonConvert.SerializeObject(List, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default });
           
        }

        public EquipmentListViewModel(EquipmentAuditCreateViewModel model)
        {
            if (model == null) throw new System.ArgumentNullException(nameof(model));

            using var db = new VPContext();
            var list = new List<DB.Infrastructure.ViewPointDB.Data.Equipment>();
            if (model.AuditType == DB.EMAuditTypeEnum.CrewAudit && model.CrewId != null)
            {
                var crew = db.Crews
                    .Include("CrewLeader")
                    .Include("CrewLeader.Crew")
                    .Include("CrewLeader.DirectReports")
                    .Include("CrewLeader.DirectReports.AssignedEquipment")
                    .Include("AssignedEquipment")
                    .FirstOrDefault(f => f.CrewId == model.CrewId);

                Co = crew.PRCo;
                PRCo = crew.PRCo;
                AssignedCrewId = crew.CrewId;
                list = crew.AssignedEquipment.ToList();

                if (model.IncludeDirectReportEmployeeEquipment)
                {
                    foreach (var emp in crew.CrewLeader.DirectReports)
                    {
                        list.AddRange(emp.AssignedEquipment.Where(f => f.Status == "A").ToList());
                    }
                }

                list.AddRange(crew.CrewLeader.AssignedEquipment.Where(f => f.Status == "A").ToList());
            }
            else if (model.AuditType == DB.EMAuditTypeEnum.EmployeeAudit && model.EmployeeId != null)
            {
                var employee = db.Employees
                    .Include("AssignedEquipment")
                    .Include("Crew")
                    .Include("Crew.AssignedEquipment")
                    .Include("DirectReports.AssignedEquipment")
                    .FirstOrDefault(f => f.EmployeeId == model.EmployeeId);
                Co = employee.PRCo;
                Operator = employee.EmployeeId;
                list = employee.AssignedEquipment.Where(f => f.Status == "A").ToList();

                if (model.IncludCrewLeaderEquipment)
                {
                    //If Employee is Crew Leader grab Assigned Equipment to Crew
                    if (employee.Crew?.CrewLeaderId == employee.EmployeeId)
                        list.AddRange(employee.Crew.AssignedEquipment.Where(f => f.Status == "A").ToList());

                }
                if (model.IncludeDirectReportEmployeeEquipment)
                {
                    foreach (var emp in employee.DirectReports)
                    {
                        list.AddRange(emp.AssignedEquipment.Where(f => f.Status == "A").ToList());
                    }
                }
            }

            else if (model.AuditType == DB.EMAuditTypeEnum.LocationAudit && model.LocationId != null)
            {
                var location = db.EMLocations.Include("Equipments").FirstOrDefault(f => f.LocationId == model.LocationId);
                Co = location.EMCo;
                LocationId = location.LocationId;
                list = location.Equipments.Where(f => f.Status == "A").ToList();
            }

            if (model.IncludeSubEquipment)
            {
                foreach (var eqp in list.Where(f => f.SubEquipments.Count > 0).ToList())
                {
                    var subList = eqp.SubEquipments.Where(f => f.Status == "A" && !list.Any(a => a.EquipmentId == f.EquipmentId)).ToList();
                    list.AddRange(subList);
                    EquipmentSubList(list, subList);
                }
            }

            List = list.GroupBy(g => g).Select(s => new EquipmentViewModel(s.Key)).ToList();
            ListJSON = JsonConvert.SerializeObject(List, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default });
        }

        public EquipmentListViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company, string statusId, DB.EMAssignedStatusEnum? assigned)
        {
            if (company == null)
            {
                throw new System.ArgumentNullException(nameof(company));
            }
            Co = company.HQCo;
            AssignedStatus = assigned;

            List = company.EMCompanyParm.Equipments.Where(f => f.Status == statusId || statusId == null).Select(s => new EquipmentViewModel(s)).ToList();
            if (assigned != null)
            {
                List = List.Where(f => f.AssignedStatus == assigned).ToList();
            }
            ListJSON = JsonConvert.SerializeObject(List, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default });

        }
        
        public EquipmentListViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company)
        {
            if (company == null)
            {
                throw new System.ArgumentNullException(nameof(company));
            }
            Co = company.HQCo;
            //AssignedStatus = assigned;

            List = company.EMCompanyParm.Equipments.Select(s => new EquipmentViewModel(s)).ToList();
            ListJSON = JsonConvert.SerializeObject(List, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default });
            //if (assigned != null)
            //{
            //    List = List.Where(f => f.AssignedStatus == assigned).ToList();
            //}

        }


        public EquipmentListViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company, List<DB.Infrastructure.ViewPointDB.Data.Equipment> list)
        {
            if (company == null)
                throw new System.ArgumentNullException(nameof(company));

            Co = company.HQCo;

            List = list.Select(s => new EquipmentViewModel(s)).ToList();
            
        }

        public EquipmentListViewModel(Crew crew, string statusId, DB.EMAssignedStatusEnum? assigned)
        {
            if (crew == null)
            {
                throw new System.ArgumentNullException(nameof(crew));
            }
            Co = crew.PRCo;
            List = crew.AssignedEquipment.Select(s => new EquipmentViewModel(s)).ToList();
            ListJSON = JsonConvert.SerializeObject(List, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default });
        }

        public EquipmentListViewModel(EMLocation loc)
        {
            if (loc == null)
            {
                throw new System.ArgumentNullException(nameof(loc));
            }
            Co = loc.EMCo;
            List = loc.Equipments.Select(s => new EquipmentViewModel(s)).ToList();
            ListJSON = JsonConvert.SerializeObject(List, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default });
        }

        public EquipmentListViewModel(EquipmentCategory loc)
        {
            if (loc == null)
            {
                throw new System.ArgumentNullException(nameof(loc));
            }
            Co = loc.EMCo;
            List = loc.Equipments.Select(s => new EquipmentViewModel(s)).ToList();
            ListJSON = JsonConvert.SerializeObject(List, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default });
        }

        public EquipmentListViewModel(DB.Infrastructure.ViewPointDB.Data.Employee employee)
        {
            if (employee == null)
            {
                throw new System.ArgumentNullException(nameof(employee));
            }
            Co = employee.PRCo;
            PRCo = employee.PRCo;
            Operator = employee.EmployeeId;
            AssignedStatus = DB.EMAssignedStatusEnum.Assigned;

            //Grab directly assigned Equipment to Employee
            var list = employee.AssignedEquipment.Where(f => f.Status == "A").ToList();


            //If Employee is Crew Leader grab Assigned Equipment to Crew
            if (employee.Crew.CrewLeaderId == employee.EmployeeId)
                list.AddRange(employee.Crew.AssignedEquipment.Where(f => f.Status == "A").ToList());

            foreach (var eqp in list.Where(f => f.SubEquipments.Count > 0).ToList())
            {
                var subList = eqp.SubEquipments.Where(f => f.Status == "A" && !list.Any(a => a.EquipmentId == f.EquipmentId)).ToList();
                list.AddRange(subList);
                EquipmentSubList(list, subList);
            }

            List = list.Select(s => new EquipmentViewModel(s)).ToList();
            ListJSON = JsonConvert.SerializeObject(List, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default });

        }
        
        private void EquipmentSubList(List<DB.Infrastructure.ViewPointDB.Data.Equipment> currentList, List<DB.Infrastructure.ViewPointDB.Data.Equipment> list)
        {
            foreach (var eqp in list.Where(f => f.SubEquipments.Count > 0).ToList())
            {
                var subList = eqp.SubEquipments.Where(f => f.Status == "A" && !list.Any(a => a.EquipmentId == f.EquipmentId)).ToList();
                currentList.AddRange(subList);
                EquipmentSubList(currentList, subList);
            }
        }


        [Key]
        public byte Co { get; set; }
        public byte? PRCo { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/PRCombo/ActiveEmployeeCombo", ComboForeignKeys = "PRCo")]
        [Display(Name = "Operator")]
        public int? Operator { get; set; }

        [Display(Name = "Status")]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/EMCombo/StatusCombo", ComboForeignKeys = "")]
        public string StatusId { get; set; }

        //DB.EMAssignedStatusEnum
        [Display(Name = "Assigned Status")]
        [UIHint("EnumBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        public DB.EMAssignedStatusEnum? AssignedStatus { get; set; }


        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/EMCombo/Combo", ComboForeignKeys = "EMCo")]
        [Display(Name = "Parent")]
        public string ParentEquimentId { get; set; }
        //public decimal? Qty { get; set; }


        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/PRCombo/CrewCombo", ComboForeignKeys = "PRCo")]
        [Display(Name = "Crew")]
        public string AssignedCrewId { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/EquipmentLocation/Combo", ComboForeignKeys = "Co")]
        [Display(Name = "Location Id")]
        public string LocationId { get; set; }

        public List<EquipmentViewModel> List { get;  }

        public string ListJSON { get; }

        
    }

    public class EquipmentViewModel
    {
        public EquipmentViewModel()
        {

        }
        
        public EquipmentViewModel(DB.Infrastructure.ViewPointDB.Data.Equipment equipment)
        {
            if (equipment == null)
            {
                throw new System.ArgumentNullException(nameof(equipment));
            }

            EMCo = equipment.EMCo;
            PRCo = equipment.PRCo;
            JCCo = equipment.JCCo;

            EquipmentId = equipment.EquipmentId;
            Description = equipment.Description;
            StatusId = equipment.Status;
            CategoryId = equipment.CategoryId;
            Manufacturer = equipment.Manufacturer;
            EquipModel = equipment.Model;
            Location = equipment.Location == null ? "Unknown" : equipment.Location.Description;
            
            Type = equipment.Type;
            ModelYr = equipment.ModelYr;
            VINNumber = equipment.VINNumber;
            Description = equipment.Description;
            MeterType = (DB.EMMeterTypeEnum?)(int.TryParse(equipment.MeterTypeId, out int meterTypeOut) ? meterTypeOut : meterTypeOut);            
            OdoReading = equipment.OdoReading;
            OdoDate = equipment.OdoDate;
            ReplacedOdoReading = equipment.ReplacedOdoReading;
            ReplacedOdoDate = equipment.ReplacedOdoDate;
            HourReading = equipment.HourReading;
            HourDate = equipment.HourDate;
            ReplacedHourReading = equipment.ReplacedHourReading;
            ReplacedHourDate = equipment.ReplacedHourDate;
            JobId = equipment.JobId;
            LicensePlateNo = equipment.LicensePlateNo;
            LicensePlateState = equipment.LicensePlateState;
            LicensePlateExpDate = equipment.LicensePlateExpDate;
            Operator = equipment.OperatorId;
            //Shop = equipment.Shop;
            InspectionExpiration = equipment.InspectionExpiration;
            InspectionExpirationStr = InspectionExpiration?.ToShortDateString();
            ParentEquimentId = equipment.ParentEquimentId;

            AssignedCrewId = equipment.AssignedCrewId;
            AssignmentType = (DB.EMAssignmentTypeEnum?)equipment.AssignmentType;

            HasAttachment = StaticFunctions.HasAttachments(equipment.UniqueAttchID);

            CompanyName = equipment.EMCompanyParm.HQCompanyParm.CompanyLabel ?? equipment.EMCompanyParm.HQCompanyParm.Name;
            Category = equipment.Category?.Description;
            Status = StaticFunctions.GetComboValues("EMEquipStatus").FirstOrDefault(f => f.DatabaseValue == equipment.Status)?.DisplayValue;
            Type = StaticFunctions.GetComboValues("EMEquipType").FirstOrDefault(f => f.DatabaseValue == equipment.Status)?.DisplayValue;

            AssignedStatus = AssignmentType switch
            {
                DB.EMAssignmentTypeEnum.Equipment => ParentEquimentId != null ? DB.EMAssignedStatusEnum.Assigned : DB.EMAssignedStatusEnum.UnAssigned,
                DB.EMAssignmentTypeEnum.Crew => AssignedCrewId != null ? DB.EMAssignedStatusEnum.Assigned : DB.EMAssignedStatusEnum.UnAssigned,
                DB.EMAssignmentTypeEnum.Employee => Operator != null ? DB.EMAssignedStatusEnum.Assigned : DB.EMAssignedStatusEnum.UnAssigned,
                _ => DB.EMAssignedStatusEnum.UnAssigned,
            };

            
            if (AssignmentType == DB.EMAssignmentTypeEnum.Crew)
            {
                AssignedTo = equipment.Crew?.CrewLeader?.FullName(false);
            }
            else if (AssignmentType == DB.EMAssignmentTypeEnum.Employee)
            {
                AssignedTo = equipment.Operator?.FullName(false);
            }
            else
            {
                AssignedTo = "UnAssigned";
            }

            AssignedTo ??= "UnAssigned";
            if (AssignedStatus == DB.EMAssignedStatusEnum.UnAssigned)
            {
                if (Location != null)
                {
                    AssignedStatus = DB.EMAssignedStatusEnum.Assigned;
                }
            }
        }

        [Key]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/HQCombo/HQCompanyCombo", ComboForeignKeys = "")]
        public byte EMCo { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Company")]
        public string CompanyName { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Equipment")]
        public string EquipmentId { get; set; }

        public byte? PRCo { get; set; }

        public byte? JCCo { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 0, TextSize = 6)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Status")]
        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/EMCombo/StatusCombo", ComboForeignKeys = "")]
        public string StatusId { get; set; }

        [Display(Name = "Status")]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        public string Status { get; set; }

        [HiddenInput]
        //[UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Location")]
        public string Location { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/EMCombo/TypeCombo", ComboForeignKeys = "")]
        [Display(Name = "Type")]
        public string TypeId { get; set; }

        [HiddenInput]
        //[UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Type")]
        public string Type { get; set; }

        [HiddenInput]
        //[UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/EMCombo/DepartmentCombo", ComboForeignKeys = "EMCo")]
        [Display(Name = "Department")]
        public string Department { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/EMCombo/CategoryCombo", ComboForeignKeys = "EMCo")]
        [Display(Name = "Category")]
        public string CategoryId { get; set; }

        [HiddenInput]
        //[UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Category")]
        public string Category { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Manufacturer")]
        public string Manufacturer { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Model")]
        public string EquipModel { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Year")]
        public string ModelYr { get; set; }

        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "VIN/Serial")]
        public string VINNumber { get; set; }

        [UIHint("EnumBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Meter Type")]
        public DB.EMMeterTypeEnum? MeterType { get; set; }

        [UIHint("LongBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Odo Reading")]
        public decimal OdoReading { get; set; }

        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Odo Date")]
        public DateTime? OdoDate { get; set; }

        [HiddenInput]
        //[UIHint("LongBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "ReplacedOdoReading")]
        public decimal ReplacedOdoReading { get; set; }

        [HiddenInput]
        //[UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "ReplacedOdoDate")]
        public DateTime? ReplacedOdoDate { get; set; }

        [UIHint("LongBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Hour Reading")]
        public decimal HourReading { get; set; }

        [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Hour Date")]
        public DateTime? HourDate { get; set; }

        [HiddenInput]
        //[UIHint("LongBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Replaced Hour Reading")]
        public decimal ReplacedHourReading { get; set; }

        [HiddenInput]
        // [UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "ReplacedHour Date")]
        public DateTime? ReplacedHourDate { get; set; }

        [HiddenInput]
        //[UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/JCCombo/JobCombo", ComboForeignKeys = "JCCo")]
        [Display(Name = "Job")]
        public string JobId { get; set; }

        [HiddenInput]
        //[UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Lic Number")]
        public string LicensePlateNo { get; set; }

        [HiddenInput]
        //[UIHint("DropdownBox")]
        [Field(ComboUrl = "/HQCombo/StateCombo", ComboForeignKeys = "")]
        //[Field(LabelSize = 2, TextSize = 4, ComboUrl = "/State/Combo", ComboForeignKeys = "")]
        [Display(Name = "State")]
        public string LicensePlateState { get; set; }

        [HiddenInput]
        //[UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Reg/Lic Exp Date")]
        public DateTime? LicensePlateExpDate { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/PRCombo/ActiveEmployeeCombo", ComboForeignKeys = "PRCo")]
        [Display(Name = "Operator")]
        public int? Operator { get; set; }

        //public string Shop { get; set; }

        [HiddenInput]
        //[UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Last Log Date")]
        public DateTime? LastUsedDate { get; set; }

        [HiddenInput]
        //[UIHint("DateBox")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Inspection Expiration")]
        public DateTime? InspectionExpiration { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Inspection Expiration")]
        public string InspectionExpirationStr { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/EMCombo/Combo", ComboForeignKeys = "EMCo")]
        [Display(Name = "Parent")]
        public string ParentEquimentId { get; set; }
        //public decimal? Qty { get; set; }

        [UIHint("DropdownBox")]
        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/PRCombo/CrewCombo", ComboForeignKeys = "PRCo")]
        [Display(Name = "Crew")]
        public string AssignedCrewId { get; set; }


        [UIHint("EnumBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Assignment Type")]
        public DB.EMAssignmentTypeEnum? AssignmentType { get; set; }

        [HiddenInput]
        public string AssignmentTypeString { get { return AssignmentType.ToString(); } }

        [HiddenInput]
        public bool HasAttachment { get; set; }

        [HiddenInput]
        [UIHint("EnumBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "Type")]
        public DB.EMAssignedStatusEnum AssignedStatus { get; set; }

        [HiddenInput]
        public string AssignedStatusString { get { return AssignedStatus.ToString(); } }


        [Display(Name = "Assigned To")]
        public string AssignedTo { get; set; }
    }
}