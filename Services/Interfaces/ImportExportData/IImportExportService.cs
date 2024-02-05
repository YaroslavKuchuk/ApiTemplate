using Services.Model.AdminSection.ExportImport;
using System.Collections.Generic;

namespace Services.Interfaces.ImportExportData
{
    public interface IImportExportService
    {
        List<UserExportModel> GetUsers(string search);
    }
}
