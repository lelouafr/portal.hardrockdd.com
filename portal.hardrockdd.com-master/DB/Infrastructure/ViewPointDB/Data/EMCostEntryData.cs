using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DB.Infrastructure.ViewPointDB.Data
{
    /// <summary>
    /// Helper class for Work Order Cost Entry operations
    /// </summary>
    public class EMCostEntryData
    {
        private static string GetConnectionString()
        {
            using (var db = new VPContext())
            {
                return db.Database.Connection.ConnectionString;
            }
        }

        #region Labor

        public static List<LaborEntry> GetLabor(byte emCo, string workOrderId, short woItem = 1)
        {
            var list = new List<LaborEntry>();
            using (var conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                var sql = @"SELECT LaborId, EmployeeId, EmployeeName, WorkDate, Hours, HourlyRate, TotalCost, Description
                           FROM budEMWOLabor 
                           WHERE EMCo = @EMCo AND WorkOrderId = @WorkOrderId AND WOItem = @WOItem
                           ORDER BY WorkDate DESC";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@EMCo", emCo);
                    cmd.Parameters.AddWithValue("@WorkOrderId", workOrderId);
                    cmd.Parameters.AddWithValue("@WOItem", woItem);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new LaborEntry
                            {
                                LaborId = reader.GetInt32(0),
                                EmployeeId = reader.IsDBNull(1) ? null : (int?)reader.GetInt32(1),
                                EmployeeName = reader.IsDBNull(2) ? null : reader.GetString(2),
                                WorkDate = reader.GetDateTime(3),
                                Hours = reader.GetDecimal(4),
                                HourlyRate = reader.GetDecimal(5),
                                TotalCost = reader.GetDecimal(6),
                                Description = reader.IsDBNull(7) ? null : reader.GetString(7)
                            });
                        }
                    }
                }
            }
            return list;
        }

        public static int AddLabor(byte emCo, string workOrderId, short woItem, LaborEntry entry, string enteredBy)
        {
            using (var conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                var sql = @"INSERT INTO budEMWOLabor (EMCo, WorkOrderId, WOItem, EmployeeId, EmployeeName, WorkDate, Hours, HourlyRate, Description, EnteredBy)
                           VALUES (@EMCo, @WorkOrderId, @WOItem, @EmployeeId, @EmployeeName, @WorkDate, @Hours, @HourlyRate, @Description, @EnteredBy);
                           SELECT SCOPE_IDENTITY();";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@EMCo", emCo);
                    cmd.Parameters.AddWithValue("@WorkOrderId", workOrderId);
                    cmd.Parameters.AddWithValue("@WOItem", woItem);
                    cmd.Parameters.AddWithValue("@EmployeeId", (object)entry.EmployeeId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@EmployeeName", (object)entry.EmployeeName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@WorkDate", entry.WorkDate);
                    cmd.Parameters.AddWithValue("@Hours", entry.Hours);
                    cmd.Parameters.AddWithValue("@HourlyRate", entry.HourlyRate);
                    cmd.Parameters.AddWithValue("@Description", (object)entry.Description ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@EnteredBy", (object)enteredBy ?? DBNull.Value);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public static bool DeleteLabor(int laborId)
        {
            using (var conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                var sql = "DELETE FROM budEMWOLabor WHERE LaborId = @LaborId";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@LaborId", laborId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        #endregion

        #region Parts

        public static List<PartEntry> GetParts(byte emCo, string workOrderId, short woItem = 1)
        {
            var list = new List<PartEntry>();
            using (var conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                var sql = @"SELECT PartId, PartNumber, PartDescription, Quantity, UnitCost, TotalCost, Vendor
                           FROM budEMWOParts 
                           WHERE EMCo = @EMCo AND WorkOrderId = @WorkOrderId AND WOItem = @WOItem
                           ORDER BY EnteredDate DESC";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@EMCo", emCo);
                    cmd.Parameters.AddWithValue("@WorkOrderId", workOrderId);
                    cmd.Parameters.AddWithValue("@WOItem", woItem);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new PartEntry
                            {
                                PartId = reader.GetInt32(0),
                                PartNumber = reader.IsDBNull(1) ? null : reader.GetString(1),
                                PartDescription = reader.GetString(2),
                                Quantity = reader.GetDecimal(3),
                                UnitCost = reader.GetDecimal(4),
                                TotalCost = reader.GetDecimal(5),
                                Vendor = reader.IsDBNull(6) ? null : reader.GetString(6)
                            });
                        }
                    }
                }
            }
            return list;
        }

        public static int AddPart(byte emCo, string workOrderId, short woItem, PartEntry entry, string enteredBy)
        {
            using (var conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                var sql = @"INSERT INTO budEMWOParts (EMCo, WorkOrderId, WOItem, PartNumber, PartDescription, Quantity, UnitCost, Vendor, EnteredBy)
                           VALUES (@EMCo, @WorkOrderId, @WOItem, @PartNumber, @PartDescription, @Quantity, @UnitCost, @Vendor, @EnteredBy);
                           SELECT SCOPE_IDENTITY();";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@EMCo", emCo);
                    cmd.Parameters.AddWithValue("@WorkOrderId", workOrderId);
                    cmd.Parameters.AddWithValue("@WOItem", woItem);
                    cmd.Parameters.AddWithValue("@PartNumber", (object)entry.PartNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PartDescription", entry.PartDescription);
                    cmd.Parameters.AddWithValue("@Quantity", entry.Quantity);
                    cmd.Parameters.AddWithValue("@UnitCost", entry.UnitCost);
                    cmd.Parameters.AddWithValue("@Vendor", (object)entry.Vendor ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@EnteredBy", (object)enteredBy ?? DBNull.Value);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public static bool DeletePart(int partId)
        {
            using (var conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                var sql = "DELETE FROM budEMWOParts WHERE PartId = @PartId";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@PartId", partId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        #endregion

        #region Other Costs

        public static List<OtherCostEntry> GetOtherCosts(byte emCo, string workOrderId, short woItem = 1)
        {
            var list = new List<OtherCostEntry>();
            using (var conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                var sql = @"SELECT OtherCostId, CostType, Description, Amount, Vendor, InvoiceNumber
                           FROM budEMWOOtherCost 
                           WHERE EMCo = @EMCo AND WorkOrderId = @WorkOrderId AND WOItem = @WOItem
                           ORDER BY EnteredDate DESC";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@EMCo", emCo);
                    cmd.Parameters.AddWithValue("@WorkOrderId", workOrderId);
                    cmd.Parameters.AddWithValue("@WOItem", woItem);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new OtherCostEntry
                            {
                                OtherCostId = reader.GetInt32(0),
                                CostType = reader.GetString(1)[0],
                                Description = reader.GetString(2),
                                Amount = reader.GetDecimal(3),
                                Vendor = reader.IsDBNull(4) ? null : reader.GetString(4),
                                InvoiceNumber = reader.IsDBNull(5) ? null : reader.GetString(5)
                            });
                        }
                    }
                }
            }
            return list;
        }

        public static int AddOtherCost(byte emCo, string workOrderId, short woItem, OtherCostEntry entry, string enteredBy)
        {
            using (var conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                var sql = @"INSERT INTO budEMWOOtherCost (EMCo, WorkOrderId, WOItem, CostType, Description, Amount, Vendor, InvoiceNumber, EnteredBy)
                           VALUES (@EMCo, @WorkOrderId, @WOItem, @CostType, @Description, @Amount, @Vendor, @InvoiceNumber, @EnteredBy);
                           SELECT SCOPE_IDENTITY();";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@EMCo", emCo);
                    cmd.Parameters.AddWithValue("@WorkOrderId", workOrderId);
                    cmd.Parameters.AddWithValue("@WOItem", woItem);
                    cmd.Parameters.AddWithValue("@CostType", entry.CostType.ToString());
                    cmd.Parameters.AddWithValue("@Description", entry.Description);
                    cmd.Parameters.AddWithValue("@Amount", entry.Amount);
                    cmd.Parameters.AddWithValue("@Vendor", (object)entry.Vendor ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@InvoiceNumber", (object)entry.InvoiceNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@EnteredBy", (object)enteredBy ?? DBNull.Value);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public static bool DeleteOtherCost(int otherCostId)
        {
            using (var conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                var sql = "DELETE FROM budEMWOOtherCost WHERE OtherCostId = @OtherCostId";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@OtherCostId", otherCostId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        #endregion

        #region Summary

        public static CostSummary GetCostSummary(byte emCo, string workOrderId, short woItem = 1)
        {
            using (var conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                var sql = @"SELECT 
                            ISNULL((SELECT SUM(TotalCost) FROM budEMWOLabor WHERE EMCo = @EMCo AND WorkOrderId = @WorkOrderId AND WOItem = @WOItem), 0),
                            ISNULL((SELECT SUM(TotalCost) FROM budEMWOParts WHERE EMCo = @EMCo AND WorkOrderId = @WorkOrderId AND WOItem = @WOItem), 0),
                            ISNULL((SELECT SUM(Amount) FROM budEMWOOtherCost WHERE EMCo = @EMCo AND WorkOrderId = @WorkOrderId AND WOItem = @WOItem), 0)";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@EMCo", emCo);
                    cmd.Parameters.AddWithValue("@WorkOrderId", workOrderId);
                    cmd.Parameters.AddWithValue("@WOItem", woItem);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var labor = reader.GetDecimal(0);
                            var parts = reader.GetDecimal(1);
                            var other = reader.GetDecimal(2);

                            return new CostSummary
                            {
                                LaborCost = labor,
                                PartsCost = parts,
                                OtherCost = other,
                                TotalCost = labor + parts + other
                            };
                        }
                    }
                }
            }
            return new CostSummary();
        }

        #endregion
    }

    #region Models

    public class LaborEntry
    {
        public int LaborId { get; set; }
        public int? EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime WorkDate { get; set; }
        public decimal Hours { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal TotalCost { get; set; }
        public string Description { get; set; }
    }

    public class PartEntry
    {
        public int PartId { get; set; }
        public string PartNumber { get; set; }
        public string PartDescription { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }
        public string Vendor { get; set; }
    }

    public class OtherCostEntry
    {
        public int OtherCostId { get; set; }
        public char CostType { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Vendor { get; set; }
        public string InvoiceNumber { get; set; }

        public string CostTypeDisplay => CostType switch
        {
            'M' => "Materials",
            'S' => "Shop Supplies",
            'V' => "Vendor Invoice",
            'O' => "Other",
            _ => "Unknown"
        };
    }

    public class CostSummary
    {
        public decimal LaborCost { get; set; }
        public decimal PartsCost { get; set; }
        public decimal OtherCost { get; set; }
        public decimal TotalCost { get; set; }
    }

    #endregion
}
