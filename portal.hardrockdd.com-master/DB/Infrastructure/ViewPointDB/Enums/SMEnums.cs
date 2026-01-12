using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DB
{
    public enum SMRequestTypeEnum
    {
        Equipment
    }

    public enum SMRequestStatusEnum
    {
        Draft,
        Submitted,
        Completed,
        Canceled
    }

    public enum SMServiceStatusIdEnum
    {
        Pending,
        WaitingParts,

    }

    public enum SMRequestLineStatusEnum
    {
        Draft,
        Pending,
        Assigned,
        [Display(Name = "In Process")]
        InProcess,
        Completed,
        Canceled,
        [Display(Name = "WO Created")]
        WorkOrderCreated,
        [Display(Name = "WO In Process")]
        WorkOrderInProcess,
        [Display(Name = "WO Completed")]
        WorkOrderCompleted,
        [Display(Name = "WO Canceled")]
        WorkOrderCanceled,
    }
}