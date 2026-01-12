using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Models.Views.HR.Request.Term
{
    public class TermRequestEmailViewModel: TermRequestViewModel
    {
        public TermRequestEmailViewModel()
        {

        }

        public TermRequestEmailViewModel(Code.Data.VP.HRTermRequest request): base(request)
        {
            
        }

    }
}