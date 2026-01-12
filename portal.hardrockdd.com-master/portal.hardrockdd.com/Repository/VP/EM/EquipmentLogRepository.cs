using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Equipment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Repository.VP.EM
{
    public static class EquipmentLogRepository
    {
        public static EquipmentLog Init(Equipment equipment)
        {
            if (equipment == null) throw new System.ArgumentNullException(nameof(equipment));

            var result = new EquipmentLog()
            {
                EMCo = equipment.EMCo,
                EquipmentId = equipment.EquipmentId,
                SeqId = equipment.Logs.DefaultIfEmpty().Max(max => max == null ? 0 : max.SeqId) + 1,
                LogDate = DateTime.Now,
                LoggedBy = StaticFunctions.GetUserId(),
            };

            return result;
        }

        public static void ProcessUpdate(EquipmentLogViewModel model, VPContext db)
        {
            if (model == null) throw new System.ArgumentNullException(nameof(model));
            if (db == null) throw new System.ArgumentNullException(nameof(db));

            var updObj = db.EquipmentLogs.FirstOrDefault(f => f.EMCo == model.EMCo && f.EquipmentId == model.EquipmentId && f.SeqId == model.SeqId);

            if (updObj != null)
            {
                updObj.LogTypeId = (int)model.LogTypeId;
                updObj.LogDate = model.LogDate;
                updObj.LoggedBy = model.LoggedBy;
                updObj.Notes = model.Notes;
            }
        }
            
    }
}