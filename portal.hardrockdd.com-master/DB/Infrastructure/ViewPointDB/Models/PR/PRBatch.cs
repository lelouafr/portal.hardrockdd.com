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
        
        private string ValidatePREntryBatch()
        {
            try
            {
                using var tmpDb = new VPContext();
                var msgParm = new ObjectParameter("errmsg", typeof(string));
                var error = tmpDb.bspPRTBVal(this.Co, this.Mth, this.BatchId, msgParm);
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

        private string PostPREntryBatch()
        {
            try
            {
                using var tmpDb = new VPContext();
                tmpDb.Database.CommandTimeout = 600;
                var msgParm = new ObjectParameter("errmsg", typeof(string));
                var error = tmpDb.bspPRTBPost(this.Co, this.Mth, this.BatchId, DateTime.Now, msgParm);
                var tmpBatch = tmpDb.Batches.FirstOrDefault(f => f.Co == this.Co && f.Mth == this.Mth && f.BatchId == this.BatchId);

                this.Status = tmpBatch.Status;
                if (BatchErrors.Any())
                {
                    this.ErrorMessage = (string)msgParm.Value;
                }

                return this.ErrorMessage;
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
                return ex.Message;
            }
        }

        private Batch ProcessPROvertime()
        {

            /** OT Entries**/
            var batch = this.Company.AddBatch(this.TableName, this.Source, Mth, (DateTime)this.PREndDate);
            batch.InUseBy = InUseBy;
            db.SaveChanges();

            var addedYN = new ObjectParameter("PRTBAddedYN", typeof(string));
            var msgParm = new ObjectParameter("msg", typeof(string));
            var error = db.bspPRAutoOT(batch.Co, batch.Mth, batch.BatchId, null, null, addedYN, msgParm);
            if (BatchErrors.Any())
            {
                batch.ErrorMessage = (string)msgParm.Value;
            }
            if (string.IsNullOrEmpty(batch.ErrorMessage))
            {
                batch.ErrorMessage = batch.ValidateBatch();
            }
            if (string.IsNullOrEmpty(batch.ErrorMessage))
            {
                batch.ErrorMessage = batch.PostBatch();
            }
            return batch;
        }

        private Batch ProcessPRSalary()
        {
            using var db = DbContextExtension.GetDbContextFromEntity(this);

            var batch = this.Company.AddBatch(this.TableName, this.Source, Mth, (DateTime)this.PREndDate);
            var addedYN = new ObjectParameter("PRTBAddedYN", typeof(string));
            var msgParm = new ObjectParameter("msg", typeof(string));
            var error = db.bspPRSalaryDistrib(batch.Co, batch.Mth, batch.BatchId, null, null, addedYN, msgParm);
            if (BatchErrors.Any())
            {
                batch.ErrorMessage = (string)msgParm.Value;
            }
            if (string.IsNullOrEmpty(batch.ErrorMessage))
            {
                batch.ErrorMessage = batch.ValidateBatch();
            }
            if (string.IsNullOrEmpty(batch.ErrorMessage))
            {
                batch.ErrorMessage = batch.PostBatch();
            }
            return batch;
        }
    }
}
