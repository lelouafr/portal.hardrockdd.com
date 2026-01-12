using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace portal.Models.Views.EM.Equipment.Forms
{
    public class ServiceHistoryListViewModel
    {
        public ServiceHistoryListViewModel()
        {

        }

        public ServiceHistoryListViewModel(DB.Infrastructure.ViewPointDB.Data.Equipment equipment)
        {
            if (equipment == null)
                return;

            EMCo = equipment.EMCo;
            EquipmentId = equipment.EquipmentId;

            List = equipment.ServiceRequestLines.Select(s => new ServiceHistoryViewModel(s)).ToList();
        }



        [Key]
        public byte EMCo { get; set; }

        [Key]
        public string EquipmentId { get; set; }


        public List<ServiceHistoryViewModel> List { get; }
    }

    public class ServiceHistoryViewModel: portal.Models.Views.SM.Request.Equipment.Forms.Line.InfoViewModel
    {
        public ServiceHistoryViewModel()
        {

        }

        public ServiceHistoryViewModel(DB.Infrastructure.ViewPointDB.Data.SMRequestLine requestLine) :base(requestLine)
        {

        }

    }
}