using System;
using System.Data.Entity.Core.Objects;
using System.Linq;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class Batch
    {
        private string ValidateEMMeter()
        {
            try
            {
                using var tmpDb = new VPContext();
                var msgParm = new ObjectParameter("errmsg", typeof(string));
                var error = tmpDb.bspEMVal_Meters_Main(this.Co, this.Mth, this.BatchId, msgParm);
                var tmpBatch = tmpDb.Batches.FirstOrDefault(f => f.Co == this.Co && f.Mth == this.Mth && f.BatchId == this.BatchId);

                this.Status = tmpBatch.Status;
                if (tmpBatch.BatchErrors.Any())
                {
                    this.ErrorMessage = (string)msgParm.Value;
                    return (string)msgParm.Value;
                }
                return "";
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
                return ex.Message;
            }
        }

        private string PostEMMeter()
        {
            try
            {
                using var tmpDb = new VPContext();
                var msgParm = new ObjectParameter("errmsg", typeof(string));
                var error = tmpDb.bspEMPost_Meters_Main(this.Co, this.Mth, this.BatchId, DateTime.Now, msgParm);
                var tmpBatch = tmpDb.Batches.FirstOrDefault(f => f.Co == this.Co && f.Mth == this.Mth && f.BatchId == this.BatchId);

                this.Status = tmpBatch.Status;
                if (BatchErrors.Any())
                {
                    this.ErrorMessage = (string)msgParm.Value;
                    return (string)msgParm.Value;
                }
                return "";
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
                return ex.Message;
            }
        }
    }
}
