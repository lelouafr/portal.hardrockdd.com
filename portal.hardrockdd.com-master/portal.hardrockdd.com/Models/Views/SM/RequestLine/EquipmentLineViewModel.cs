using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Models.Views.SM.RequestLine
{
    public class EquipmentListLineViewModel
    {
        public EquipmentListLineViewModel(List<SMRequestLine> requests)
        {
            if (requests == null)
                return;
            //var requests = db.SMRequestLines.Where(f => f.Request.RequestTypeId == (int)DB.SMRequestTypeEnum.Equipment).ToList();

            List = requests.Select(s => new EquipmentLineViewModel(s)).ToList();
        }

        public List<EquipmentLineViewModel> List { get; }
    }

    public class EquipmentLineViewModel
    {

        public EquipmentLineViewModel()
        {

        }

        public EquipmentLineViewModel(SMRequestLine line)
        {
            if (line == null)
                return;

            EMCo = (byte)(line.EMCo ?? 1);
            EquipmentId = line.EquipmentId;
            SMCo = line.SMCo;
            RequestId = line.RequestId;
            LineId = line.LineId;
            EquipmentName = line.Equipment?.Description ?? "Empty";
            RequestedUser = line.Request.RequestUser.FullName();
            RequestDate = line.Request.RequestDate;
            Status = line.Status.ToString();
            if (line.Request.Status == DB.SMRequestStatusEnum.Draft)
            {
                Status = line.Request.Status.ToString();
            }
            Comments = line.RequestComments;
        }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "Co")]
        public byte EMCo { get; set; }


        [Key]
        [UIHint("LongBox")]
        [Display(Name = "Equipment Id")]
        public string EquipmentId { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "Co")]
        public byte SMCo { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "Id")]
        public int RequestId { get; set; }

        [Key]
        [UIHint("LongBox")]
        [Display(Name = "#")]
        public int LineId { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Equipment")]
        public string EquipmentName { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Requested User")]
        public string RequestedUser { get; set; }

        [UIHint("DateBox")]
        [Display(Name = "Requested Date")]
        public DateTime RequestDate { get; set; }

        [UIHint("TextBox")]
        [Display(Name = "Status")]
        public string Status { get; set; }

        [UIHint("TextAreaBox")]
        [Display(Name = "Comments")]
        public string Comments { get; set; }

    }
}