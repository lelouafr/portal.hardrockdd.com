using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class Batch
    {
        private string ValidatePOEntryBatch()
        {
            try
            {
                using var tmpDb = new VPContext();
                var msgParm = new ObjectParameter("errmsg", typeof(string));
                var error = tmpDb.bspPOHBVal(this.Co, this.Mth, this.BatchId, this.Source, msgParm);
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

        private string PostPOEntryBatch()
        {
            try
            {
                using var tmpDb = new VPContext();
                var msgParm = new ObjectParameter("errmsg", typeof(string));
                var error = tmpDb.bspPOHBPost(this.Co, this.Mth, this.BatchId, DateTime.Now, this.Source, msgParm);
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
