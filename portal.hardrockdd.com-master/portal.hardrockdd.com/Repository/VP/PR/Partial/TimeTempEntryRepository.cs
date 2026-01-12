using DB.Infrastructure.ViewPointDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web;
using Microsoft.AspNet.Identity;
using DocumentFormat.OpenXml.Bibliography;
using System.Windows.Forms;
using System.EnterpriseServices.Internal;

namespace portal.Repository.VP.PR
{
    public partial class PRBatchTimeEntryRepository
    {
        public static PRBatchTimeEntry Init(Batch batch)
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }
            var model = new PRBatchTimeEntry
            {
                db = batch.db,
                Batch = batch,

                Co = batch.Co,
                Mth = batch.Mth,
                BatchId = batch.BatchId,
                BatchTransType = "A",
                PaySeq = 1,
                PostSeq = 1,
                Type = "J",
                BreakHours = 0M,
            };

            return model;
        }

        public static PRBatchTimeEntry Init(Batch batch, DTPayrollHour hour)
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }
            if (hour == null)
            {
                throw new ArgumentNullException(nameof(hour));
            }
            var entry = new PRBatchTimeEntry
            {
                db = batch.db,
                Batch = batch,

                Co = batch.Co,
                Mth = batch.Mth,
                BatchId = batch.BatchId,
                BatchTransType = "A",
                EmployeeId = hour.EmployeeId,
                PaySeq = 1,
                PostSeq = 1,
                Type = "J",
                DayNum = 0,
                PostDate = hour.WorkDate,
                TaxState = hour.Employee.TaxState,
                UnempState = hour.Employee.UnempState,
                InsState = hour.Employee.InsState,
                InsCode = hour.Employee.InsCode,
                PRDept = hour.Employee.PRDept,
                CrewId = hour.Employee.CrewId,
                Cert = "Y",
                EarnCodeId = hour.EarnCodeId ?? hour.Employee.EarnCodeId,
                Shift = hour.Employee.Shift ?? 1,
                Hours = hour.HoursAdj ?? (hour.Hours ?? 0),
                Rate = hour.Rate ?? hour.Employee.HrlyRate,
                BreakHours = 0M,
				udDTCo = hour.DTCo,
				udTicketId = hour.TicketId,
				udTicketLineId = hour.TicketLineNum,

                EarnCode = hour.EarnCode,
                JCJob = hour.Job,
                EMEquipment = hour.Equipment,
                Employee = hour.Employee,
            };
            entry.Amt = entry.Hours * entry.Rate;
			entry.EarnCode ??= entry.db.EarnCodes.FirstOrDefault(f => f.PRCo == entry.Employee.PRCo && entry.EarnCodeId == f.EarnCodeId);

            if (entry.EarnCode.Factor != 1  && entry.Rate == hour.Employee.HrlyRate)
            { 
                entry.Amt *= entry.EarnCode.Factor;
                entry.Rate *= entry.EarnCode.Factor;
            }

            if (hour.Equipment != null)
            {
                entry.EMCo = hour.EMCo;
                entry.EquipmentId = hour.EquipmentId;
                entry.Type = "M";
                entry.CostCodeId = "300";
                entry.PhaseGroupId = null;
                entry.JCCo = null;
                entry.JobId = null;
                entry.PhaseId = null;
                entry.GLCo = hour.Equipment.EMCompanyParm.GLCo;
                entry.EMGroupId = hour.Equipment.EMCompanyParm.HQCompanyParm.EMGroupId;
            }
            else if (hour.Job != null)
            {
                entry.JCCo = hour.JCCo;
                entry.JobId = hour.JobId;
                entry.PhaseGroupId = hour.PhaseGroupId ?? hour.Job?.JCCompanyParm?.HQCompanyParm?.PhaseGroupId;
                entry.PhaseId = hour.PhaseId;
                entry.GLCo = hour.Job.JCCompanyParm.GLCo;
            }
            else
            {
                entry.GLCo = (byte)(hour.Employee.GLCo ?? hour.Employee.PRCompanyParm.GLCo);
            }

            if (hour.DailyTicket != null)
            {
                
                if (hour.DailyTicket.DailyJobTicket != null)
                {
                    entry.CrewId = hour.DailyTicket?.DailyJobTicket?.Crew?.CrewId;
                }
                else if (hour.DailyTicket.DailyShopTicket != null)
                {
                    entry.CrewId = hour.DailyTicket.DailyShopTicket.Crew?.CrewId;
                }
            }
            return entry;
        }

        public static PRBatchTimeEntry Init(Batch batch, DTPayrollPerdiem perdiem)
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }
            if (perdiem == null)
            {
                throw new ArgumentNullException(nameof(perdiem));
            }
            var entry = new PRBatchTimeEntry
            {
                db = batch.db,
                Batch = batch,

                Co = batch.Co,
                Mth = batch.Mth,
                BatchId = batch.BatchId,
                BatchTransType = "A",
                EmployeeId = perdiem.EmployeeId,
                PaySeq = 1,
                PostSeq = 1,
                Type = "J",
                DayNum = 0,
                PostDate = perdiem.WorkDate,

                TaxState = perdiem.Employee.TaxState,
                UnempState = perdiem.Employee.UnempState,
                InsState = perdiem.Employee.InsState,
                InsCode = perdiem.Employee.InsCode,
                PRDept = perdiem.Employee.PRDept,
                CrewId = perdiem.Employee.CrewId,
                Cert = "Y",
                EarnCodeId = perdiem.EarnCodeId ?? perdiem.Employee.EarnCodeId,
                Shift = perdiem.Employee.Shift ?? 1,
                Hours = 0,
                Rate = 0,
                Amt = 0,
                BreakHours = 0M,
				udDTCo = perdiem.DTCo,
				udTicketId = perdiem.TicketId,
                udTicketLineId = perdiem.TicketLineNum,


                JCJob = perdiem.Job,
                EMEquipment = perdiem.Equipment,
                Employee = perdiem.Employee,
            };


            if (perdiem.Equipment != null)
            {
                entry.EMCo = perdiem.EMCo;
                entry.EquipmentId = perdiem.EquipmentId;
                entry.Type = "M";
                entry.CostCodeId = "300";
                entry.JCCo = null;
                entry.JobId = null;
                entry.PhaseId = null;
                entry.GLCo = perdiem.Equipment.EMCompanyParm.GLCo;
                entry.EMGroupId = perdiem.Equipment.EMGroupId;
            }
            else if (perdiem.Job != null)
            {
                entry.JCCo = perdiem.JCCo;
                entry.JobId = perdiem.JobId;
                entry.PhaseGroupId = perdiem.PhaseGroupId ?? perdiem.Job?.JCCompanyParm?.HQCompanyParm?.PhaseGroupId;
                entry.PhaseId = perdiem.PhaseId;
                entry.GLCo = perdiem.Job.JCCompanyParm.GLCo;
            }
            else
            {
                entry.GLCo = (byte)perdiem.Employee.GLCo;
            }

            return entry;
        }
        
        public static PRBatchTimeEntry Init(PRBatchTimeEntry entry)
        {
            if (entry == null)
            {
                throw new ArgumentNullException(nameof(entry));
            }
            var model = new PRBatchTimeEntry
            {
                db = entry.db,
                Batch = entry.Batch,
				EarnCode = entry.EarnCode,
				Employee = entry.Employee,

				Co = entry.Co,
                Mth = entry.Mth,
                BatchId = entry.BatchId,
                BatchTransType = entry.BatchTransType,
                EmployeeId = entry.EmployeeId,

                PaySeq = entry.PaySeq,
                PostSeq = entry.PostSeq,
                Type = entry.Type,
                DayNum = 0,
                PostDate = entry.PostDate,

                JCCo = entry.JCCo,
                JobId = entry.JobId,
                PhaseGroupId = entry.PhaseGroupId ?? entry.Co,
                PhaseId = entry.PhaseId,

                GLCo = entry.GLCo,

                EMCo = entry.EMCo,
                EMGroupId = entry.EMGroupId,
                EquipmentId = entry.EquipmentId,
                CostCodeId = entry.CostCodeId,

                TaxState = entry.TaxState,
                UnempState = entry.UnempState,
                InsState = entry.InsState,
                InsCode = entry.InsCode,
                PRDept = entry.PRDept,
                CrewId = entry.CrewId,
                Cert = entry.Cert,
                EarnCodeId = entry.EarnCodeId,
                Shift = entry.Shift,
                Hours = entry.Hours,
                Rate = entry.Rate,
                Amt = entry.Amt,
                BreakHours = entry.BreakHours,
				udDTCo = entry.udDTCo,
				udTicketId = entry.udTicketId,
				udTicketLineId = entry.udTicketLineId
            };

            return model;
        }

        public static PRBatchTimeEntry Init(Batch batch, PayrollEntry entry)
        {
            if (batch == null)
            {
                throw new ArgumentNullException(nameof(batch));
            }
            if (entry == null)
            {
                throw new ArgumentNullException(nameof(entry));
            }
            var model = new PRBatchTimeEntry
            {
                db = batch.db,
                Batch = batch,

                Co = batch.Co,
                Mth = batch.Mth,
                BatchId = batch.BatchId,
                BatchTransType = "A",
                EmployeeId = entry.EmployeeId,
                PaySeq = entry.PaySeq,
                PostSeq = entry.PostSeq,
                Type = entry.Type,
                DayNum = 0,
                PostDate = entry.PostDate,
                JCCo = entry.JCCo,
                JobId = entry.JobId,
                PhaseGroupId = entry.PhaseGroupId ?? batch.Company.PhaseGroupId,
                PhaseId = entry.PhaseId,
                GLCo = entry.GLCo,
                EMCo = entry.EMCo,
                EMGroupId = entry.EMGroupId,
                EquipmentId = entry.EquipmentId,
                CostCodeId = entry.CostCode,
                TaxState = entry.TaxState,
                UnempState = entry.UnempState,
                InsState = entry.InsState,
                InsCode = entry.InsCode,
                PRDept = entry.PRDept,
                CrewId = entry.CrewId,
                Cert = entry.Cert,
                EarnCodeId = entry.EarnCodeId,
                Shift = entry.Shift,
                Hours = entry.Hours,
                Rate = entry.Rate,
                Amt = entry.Amt,
                BreakHours = entry.BreakHours,
				udDTCo = entry.udDTCo,
				udTicketId = entry.udTicketId,
                udTicketLineId = entry.udTicketLineId
            };

            return model;
        }

		public static List<PRBatchTimeEntry> GenerateEntries(VPContext db, Batch batch, List<int> employees, int weekId)
		{
			var results = new List<PRBatchTimeEntry>();
			if (batch == null)
				throw new ArgumentNullException(nameof(batch));
			if (db == null)
				throw new ArgumentNullException(nameof(db));

			var perdiems = db.DTPayrollPerdiems
				.Include("Employee")
				.Include("Equipment")
				.Include("Equipment.EMCompanyParm")
				.Include("Equipment.EMCompanyParm.HQCompanyParm")
				.Include("Job")
				.Include("Job.JCCompanyParm")
				.Include("Job.JCCompanyParm.HQCompanyParm")
				.Where(f => employees.Contains(f.EmployeeId) &&
				            f.Calendar.Week == weekId &&
				            (f.StatusId == (int)DB.PayrollEntryStatusEnum.Accepted || f.StatusId == (int)DB.PayrollEntryStatusEnum.Reversal)
				).ToList();

			var hours = db.DTPayrollHours
				.Include("Calendar")
				.Include("EarnCode")
				.Include("Employee")
				.Include("Employee.PRCompanyParm")
				.Include("DailyTicket")
				.Include("DailyTicket.DailyJobTicket")
				.Include("DailyTicket.DailyJobTicket.Crew")
				.Include("DailyTicket.DailyShopTicket")
				.Include("DailyTicket.DailyShopTicket.Crew")
				.Include("Equipment")
				.Include("Equipment.EMCompanyParm")
				.Include("Equipment.EMCompanyParm.HQCompanyParm")
				.Include("Job")
				.Include("Job.JCCompanyParm")
				.Include("Job.JCCompanyParm.HQCompanyParm")
				.Where(f =>
				            employees.Contains(f.EmployeeId) &&
				            f.Calendar.Week == weekId &&
				            (f.StatusId == (int)DB.PayrollEntryStatusEnum.Accepted || f.StatusId == (int)DB.PayrollEntryStatusEnum.Reversal)
				).ToList();


			GenerateHoursEntries(db, batch, hours, results);
			GeneratePerDiemEntries(db, batch, perdiems, results);
			GenerateDailyRateEntries(db, batch, hours, results);

			foreach (var perdiem in perdiems)
			{
				perdiem.DailyTicket.Status = DB.DailyTicketStatusEnum.Processed;
				perdiem.DailyTicket.ProcessedBy = StaticFunctions.GetUserId();
				perdiem.DailyTicket.ProcessedOn = DateTime.Now;

				perdiem.Status = DB.PayrollEntryStatusEnum.Posted;
				perdiem.PayrollBy = StaticFunctions.GetUserId();
				perdiem.PayrollOn = DateTime.Now;
			}
			foreach (var hour in hours)
			{
				hour.DailyTicket.Status = DB.DailyTicketStatusEnum.Processed;
				hour.DailyTicket.ProcessedBy = StaticFunctions.GetUserId();
				hour.DailyTicket.ProcessedOn = DateTime.Now;

				hour.Status = DB.PayrollEntryStatusEnum.Posted;
				hour.PayrollBy = StaticFunctions.GetUserId();
				hour.PayrollOn = DateTime.Now;
			}

			return results;
		}

		public static List<PRBatchTimeEntry> GenerateEntries(VPContext db, Batch batch, int employeeId, int weekId)
        {
            var results = new List<PRBatchTimeEntry>();
            if (batch == null)
				throw new ArgumentNullException(nameof(batch));
			if (db == null)
				throw new ArgumentNullException(nameof(db));

			var perdiems = db.DTPayrollPerdiems
				.Include("Employee")
				.Include("Equipment")
				.Include("Equipment.EMCompanyParm")
				.Include("Equipment.EMCompanyParm.HQCompanyParm")
				.Include("Job")
				.Include("Job.JCCompanyParm")
				.Include("Job.JCCompanyParm.HQCompanyParm")
				.Where(f => f.PRCo == batch.Co && 
                f.EmployeeId == employeeId && 
                f.Calendar.Week == weekId && 
                (f.StatusId == (int)DB.PayrollEntryStatusEnum.Accepted || f.StatusId == (int)DB.PayrollEntryStatusEnum.Reversal)
                ).ToList();

            var hours = db.DTPayrollHours
				.Include("Calendar")
				.Include("EarnCode")
				.Include("Employee")
				.Include("Employee.PRCompanyParm")
				.Include("DailyTicket")
				.Include("DailyTicket.DailyJobTicket")
				.Include("DailyTicket.DailyJobTicket.Crew")
				.Include("DailyTicket.DailyShopTicket")
				.Include("DailyTicket.DailyShopTicket.Crew")
				.Include("Equipment")
				.Include("Equipment.EMCompanyParm")
				.Include("Equipment.EMCompanyParm.HQCompanyParm")
				.Include("Job")
				.Include("Job.JCCompanyParm")
				.Include("Job.JCCompanyParm.HQCompanyParm")
				.Where(f => f.EmployeeId == employeeId && 
                f.Calendar.Week == weekId &&
                (f.StatusId == (int)DB.PayrollEntryStatusEnum.Accepted || f.StatusId == (int)DB.PayrollEntryStatusEnum.Reversal)
                ).ToList();

            
            GenerateHoursEntries(db, batch, hours, results);
            GeneratePerDiemEntries(db, batch, perdiems, results);
            GenerateDailyRateEntries(db, batch, hours, results);

            foreach (var perdiem in perdiems)
            {
                perdiem.DailyTicket.Status = DB.DailyTicketStatusEnum.Processed;
                perdiem.DailyTicket.ProcessedBy = StaticFunctions.GetUserId();
                perdiem.DailyTicket.ProcessedOn = DateTime.Now;
                
                perdiem.Status = DB.PayrollEntryStatusEnum.Posted;
                perdiem.PayrollBy = StaticFunctions.GetUserId();
                perdiem.PayrollOn = DateTime.Now;
            }
            foreach (var hour in hours)
            {
                hour.DailyTicket.Status = DB.DailyTicketStatusEnum.Processed;
                hour.DailyTicket.ProcessedBy = StaticFunctions.GetUserId();
                hour.DailyTicket.ProcessedOn = DateTime.Now;

                hour.Status = DB.PayrollEntryStatusEnum.Posted;
                hour.PayrollBy = StaticFunctions.GetUserId();
                hour.PayrollOn = DateTime.Now;
            }

            return results;
        }

        private static void GenerateDailyRateEntries(VPContext db, Batch batch, List<DTPayrollHour> hours, List<PRBatchTimeEntry> results)
        {            
            var resultsbatchSeq = results.DefaultIfEmpty().Max(max => max == null ? 0 : max.BatchSeq);
            var batchSeq = batch.PRBatchTimeEntries.DefaultIfEmpty().Max(max => max == null ? 0 : max.BatchSeq);
            batchSeq = resultsbatchSeq > batchSeq ? resultsbatchSeq : batchSeq;

            batchSeq++;
            var dailyEarnCode = db.EarnCodes.FirstOrDefault(f => f.PRCo == batch.Co && f.EarnCodeId == 30);
            foreach (var entry in hours.Where(w => (w.Employee.udDailyRate ?? 0) != 0)
                                        .GroupBy(g => new { g.WorkDate, g.Employee, g.EarnCodeId })
                                        .Select(s => new
                                        {
                                            s.Key.WorkDate,
                                            s.Key.Employee,
                                            s.Key.EarnCodeId,
                                            totalHours = s.Sum(max => max.Hours ?? 0),
                                            List = s.ToList()
                                        })
                                        .Where(w => w.totalHours != 0)
                                        .ToList())
            {
                foreach (var hour in entry.List)
                {
                    if (hour.Hours != 0)
					{
						var item = Init(batch, hour);
						item.BatchSeq = batchSeq;
						batchSeq++;
						item.EarnCodeId = dailyEarnCode.EarnCodeId;
						//item.Hours ??= 0.0m;

						if (item.EarnCode?.EarnCodeId != item.EarnCodeId)
							item.EarnCode = null;
						if ((hour.Hours ?? 0) != 0)
							item.Hours = entry.totalHours / (hour.Hours ?? 0);
						else
							item.Hours = 0;

						if (item.Hours != 0)
						{
							item.Rate = hour.Employee.udDailyRate ?? 0;
							item.Amt = item.Hours * item.Rate;
						}
						item.Hours = 0;
						results.Add(item);
					}
                }
            }
        }

        private static void GenerateHoursEntries(VPContext db, Batch batch, List<DTPayrollHour> hours, List<PRBatchTimeEntry> results)
        {
            var resultsbatchSeq = results.DefaultIfEmpty().Max(max => max == null ? 0 : max.BatchSeq);
            var batchSeq = batch.PRBatchTimeEntries.DefaultIfEmpty().Max(max => max == null ? 0 : max.BatchSeq);
            batchSeq = resultsbatchSeq > batchSeq ? resultsbatchSeq : batchSeq;
            batchSeq++;

            var emps = hours
                .GroupBy(g => new { g.EmployeeId,})
                .Select(s => new { 
                    s.Key.EmployeeId,
                    Days = s.GroupBy(dGrp => dGrp.WorkDate)
                            .Select(d => new { 
                                WorkDate = d.Key,
                                DayTotal = d.Sum(sum => sum.PayHours),
								ReverseTotal = d.Sum(sum => sum.Status == DB.PayrollEntryStatusEnum.Reversal ? sum.PayHours : 0),
								NewTotal = d.Sum(sum => sum.Status == DB.PayrollEntryStatusEnum.Accepted ? sum.PayHours : 0),
								Entries = d.ToList()
                            }).ToList()
                    }).ToList();


            foreach (var emp in emps)
            {
                foreach (var day in emp.Days)
                {

                    if (day.NewTotal + day.ReverseTotal == 0)
						day.Entries.ForEach(e => e.Status = DB.PayrollEntryStatusEnum.Posted);
					else
					{
                        var isRetro = day.WorkDate < batch.PREndDate.Value.AddDays(-6);
                        foreach (var entry in day.Entries)
                        {
                            var item = Init(batch, entry);
                            if (item.Hours != 0)
                            {
                                entry.EarnCode ??= entry.Employee.EarnCode;
                                if (isRetro)
                                    item.EarnCodeId = entry.EarnCode.udRetroEarnCode ?? entry.EarnCode.EarnCodeId;

                                if (item.EarnCode?.EarnCodeId != item.EarnCodeId)
									item.EarnCode = null;

								item.BatchSeq = batchSeq;
								batchSeq++;
								results.Add(item);
							}

							entry.Status = DB.PayrollEntryStatusEnum.Posted;
							entry.BatchId = batch.BatchId;
							entry.BatchSeq = item.BatchSeq;
						}
					}

                }
            }
            //foreach (var entry in hours)
            //{
               
            //    var isRetro = entry.WorkDate < batch.PREndDate.Value.AddDays(-6);
            //    var item = Init(batch, entry);
            //    if (item.Hours != 0)
            //    {
            //        entry.EarnCode ??= entry.Employee.EarnCode;
            //        if (entry.EarnCode != null)
            //        {
            //            item.EarnCodeId = isRetro ? entry.EarnCode.udRetroEarnCode ?? entry.EarnCode.EarnCodeId : entry.EarnCode.EarnCodeId;
            //        }
            //        else
            //        {
            //            item.EarnCodeId = isRetro ? entry.Employee.EarnCode.udRetroEarnCode ?? entry.Employee.EarnCode.EarnCodeId : entry.Employee.EarnCode.EarnCodeId;
            //            entry.EarnCodeId = entry.Employee.EarnCode.EarnCodeId;
            //        }
            //        if (item.EarnCode?.EarnCodeId != item.EarnCodeId)
            //            item.EarnCode = null;

            //        item.BatchSeq = batchSeq;
            //        batchSeq++;
            //        results.Add(item);
            //    }

            //    entry.Status = DB.PayrollEntryStatusEnum.Posted;
            //    entry.BatchId = batch.BatchId;
            //    entry.BatchSeq = item.BatchSeq;
            //}

        }

        private static void GeneratePerDiemEntries(VPContext db, Batch batch, List<DTPayrollPerdiem> perDiems, List<PRBatchTimeEntry> results)
        {
            var resultsBatchSeq = results.DefaultIfEmpty().Max(max => max == null ? 0 : max.BatchSeq);
            var batchSeq = batch.PRBatchTimeEntries.DefaultIfEmpty().Max(max => max == null ? 0 : max.BatchSeq);
            batchSeq = resultsBatchSeq > batchSeq ? resultsBatchSeq : batchSeq;

            var perDiemEarnCode = db.EarnCodes.FirstOrDefault(f => f.PRCo == batch.Co && f.EarnCodeId == 20);

            batchSeq++;
			var emps = perDiems
			   .GroupBy(g => new { g.EmployeeId, })
			   .Select(s => new {
				   s.Key.EmployeeId,
				   Days = s.GroupBy(dGrp => dGrp.WorkDate)
						   .Select(d => new
                           {
                               WorkDate = d.Key,
                               IsRetro = d.Key < batch.PREndDate.Value.AddDays(-6),
							   PerDiemId = d.Max(max => max.PayPerDiem),
							   PerDiemAmt = d.Max(max => max.PayAmt),
							   Entries = d.ToList(),
                               
						   }).ToList()
			   }).ToList();


			foreach (var emp in emps)
			{
				foreach (var day in emp.Days)
				{
                    var hourEntries = db.DTPayrollHours.Where(f => f.EmployeeId == emp.EmployeeId &&
                                                                   f.WorkDate == day.WorkDate &&
                                                                   ((f.HoursAdj ?? f.Hours) ?? 0) != 0).ToList();
                    var postedEntries = db.PayrollEntries.Where(f => f.PRCo == batch.Co &&
                                                                f.PostDate == day.WorkDate &&
                                                                f.EmployeeId == emp.EmployeeId &&
                                                                (f.EarnCodeId == perDiemEarnCode.EarnCodeId ||
                                                                 f.EarnCodeId == perDiemEarnCode.udRetroEarnCode)).ToList();

					var runningTotal = 0m;
                    var reverseEntries = new List<PRBatchTimeEntry>();
					foreach (var postedEntry in postedEntries)
					{
						var item = Init(batch, postedEntry);
						item.EarnCodeId = day.IsRetro ? postedEntry.EarnCode.udRetroEarnCode ?? item.EarnCodeId : item.EarnCodeId;
						item.Hours *= -1;
						item.Amt *= -1;
						item.Rate *= -1;
						item.BatchSeq = batchSeq;
						reverseEntries.Add(item);
						batchSeq++;
					}
                    var reverseTotal = reverseEntries.Sum(sum => sum.Amt);

					if (day.PerDiemAmt + reverseTotal == 0)
						day.Entries.ForEach(e => e.Status = DB.PayrollEntryStatusEnum.Posted);
                    else
					{
                        //Add the reversal entries to the result
                        reverseEntries.ForEach(e => results.Add(e));

						/** Weight the per diem amount based on day total hours**/
						if (hourEntries.Any())
						{
							var totalHours = hourEntries.Sum(sum => sum.PayHours);
							foreach (var hourEntry in hourEntries)
							{
								var calPerDiemAmt = Math.Round(day.PerDiemAmt * ((hourEntry.PayHours) / totalHours), 2);

								if (calPerDiemAmt != 0)
								{
									var item = Init(batch, hourEntry);
									item.EarnCodeId = day.IsRetro ? perDiemEarnCode.udRetroEarnCode ?? perDiemEarnCode.EarnCodeId : perDiemEarnCode.EarnCodeId;
									item.Hours = 0;
									item.Rate = calPerDiemAmt;
									item.Amt = calPerDiemAmt;
									item.BatchSeq = batchSeq;

									runningTotal += calPerDiemAmt;
									batchSeq++;
									results.Add(item);
								}
							}
						}
						/** Weight the per diem amount based on total entries when no hours**/
						else
						{
							var percent = 1M / day.Entries.Count;
							foreach (var perDiemEntry in day.Entries)
							{
								var calPerDiemAmt = Math.Round(day.PerDiemAmt * percent, 2);
								if (calPerDiemAmt != 0)
								{
									var item = Init(batch, perDiemEntry);
									item.EarnCodeId = day.IsRetro ? perDiemEarnCode.udRetroEarnCode ?? perDiemEarnCode.EarnCodeId : perDiemEarnCode.EarnCodeId;
									item.Hours = 0;
									item.Rate = calPerDiemAmt;
									item.Amt = calPerDiemAmt;
									item.BatchSeq = batchSeq;

									runningTotal += calPerDiemAmt;
									batchSeq++;
									results.Add(item);
								}
							}
						}

						//Correct for rounding issues of per diem
						if (runningTotal != day.PerDiemAmt)
						{
                            var earnCodeId = day.IsRetro ? perDiemEarnCode.udRetroEarnCode ?? perDiemEarnCode.EarnCodeId : perDiemEarnCode.EarnCodeId;
							var perDiemEntry = results.FirstOrDefault(f => f.PostDate == day.WorkDate &&
																			f.EmployeeId == emp.EmployeeId &&
																			f.EarnCodeId == earnCodeId);
							perDiemEntry.Amt += (day.PerDiemAmt - runningTotal);
							perDiemEntry.Rate += (day.PerDiemAmt - runningTotal);
						}

						day.Entries.ForEach(e => {
							e.Status = DB.PayrollEntryStatusEnum.Posted;
							e.BatchId = batch.BatchId;
						});

					}
				}
			}
        }

        public static void GenerateRetroOTEntries(VPContext db, Batch batch, List<int> employees, int weekId)
		{
			if (db == null || batch== null)
				return;

			var prParm = db.PRCompanyParms.FirstOrDefault(f => f.PRCo == batch.Co);
			//var retroDate = ((DateTime)batch.PREndDate).AddDays(-6);
            var retroEntries = db.PRBatchTimeEntries.Where(f => f.Co == batch.Co && 
                                                                f.BatchId == batch.BatchId && 
                                                                employees.Contains(f.EmployeeId) &&
                                                                //f.PostDate < retroDate && 
                                                                f.Calendar.Week == weekId &&
															    f.EarnCode.Method == "H")
                                                          .ToList();
			var batchSeq = db.PRBatchTimeEntries.Where(f => f.Co == batch.Co &&
														    f.Mth == batch.Mth &&
														    f.BatchId == batch.BatchId)
											.DefaultIfEmpty()
											.Max(max => max == null ? 0 : max.BatchSeq);
			batchSeq++;
            var empGroup = retroEntries.GroupBy(eGrp => eGrp.Employee)
                                        .Select(emp => new
                                        {
                                            Employee = emp.Key,
                                            emp.Key.PRCo,
                                            emp.Key.EmployeeId,
                                            PostedEntries = db.PayrollEntries.Where(f => f.PRCo == emp.Key.PRCo &&
                                                                                    f.EmployeeId == emp.Key.EmployeeId &&
                                                                                    f.Calendar.Week == weekId &&
                                                                                    f.EarnCode.Method == "H" &&
                                                                                    f.Hours != 0 &&
                                                                                    (   f.EarnCodeId == emp.Key.EarnCodeId ||
                                                                                        f.EarnCodeId == emp.Key.EarnCode.udRetroEarnCode ||
                                                                                        f.EarnCodeId == prParm.OTEarnCode.EarnCodeId ||
                                                                                        f.EarnCodeId == prParm.OTEarnCode.udRetroEarnCode))
                                                                                    .ToList(),
                                            RetroEntries = emp.Where(f => f.Hours != 0 &&
                                                                        (f.EarnCodeId == emp.Key.EarnCodeId ||
                                                                        f.EarnCodeId == emp.Key.EarnCode.udRetroEarnCode ||
                                                                        f.EarnCodeId == prParm.OTEarnCode.EarnCodeId ||
                                                                        f.EarnCodeId == prParm.OTEarnCode.udRetroEarnCode)).ToList(),
                                        }).ToList();
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
            foreach (var emp in empGroup)
            {
                var runningTotal = 0M;
                var newEntries = new List<PRBatchTimeEntry>();

                var correctionEntries = emp.PostedEntries.Select(s => s.ToCorrectionTimeEntry(batch)).ToList();
				var entries = emp.RetroEntries.Union(correctionEntries)
                    .OrderBy(o => o.Hours <= 0 ? new DateTime(2000,1,1) : o.PostDate) //Make sure you roll up negative hours first!
                    .ThenBy(o => o.Hours)
                    .ToList();
                foreach (var entry in entries)
                {
                    var updEntry = entry;
                    var newRunningTotal = runningTotal + updEntry.Hours;

					if (updEntry.IsInBatch != false)
						updEntry = emp.RetroEntries.FirstOrDefault(f => f.BatchSeq == entry.BatchSeq);

					updEntry.EarnCodeId = emp.Employee.EarnCode.udRetroEarnCode ?? emp.Employee.EarnCodeId;
					updEntry.Rate = updEntry.Employee.HrlyRate;
					updEntry.Amt = updEntry.Rate * updEntry.Hours;
					if (runningTotal >= 40)
					{
						updEntry.EarnCodeId = prParm.OTEarnCode.udRetroEarnCode ?? prParm.OTEarnCode.EarnCodeId;
						updEntry.Rate *= prParm.OTEarnCode.Factor;
						updEntry.Amt = updEntry.Rate * updEntry.Hours;
					}
					//Split the entry into two if total is over 40
					else if (runningTotal + updEntry.Hours > 40)
					{
						var regHour = 40 - runningTotal;
						var otHour = updEntry.Hours - regHour;

						updEntry.Hours = regHour;
						updEntry.Amt = updEntry.Hours * updEntry.Rate;

						if (otHour > 0)
						{
							var OTEntry = Init(entry);
                            OTEntry.BatchTransType = "A";
							OTEntry.Rate = prParm.OTEarnCode.Factor * OTEntry.Rate;
							OTEntry.Hours = otHour;
							OTEntry.Amt = OTEntry.Rate * OTEntry.Hours;
							OTEntry.EarnCodeId = prParm.OTEarnCode.udRetroEarnCode ?? OTEntry.EarnCodeId;
							OTEntry.IsInBatch = false;

							if (OTEntry.EarnCode?.EarnCodeId != OTEntry.EarnCodeId)
								OTEntry.EarnCode = null;

							newEntries.Add(OTEntry);
						}

					}
					runningTotal += updEntry.Hours;
				}

				entries.AddRange(newEntries);

                var updEntries = entries.Where(a => a.IsInBatch == false &&
                                                    (a.OldRate != a.Rate ||
                                                     a.OldAmt != a.Amt ||
                                                     a.OldHours != a.Hours)).ToList();
				
				foreach (var entry in updEntries)
                {
                    
					var postedEntry = db.PayrollEntries.FirstOrDefault(f => f.PRCo == entry.Co &&
																			f.PRGroup == entry.OldPRGroup &&
																			f.PREndDate == entry.OldPREndDate &&
																			f.EmployeeId == entry.EmployeeId &&
																			f.PostDate == entry.OldPostDate &&
																			f.PaySeq == entry.OldPaySeq &&
																			f.PostSeq == entry.OldPostSeq);
                    if (postedEntry != null)
					{
                        var reverseEntry = postedEntry.ToCorrectionTimeEntry(batch);
						reverseEntry.BatchTransType = "A";
						reverseEntry.EarnCodeId = postedEntry.EarnCode.udRetroEarnCode ?? reverseEntry.EarnCodeId;
						reverseEntry.Hours *= -1;
						reverseEntry.Amt *= -1;
						reverseEntry.Rate *= -1;
						reverseEntry.BatchSeq = batchSeq;
                        batchSeq++;
						db.PRBatchTimeEntries.Add(reverseEntry);

						entry.BatchTransType = "A"; 
						entry.BatchSeq = batchSeq;
						batchSeq++;
						db.PRBatchTimeEntries.Add(entry);
					}
                    else
                    {
						entry.BatchSeq = batchSeq;
						batchSeq++;

						db.PRBatchTimeEntries.Add(entry);
					}
                }
                    
			}
        }


		public static void GenerateRetroOTEntries(VPContext db, Batch batch, int employeeId, int weekId)
        {
            if (db == null)
                return;
            var entries = db.PRBatchTimeEntries.Where(f => f.Co == batch.Co && 
                                                        f.Mth == batch.Mth && 
                                                        f.BatchId == batch.BatchId &&
                                                        f.EarnCode.Description.ToLower().Contains("retro") &&
                                                        f.EarnCode.Method == "H" &&
                                                        f.EmployeeId == employeeId &&
                                                        f.Calendar.Week == weekId
                                                        ).ToList();
            var prParm = db.PRCompanyParms.FirstOrDefault(f => f.PRCo == batch.Co);

            var batchSeq = db.PRBatchTimeEntries.Where(f => f.Co == batch.Co &&
                                                         f.Mth == batch.Mth &&
                                                         f.BatchId == batch.BatchId)
                                             .DefaultIfEmpty()
                                             .Max(max => max == null ? 0 : max.BatchSeq);
            batchSeq++;
            foreach (var entry in entries.GroupBy(g => new { g.Co, g.Employee })                                            
                                         .Select(s => new { s.Key.Co, s.Key.Employee, List = s }).ToList())
            {
                //Get all Posted hours that are eligible for OT calculation
                var postedHours = db.PayrollEntries.Where(f => f.PRCo == entry.Co &&
                                                               f.EmployeeId == entry.Employee.EmployeeId &&
															   f.Calendar.Week == weekId &&
															   f.EarnCode.Method == "H" &&
                                                               //(f.EarnCode.OTCalcs == "Y"  ||  f.EarnCode.Description.ToLower().Contains("retro")) &&   //Removed 2-8-23 this filtered OT Hourly which caused the posted hours to be off
                                                               (f.EarnCodeId == entry.Employee.EarnCodeId || 
                                                                f.EarnCodeId == entry.Employee.EarnCode.udRetroEarnCode ||
                                                                f.EarnCodeId == prParm.OTEarnCode.EarnCodeId  ||
                                                                f.EarnCodeId == prParm.OTEarnCode.udRetroEarnCode))
                                                    .Sum(sum => sum.Hours);
                var days = entry.List.GroupBy(g => g.PostDate)
                                    .Select(s => new { 
                                        PostDate = s.Key, 

                                        list = s.OrderBy(o => o.Hours).ToList() 
                                    })
                                    .OrderBy(o => o.PostDate)
                                    .ToList();

				//Added grouping by day for in and out total 9-24-2020
				//Added Order by hours to ensure it subtracted the negatives prior to adding the positives.
				foreach (var day in days)
                {
                    var dayTotal = day.list.Sum(sum => sum.Hours);
                    if (postedHours + dayTotal > 40)
                    {
                        foreach (var hour in day.list)
                        {
                            if (postedHours  > 40)
                            {
                                hour.EarnCodeId = prParm.OTEarnCode.udRetroEarnCode ?? hour.EarnCodeId;
                                hour.Rate = hour.Employee.HrlyRate;
								hour.Rate *= prParm.OTEarnCode.Factor;   //add 7-7-2020
                                hour.Amt = hour.Rate * hour.Hours;                  //add 7-7-2020
							}
							//Split the entry into two if total is over 40
							else if (postedHours + hour.Hours <= 40)
                            {
								hour.EarnCodeId = hour.Employee.EarnCode.udRetroEarnCode ?? hour.Employee.EarnCodeId;
								hour.Rate = hour.Employee.HrlyRate;
								hour.Rate *= prParm.OTEarnCode.Factor;   //add 7-7-2020
								hour.Amt = hour.Rate * hour.Hours;                  //add 7-7-2020
							}
							//Split the entry into two if total is over 40
							else if (postedHours + hour.Hours > 40)
                            {
                                var regHour = 40 - postedHours;
                                var otHour = hour.Hours - regHour;

                                hour.Hours = regHour;
                                hour.Amt = hour.Hours * hour.Rate;  //add 3-20-2020

                                if (otHour > 0)
                                {
                                    var OTEntry = Init(hour);
									if (hour.Employee.HrlyRate != OTEntry.Rate && OTEntry.EarnCode.Factor != 1)
										OTEntry.Rate = OTEntry.Employee.HrlyRate;
									OTEntry.Rate = prParm.OTEarnCode.Factor * OTEntry.Rate;
                                    OTEntry.Hours = otHour;
                                    OTEntry.Amt = OTEntry.Rate * OTEntry.Hours;
                                    OTEntry.EarnCodeId = prParm.OTEarnCode.udRetroEarnCode ?? OTEntry.EarnCodeId;
                                    OTEntry.BatchSeq = batchSeq;
                                    batchSeq++;

                                    if (OTEntry.EarnCode?.EarnCodeId != OTEntry.EarnCodeId)
                                        OTEntry.EarnCode = null;

                                    db.PRBatchTimeEntries.Add(OTEntry);
                                }
                            }
                            postedHours += hour.Hours;
                        }
                    }
                    else
                    {
                        postedHours += dayTotal;
                    }                        
                }
            }
        }
    }
}