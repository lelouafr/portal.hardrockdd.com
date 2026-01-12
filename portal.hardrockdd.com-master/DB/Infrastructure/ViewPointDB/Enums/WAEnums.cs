using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DB
{
    public enum WAApplicationStatusEnum
    {
        Draft,
        Started,
        Submitted,
        Approved,
        Hired,
        Shelved,
        Denied,
        Canceled,
    }

    public enum WAApplicationDeniedReasonEnum
    {
        NotHired,        
        ApplicationDenied,
        DeniedJob,
        FailedDrugTest,
        SuspendedLicense,
        NoShow,
        IncompleteApplication
    }
    
    public enum WAIncidentTypeEnum
    {
        Accident,
        Citation
    }
}