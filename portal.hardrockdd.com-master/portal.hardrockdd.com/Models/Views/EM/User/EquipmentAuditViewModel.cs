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
    public class EquipmentAssignedListViewModel
    {
        public EquipmentAssignedListViewModel()
        {
            List = new List<EquipmentViewModel>();
        }

        public EquipmentAssignedListViewModel(DB.Infrastructure.ViewPointDB.Data.HQCompanyParm company, string statusId, DB.EMAssignedStatusEnum? assigned)
        {
            if (company == null)
            {
                throw new System.ArgumentNullException(nameof(company));
            }
            PRCo = company.PRCo;
            EMCo = company.EMCo;
            AssignedStatus = assigned;

            List = company.EMCompanyParm.Equipments.Where(f => f.Status == statusId || statusId == null).Select(s => new EquipmentViewModel(s)).ToList();
            if (assigned != null)
            {
                List = List.Where(f => f.AssignedStatus == assigned).ToList();
            }
        }

        public EquipmentAssignedListViewModel(EquipmentAuditCreateViewModel createModel)
        {
            if (createModel == null)
            {
                throw new System.ArgumentNullException(nameof(createModel));
            }
            PRCo = 1;
            EMCo = createModel.EMCo;

            AssignedCrewId = createModel.CrewId;
            using var db = new VPContext();
            var list = new List<DB.Infrastructure.ViewPointDB.Data.Equipment>();
            if (createModel.AuditType == DB.EMAuditTypeEnum.CrewAudit)
            {

                var crew = db.Crews.FirstOrDefault(f => f.PRCo == PRCo && f.CrewId == createModel.CrewId);
                list = crew.CrewLeader.AssignedEquipment.Where(f => f.Status == "A").ToList();
                list.AddRange(crew.AssignedEquipment.Where(f => f.Status == "A").ToList());

                if (createModel.IncludeDirectReportEmployeeEquipment)
                {
                    foreach (var subEmp in crew.CrewLeader.DirectReports.Where(f => f.ActiveYN == "Y").ToList())
                    {
                        list.AddRange(subEmp.AssignedEquipment.Where(f => f.Status == "A").ToList());
                    }
                }
            }
            else if (createModel.AuditType == DB.EMAuditTypeEnum.EmployeeAudit)
            {
                var employee = db.Employees.FirstOrDefault(f => f.PRCo == PRCo && f.EmployeeId == createModel.EmployeeId);
                list = employee.AssignedEquipment.Where(f => f.Status == "A").ToList();
                if (employee.Crew.CrewLeaderId == employee.EmployeeId)
                    list.AddRange(employee.Crew.AssignedEquipment.Where(f => f.Status == "A").ToList());
            }

            if (createModel.IncludeSubEquipment)
            {
                foreach (var eqp in list.Where(f => f.SubEquipments.Count > 0).ToList())
                {
                    var subList = eqp.SubEquipments.Where(f => f.Status == "A" && !list.Any(a => a.EquipmentId == f.EquipmentId)).ToList();
                    list.AddRange(subList);
                    EquipmentSubList(list, subList);
                }
            }

        }

        public EquipmentAssignedListViewModel(Crew crew)
        {
            if (crew == null)
            {
                throw new System.ArgumentNullException(nameof(crew));
            }

            EMCo = crew.PRCo;
            PRCo = crew.PRCo;
            AssignedCrewId = crew.CrewId;

            //Grab directly assigned Equipment to Employee
            var list = crew.CrewLeader.AssignedEquipment.Where(f => f.Status == "A").ToList();
            list.AddRange(crew.AssignedEquipment.Where(f => f.Status == "A").ToList());

            foreach (var eqp in list.Where(f => f.SubEquipments.Count > 0).ToList())
            {
                var subList = eqp.SubEquipments.Where(f => f.Status == "A" && !list.Any(a => a.EquipmentId == f.EquipmentId)).ToList();
                list.AddRange(subList);
                EquipmentSubList(list, subList);
            }
            List = list.Select(s => new EquipmentViewModel(s)).ToList();
        }

        public EquipmentAssignedListViewModel(DB.Infrastructure.ViewPointDB.Data.Employee employee)
        {
            if (employee == null)
            {
                throw new System.ArgumentNullException(nameof(employee));
            }
            EMCo = employee.PRCo;
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
        public byte? EMCo { get; set; }

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

        public List<EquipmentViewModel> List { get; }
    }

    //public class EquipmentAuditViewModel
    //{
    //    public EquipmentAuditViewModel()
    //    {

    //    }

    //    public EquipmentAuditViewModel(DB.Infrastructure.ViewPointDB.Data.Equipment equipment)
    //    {
    //        if (equipment == null)
    //        {
    //            throw new System.ArgumentNullException(nameof(equipment));
    //        }

    //        Co = equipment.EMCo;
    //        EquipmentId = equipment.EquipmentId;
    //        Description = equipment.Description;
    //        StatusId = equipment.Status;
    //        //CategoryId = equipment.CategoryId;
    //        Manufacturer = equipment.Manufacturer;
    //        EquipModel = equipment.Model;
    //        Location = equipment.LocationId;
    //        Type = equipment.Type;
    //        ModelYr = equipment.ModelYr;
    //        VINNumber = equipment.VINNumber;
    //        Description = equipment.Description;
    //        MeterType = (DB.EMMeterTypeEnum?)(int.TryParse(equipment.MeterTypeId, out int meterTypeOut) ? meterTypeOut : meterTypeOut);            
    //        OdoReading = equipment.OdoReading;
    //        OdoDate = equipment.OdoDate;
    //        ReplacedOdoReading = equipment.ReplacedOdoReading;
    //        ReplacedOdoDate = equipment.ReplacedOdoDate;
    //        HourReading = equipment.HourReading;
    //        HourDate = equipment.HourDate;
    //        ReplacedHourReading = equipment.ReplacedHourReading;
    //        ReplacedHourDate = equipment.ReplacedHourDate;
    //        JobId = equipment.JobId;
    //        LicensePlateNo = equipment.LicensePlateNo;
    //        LicensePlateState = equipment.LicensePlateState;
    //        LicensePlateExpDate = equipment.LicensePlateExpDate;
    //        Operator = equipment.OperatorId;
    //        //Shop = equipment.Shop;
    //        InspectionExpiration = equipment.InspectionExpiration;
    //        ParentEquimentId = equipment.ParentEquimentId;

    //        AssignedCrewId = equipment.AssignedCrewId;
    //        AssignmentType = (DB.EMAssignmentTypeEnum?)equipment.AssignmentType;

    //        HasAttachment = StaticFunctions.HasAttachments(equipment.UniqueAttchID);

    //        //Category = equipment.Category?.Description;
    //        Status = StaticFunctions.GetComboValues("EMEquipStatus").FirstOrDefault(f => f.DatabaseValue == equipment.Status)?.DisplayValue;
    //        Type = StaticFunctions.GetComboValues("EMEquipType").FirstOrDefault(f => f.DatabaseValue == equipment.Status)?.DisplayValue;

    //        AssignedStatus = AssignmentType switch
    //        {
    //            DB.EMAssignmentTypeEnum.Equipment => ParentEquimentId != null ? DB.EMAssignedStatusEnum.Assigned : DB.EMAssignedStatusEnum.UnAssigned,
    //            DB.EMAssignmentTypeEnum.Crew => AssignedCrewId != null ? DB.EMAssignedStatusEnum.Assigned : DB.EMAssignedStatusEnum.UnAssigned,
    //            DB.EMAssignmentTypeEnum.Employee => Operator != null ? DB.EMAssignedStatusEnum.Assigned : DB.EMAssignedStatusEnum.UnAssigned,
    //            _ => DB.EMAssignedStatusEnum.UnAssigned,
    //        };
    //    }

    //    [Key]
    //    [HiddenInput]
    //    public byte Co { get; set; }

    //    [Key]
    //    [UIHint("TextBox")]
    //    [Field(LabelSize = 2, TextSize = 4)]
    //    [Display(Name = "Equipment")]
    //    public string EquipmentId { get; set; }

    //    [UIHint("TextBox")]
    //    [Field(LabelSize = 0, TextSize = 6)]
    //    [Display(Name = "Description")]
    //    public string Description { get; set; }

    //    [Display(Name = "Status")]
    //    [UIHint("DropdownBox")]
    //    [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/EMCombo/StatusCombo", ComboForeignKeys = "")]
    //    public string StatusId { get; set; }

    //    [Display(Name = "Status")]
    //    [UIHint("TextBox")]
    //    [Field(LabelSize = 2, TextSize = 4)]
    //    public string Status { get; set; }

    //    [HiddenInput]
    //    //[UIHint("TextBox")]
    //    [Field(LabelSize = 2, TextSize = 4)]
    //    [Display(Name = "Location")]
    //    public string Location { get; set; }

    //    [UIHint("DropdownBox")]
    //    [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/EMCombo/TypeCombo", ComboForeignKeys = "")]
    //    [Display(Name = "Type")]
    //    public string TypeId { get; set; }

    //    [HiddenInput]
    //    //[UIHint("TextBox")]
    //    [Field(LabelSize = 2, TextSize = 4)]
    //    [Display(Name = "Type")]
    //    public string Type { get; set; }

    //    [UIHint("TextBox")]
    //    [Field(LabelSize = 2, TextSize = 4)]
    //    [Display(Name = "Manufacturer")]
    //    public string Manufacturer { get; set; }

    //    [UIHint("TextBox")]
    //    [Field(LabelSize = 2, TextSize = 4)]
    //    [Display(Name = "Model")]
    //    public string EquipModel { get; set; }

    //    [UIHint("TextBox")]
    //    [Field(LabelSize = 2, TextSize = 4)]
    //    [Display(Name = "Year")]
    //    public string ModelYr { get; set; }

    //    [UIHint("TextBox")]
    //    [Field(LabelSize = 2, TextSize = 4)]
    //    [Display(Name = "VIN/Serial")]
    //    public string VINNumber { get; set; }

    //    [UIHint("EnumBox")]
    //    [Field(LabelSize = 2, TextSize = 4)]
    //    [Display(Name = "Meter Type")]
    //    public DB.EMMeterTypeEnum? MeterType { get; set; }

    //    [UIHint("LongBox")]
    //    [Field(LabelSize = 2, TextSize = 4)]
    //    [Display(Name = "Odo Reading")]
    //    public decimal OdoReading { get; set; }

    //    [UIHint("DateBox")]
    //    [DataType(DataType.Date)]
    //    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    //    [Display(Name = "Odo Date")]
    //    public DateTime? OdoDate { get; set; }

    //    [HiddenInput]
    //    //[UIHint("LongBox")]
    //    [Field(LabelSize = 2, TextSize = 4)]
    //    [Display(Name = "ReplacedOdoReading")]
    //    public decimal ReplacedOdoReading { get; set; }

    //    [HiddenInput]
    //    //[UIHint("DateBox")]
    //    [DataType(DataType.Date)]
    //    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    //    [Display(Name = "ReplacedOdoDate")]
    //    public DateTime? ReplacedOdoDate { get; set; }

    //    [UIHint("LongBox")]
    //    [Field(LabelSize = 2, TextSize = 4)]
    //    [Display(Name = "Hour Reading")]
    //    public decimal HourReading { get; set; }

    //    [UIHint("DateBox")]
    //    [DataType(DataType.Date)]
    //    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    //    [Display(Name = "Hour Date")]
    //    public DateTime? HourDate { get; set; }

    //    [HiddenInput]
    //    //[UIHint("LongBox")]
    //    [Field(LabelSize = 2, TextSize = 4)]
    //    [Display(Name = "Replaced Hour Reading")]
    //    public decimal ReplacedHourReading { get; set; }

    //    [HiddenInput]
    //    // [UIHint("DateBox")]
    //    [DataType(DataType.Date)]
    //    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    //    [Display(Name = "ReplacedHour Date")]
    //    public DateTime? ReplacedHourDate { get; set; }

    //    [HiddenInput]
    //    //[UIHint("DropdownBox")]
    //    [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/JCCombo/JobCombo", ComboForeignKeys = "")]
    //    [Display(Name = "Job")]
    //    public string JobId { get; set; }

    //    [HiddenInput]
    //    //[UIHint("TextBox")]
    //    [Field(LabelSize = 2, TextSize = 4)]
    //    [Display(Name = "VIN/Serial")]
    //    public string LicensePlateNo { get; set; }

    //    [HiddenInput]
    //    //[UIHint("DropdownBox")]
    //    [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/State/Combo", ComboForeignKeys = "")]
    //    [Display(Name = "State")]
    //    public string LicensePlateState { get; set; }

    //    [HiddenInput]
    //    //[UIHint("DateBox")]
    //    [DataType(DataType.Date)]
    //    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    //    [Display(Name = "Exp Date")]
    //    public DateTime? LicensePlateExpDate { get; set; }

    //    [UIHint("DropdownBox")]
    //    [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/PRCombo/ActiveEmployeeCombo", ComboForeignKeys = "Co")]
    //    [Display(Name = "Operator")]
    //    public int? Operator { get; set; }

    //    //public string Shop { get; set; }

    //    [HiddenInput]
    //    //[UIHint("DateBox")]
    //    [DataType(DataType.Date)]
    //    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    //    [Display(Name = "Last Log Date")]
    //    public DateTime? LastUsedDate { get; set; }

    //    [HiddenInput]
    //    //[UIHint("DateBox")]
    //    [DataType(DataType.Date)]
    //    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    //    [Display(Name = "Inspection Expiration")]
    //    public DateTime? InspectionExpiration { get; set; }

    //    [UIHint("DropdownBox")]
    //    [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/EMCombo/Combo", ComboForeignKeys = "Co")]
    //    [Display(Name = "Parent")]
    //    public string ParentEquimentId { get; set; }
    //    //public decimal? Qty { get; set; }

    //    [UIHint("DropdownBox")]
    //    [Field(LabelSize = 2, TextSize = 4, ComboUrl = "/PRCombo/CrewCombo", ComboForeignKeys = "Co")]
    //    [Display(Name = "Crew")]
    //    public string AssignedCrewId { get; set; }


    //    [UIHint("EnumBox")]
    //    [Field(LabelSize = 2, TextSize = 4)]
    //    [Display(Name = "Assignment Type")]
    //    public DB.EMAssignmentTypeEnum? AssignmentType { get; set; }

    //    [HiddenInput]
    //    public string AssignmentTypeString { get { return AssignmentType.ToString(); } }

    //    [HiddenInput]
    //    public bool HasAttachment { get; set; }

    //    [HiddenInput]
    //    [UIHint("EnumBox")]
    //    [Field(LabelSize = 2, TextSize = 4)]
    //    [Display(Name = "Type")]
    //    public DB.EMAssignedStatusEnum AssignedStatus { get; set; }

    //    [HiddenInput]
    //    public string AssignedStatusString { get { return AssignedStatus.ToString(); } }

    //    //public string CrossReferenceUsage { get; set; }
    //    //public int? AssignedEmployeeId { get; set; }

    //    //public byte? MatlGroup { get; set; }
    //    //public string FuelMatlCode { get; set; }
    //    //public decimal FuelCapacity { get; set; }
    //    //public string FuelCapUM { get; set; }
    //    //public decimal FuelUsed { get; set; }
    //    //public byte? EMGroup { get; set; }
    //    //public string FuelCostCode { get; set; }
    //    //public byte? FuelCostType { get; set; }
    //    //public DateTime? LastFuelDate { get; set; }
    //    //public string AttachToEquip { get; set; }
    //    //public string AttachPostRevenue { get; set; }
    //    //public byte? JCCo { get; set; }


    //    //public byte? PhaseGrp { get; set; }
    //    //public byte? UsageCostType { get; set; }
    //    //public string WeightUM { get; set; }
    //    //public decimal WeightCapacity { get; set; }
    //    //public string VolumeUM { get; set; }
    //    //public decimal VolumeCapacity { get; set; }
    //    //public string Capitalized { get; set; }


    //    //public string IRPFleet { get; set; }
    //    //public string CompOfEquip { get; set; }
    //    //public string ComponentTypeCode { get; set; }
    //    //public string CompUpdateHrs { get; set; }
    //    //public string CompUpdateMiles { get; set; }
    //    //public string CompUpdateFuel { get; set; }
    //    //public string PostCostToComp { get; set; }
    //    //public byte? PRCo { get; set; }
    //    //public decimal GrossVehicleWeight { get; set; }
    //    //public decimal TareWeight { get; set; }
    //    //public string Height { get; set; }
    //    //public string Wheelbase { get; set; }
    //    //public byte NoAxles { get; set; }
    //    //public string Width { get; set; }
    //    //public string OverallLength { get; set; }
    //    //public string HorsePower { get; set; }
    //    //public string TireSize { get; set; }
    //    //public string OwnershipStatus { get; set; }
    //    //public DateTime? InServiceDate { get; set; }
    //    //public short ExpLife { get; set; }
    //    //public decimal ReplCost { get; set; }
    //    //public decimal CurrentAppraisal { get; set; }
    //    //public DateTime? SoldDate { get; set; }
    //    //public decimal SalePrice { get; set; }
    //    //public string PurchasedFrom { get; set; }
    //    //public decimal PurchasePrice { get; set; }
    //    //public DateTime? PurchDate { get; set; }
    //    //public byte? APCo { get; set; }
    //    //public byte? VendorGroup { get; set; }
    //    //public int?LeasedFrom { get; set; }
    //    //public DateTime? LeaseStartDate { get; set; }
    //    //public DateTime? LeaseEndDate { get; set; }
    //    //public decimal LeasePayment { get; set; }
    //    //public decimal LeaseResidualValue { get; set; }
    //    //public byte? ARCo { get; set; }
    //    //public byte? CustGroupId { get; set; }
    //    //public int?CustomerId { get; set; }
    //    //public string CustEquipNo { get; set; }
    //    //public string MSTruckType { get; set; }
    //    //public string RevenueCodeId { get; set; }
    //    //public string Notes { get; set; }
    //    //public string MechanicNotes { get; set; }
    //    //public DateTime? JobDate { get; set; }
    //    //public string FuelType { get; set; }
    //    //public string UpdateYN { get; set; }
    //    //public byte ShopGroup { get; set; }
    //    //public string IFTAState { get; set; }
    //    //public Guid? UniqueAttchID { get; set; }
    //    //public long KeyID { get; set; }
    //    //public string ChangeInProgress { get; set; }
    //    //public string LastUsedEquipmentCode { get; set; }
    //    //public DateTime? LastEquipmentChangeDate { get; set; }
    //    //public string LastEquipmentChangeUser { get; set; }
    //    //public int?EquipmentCodeChanges { get; set; }
    //    //public string OriginalEquipmentCode { get; set; }
    //    //public string ExpLifeTimeFrame { get; set; }
    //    //public decimal? RegisteredGVWR { get; set; }
    //    //public decimal? OriginalPurchasePrice { get; set; }
    //    //public DateTime? OrigPurchaseDate { get; set; }
    //}
}