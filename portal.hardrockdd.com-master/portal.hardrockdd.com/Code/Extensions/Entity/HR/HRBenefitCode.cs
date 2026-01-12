using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace portal.Code.Data.VP
{
    public partial class HRBenefitCode
    {
        private VPEntities _db;

        public VPEntities db
        {
            set
            {
                _db = value;
            }
            get
            {
                if (_db == null)
                {
                    _db = this.HRCompany.db;

                    if (_db == null)
                        _db = VPEntities.GetDbContextFromEntity(this);

                    //if (_db == null)
                    //    throw new NullReferenceException("GetDbContextFromEntity is null");

                }
                return _db;
            }
        }

    }
}