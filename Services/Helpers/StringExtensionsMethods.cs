using System;
using System.Linq;

namespace Services.Helpers
{
    public static class StringExtensionsMethods
    {
        public static string GetUniqueFileName(this string inputFileName)
        {
            var newName = inputFileName;
            try
            {
                var fileNameParts = inputFileName.Split('.');
                var timeStampName = string.Join("_", fileNameParts[0], DateTime.UtcNow.Ticks.ToString());
                newName = fileNameParts.Count() == 2 ? string.Join(".", timeStampName, fileNameParts[1]) : timeStampName;
                return newName;
            }
            catch { }
            return newName;
        }
    }
}
