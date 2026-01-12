using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Linq;
using System.Web.Mvc;

namespace portal.Repository.VP.EM
{
    public static class EquipmentAuditStatusLogRepository 
    {
        
        public static EMAuditStatusLog Init(EMAudit entity)
        {
            if (entity == null)
            {
                throw new System.ArgumentNullException(nameof(entity));
            }
            var model = new EMAuditStatusLog
            {
                EMCo = entity.EMCo,
                AuditId = entity.AuditId,
                LineNum = entity.StatusLogs.DefaultIfEmpty().Max(f => f == null ? 0 : f.LineNum) + 1,
                Status = (short)entity.Status,
                CreatedOn = DateTime.Now,
                CreatedBy = StaticFunctions.GetUserId()
            };

            return model;
        }

        
    }
}