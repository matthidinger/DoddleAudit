using System.Reflection;

namespace Doddle.Linq.Audit
{
    public class MemberAudit
    {
        public MemberAudit(MemberInfo mi, object originalValue, object currentValue)
        {
            Member = mi;
            OriginalValue = originalValue;
            CurrentValue = currentValue;
        }

        public MemberInfo Member { get; set; }
        public object OriginalValue { get; set; }
        public object CurrentValue { get; set; }
    }
}