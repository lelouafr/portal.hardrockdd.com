//using DB.Infrastructure.ViewPointDB.Data;
//using portal.Repository.VP.JC;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.Caching;
//using System.Web;

//namespace portal.Repository.VP.GL
//{
//    public static class AccountRepository
//    {

//        public static string GetDefaultJobGL(byte co, string jobId, string phaseId, byte costTypeId)
//        {
//            using var db = new VPContext();
//            var userId = StaticFunctions.GetUserId();
//            ObjectCache systemCache = MemoryCache.Default;
//            CacheItemPolicy policy = new CacheItemPolicy
//            {
//                AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(15)
//            };

//            //var memKey = "JobList";
//            //if (!(MemoryCache.Default[memKey] is List<Job> jobList))
//            //{
//            //    jobList = db.Jobs.Where(f => f.JCCo == co).ToList();
//            //    systemCache.Set(memKey, jobList, policy);
//            //}
//            var jobList = JobRepository.GetJobs(co, false);

//            var memKey = "JobContractList";
//            if (!(MemoryCache.Default[memKey] is List<JCContract> contractList))
//            {
//                contractList = db.JCContracts.Where(f => f.JCCo == co).ToList();
//                systemCache.Set(memKey, contractList, policy);
//            }

//            memKey = "JobDepartmentCodeList";
//            if (!(MemoryCache.Default[memKey] is List<JobDepartmentCode> jobDepartmentCodeList))
//            {
//                jobDepartmentCodeList = db.JobDepartmentCodes.Where(f => f.JCCo == co).ToList();
//                systemCache.Set(memKey, jobDepartmentCodeList, policy);
//            }

//            var job = jobList.FirstOrDefault(f => f.JobId == jobId);
//            var contract = contractList.FirstOrDefault(f => f.ContractId == job.ContractId);
//            var coding = jobDepartmentCodeList.FirstOrDefault(f => f.DepartmentId == contract.DepartmentId && f.CostTypeId == costTypeId);

//            return job.JobStatus == 3 ? coding.ClosedExpAcct : coding.OpenWIPAcct;
//        }

//        public static string GetDefaultEquipmentGL(byte co, string equipmentId, string costCode, byte costTypeId)
//        {
//            using var db = new VPContext();
//            var userId = StaticFunctions.GetUserId();
//            ObjectCache systemCache = MemoryCache.Default;
//            CacheItemPolicy policy = new CacheItemPolicy
//            {
//                AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(15)
//            };

//            var memKey = "EquipmnetList";
//            if (!(MemoryCache.Default[memKey] is List<Equipment> eqpList))
//            {
//                eqpList = db.Equipments.Where(f => f.EMCo == co).ToList();
//                systemCache.Set(memKey, eqpList, policy);
//            }

//            memKey = "EMDeptCostCodeGLAccountList";
//            if (!(MemoryCache.Default[memKey] is List<EMDeptCostCodeGLAccount> deptCostCodeGLAccount))
//            {
//                deptCostCodeGLAccount = db.EMDeptCostCodeGLAccounts.Where(f => f.EMCo == co).ToList();
//                systemCache.Set(memKey, deptCostCodeGLAccount, policy);
//            }

//            memKey = "EMDepartmentCostTypeGLAcctList";
//            if (!(MemoryCache.Default[memKey] is List<EMDepartmentCostTypeGLAcct> departmentCostTypeGLAcct))
//            {
//                departmentCostTypeGLAcct = db.EMDepartmentCostTypeGLAccts.Where(f => f.EMCo == co).ToList();
//                systemCache.Set(memKey, departmentCostTypeGLAcct, policy);
//            }

//            var equipment = eqpList.FirstOrDefault(f => f.EquipmentId == equipmentId);
//            var costCodeOverride = deptCostCodeGLAccount.FirstOrDefault(f => f.DepartmentId == equipment.DepartmentId && f.CostCodeID == costCode);
//            if (costCodeOverride != null)
//            {
//                return costCodeOverride.GLAcct;
//            }

//            var coding = departmentCostTypeGLAcct.FirstOrDefault(f => f.DepartmentId == equipment.DepartmentId && f.CostTypeId == costTypeId);

//            return coding.GLAcct;
//        }
//    }
//}