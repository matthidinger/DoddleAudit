﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Doddle.Linq.Audit
{
    public interface IAuditAssociation
    {
        Type ParentEntityType { get; }
        Type EntityType { get; }
        LambdaExpression PkSelector { get; set; }
        LambdaExpression FkSelector { get; set; }
    }
}
