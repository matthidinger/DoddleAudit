﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Doddle.Linq.Audit
{
    public interface IAuditValueResolver
    {
        AuditValue GetAuditValue(MemberInfo member, object oldValue, object newValue);
    }
}
