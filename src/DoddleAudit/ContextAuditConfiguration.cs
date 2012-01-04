using System;
using System.Collections.Generic;
using System.Reflection;

namespace DoddleAudit
{
    /// <summary>
    /// Customize the auditing process at the Context level
    /// </summary>
    public class ContextAuditConfiguration
    {
        public ContextAuditConfiguration()
        {
            PropertyAuditRules = new List<Func<PropertyInfo, object, bool>>();
            AuditingEnabled = true;
        }

        /// <summary>
        /// Set to false to disable automatic auditing
        /// </summary>
        public bool AuditingEnabled { get; set; }

        /// <summary>
        /// Only Audit properties which match the following rules
        /// </summary>
        public IList<Func<PropertyInfo, object, bool>> PropertyAuditRules { get; private set; }

        /// <summary>
        /// Specify how to audit properties which have a null or empty value
        /// </summary>
        public EmptyPropertyMode EmptyPropertyMode { get; set; }
        
    }
}