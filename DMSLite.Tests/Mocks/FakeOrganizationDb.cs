using DMSLite.DataContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMSLite.Tests.Mocks
{
    class FakeOrganizationDb : OrganizationDb
    {
        private int _tenantId;
        public int TenantId
        {
            get
            {
                return _tenantId;
            }
            set
            {
                _tenantId = value;
            }
        }
        public FakeOrganizationDb() : base()
        {
            _tenantId = 0;
        }

        public FakeOrganizationDb(int tenantId) : base()
        {
            this._tenantId = tenantId;
        }

        protected override int GetTenantId()
        {
            return _tenantId;
        }
    }
}
