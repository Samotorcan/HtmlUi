using System;

namespace Samotorcan.HtmlUi.Core.Utilities
{
    internal static class ConvertUtility
    {
        public static object ChangeType(object value, Type conversionType)
        {
            if (conversionType.IsEnum)
                return Enum.ToObject(conversionType, value);

            return Convert.ChangeType(value, conversionType);
        }
    }
}
