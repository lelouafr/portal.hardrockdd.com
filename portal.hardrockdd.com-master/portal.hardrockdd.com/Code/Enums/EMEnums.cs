//using Newtonsoft.Json;
//using Newtonsoft.Json.Converters;
//using System.ComponentModel.DataAnnotations;

//namespace portal
//{
//    public enum DB.EMServiceDateIntervalEnum
//    {
//        Days,
//        Months,
//        Years,
//    }

//    public enum DB.EMAuditStatusEnum
//    {
//        New,
//        Started,
//        Submitted,
//        Approved,
//        Rejected,
//        Completed,
//        Processed,
//        Canceled,
//    }

//    public enum EMApprovalStatusEnum
//    {
//        Open,
//        Approved,
//        Rejected,
//    }

//    public enum DB.EMAuditLineActionEnum
//    {
//        Add,
//        Remove,
//        Update,
//        Transfer
//    }

//    public enum DB.EMAuditTypeEnum
//    {
//        //[Display(Name = "Site Audit")]
//        //SiteAudit,
//        [Display(Name = "Crew Audit")]
//        CrewAudit = 0,
//        [Display(Name = "Employee Audit")]
//        EmployeeAudit = 1,
//        [Display(Name = "Location Audit")]
//        LocationAudit = 2,

//    }
//    public enum DB.EMAuditFormEnum
//    {
//        //[Display(Name = "Site Audit")]
//        //SiteAudit,
//        [Display(Name = "Meter")]
//        Meter = 0,
//        [Display(Name = "Inventory")]
//        Inventory = 1,

//    }

//    public enum DB.EMMeterTypeEnum
//    {
//        Hours,
//        Odometer,
//        Both,
//        None
//    }

//    public enum DB.EMLogAssignmentType
//    {
//        Employee,
//        Crew,
//        Location
//    }

//    public enum DB.EMLogTypeEnum
//    {
//        Assignment,
//        Unassignment,
//        Transfrer,
//        Usage,
//        MeterUpdate
//    }

//    [JsonConverter(typeof(StringEnumConverter))]
//    public enum DB.EMAssignedStatusEnum
//    {
//        Assigned,
//        UnAssigned
//    }
//    public enum DB.EMAssignmentTypeEnum
//    {
//        Equipment = 1,
//        Crew = 2,
//        Employee = 3
//    }
//    public enum DB.EMUsageTransStatusEnum
//    {
//        New,
//        Posted,
//    }

//}