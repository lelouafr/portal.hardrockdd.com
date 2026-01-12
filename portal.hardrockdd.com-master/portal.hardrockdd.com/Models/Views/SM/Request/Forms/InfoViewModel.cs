using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Mvc;

namespace portal.Models.Views.SM.Request.Forms
{
    public class InfoViewModel
    {
        public InfoViewModel()
        {

        }

        public InfoViewModel(DB.Infrastructure.ViewPointDB.Data.SMRequest request)
        {
            if (request == null) throw new System.ArgumentNullException(nameof(request));

            SMCo = request.SMCo;
            RequestId = request.RequestId;
            Comments = request.Comments;
            RequestType = request.RequestType;
            Status = request.Status;

            RequestDate = request.RequestDate;
            RequestByName = request.RequestUser.PREmployee.FullName;

            WorkFlowActions = new ActionViewModel(request);
        }

        [Key]
        [HiddenInput]
        public byte SMCo { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Field(LabelSize = 2, TextSize = 4)]
        [Display(Name = "#")]
        public int RequestId { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "Type")]
        public DB.SMRequestTypeEnum RequestType { get; set; }

        [UIHint("EnumBox")]
        [Display(Name = "Status")]
        public DB.SMRequestStatusEnum Status { get; set; }

        [UIHint("TextAreaBox")]
        [Field(LabelSize = 2, TextSize = 10)]
        [Display(Name = "Comments")]
        public string Comments { get; set; }


        [UIHint("DateBox")]
        [Display(Name = "Request Date")]
        public DateTime RequestDate { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Request By")]
        public string RequestByName { get; set; }


        public ActionViewModel WorkFlowActions { get; set; }
    }
}