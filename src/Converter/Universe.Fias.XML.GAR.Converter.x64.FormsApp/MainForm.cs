using System;
using System.Windows.Forms;
using Universe.Algorithm.MultiThreading;
using Universe.Diagnostic.Logger;
using Universe.Fias.Core.Extensions;
using Universe.Fias.Core.Infrastructure.DatabaseContexts;
using Universe.Fias.DataContracts;
using Universe.Fias.XML.GAR.Converter.x64.Algorithms.AddressSystem;
using Universe.Fias.XML.GAR.Converter.x64.FormsApp.Infastructure;
using Universe.Helpers.Extensions;
using Universe.Windows.Forms.Controls;
using Universe.Windows.Forms.Controls.Settings;

namespace Universe.Fias.XML.GAR.Converter.x64.FormsApp
{
    public partial class MainForm : Form
    {
        private EventLogger _log;

        private FormsAppSettings _programSettings;

        protected ThreadMachine ThreadMachine;

        public MainForm()
        {
            InitializeComponent();

            _log = new EventLogger();

            _log.LogInfo += e => {
                if (e.AllowReport)
                {
                    var currentDate = DateTime.Now;
                    var message = $"[{currentDate}] {e.Message}{Environment.NewLine}";
                    this.SafeCall(() => this.tbLog.AppendText(message));
                }
            };

            _log.LogError += e => {
                if (e.AllowReport)
                {
                    var currentDate = DateTime.Now;
                    var message = $"[{currentDate}] Во время выполнения операции произошла ошибка. Текст ошибки: {e.Message}.{Environment.NewLine} Трассировка стека: {e.Ex.StackTrace}{Environment.NewLine}";
                    this.SafeCall(() => this.tbLog.AppendText(message));
                }
            };
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _programSettings = _programSettings.Load() ?? new FormsAppSettings();
            LoadOnForm();
        }

        public void LoadOnForm()
        {
            tbAddrSysImportBaseFolderPath.Text = _programSettings.Default.AddrSysImportBaseFolderPath;
            tbSevenZipFolderPath.Text = _programSettings.Default.SevenZipFolderPath;
            tbConvertedCsvFilesPath.Text = _programSettings.Default.AddrSysConvertedCsvFilesPath;

            tbConnectionString.Text = _programSettings.Default.ConnectionString;
            cbAddrSysFiasDownloadServiceEnable.Checked =
                _programSettings.Default.AddrSysImportFiasDownloadServiceEnable;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnLoadFromForm();
            _programSettings.Save();
        }

        public void UnLoadFromForm()
        {
            _programSettings.Default.AddrSysImportBaseFolderPath = tbAddrSysImportBaseFolderPath.Text;
            _programSettings.Default.SevenZipFolderPath = tbSevenZipFolderPath.Text;
            _programSettings.Default.AddrSysConvertedCsvFilesPath = tbConvertedCsvFilesPath.Text;

            _programSettings.Default.ConnectionString = tbConnectionString.Text;
            _programSettings.Default.AddrSysImportFiasDownloadServiceEnable =
                cbAddrSysFiasDownloadServiceEnable.Checked;
        }

        private void btSync_Click(object sender, EventArgs e)
        {
            btSync.Enabled = false;
            btCancel.Enabled = true;

            var addrSysImportFiasDownloadServiceEnable = cbAddrSysFiasDownloadServiceEnable.Checked;

            var addrSysImportBaseFolderPath = tbAddrSysImportBaseFolderPath.Text;
            _programSettings.Default.AddrSysConvertedCsvFilesPath = tbConvertedCsvFilesPath.Text;

            var connectionString = tbConnectionString.Text;
            if (connectionString.IsNullOrEmpty())
            {
                _log.Info("Не указана строка подключения к базе данных. Укажите строку подключения и попробуйте запустить миграцию базы данных снова.");
                return;
            }

            _programSettings.Default.ConnectionString = connectionString;
            var dbType = connectionString.DeterminateDbTypeFromConnectionString();

            ThreadMachine = ThreadMachine.Create(1).RunInMultiThreadsWithoutWaiting(() =>
            {
                try
                {
                    using (var sync = new AddressSystemGarConverter(
                        _log,
                        _programSettings.Default,
                        addrSysImportBaseFolderPath,
                        dbType,
                        new AddrSysDownloaderSettings
                        {
                            Enable = addrSysImportFiasDownloadServiceEnable,
                            FiasDownloadServiceUrl = _programSettings.Default.AddrSysImportFiasDownloadServiceUrl
                        }))
                    {
                        sync.Run();
                    }
                }
                catch (Exception ex)
                {
                    _log.Error(ex, ex.Message);
                }
                finally
                {
                    btSync.SafeCall(() => btSync.Enabled = true);
                    btCancel.SafeCall(() => btCancel.Enabled = false);
                }
            });
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            ThreadMachine?.CancelAllThreads(true);
            btCancel.SafeCall(() => btCancel.Enabled = false);
        }

        private void btAddrSysImportBaseFolderPath_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    tbAddrSysImportBaseFolderPath.Text = dialog.SelectedPath;
                }
            }
        }

        private void btSevenZipFolderPath_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    tbSevenZipFolderPath.Text = dialog.SelectedPath;
                }
            }
        }

        private void btConvertedCsvFilesPath_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    tbConvertedCsvFilesPath.Text = dialog.SelectedPath;
                }
            }
        }

        private void btMigrateDb_Click(object sender, EventArgs e)
        {
            btMigrateDb.Enabled = false;
            btSync.Enabled = false;

            var connectionString = tbConnectionString.Text;
            if (connectionString.IsNullOrEmpty())
            {
                _log.Info("Не указана строка подключения к базе данных. Укажите строку подключения и попробуйте запустить миграцию базы данных снова.");
                return;
            }

            _programSettings.Default.ConnectionString = connectionString;
            var dbType = connectionString.DeterminateDbTypeFromConnectionString();

            ThreadMachine.Create(1).RunInMultiThreadsWithoutWaiting(() => {
                try
                {
                    var commandTimeout = 60 * 60;

                    _log.Info("Запуск миграции модельной БД...");
                    using (var dbContext = UniverseFiasDbContextFactory.Initialize.Create(connectionString, dbType))
                    {
                        dbContext.Migrate(commandTimeout);
                    }
                    _log.Info("Успешно завершена миграция модельной БД!");

                    _log.Info("Запуск \"контрольной\" миграции модельной БД...");
                    using (var dbContext = UniverseFiasDbContextFactory.Initialize.Create(connectionString, dbType))
                    {
                        dbContext.Migrate(commandTimeout);
                    }
                    _log.Info("Успешно завершена \"контрольная\" миграция модельной БД!");
                }
                catch (Exception ex)
                {
                    _log.Error(ex, ex.Message);
                }
                finally
                {
                    this.SafeCall(() => btMigrateDb.Enabled = true);
                    this.SafeCall(() => btSync.Enabled = true);
                }
            });
        }
    }
}
