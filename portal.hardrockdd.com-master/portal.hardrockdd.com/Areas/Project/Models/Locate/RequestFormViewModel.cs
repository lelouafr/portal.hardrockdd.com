using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace portal.Areas.Project.Models.Locate
{
    public class RequestFormViewModel
    {
        public RequestFormViewModel()
        {

        }

        public RequestFormViewModel(DB.Infrastructure.ViewPointDB.Data.PMLocateRequest request)
        {
            if (request == null)
                return;

            RequestId = request.RequestId;
            Locates = new LocateListViewModel(request);
            Info = new RequestViewModel(request);
        }


        [Key]
        [HiddenInput]
        public int RequestId { get; set; }

        public RequestViewModel Info { get; set; }

        public LocateListViewModel Locates { get; set; }


    }
}