using DMSLite.DataContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMSLite.Tests.Mocks
{
    // A fake organization db context to allow us to test without
    // the MS OWIN security framework and to easily switch between
    // contexts in order to test them
    class FakeOrganizationDb : OrganizationDb
    {
        private static int _tenantId;

        public FakeOrganizationDb() : base()
        {
            _tenantId = 0;
        }

        public void SetTenantId(int id)
        {
            _tenantId = id;
        }

        public override int GetTenantId()
        {
            return _tenantId;
        }
    }
}
