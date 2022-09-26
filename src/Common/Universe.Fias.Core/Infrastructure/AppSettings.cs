using Universe.CQRS.Infrastructure;
using Universe.Helpers.Extensions;

namespace Universe.Fias.Core.Infrastructure
{
    public class AppSettings: IAppSettings
    {
        private int _addrSysImportManagerPortionRecCount;
        private string _sevenZipFolderPath;
        private int _addrSysManagerParallelTaskCount;

        private string _addrSysImportFiasDownloadServiceUrl;
        private string _addrSysImportBaseFolderPath;
        private bool _addrSysImportFiasDownloadServiceEnable;

        private string _connectionString;
        private string _addrSysConvertedCsvFilesPath;

        private bool _allowedHousesImport;
        private bool _deleteFromTables;
        public string ConnectionString
        {
            get
            {
                if (_connectionString.IsNullOrEmpty())
                    return @"data source=localhost;initial catalog=UniverseFiasDb;User ID=sa;Password=************;MultipleActiveResultSets=True;App=UniverseFiasApp;Connection Timeout=300";
            
                return _connectionString;
            }
            set => _connectionString = value;
        }

        public string GetUniverseDbConnectionString()
        {
            var connectionString = ConnectionString;
            return connectionString;
        }

        public bool AddrSysImportFiasDownloadServiceEnable
        {
            get => _addrSysImportFiasDownloadServiceEnable;
            set => _addrSysImportFiasDownloadServiceEnable = value;
        }

        public string AddrSysImportBaseFolderPath
        {
            get
            {
                if (_addrSysImportBaseFolderPath.IsNullOrEmpty())
                    return @"C:\APPS\FIAS";

                return _addrSysImportBaseFolderPath;
            }
            set => _addrSysImportBaseFolderPath = value;
        }

        public string SevenZipFolderPath
        {
            get
            {
                if (_sevenZipFolderPath.IsNullOrEmpty())
                    return @"C:\Program Files\7-Zip";

                return _sevenZipFolderPath;
            }
            set => _sevenZipFolderPath = value;
        }

        public string AddrSysConvertedCsvFilesPath
        {
            get
            {
                if (_addrSysConvertedCsvFilesPath.IsNullOrEmpty())
                    return @"C:\APPS\FIAS\FIAS_CSV";

                return _addrSysConvertedCsvFilesPath;
            }
            set => _addrSysConvertedCsvFilesPath = value;
        }

        public int AddrSysImportManagerPortionRecCount
        {
            get
            {
                if (_addrSysImportManagerPortionRecCount == 0)
                    return 1000;

                return _addrSysImportManagerPortionRecCount;
            }
            set => _addrSysImportManagerPortionRecCount = value;
        }

        public int AddrSysManagerParallelTaskCount
        {
            get
            {
                if (_addrSysManagerParallelTaskCount == 0)
                    return 3;

                return _addrSysManagerParallelTaskCount;
            }
            set => _addrSysManagerParallelTaskCount = value;
        }

        public string AddrSysImportFiasDownloadServiceUrl
        {
            get
            {
                if (_addrSysImportFiasDownloadServiceUrl.IsNullOrEmpty())
                    return @"https://fias.nalog.ru/WebServices/Public/DownloadService.asmx";

                return _addrSysImportFiasDownloadServiceUrl;
            }
            set => _addrSysImportFiasDownloadServiceUrl = value;
        }

        public bool AllowedHousesImport
        {
            get => _allowedHousesImport;
            set => _allowedHousesImport = value;
        }

        public bool DeleteFromTables
        {
            get => _deleteFromTables;
            set => _deleteFromTables = value;
        }
    }
}