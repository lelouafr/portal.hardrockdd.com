using System.ComponentModel.DataAnnotations;

namespace DB
{
    public enum EntryTypeEnum
    {
        [Display(Name = "Admin/Office")]
        Admin,
        [Display(Name = "Job")]
        Job,
        [Display(Name = "Equipment (Non WO)")]
        Equipment,
        //[Display(Name = "Work Order")]
        //WorkOrder,
    }

    public enum PerdiemEnum
    {
        No,
        Yes,
        Cabin
    }

    public enum DailyTicketStatusEnum
    {
        Draft,
        Submitted,
        Approved,
        Processed,
        Rejected,
        Canceled,
        Deleted,
        UnPosted,
        Reviewed,
        Report,
    }
    public enum EmployeeEntryStatusEnum
    {
        Pending,
        Approved,
        Processed,
        Posted,
        Empty,
    }

    public enum DTFormEnum
    {
        JobFieldTicket = 1,
        ProjectManager = 2,
        TruckingTicket = 3,
        EmployeeDetailTicket = 4,
        EmployeeTicket = 5,
        ShopTicket = 6,
        SubContractorTicket = 7,
        CrewTicket = 8,
        TimeOff = 9,
        PayrollEntriesTicket = 10,
        HolidayTicket = 11,
    }
}