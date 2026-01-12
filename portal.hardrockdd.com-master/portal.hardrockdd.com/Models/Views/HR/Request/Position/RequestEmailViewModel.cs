using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Models.Views.HR.Request.Position
{
    public class RequestEmailViewModel: RequestViewModel
    {
        public RequestEmailViewModel()
        {

        }

        public RequestEmailViewModel(Code.Data.VP.HRPositionRequest request): base(request)
        {
            
        }

    }
}