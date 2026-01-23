using System;
using System.Data.SqlClient;
using System.Data.Entity.Core.EntityClient;

namespace DB.Infrastructure.ViewPointDB.Data
{
    /// <summary>
    /// Helper class for Work Performed documentation
    /// </summary>
    public class EMWorkPerformedData
    {
        private static string GetConnectionString()
        {
            using (var db = new VPContext())
            {
                return db.Database.Connection.ConnectionString;
            }
        }

        /// <summary>
        /// Get work performed record for a work order
        /// </summary>
        public static WorkPerformedRecord GetWorkPerformed(byte emCo, string workOrderId, short woItem = 1)
        {
            using (var conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                var sql = @"SELECT WorkPerformedId, EMCo, WorkOrderId, WOItem, 
                                  RepairsCompleted, PartsReplaced, PartsBackordered, BackorderETA,
                                  TemporaryRepairs, RecommendedFutureWork, CompletionStatus,
                                  CompletedBy, CompletedDate
                           FROM budEMWOWorkPerformed 
                           WHERE EMCo = @EMCo AND WorkOrderId = @WorkOrderId AND WOItem = @WOItem";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@EMCo", emCo);
                    cmd.Parameters.AddWithValue("@WorkOrderId", workOrderId);
                    cmd.Parameters.AddWithValue("@WOItem", woItem);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new WorkPerformedRecord
                            {
                                WorkPerformedId = reader.GetInt32(0),
                                EMCo = reader.GetByte(1),
                                WorkOrderId = reader.GetString(2),
                                WOItem = reader.GetInt16(3),
                                RepairsCompleted = reader.IsDBNull(4) ? null : reader.GetString(4),
                                PartsReplaced = reader.IsDBNull(5) ? null : reader.GetString(5),
                                PartsBackordered = reader.IsDBNull(6) ? null : reader.GetString(6),
                                BackorderETA = reader.IsDBNull(7) ? null : (DateTime?)reader.GetDateTime(7),
                                TemporaryRepairs = reader.IsDBNull(8) ? null : reader.GetString(8),
                                RecommendedFutureWork = reader.IsDBNull(9) ? null : reader.GetString(9),
                                CompletionStatus = reader.GetString(10)[0],
                                CompletedBy = reader.IsDBNull(11) ? null : (int?)reader.GetInt32(11),
                                CompletedDate = reader.GetDateTime(12)
                            };
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Save work performed record (insert or update)
        /// </summary>
        public static int SaveWorkPerformed(WorkPerformedRecord record)
        {
            using (var conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();

                // Check if exists
                var checkSql = "SELECT WorkPerformedId FROM budEMWOWorkPerformed WHERE EMCo = @EMCo AND WorkOrderId = @WorkOrderId AND WOItem = @WOItem";
                int? existingId = null;

                using (var checkCmd = new SqlCommand(checkSql, conn))
                {
                    checkCmd.Parameters.AddWithValue("@EMCo", record.EMCo);
                    checkCmd.Parameters.AddWithValue("@WorkOrderId", record.WorkOrderId);
                    checkCmd.Parameters.AddWithValue("@WOItem", record.WOItem);

                    var obj = checkCmd.ExecuteScalar();
                    if (obj != null)
                        existingId = Convert.ToInt32(obj);
                }

                if (existingId.HasValue)
                {
                    // Update
                    var updateSql = @"UPDATE budEMWOWorkPerformed SET 
                                     RepairsCompleted = @RepairsCompleted,
                                     PartsReplaced = @PartsReplaced,
                                     PartsBackordered = @PartsBackordered,
                                     BackorderETA = @BackorderETA,
                                     TemporaryRepairs = @TemporaryRepairs,
                                     RecommendedFutureWork = @RecommendedFutureWork,
                                     CompletionStatus = @CompletionStatus,
                                     CompletedBy = @CompletedBy,
                                     CompletedDate = GETDATE()
                                     WHERE WorkPerformedId = @WorkPerformedId";

                    using (var updateCmd = new SqlCommand(updateSql, conn))
                    {
                        updateCmd.Parameters.AddWithValue("@WorkPerformedId", existingId.Value);
                        AddParameters(updateCmd, record);
                        updateCmd.ExecuteNonQuery();
                    }
                    return existingId.Value;
                }
                else
                {
                    // Insert
                    var insertSql = @"INSERT INTO budEMWOWorkPerformed 
                                     (EMCo, WorkOrderId, WOItem, RepairsCompleted, PartsReplaced, 
                                      PartsBackordered, BackorderETA, TemporaryRepairs, 
                                      RecommendedFutureWork, CompletionStatus, CompletedBy)
                                     VALUES (@EMCo, @WorkOrderId, @WOItem, @RepairsCompleted, @PartsReplaced,
                                             @PartsBackordered, @BackorderETA, @TemporaryRepairs,
                                             @RecommendedFutureWork, @CompletionStatus, @CompletedBy);
                                     SELECT SCOPE_IDENTITY();";

                    using (var insertCmd = new SqlCommand(insertSql, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@EMCo", record.EMCo);
                        insertCmd.Parameters.AddWithValue("@WorkOrderId", record.WorkOrderId);
                        insertCmd.Parameters.AddWithValue("@WOItem", record.WOItem);
                        AddParameters(insertCmd, record);
                        return Convert.ToInt32(insertCmd.ExecuteScalar());
                    }
                }
            }
        }

        private static void AddParameters(SqlCommand cmd, WorkPerformedRecord record)
        {
            cmd.Parameters.AddWithValue("@RepairsCompleted", (object)record.RepairsCompleted ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PartsReplaced", (object)record.PartsReplaced ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PartsBackordered", (object)record.PartsBackordered ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BackorderETA", (object)record.BackorderETA ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TemporaryRepairs", (object)record.TemporaryRepairs ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RecommendedFutureWork", (object)record.RecommendedFutureWork ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CompletionStatus", record.CompletionStatus.ToString());
            cmd.Parameters.AddWithValue("@CompletedBy", (object)record.CompletedBy ?? DBNull.Value);
        }
    }

    /// <summary>
    /// Work Performed record model
    /// </summary>
    public class WorkPerformedRecord
    {
        public int WorkPerformedId { get; set; }
        public byte EMCo { get; set; }
        public string WorkOrderId { get; set; }
        public short WOItem { get; set; }
        
        public string RepairsCompleted { get; set; }
        public string PartsReplaced { get; set; }
        public string PartsBackordered { get; set; }
        public DateTime? BackorderETA { get; set; }
        public string TemporaryRepairs { get; set; }
        public string RecommendedFutureWork { get; set; }
        
        public char CompletionStatus { get; set; } = 'C';
        public int? CompletedBy { get; set; }
        public DateTime CompletedDate { get; set; }

        public string CompletionStatusDisplay => CompletionStatus switch
        {
            'C' => "Completed",
            'P' => "Partially Completed",
            'F' => "Requires Follow-up",
            _ => "Unknown"
        };
    }
}
