using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Models.Views.SM.Request
{
    public class ServiceRequestListViewModel
    {
        public ServiceRequestListViewModel()
        {

        }

        public ServiceRequestListViewModel(List<SMRequest> list)
        {
            List = list.Select(s => new ServiceRequestViewModel(s)).ToList();
        }

        public List<ServiceRequestViewModel> List { get; }
    }

    public class ServiceRequestViewModel
    {
        public ServiceRequestViewModel()
        {
            RequestDate = DateTime.Now;
            RequestByName = StaticFunctions.GetCurrentEmployee().FullName;
            RequestType = DB.SMRequestTypeEnum.Equipment;
        }

        public ServiceRequestViewModel(DB.Infrastructure.ViewPointDB.Data.SMRequest request)
        {
            if (request == null)
                return;

            SMCo = request.SMCo;
            RequestId = request.RequestId;
            RequestType = request.RequestType;
            RequestDate = request.RequestDate;
            RequestByName = request.RequestUser.PREmployee.FullName;
            Status = request.Status;
            StatusString = request.Status.ToString();

            Comments = request.Comments;
        }


        [Key]
        [UIHint("LongBox")]
        [Display(Name = "Co")]
        public byte SMCo { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "Id")]
        public int RequestId { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Request Date")]
        public DateTime RequestDate { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "Type")]
        public DB.SMRequestTypeEnum RequestType { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Request By")]
        public string RequestByName { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Notes")]
        public string Comments { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        public DB.SMRequestStatusEnum Status { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Status")]
        public string StatusString { get; set; }
    }
}