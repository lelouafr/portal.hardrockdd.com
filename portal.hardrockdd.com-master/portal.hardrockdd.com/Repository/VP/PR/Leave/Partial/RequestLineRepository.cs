using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Payroll.Leave;
using System;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web.Mvc;

namespace portal.Repository.VP.PR
{
    public partial class RequestLineRepository : IDisposable
    {
        public static LeaveRequestLineViewModel ProcessUpdate(LeaveRequestLineViewModel model, ModelStateDictionary modelState)
        {
            using var db = new VPContext();
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            var updObj = db.LeaveRequestLines.FirstOrDefault(f => f.PRCo == model.PRCo && f.RequestId == model.RequestId && f.LineId == model.LineId);
            if (updObj != null)
            {
                
                /****Write the changes to object****/
                updObj.WorkDate = model.WorkDate;
                updObj.LeaveCodeId = model.LeaveCodeId;
                updObj.Hours = model.Hours;
                updObj.Comments = model.Comments;

                db.SaveChanges(modelState);
            }
            return new LeaveRequestLineViewModel(updObj);
        }
    }
}