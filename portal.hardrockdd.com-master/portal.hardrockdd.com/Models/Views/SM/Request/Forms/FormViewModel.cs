using DB.Infrastructure.ViewPointDB.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace portal.Models.Views.SM.Request.Forms
{

    public class FormViewModel : AuditBaseViewModel
    {
        public FormViewModel()
        {

        }

        public FormViewModel(SMRequest request) : base(request)
        {
            if (request == null)
                return;

            SMCo = request.SMCo;
            RequestId = request.RequestId;

            Info = new InfoViewModel(request);
            EquipmentLines = new EquipmentLineListViewModel(request);
            WorkFlowActions = new ActionViewModel(request);

            Info.WorkFlowActions = WorkFlowActions;
        }

        [Key]
        [HiddenInput]
        public byte SMCo { get; set; }

        [Key]
        [HiddenInput]
        public int RequestId { get; set; }

        public InfoViewModel Info { get; set; }

        //public TriggerViewModel Trigger { get; set; }

        public EquipmentLineListViewModel EquipmentLines { get; set; }

        public ActionViewModel WorkFlowActions { get; set; }

        //public CategoryLinkListViewModel AssignedCategories { get; set; }

    }
}