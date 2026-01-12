using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class DailyForm
    {
        private VPContext _db;
        public VPContext db
        {
            set
            {
                _db = value;
            }
            get
            {
                _db ??= VPContext.GetDbContextFromEntity(this);
                return _db;
            }
        }
    }

    public static class DailyFormExt
    {
        public static DailyForm AddDailyForm(this VPContext db)
        {
            var form = new DailyForm() {
                FormId = db.DailyForms.DefaultIfEmpty().Max(f => f == null ? 0 : f.FormId) + 1,
            };
            db.DailyForms.Add(form);
            return form;
        }
    }
}
