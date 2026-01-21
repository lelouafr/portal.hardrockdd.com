using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace DB.Infrastructure.ViewPointDB.Data
{
    /// <summary>
    /// Helper class for custom SM tables using raw SQL (avoids EF conflicts)
    /// </summary>
    public static class SMCustomData
    {
        private static string ConnectionString
        {
            get
            {
                var efConnectionString = ConfigurationManager.ConnectionStrings["VPContext"].ConnectionString;
                
                // If it's an EF connection string (contains "metadata="), extract the provider connection string
                if (efConnectionString.Contains("metadata="))
                {
                    var builder = new EntityConnectionStringBuilder(efConnectionString);
                    return builder.ProviderConnectionString;
                }
                
                return efConnectionString;
            }
        }

        #region SMRequestLineCustom

        public static SMRequestLineCustom GetCustomData(byte smCo, int requestId, int lineId)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"
                    SELECT SMCo, RequestId, LineId, PriorityId, RequestTypeId, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy
                    FROM budSMRLCustom 
                    WHERE SMCo = @SMCo AND RequestId = @RequestId AND LineId = @LineId", conn))
                {
                    cmd.Parameters.AddWithValue("@SMCo", smCo);
                    cmd.Parameters.AddWithValue("@RequestId", requestId);
                    cmd.Parameters.AddWithValue("@LineId", lineId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new SMRequestLineCustom
                            {
                                SMCo = (byte)reader["SMCo"],
                                RequestId = (int)reader["RequestId"],
                                LineId = (int)reader["LineId"],
                                PriorityId = reader["PriorityId"] as byte?,
                                RequestTypeId = reader["RequestTypeId"] as byte?,
                                CreatedDate = reader["CreatedDate"] as DateTime?,
                                CreatedBy = reader["CreatedBy"] as string,
                                ModifiedDate = reader["ModifiedDate"] as DateTime?,
                                ModifiedBy = reader["ModifiedBy"] as string
                            };
                        }
                    }
                }
            }
            return null;
        }

        public static void SaveCustomData(SMRequestLineCustom data, string userId)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                
                // Check if exists
                using (var checkCmd = new SqlCommand(@"
                    SELECT COUNT(*) FROM budSMRLCustom 
                    WHERE SMCo = @SMCo AND RequestId = @RequestId AND LineId = @LineId", conn))
                {
                    checkCmd.Parameters.AddWithValue("@SMCo", data.SMCo);
                    checkCmd.Parameters.AddWithValue("@RequestId", data.RequestId);
                    checkCmd.Parameters.AddWithValue("@LineId", data.LineId);
                    
                    int count = (int)checkCmd.ExecuteScalar();
                    
                    if (count == 0)
                    {
                        // Insert
                        using (var insertCmd = new SqlCommand(@"
                            INSERT INTO budSMRLCustom (SMCo, RequestId, LineId, PriorityId, RequestTypeId, CreatedDate, CreatedBy)
                            VALUES (@SMCo, @RequestId, @LineId, @PriorityId, @RequestTypeId, GETDATE(), @CreatedBy)", conn))
                        {
                            insertCmd.Parameters.AddWithValue("@SMCo", data.SMCo);
                            insertCmd.Parameters.AddWithValue("@RequestId", data.RequestId);
                            insertCmd.Parameters.AddWithValue("@LineId", data.LineId);
                            insertCmd.Parameters.AddWithValue("@PriorityId", (object)data.PriorityId ?? DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@RequestTypeId", (object)data.RequestTypeId ?? DBNull.Value);
                            insertCmd.Parameters.AddWithValue("@CreatedBy", (object)userId ?? DBNull.Value);
                            insertCmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        // Update
                        using (var updateCmd = new SqlCommand(@"
                            UPDATE budSMRLCustom 
                            SET PriorityId = @PriorityId, RequestTypeId = @RequestTypeId, ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy
                            WHERE SMCo = @SMCo AND RequestId = @RequestId AND LineId = @LineId", conn))
                        {
                            updateCmd.Parameters.AddWithValue("@SMCo", data.SMCo);
                            updateCmd.Parameters.AddWithValue("@RequestId", data.RequestId);
                            updateCmd.Parameters.AddWithValue("@LineId", data.LineId);
                            updateCmd.Parameters.AddWithValue("@PriorityId", (object)data.PriorityId ?? DBNull.Value);
                            updateCmd.Parameters.AddWithValue("@RequestTypeId", (object)data.RequestTypeId ?? DBNull.Value);
                            updateCmd.Parameters.AddWithValue("@ModifiedBy", (object)userId ?? DBNull.Value);
                            updateCmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        #endregion

        #region Lookup Tables

        public static List<SelectListItem> GetPriorityOptions()
        {
            var list = new List<SelectListItem>();
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT PriorityId, Description FROM budSMPriority ORDER BY SortOrder", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new SelectListItem
                            {
                                Value = reader["PriorityId"].ToString(),
                                Text = reader["Description"].ToString()
                            });
                        }
                    }
                }
            }
            return list;
        }

        public static List<SelectListItem> GetRequestTypeOptions()
        {
            var list = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "-- Select --" }
            };
            
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT RequestTypeId, Description FROM budSMRequestType ORDER BY SortOrder", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new SelectListItem
                            {
                                Value = reader["RequestTypeId"].ToString(),
                                Text = reader["Description"].ToString()
                            });
                        }
                    }
                }
            }
            return list;
        }

        #endregion
    }

    /// <summary>
    /// Simple data class for custom fields
    /// </summary>
    public class SMRequestLineCustom
    {
        public byte SMCo { get; set; }
        public int RequestId { get; set; }
        public int LineId { get; set; }
        public byte? PriorityId { get; set; }
        public byte? RequestTypeId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
    }
}
