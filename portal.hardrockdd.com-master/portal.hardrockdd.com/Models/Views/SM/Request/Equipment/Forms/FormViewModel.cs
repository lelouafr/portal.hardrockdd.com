using DB.Infrastructure.ViewPointDB.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace portal.Models.Views.SM.Request.Equipment.Forms
{

    public class FormViewModel
    {
        public FormViewModel()
        {

        }

        public FormViewModel(DB.Infrastructure.ViewPointDB.Data.Equipment equipment)
        {
            if (equipment == null)
                return;
            //if (controller == null)
            //    return;

            EMCo = equipment.EMCo;
            EquipmentId = equipment.EquipmentId;
            EquipmentName = equipment.Description;
            Info = new InfoViewModel(equipment);
            RequestLines = new RequestLineListViewModel(equipment);

            WorkFlowActions = BuildActions();
        }

        [Key]
        [HiddenInput]
        public byte? EMCo { get; set; }

        [Key]
        [UIHint("TextBox")]
        [Display(Name = "Equipment")]
        public string EquipmentId { get; set; }

        [Display(Name = "Equipment")]
        [UIHint("TextBox")]
        public string EquipmentName { get; set; }

        public RequestLineListViewModel RequestLines { get; set; }

        public InfoViewModel Info { get; set; }

        public List<WF.WorkFlowAction> WorkFlowActions { get; }

        public List<WF.WorkFlowAction> BuildActions()
        {
            var results = new List<WF.WorkFlowAction>();

            //results.Add(new WF.WorkFlowAction() { Title = "Create WO For All", GotoStatusId = (int)DB.SMRequestLineStatusEnum.WorkOrderCreated, ButtonClass = "btn-primary", ActionRedirect = "Reload", IsAjax = true});
            results.Add(new WF.WorkFlowAction() { Title = "Complete All", GotoStatusId = (int)DB.SMRequestLineStatusEnum.Canceled, ButtonClass = "btn-primary", ActionRedirect = "Reload", IsAjax = true });
            results.Add(new WF.WorkFlowAction() { Title = "Cancel All", GotoStatusId = (int)DB.SMRequestLineStatusEnum.Canceled, ButtonClass = "btn-danger", ActionRedirect = "Reload", IsAjax = true });

            return results;
        }
    }
}