using System;
using System.Linq;
using System.ComponentModel;
using System.Reflection;

namespace Common.Helpers
{
    public static class EnumDescriptionUtility
    {
        public static string GetEnumDescription(this Enum value)
        {
            return
                    value
                        .GetType()
                        .GetMember(value.ToString())
                        .FirstOrDefault()
                        ?.GetCustomAttribute<DescriptionAttribute>()
                        ?.Description;
        }
    }
}
