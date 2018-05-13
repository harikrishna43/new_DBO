using DBO.Data.Utilities;

using System.Collections.Generic;
using System.ComponentModel;

namespace DBO.Data.ValidationAttributes
{
    public class LocalizedDisplayNameAttribute : DisplayNameAttribute
    {
        private readonly string _displayName;

        public override string DisplayName
        {
            get
            {
                return ResourceString.Get(_displayName) ?? string.Empty;
            }
        }

        public LocalizedDisplayNameAttribute(string displayName)
        {
            _displayName = displayName;
        }
    }
}