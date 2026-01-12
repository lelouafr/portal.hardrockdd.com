using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Areas.SM.Models.Request
{
    public class RequestListViewModel
    {
        public RequestListViewModel()
        {

        }

        public RequestListViewModel(List<SMRequest> list)
        {
            List = list.Select(s => new RequestViewModel(s)).ToList();
        }

        public List<RequestViewModel>? List { get; }
    }

    public class RequestViewModel
    {
        public RequestViewModel()
        {
            RequestDate = DateTime.Now;
            RequestByName = StaticFunctions.GetCurrentEmployee().FullName;
            RequestType = DB.SMRequestTypeEnum.Equipment;
        }

        public RequestViewModel(DB.Infrastructure.ViewPointDB.Data.SMRequest request)
        {
            if (request == null)
                return;

            SMCo = request.SMCo;
            RequestId = request.RequestId;
            RequestType = request.RequestType;
            RequestDate = request.RequestDate;
            RequestByName = request.RequestUser.PREmployee.FullName;
            Status = request.Status;
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


        internal RequestViewModel ProcessUpdate(VPContext db, ModelStateDictionary modelState)
        {
            var updObj = db.SMRequests.FirstOrDefault(f => f.SMCo == this.SMCo && f.RequestId == this.RequestId);

            if (updObj != null)
            {
                updObj.Comments = this.Comments;
                try
                {
                    db.BulkSaveChanges();
                    return new RequestViewModel(updObj);
                }
                catch (Exception ex)
                {
                    modelState.AddModelError("", ex.Message);
                    return this;
                }
            }
            else
            {
                modelState.AddModelError("", "Object Doesn't Exist For Update!");
                return this;
            }
        }
    }
}