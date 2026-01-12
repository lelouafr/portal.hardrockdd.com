//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Models.Views.Equipment.Audit;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Runtime.Caching;
//using System.Web.Mvc;

//namespace portal.Models.Views.Equipment
//{
//    public class EquipmentTreeListViewModel_DNU
//    {
//        public EquipmentTreeListViewModel()
//        {
//            List = new List<EquipmentTreeViewModel>();
//        }

//        public EquipmentTreeListViewModel(EquipmentAuditCreateViewModel model)
//        {
//            if (model == null) throw new System.ArgumentNullException(nameof(model));

//            using var db = new VPContext();
//            var list = new List<DB.Infrastructure.ViewPointDB.Data.Equipment>();
//            if (model.AuditType == DB.EMAuditTypeEnum.CrewAudit && model.CrewId != null)
//            {
//                var crew = db.Crews.FirstOrDefault(f => f.PRCo == model.Co && f.CrewId == model.CrewId);

//                var parent = new EquipmentTreeViewModel();
//                parent.AssignedCrewId = crew.CrewId;
//                parent.ParentTypeId = 1;
//                parent.Co = crew.PRCo;
//                parent.Description = crew.Description;

//                list = crew.AssignedEquipment.ToList();

//                if (model.IncludeDirectReportEmployeeEquipment)
//                {
//                    foreach (var emp in crew.CrewLeader.DirectReports)
//                    {
//                        list.AddRange(emp.AssignedEquipment.Where(f => f.Status == "A").ToList());
//                    }
//                }

//                list.AddRange(crew.CrewLeader.AssignedEquipment.Where(f => f.Status == "A").ToList());
//            }
//            else if (model.AuditType == DB.EMAuditTypeEnum.EmployeeAudit && model.EmployeeId != null)
//            {
//                var employee = db.Employees.FirstOrDefault(f => f.PRCo == model.Co && f.EmployeeId == model.EmployeeId);
//                Co = employee.PRCo;
//                Operator = employee.EmployeeId;
//                list = employee.AssignedEquipment.Where(f => f.Status == "A").ToList();

//                if (model.IncludCrewLeaderEquipment)
//                {
//                    //If Employee is Crew Leader grab Assigned Equipment to Crew
//                    if (employee.Crew.CrewLeaderID == employee.EmployeeId)
//                        list.AddRange(employee.Crew.AssignedEquipment.Where(f => f.Status == "A").ToList());

//                }
//                if (model.IncludeDirectReportEmployeeEquipment)
//                {
//                    foreach (var emp in employee.DirectReports)
//                    {
//                        list.AddRange(emp.AssignedEquipment.Where(f => f.Status == "A").ToList());
//                    }
//                }
//            }

//            if (model.IncludeSubEquipment)
//            {
//                foreach (var eqp in list.Where(f => f.SubEquipments.Count > 0).ToList())
//                {
//                    var subList = eqp.SubEquipments.Where(f => f.Status == "A" && !list.Any(a => a.EquipmentId == f.EquipmentId)).ToList();
//                    list.AddRange(subList);
//                    EquipmentSubList(list, subList);
//                }
//            }

//            List = list.Select(s => new EquipmentTreeViewModel(s)).ToList();
//        }

//        private void EquipmentSubList(List<DB.Infrastructure.ViewPointDB.Data.Equipment> currentList, List<DB.Infrastructure.ViewPointDB.Data.Equipment> list)
//        {
//            foreach (var eqp in list.Where(f => f.SubEquipments.Count > 0).ToList())
//            {
//                var subList = eqp.SubEquipments.Where(f => f.Status == "A" && !list.Any(a => a.EquipmentId == f.EquipmentId)).ToList();
//                currentList.AddRange(subList);
//                EquipmentSubList(currentList, subList);
//            }
//        }

//        [Key]
//        public byte Co { get; set; }

//        [UIHint("DropdownBox")]
//        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/PRCombo/ActiveEmployeeCombo", ComboForeignKeys = "Co")]
//        [Display(Name = "Operator")]
//        public int? Operator { get; set; }

//        [Display(Name = "Status")]
//        [UIHint("DropdownBox")]
//        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/EMCombo/StatusCombo", ComboForeignKeys = "")]
//        public string StatusId { get; set; }

//        //DB.EMAssignedStatusEnum
//        [Display(Name = "Assigned Status")]
//        [UIHint("EnumBox")]
//        [Field(LabelSize = 2, TextSize = 4)]
//        public DB.EMAssignedStatusEnum? AssignedStatus { get; set; }


//        [UIHint("DropdownBox")]
//        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/EMCombo/Combo", ComboForeignKeys = "EMCo")]
//        [Display(Name = "Parent")]
//        public string ParentEquimentId { get; set; }
//        //public decimal? Qty { get; set; }

//        [UIHint("DropdownBox")]
//        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/PRCombo/CrewCombo", ComboForeignKeys = "PRCo=Co")]
//        [Display(Name = "Crew")]
//        public string AssignedCrewId { get; set; }

//        public void AddChildren(EquipmentTreeViewModel parent, EquipmentAuditCreateViewModel model, VPContext db)
//        {

//        }

//        public List<EquipmentTreeViewModel> List { get;  }
//    }

//    public class EquipmentTreeViewModel
//    {
//        public EquipmentTreeViewModel()
//        {

//        }

//        public EquipmentTreeViewModel(DB.Infrastructure.ViewPointDB.Data.Equipment equipment)
//        {
//            if (equipment == null)
//            {
//                throw new System.ArgumentNullException(nameof(equipment));
//            }

//            Co = equipment.EMCo;
//            EquipmentId = equipment.EquipmentId;
//            Description = equipment.Description;
//            StatusId = equipment.Status;
//            CategoryId = equipment.CategoryId;
//            Manufacturer = equipment.Manufacturer;
//            EquipModel = equipment.Model;
//            Location = equipment.LocationId;
//            Type = equipment.Type;
//            ModelYr = equipment.ModelYr;
//            VINNumber = equipment.VINNumber;
//            Description = equipment.Description;
//            MeterType = (DB.EMMeterTypeEnum?)(int.TryParse(equipment.MeterTypeId, out int meterTypeOut) ? meterTypeOut : meterTypeOut);            
//            OdoReading = equipment.OdoReading;
//            OdoDate = equipment.OdoDate;
//            ReplacedOdoReading = equipment.ReplacedOdoReading;
//            ReplacedOdoDate = equipment.ReplacedOdoDate;
//            HourReading = equipment.HourReading;
//            HourDate = equipment.HourDate;
//            ReplacedHourReading = equipment.ReplacedHourReading;
//            ReplacedHourDate = equipment.ReplacedHourDate;
//            JobId = equipment.JobId;
//            LicensePlateNo = equipment.LicensePlateNo;
//            LicensePlateState = equipment.LicensePlateState;
//            LicensePlateExpDate = equipment.LicensePlateExpDate;
//            Operator = equipment.OperatorId;
//            //Shop = equipment.Shop;
//            InspectionExpiration = equipment.InspectionExpiration;
//            ParentEquimentId = equipment.ParentEquimentId;

//            AssignedCrewId = equipment.AssignedCrewId;
//            AssignmentType = (DB.EMAssignmentTypeEnum?)equipment.AssignmentType;

//            HasAttachment = StaticFunctions.HasAttachments(equipment.UniqueAttchID);

//            Category = equipment.Category?.Description;
//            Status = StaticFunctions.GetComboValues("EMEquipStatus").FirstOrDefault(f => f.DatabaseValue == equipment.Status)?.DisplayValue;
//            Type = StaticFunctions.GetComboValues("EMEquipType").FirstOrDefault(f => f.DatabaseValue == equipment.Status)?.DisplayValue;

//            AssignedStatus = AssignmentType switch
//            {
//                DB.EMAssignmentTypeEnum.Equipment => ParentEquimentId != null ? DB.EMAssignedStatusEnum.Assigned : DB.EMAssignedStatusEnum.UnAssigned,
//                DB.EMAssignmentTypeEnum.Crew => AssignedCrewId != null ? DB.EMAssignedStatusEnum.Assigned : DB.EMAssignedStatusEnum.UnAssigned,
//                DB.EMAssignmentTypeEnum.Employee => Operator != null ? DB.EMAssignedStatusEnum.Assigned : DB.EMAssignedStatusEnum.UnAssigned,
//                _ => DB.EMAssignedStatusEnum.UnAssigned,
//            };
//        }

//        [Key]
//        [HiddenInput]
//        public byte Co { get; set; }

//        [Key]
//        [UIHint("TextBox")]
//        [Field(LabelSize = 2, TextSize = 4)]
//        [Display(Name = "Equipment")]
//        public string EquipmentId { get; set; }

//        [UIHint("TextBox")]
//        [Field(LabelSize = 0, TextSize = 6)]
//        [Display(Name = "Description")]
//        public string Description { get; set; }

//        [Display(Name = "Status")]
//        [UIHint("DropdownBox")]
//        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/EMCombo/StatusCombo", ComboForeignKeys = "")]
//        public string StatusId { get; set; }

//        [Display(Name = "Status")]
//        [UIHint("TextBox")]
//        [Field(LabelSize = 2, TextSize = 4)]
//        public string Status { get; set; }

//        [HiddenInput]
//        //[UIHint("TextBox")]
//        [Field(LabelSize = 2, TextSize = 4)]
//        [Display(Name = "Location")]
//        public string Location { get; set; }

//        [UIHint("DropdownBox")]
//        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/EMCombo/TypeCombo", ComboForeignKeys = "")]
//        [Display(Name = "Type")]
//        public string TypeId { get; set; }

//        [HiddenInput]
//        //[UIHint("TextBox")]
//        [Field(LabelSize = 2, TextSize = 4)]
//        [Display(Name = "Type")]
//        public string Type { get; set; }

//        [HiddenInput]
//        //[UIHint("DropdownBox")]
//        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/EMCombo/DepartmentCombo", ComboForeignKeys = "Co")]
//        [Display(Name = "Department")]
//        public string Department { get; set; }

//        [UIHint("DropdownBox")]
//        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/EMCombo/CategoryCombo", ComboForeignKeys = "EMCo")]
//        [Display(Name = "Category")]
//        public string CategoryId { get; set; }

//        [HiddenInput]
//        //[UIHint("TextBox")]
//        [Field(LabelSize = 2, TextSize = 4)]
//        [Display(Name = "Category")]
//        public string Category { get; set; }

//        [UIHint("TextBox")]
//        [Field(LabelSize = 2, TextSize = 4)]
//        [Display(Name = "Manufacturer")]
//        public string Manufacturer { get; set; }

//        [UIHint("TextBox")]
//        [Field(LabelSize = 2, TextSize = 4)]
//        [Display(Name = "Model")]
//        public string EquipModel { get; set; }

//        [UIHint("TextBox")]
//        [Field(LabelSize = 2, TextSize = 4)]
//        [Display(Name = "Year")]
//        public string ModelYr { get; set; }

//        [UIHint("TextBox")]
//        [Field(LabelSize = 2, TextSize = 4)]
//        [Display(Name = "VIN/Serial")]
//        public string VINNumber { get; set; }

//        [UIHint("EnumBox")]
//        [Field(LabelSize = 2, TextSize = 4)]
//        [Display(Name = "Meter Type")]
//        public DB.EMMeterTypeEnum? MeterType { get; set; }

//        [UIHint("LongBox")]
//        [Field(LabelSize = 2, TextSize = 4)]
//        [Display(Name = "Odo Reading")]
//        public decimal OdoReading { get; set; }

//        [UIHint("DateBox")]
//        [DataType(DataType.Date)]
//        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
//        [Display(Name = "Odo Date")]
//        public DateTime? OdoDate { get; set; }

//        [HiddenInput]
//        //[UIHint("LongBox")]
//        [Field(LabelSize = 2, TextSize = 4)]
//        [Display(Name = "ReplacedOdoReading")]
//        public decimal ReplacedOdoReading { get; set; }

//        [HiddenInput]
//        //[UIHint("DateBox")]
//        [DataType(DataType.Date)]
//        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
//        [Display(Name = "ReplacedOdoDate")]
//        public DateTime? ReplacedOdoDate { get; set; }

//        [UIHint("LongBox")]
//        [Field(LabelSize = 2, TextSize = 4)]
//        [Display(Name = "Hour Reading")]
//        public decimal HourReading { get; set; }

//        [UIHint("DateBox")]
//        [DataType(DataType.Date)]
//        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
//        [Display(Name = "Hour Date")]
//        public DateTime? HourDate { get; set; }

//        [HiddenInput]
//        //[UIHint("LongBox")]
//        [Field(LabelSize = 2, TextSize = 4)]
//        [Display(Name = "Replaced Hour Reading")]
//        public decimal ReplacedHourReading { get; set; }

//        [HiddenInput]
//        // [UIHint("DateBox")]
//        [DataType(DataType.Date)]
//        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
//        [Display(Name = "ReplacedHour Date")]
//        public DateTime? ReplacedHourDate { get; set; }

//        [HiddenInput]
//        //[UIHint("DropdownBox")]
//        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/JCCombo/JobCombo", ComboForeignKeys = "")]
//        [Display(Name = "Job")]
//        public string JobId { get; set; }

//        [HiddenInput]
//        //[UIHint("TextBox")]
//        [Field(LabelSize = 2, TextSize = 4)]
//        [Display(Name = "VIN/Serial")]
//        public string LicensePlateNo { get; set; }

//        [HiddenInput]
//        //[UIHint("DropdownBox")]
//        [Field(ComboUrl = "/HQCombo/StateCombo", ComboForeignKeys = "")]
//        //[Field(LabelSize = 2, TextSize = 4, ComboUrl = "/State/Combo", ComboForeignKeys = "")]
//        [Display(Name = "State")]
//        public string LicensePlateState { get; set; }

//        [HiddenInput]
//        //[UIHint("DateBox")]
//        [DataType(DataType.Date)]
//        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
//        [Display(Name = "Exp Date")]
//        public DateTime? LicensePlateExpDate { get; set; }

//        [UIHint("DropdownBox")]
//        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/PRCombo/ActiveEmployeeCombo", ComboForeignKeys = "Co")]
//        [Display(Name = "Operator")]
//        public int? Operator { get; set; }

//        //public string Shop { get; set; }

//        [HiddenInput]
//        //[UIHint("DateBox")]
//        [DataType(DataType.Date)]
//        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
//        [Display(Name = "Last Log Date")]
//        public DateTime? LastUsedDate { get; set; }

//        [HiddenInput]
//        //[UIHint("DateBox")]
//        [DataType(DataType.Date)]
//        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
//        [Display(Name = "Inspection Expiration")]
//        public DateTime? InspectionExpiration { get; set; }

//        [UIHint("DropdownBox")]
//        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/EMCombo/Combo", ComboForeignKeys = "EMCo")]
//        [Display(Name = "Parent")]
//        public string ParentEquimentId { get; set; }
//        //public decimal? Qty { get; set; }

//        [UIHint("DropdownBox")]
//        [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/PRCombo/CrewCombo", ComboForeignKeys = "PRCo=Co")]
//        [Display(Name = "Crew")]
//        public string AssignedCrewId { get; set; }


//        [UIHint("EnumBox")]
//        [Field(LabelSize = 2, TextSize = 4)]
//        [Display(Name = "Assignment Type")]
//        public DB.EMAssignmentTypeEnum? AssignmentType { get; set; }

//        [HiddenInput]
//        public string AssignmentTypeString { get { return AssignmentType.ToString(); } }

//        [HiddenInput]
//        public bool HasAttachment { get; set; }

//        [HiddenInput]
//        [UIHint("EnumBox")]
//        [Field(LabelSize = 2, TextSize = 4)]
//        [Display(Name = "Type")]
//        public DB.EMAssignedStatusEnum AssignedStatus { get; set; }

//        public int ParentTypeId { get; set; }

//        [HiddenInput]
//        public string AssignedStatusString { get { return AssignedStatus.ToString(); } }

//        public List<EquipmentTreeViewModel> Children { get; }
//    }
//}