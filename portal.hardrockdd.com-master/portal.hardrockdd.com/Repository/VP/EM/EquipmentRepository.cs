using DB.Infrastructure.ViewPointDB.Data;
using portal.Models.Views.Equipment;
using portal.Models.Views.Equipment.Forms;
using System;
using System.Linq;

namespace portal.Repository.VP.EM
{
    public static class EquipmentRepository
    {

        public static EquipmentInfoViewModel ProcessUpdate(EquipmentInfoViewModel model, VPContext db)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            if (db == null)
            {
                throw new ArgumentNullException(nameof(db));
            }
            var updObj = db.Equipments.Where(f => f.EMCo == model.EMCo && f.EquipmentId == model.EquipmentId).FirstOrDefault();

            if (updObj != null)
            {
                /****Write the changes to object****/
                updObj.Description = model.Description;
                updObj.Status = model.StatusId;
                updObj.CategoryId = model.CategoryId;
                updObj.Manufacturer = model.Manufacturer;
                updObj.Model = model.EquipModel;
                updObj.ModelYr = model.ModelYr;
                updObj.VINNumber = model.VINNumber;
                updObj.TXTagId = model.TXTagId;
                //db.SaveChanges(modelState);
                return new EquipmentInfoViewModel(updObj);
            }
            return model;
        }

        public static EquipmentAssignmentViewModel ProcessUpdate(EquipmentAssignmentViewModel model, VPContext db)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            if (db == null)
            {
                throw new ArgumentNullException(nameof(db));
            }
            var updObj = db.Equipments.Where(f => f.EMCo == model.EMCo && f.EquipmentId == model.EquipmentId).FirstOrDefault();

            if (updObj != null)
            {
                var addLogAssignment = false;
                if (updObj.AssignmentType != (byte?)model.AssignmentType ||
                    updObj.ParentEquimentId != model.ParentEquimentId ||
                    updObj.AssignedCrewId != model.AssignedCrewId ||
                    updObj.OperatorId != model.Operator)
                {
                    addLogAssignment = true;
                }

                /****Write the changes to object****/
                updObj.AssignmentType = (byte?)model.AssignmentType;
                updObj.ParentEquimentId = model.ParentEquimentId;
                updObj.AssignedCrewId = model.AssignedCrewId;
                updObj.OperatorId = model.Operator;

                if (addLogAssignment)
                {
                    var log = EquipmentLogRepository.Init(updObj);
                    log.LogTypeId = (int)DB.EMLogTypeEnum.Assignment;
                    updObj.Logs.Add(log);
                }
                return new EquipmentAssignmentViewModel(updObj);
            }
            return model;
        }

        public static EquipmentOwnershipViewModel ProcessUpdate(EquipmentOwnershipViewModel model, VPContext db)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            if (db == null)
            {
                throw new ArgumentNullException(nameof(db));
            }
            var updObj = db.Equipments.Where(f => f.EMCo == model.EMCo && f.EquipmentId == model.EquipmentId).FirstOrDefault();

            if (updObj != null)
            {
                /****Write the changes to object****/
                updObj.OwnershipStatus = model.OwnershipStatusId;

                updObj.PurchasedFrom = model.PurchasedFrom;
                updObj.PurchasePrice = model.PurchasePrice;
                updObj.PurchDate = model.PurchDate;

                updObj.LeasedFrom = model.LeasedFrom;
                updObj.LeaseStartDate = model.LeaseStartDate;
                updObj.LeaseEndDate = model.LeaseEndDate;
                updObj.LeasePayment = model.LeasePayment;
                return new EquipmentOwnershipViewModel(updObj);
            }
            return model;
        }

        public static EquipmentLicInfoViewModel ProcessUpdate(EquipmentLicInfoViewModel model, VPContext db)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (db == null) throw new ArgumentNullException(nameof(db));

            var updObj = db.Equipments.Where(f => f.EMCo == model.EMCo && f.EquipmentId == model.EquipmentId).FirstOrDefault();
            if (updObj != null)
            {
                /****Write the changes to object****/
                updObj.LicensePlateNo = model.LicensePlateNo;
                updObj.LicensePlateState = model.LicensePlateState;
                updObj.LicensePlateExpDate = model.LicensePlateExpDate;
                updObj.InspectionExpiration = model.InspectionExpDate;
                updObj.RegisteredGVWR = model.RegGVWR;
            }
            return model;
        }
        
        public static EquipmentSpecViewModel ProcessUpdate(EquipmentSpecViewModel model, VPContext db)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (db == null) throw new ArgumentNullException(nameof(db));

            var updObj = db.Equipments.Where(f => f.EMCo == model.EMCo && f.EquipmentId == model.EquipmentId).FirstOrDefault();
            if (updObj != null)
            {
                /****Write the changes to object****/
                updObj.GrossVehicleWeight = model.GrossVehicleWeight;
                updObj.TareWeight = model.TareWeight;
                updObj.Height = model.Height;
                updObj.Wheelbase = model.Wheelbase;
                updObj.NoAxles = model.NoAxles;
                updObj.Width = model.Width;
                updObj.OverallLength = model.OverallLength;
                updObj.HorsePower = model.HorsePower;
                updObj.TireSize = model.TireSize;
            }
            return model;
        }

        public static EquipmentMeterViewModel ProcessUpdate(EquipmentMeterViewModel model, VPContext db)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            if (db == null)
            {
                throw new ArgumentNullException(nameof(db));
            }
            var updObj = db.Equipments.Where(f => f.EMCo == model.EMCo && f.EquipmentId == model.EquipmentId).FirstOrDefault();

            if (updObj != null)
            {
                var logMeterUpdate = false;
                if (updObj.MeterTypeId != ((int?)model.MeterType).ToString() &&
                    (updObj.OdoReading != model.OdoReading ||
                    updObj.HourReading != model.HourReading))
                {
                    logMeterUpdate = true;
                }
                /****Write the changes to object****/
                updObj.MeterTypeId = ((int?)model.MeterType).ToString();
                updObj.OdoReading = model.OdoReading;
                updObj.OdoDate = updObj.OdoReading != model.OdoReading ? DateTime.UtcNow : updObj.OdoDate;
                //updObj.OdoDate = model.OdoDate >= new DateTime(1900, 1, 1) ? model.OdoDate : updObj.OdoDate;
                //updObj.ReplacedOdoReading = model.ReplacedOdoReading;
                //updObj.ReplacedOdoDate = model.ReplacedOdoDate >= new DateTime(1900, 1, 1) ? model.ReplacedOdoDate : updObj.ReplacedOdoDate;
                updObj.HourReading = model.HourReading;
                //updObj.HourDate = model.HourDate >= new DateTime(1900, 1, 1) ? model.HourDate : updObj.HourDate;
                updObj.HourDate = updObj.HourReading != model.HourReading ? DateTime.UtcNow : updObj.HourDate;
                //updObj.ReplacedHourReading = model.ReplacedHourReading;
                //updObj.ReplacedHourDate = model.ReplacedHourDate >= new DateTime(1900, 1, 1) ? model.ReplacedHourDate : updObj.ReplacedHourDate;

                if (logMeterUpdate)
                {
                    var log = EquipmentLogRepository.Init(updObj);
                    log.LogTypeId = (int)DB.EMLogTypeEnum.MeterUpdate;
                    updObj.Logs.Add(log);
                }
                //db.SaveChanges(modelState);
                return new EquipmentMeterViewModel(updObj);
            }
            return model;
        }

        public static EquipmentViewModel ProcessUpdate(EquipmentViewModel model, VPContext db)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            if (db == null)
            {
                throw new ArgumentNullException(nameof(db));
            }
            var updObj = db.Equipments.Where(f => f.EMCo == model.EMCo && f.EquipmentId == model.EquipmentId).FirstOrDefault();

            if (updObj != null)
            {
                var addLogAssignment = false;
                if (updObj.AssignmentType != (byte?)model.AssignmentType ||
                    updObj.ParentEquimentId != model.ParentEquimentId ||
                    updObj.AssignedCrewId != model.AssignedCrewId ||
                    updObj.OperatorId != model.Operator)
                {
                    addLogAssignment = true;
                }
                var logMeterUpdate = false;
                if (updObj.MeterTypeId != ((int?)model.MeterType).ToString() ||
                    updObj.OdoReading != model.OdoReading ||
                    updObj.HourReading != model.HourReading)
                {
                    logMeterUpdate = true;
                }
                /****Write the changes to object****/
                //updObj.Location = model.Location;
                //updObj.Type = model.TypeId;
                //updObj.Department = model.Department;
                updObj.CategoryId = model.CategoryId;
                updObj.Manufacturer = model.Manufacturer;
                updObj.Model = model.EquipModel;
                updObj.ModelYr = model.ModelYr;
                updObj.VINNumber = model.VINNumber;
                updObj.Description = model.Description;
                updObj.Status = model.StatusId;
                updObj.MeterTypeId = ((int?)model.MeterType).ToString();
                updObj.OdoReading = model.OdoReading;
                updObj.OdoDate = updObj.OdoReading != model.OdoReading ? DateTime.UtcNow : updObj.OdoDate;
                //updObj.OdoDate = model.OdoDate >= new DateTime(1900, 1, 1) ? model.OdoDate : updObj.OdoDate;
                //updObj.ReplacedOdoReading = model.ReplacedOdoReading;
                //updObj.ReplacedOdoDate = model.ReplacedOdoDate >= new DateTime(1900, 1, 1) ? model.ReplacedOdoDate : updObj.ReplacedOdoDate;
                updObj.HourReading = model.HourReading;
                //updObj.HourDate = model.HourDate >= new DateTime(1900, 1, 1) ? model.HourDate : updObj.HourDate;
                updObj.HourDate = updObj.HourReading != model.HourReading ? DateTime.UtcNow : updObj.HourDate;
                //updObj.ReplacedHourReading = model.ReplacedHourReading;
                //updObj.ReplacedHourDate = model.ReplacedHourDate >= new DateTime(1900, 1, 1) ? model.ReplacedHourDate : updObj.ReplacedHourDate;
                //updObj.JCCo = model.Co;
                //updObj.JobId = model.JobId;
                //updObj.PhaseGrp = model.Co;
                //updObj.LicensePlateNo = model.LicensePlateNo;
                //updObj.LicensePlateState = model.LicensePlateState;
                //updObj.LicensePlateExpDate = model.LicensePlateExpDate >= new DateTime(1900, 1, 1) ? model.LicensePlateExpDate : updObj.LicensePlateExpDate;
                updObj.PRCo = updObj.EMCompanyParm.PRCo;
                //updObj.Shop = model.Shop;
                //updObj.LastUsedDate = model.LastUsedDate >= new DateTime(1900, 1, 1) ? model.LastUsedDate : updObj.LastUsedDate;
                //updObj.InspectionExpiration = model.InspectionExpiration >= new DateTime(1900, 1, 1) ? model.InspectionExpiration : updObj.InspectionExpiration;

                updObj.AssignmentType = (byte?)model.AssignmentType;
                updObj.ParentEquimentId = model.ParentEquimentId;
                updObj.AssignedCrewId = model.AssignedCrewId;
                updObj.OperatorId = model.Operator;

                if (addLogAssignment)
                {
                    var log = EquipmentLogRepository.Init(updObj);
                    log.LogTypeId = (int)DB.EMLogTypeEnum.Assignment;
                    updObj.Logs.Add(log);
                }
                //db.SaveChanges(modelState);
                return new EquipmentViewModel(updObj);
            }
            return model;
        }

        public static Equipment ProcessUpdate(Equipment model, VPContext db)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            if (db == null)
            {
                throw new ArgumentNullException(nameof(db));
            }
            var updObj = db.Equipments.Where(f => f.EMCo == model.EMCo && f.EquipmentId == model.EquipmentId).FirstOrDefault();
            if (updObj != null)
            {
                /****Write the changes to object****/
                updObj.Location = model.Location;
                updObj.Type = model.Type;
                updObj.DepartmentId = model.DepartmentId;
                updObj.CategoryId = model.CategoryId;
                updObj.Manufacturer = model.Manufacturer;
                updObj.Model = model.Model;
                updObj.ModelYr = model.ModelYr;
                updObj.VINNumber = model.VINNumber;
                updObj.Description = model.Description;
                updObj.Status = model.Status;
                updObj.OdoReading = model.OdoReading;
                if (model.OdoDate >= new DateTime(1900, 1, 1))
                {
                    updObj.OdoDate = model.OdoDate ?? updObj.OdoDate;
                }
                updObj.ReplacedOdoReading = model.ReplacedOdoReading;
                if (model.ReplacedOdoDate >= new DateTime(1900, 1, 1))
                {
                    updObj.ReplacedOdoDate = model.ReplacedOdoDate ?? updObj.ReplacedOdoDate;
                }
                updObj.HourReading = model.HourReading;
                if (model.HourDate >= new DateTime(1900, 1, 1))
                {
                    updObj.HourDate = model.HourDate ?? updObj.HourDate;
                }
                updObj.ReplacedHourReading = model.ReplacedHourReading;
                if (model.ReplacedHourDate >= new DateTime(1900, 1, 1))
                {
                    updObj.ReplacedHourDate = model.ReplacedHourDate ?? updObj.ReplacedHourDate;
                }
                updObj.MatlGroup = model.MatlGroup;
                updObj.FuelMatlCode = model.FuelMatlCode;
                updObj.FuelCapacity = model.FuelCapacity;
                updObj.FuelCapUM = model.FuelCapUM;
                updObj.FuelUsed = model.FuelUsed;
                updObj.EMGroupId = model.EMGroupId;
                updObj.FuelCostCode = model.FuelCostCode;
                updObj.FuelCostType = model.FuelCostType;
                if (model.LastFuelDate >= new DateTime(1900, 1, 1))
                {
                    updObj.LastFuelDate = model.LastFuelDate ?? updObj.LastFuelDate;
                }
                updObj.AttachToEquip = model.AttachToEquip;
                updObj.AttachPostRevenue = model.AttachPostRevenue;
                updObj.JCCo = model.JCCo;
                updObj.JobId = model.JobId;
                updObj.PhaseGroupId = model.PhaseGroupId;
                updObj.UsageCostType = model.UsageCostType;
                updObj.WeightUM = model.WeightUM;
                updObj.WeightCapacity = model.WeightCapacity;
                updObj.VolumeUM = model.VolumeUM;
                updObj.VolumeCapacity = model.VolumeCapacity;
                updObj.Capitalized = model.Capitalized;
                updObj.LicensePlateNo = model.LicensePlateNo;
                updObj.LicensePlateState = model.LicensePlateState;
                if (model.LicensePlateExpDate >= new DateTime(1900, 1, 1))
                {
                    updObj.LicensePlateExpDate = model.LicensePlateExpDate ?? updObj.LicensePlateExpDate;
                }
                updObj.IRPFleet = model.IRPFleet;
                updObj.CompOfEquip = model.CompOfEquip;
                updObj.ComponentTypeCode = model.ComponentTypeCode;
                updObj.CompUpdateHrs = model.CompUpdateHrs;
                updObj.CompUpdateMiles = model.CompUpdateMiles;
                updObj.CompUpdateFuel = model.CompUpdateFuel;
                updObj.PostCostToComp = model.PostCostToComp;
                updObj.PRCo = model.PRCo;
                updObj.Operator = model.Operator;
                updObj.Shop = model.Shop;
                updObj.GrossVehicleWeight = model.GrossVehicleWeight;
                updObj.TareWeight = model.TareWeight;
                updObj.Height = model.Height;
                updObj.Wheelbase = model.Wheelbase;
                updObj.NoAxles = model.NoAxles;
                updObj.Width = model.Width;
                updObj.OverallLength = model.OverallLength;
                updObj.HorsePower = model.HorsePower;
                updObj.TireSize = model.TireSize;
                updObj.OwnershipStatus = model.OwnershipStatus;
                if (model.InServiceDate >= new DateTime(1900, 1, 1))
                {
                    updObj.InServiceDate = model.InServiceDate ?? updObj.InServiceDate;
                }
                updObj.ExpLife = model.ExpLife;
                updObj.ReplCost = model.ReplCost;
                updObj.CurrentAppraisal = model.CurrentAppraisal;
                if (model.SoldDate >= new DateTime(1900, 1, 1))
                {
                    updObj.SoldDate = model.SoldDate ?? updObj.SoldDate;
                }
                updObj.SalePrice = model.SalePrice;
                updObj.PurchasedFrom = model.PurchasedFrom;
                updObj.PurchasePrice = model.PurchasePrice;
                if (model.PurchDate >= new DateTime(1900, 1, 1))
                {
                    updObj.PurchDate = model.PurchDate ?? updObj.PurchDate;
                }
                updObj.APCo = model.APCo;
                updObj.VendorGroup = model.VendorGroup;
                updObj.LeasedFrom = model.LeasedFrom;
                if (model.LeaseStartDate >= new DateTime(1900, 1, 1))
                {
                    updObj.LeaseStartDate = model.LeaseStartDate ?? updObj.LeaseStartDate;
                }
                if (model.LeaseEndDate >= new DateTime(1900, 1, 1))
                {
                    updObj.LeaseEndDate = model.LeaseEndDate ?? updObj.LeaseEndDate;
                }
                updObj.LeasePayment = model.LeasePayment;
                updObj.LeaseResidualValue = model.LeaseResidualValue;
                updObj.ARCo = model.ARCo;
                updObj.CustGroupId = model.CustGroupId;
                updObj.CustomerId = model.CustomerId;
                updObj.CustEquipNo = model.CustEquipNo;
                updObj.MSTruckType = model.MSTruckType;
                updObj.RevenueCode = model.RevenueCode;
                updObj.Notes = model.Notes;
                updObj.MechanicNotes = model.MechanicNotes;
                if (model.JobDate >= new DateTime(1900, 1, 1))
                {
                    updObj.JobDate = model.JobDate ?? updObj.JobDate;
                }
                updObj.FuelType = model.FuelType;
                updObj.UpdateYN = model.UpdateYN;
                updObj.ShopGroup = model.ShopGroup;
                updObj.IFTAState = model.IFTAState;
                if (model.LastUsedDate >= new DateTime(1900, 1, 1))
                {
                    updObj.LastUsedDate = model.LastUsedDate ?? updObj.LastUsedDate;
                }
                updObj.ChangeInProgress = model.ChangeInProgress;
                updObj.LastUsedEquipmentCode = model.LastUsedEquipmentCode;
                if (model.LastEquipmentChangeDate >= new DateTime(1900, 1, 1))
                {
                    updObj.LastEquipmentChangeDate = model.LastEquipmentChangeDate ?? updObj.LastEquipmentChangeDate;
                }
                updObj.LastEquipmentChangeUser = model.LastEquipmentChangeUser;
                updObj.EquipmentCodeChanges = model.EquipmentCodeChanges;
                updObj.OriginalEquipmentCode = model.OriginalEquipmentCode;
                updObj.ExpLifeTimeFrame = model.ExpLifeTimeFrame;
                updObj.RegisteredGVWR = model.RegisteredGVWR;
                updObj.OriginalPurchasePrice = model.OriginalPurchasePrice;
                if (model.OrigPurchaseDate >= new DateTime(1900, 1, 1))
                {
                    updObj.OrigPurchaseDate = model.OrigPurchaseDate ?? updObj.OrigPurchaseDate;
                }
                if (model.InspectionExpiration >= new DateTime(1900, 1, 1))
                {
                    updObj.InspectionExpiration = model.InspectionExpiration ?? updObj.InspectionExpiration;
                }
                updObj.CrossReferenceUsage = model.CrossReferenceUsage;
                updObj.ParentEquimentId = model.ParentEquimentId;
                updObj.Qty = model.Qty;

                //db.SaveChanges(modelState);
            }
            return updObj;
        }
        
    }
}