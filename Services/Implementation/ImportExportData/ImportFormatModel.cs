using Services.Interfaces.ImportExportData;
using System.Collections.Generic;

namespace Services.Implementation.ImportExportData
{
    public class ImportFormatModel<T> : IImportFormatModel
    {
        public IList<T> Values { get; set; }
        public bool IsOverride { get; set; }

        public ImportFormatModel()
        {
           Values = new List<T>();
        }
        
    }
}
