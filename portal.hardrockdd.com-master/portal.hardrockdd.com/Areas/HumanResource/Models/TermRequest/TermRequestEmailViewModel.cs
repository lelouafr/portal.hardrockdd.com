using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Areas.HumanResource.Models.TermRequest
{
    public class TermRequestEmailViewModel: TermRequestViewModel
    {
        public TermRequestEmailViewModel()
        {

        }

        public TermRequestEmailViewModel(HRTermRequest request): base(request)
        {
            
        }

    }
}