using System.Collections.Generic;
using Core.Enums.Orders;
using Services.Interfaces;
using Services.Interfaces.ImportExportData;
using Services.Model.AdminSection.ExportImport;

namespace Services.Implementation.ImportExportData
{
    public class ImportExportService : IImportExportService
    {
        private readonly IUserService _userService;
        public ImportExportService(IUserService userService)
        {
            _userService = userService;
        }

        public List<UserExportModel> GetUsers(string search)
        {
            return _userService.GetUsers(search, UserOrderBy.Name);
        }
    }
}
