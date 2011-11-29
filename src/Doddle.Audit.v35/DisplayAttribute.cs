using System;

namespace Doddle.Audit
{
    /// <summary>
    /// DO NOT USE! Dummy DisplayAttribute to compile under .NET 3.5
    /// </summary>
    [Obsolete]
    public class DisplayAttribute
    {
        public string GetShortName()
        {
            return string.Empty;
        }

        public string GetName()
        {
            return string.Empty;
        }
    }
}