using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace DB
{
    public enum EMServiceDateIntervalEnum
    {
        Days,
        Months,
        Years,
    }

    public enum EMAuditStatusEnum
    {
        New,
        Started,
        Submitted,
        Approved,
        Rejected,
        Completed,
        Processed,
        Canceled,
    }

    public enum EMApprovalStatusEnum
    {
        Open,
        Approved,
        Rejected,
    }

    public enum EMAuditLineActionEnum
    {
        Add,
        Remove,
        Update,
        Transfer
    }

    public enum EMAuditTypeEnum
    {
        //[Display(Name = "Site Audit")]
        //SiteAudit,
        [Display(Name = "Crew Audit")]
        CrewAudit = 0,
        [Display(Name = "Employee Audit")]
        EmployeeAudit = 1,
        [Display(Name = "Location Audit")]
        LocationAudit = 2,

    }
    public enum EMAuditFormEnum
    {
        //[Display(Name = "Site Audit")]
        //SiteAudit,
        [Display(Name = "Meter")]
        Meter = 0,
        [Display(Name = "Inventory")]
        Inventory = 1,

    }

    public enum EMMeterTypeEnum
    {
        Hours,
        Odometer,
        Both,
        None
    }

    public enum EMLogAssignmentType
    {
        Employee,
        Crew,
        Location
    }

    public enum EMLogTypeEnum
    {
        Assignment,
        Unassignment,
        Transfrer,
        Usage,
        MeterUpdate
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum EMAssignedStatusEnum
    {
        Assigned,
        UnAssigned
    }
    public enum EMAssignmentTypeEnum
    {
        Equipment = 1,
        Crew = 2,
        Employee = 3
    }
    public enum EMUsageTransStatusEnum
    {
        New,
        Posted,
    }

}