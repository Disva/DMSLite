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
        protected override int GetTenantId()
        {
            return 0;
        }
    }
}
