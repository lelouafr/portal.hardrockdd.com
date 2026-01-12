using Microsoft.AspNet.Identity;
using DB.Infrastructure.ViewPointDB.Data;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace portal.Models.Views.AP.Document
{
    public class ActionViewModel
    {
        public ActionViewModel()
        {


        }

        public ActionViewModel(APDocument entity)
        {
            if (entity == null)
            {
                throw new System.ArgumentNullException(nameof(entity));
            }
            APCo = entity.APCo;
            DocId = entity.DocId;
            CanUnCancel = false;
            CanCancel = false;
            CanSave = false;
            CanForward = false;

            var userId = StaticFunctions.GetUserId();
            if (entity.WorkFlow == null)
            {
                entity.AddWorkFlow();
                entity.WorkFlow.CreateSequence(entity.StatusId);
                entity.AddWorkFlowAssignments();
                entity.db.SaveChanges();
            }
            else if (entity.WorkFlow.CurrentSequence() == null)
            {
                entity.WorkFlow.CreateSequence(entity.StatusId);
                entity.AddWorkFlowAssignments();
                entity.db.SaveChanges();
            }
            //var inWorkFlow = entity.WorkFlows.Any(w => w.AssignedTo == userId && w.Status == entity.Status && w.Active == "Y");
            var inWorkFlow = entity.WorkFlow.CurrentSequence().AssignedUsers.Any(f => f.AssignedTo == userId);
            Access = DB.SessionAccess.View;
            switch ((DB.APDocumentStatusEnum)entity.Status)
            {
                case DB.APDocumentStatusEnum.Canceled:
                    if (inWorkFlow)
                    {
                        CanUnCancel = true;
                    }
                    break;
                case DB.APDocumentStatusEnum.New:
                    if (inWorkFlow && HttpContext.Current.User.FormAccess("APDocument", "NewIndex") >= DB.AccessLevelEnum.Write)
                    {
                        Access = DB.SessionAccess.Edit;
                        CanFile = true;                        
                        CanCancel = true;
                        CanSave = true;
                    }
                    break;
                case DB.APDocumentStatusEnum.Filed:
                case DB.APDocumentStatusEnum.Reviewed:
                case DB.APDocumentStatusEnum.LinesAdded:
                    if (HttpContext.Current.User.FormAccess("APDocument", "FiledIndex") >= DB.AccessLevelEnum.Write)
                    {
                        Access = DB.SessionAccess.Edit;
                        CanProcess = true;
                        CanCancel = true;
                        CanForward = true;
                        CanCancel = true;
                    }
                    else if (inWorkFlow)
                    {
                        CanForward = true;
                        CanFile = true;
                    }
                    break;
                case DB.APDocumentStatusEnum.RequestedInfo:
                    if (HttpContext.Current.User.FormAccess("APDocument", "FiledIndex") >= DB.AccessLevelEnum.Write)
                    {
                        Access = DB.SessionAccess.Edit;
                        CanCancel = true;
                        CanForward = true;
                        CanReFile = true;
                    }
                    else if (inWorkFlow)
                    {
                        CanForward = true;
                        CanReFile = true;
                    }
                    break;
                case DB.APDocumentStatusEnum.Error:
                    //CanUnSubmit = StaticFunctions.GetUserId() == entity.CreatedBy;
                    if (HttpContext.Current.User.IsInRole("Admin") || HttpContext.Current.User.IsInPosition("CFO,COO,CEO,FIN-AP,FIN-APMGR,FIN-CTRL,IT-DIR"))
                        CanCancel = true;
                    if (inWorkFlow || HttpContext.Current.User.IsInPosition("CFO,COO,CEO,FIN-AP,FIN-APMGR,FIN-CTRL,IT-DIR"))
                    {
                        Access = DB.SessionAccess.Edit;
                        CanProcess = true;
                        CanCancel = true;
                        CanReFile = true;
                    }
                    break;
                case DB.APDocumentStatusEnum.Processed:
                default:
                    break;
            }
            //if (HttpContext.Current.User.IsInRole("Admin"))
            //    Access = DB.SessionAccess.Edit;
           
        }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Co")]
        public byte APCo { get; set; }

        [Key]
        [Required]
        [HiddenInput]
        [Display(Name = "Id")]
        public int DocId { get; set; }

        public DB.SessionAccess Access { get; set; }

        public bool CanSave { get; set; }

        public bool CanUnCancel { get; set; }

        public bool CanFile { get; set; }

        public bool CanReFile { get; set; }

        public bool CanCancel { get; set; }

        public bool CanProcess { get; set; }

        public bool CanForward { get; set; }
    }


}