using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Newtonsoft.Json;
using Universe.Algorithm.MultiThreading;
using Universe.CQRS.Models.Enums;
using Universe.Diagnostic;
using Universe.Diagnostic.Logger;
using Universe.Fias.Core.Extensions;
using Universe.Fias.Core.Infrastructure;
using Universe.Fias.Core.Infrastructure.DatabaseContexts;
using Universe.Fias.Normalizer.Algorithms.AddressSystem;
using Universe.Fias.Normalizer.Algorithms.AddressSystem.Settings;
using Universe.Fias.Normalizer.FormsApp.Infrastructure;
using Universe.Helpers.Extensions;
using Universe.Windows.Forms.Controls;
using Universe.Windows.Forms.Controls.Settings;

namespace Universe.Fias.Normalizer.FormsApp
{
    public partial class MainForm : Form
    {
        private EventLogger _log;

        private FormsAppSettings _programSettings;

        protected ThreadMachine ThreadMachine;

        /// <summary>
        ///     Названия городов с правильным написанием
        /// </summary>
        protected Dictionary<string, string> CorrectSpellCitiesNames = new Dictionary<string, string> {
            { "Орел","Орёл"}
        };

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

            _log.LogWarning += e => {
                if (e.AllowReport)
                {
                    var currentDate = DateTime.Now;
                    var message = $"[{currentDate}] {e.Message}{Environment.NewLine}";
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
            tbAddrOne.Text = _programSettings.AddressOne;
            tbAddrTwo.Text = _programSettings.AddressTwo;

            tbConnectionString.Text = _programSettings.Default.ConnectionString;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnLoadFromForm();
            _programSettings.Save();
        }

        public void UnLoadFromForm()
        {
            _programSettings.AddressOne = tbAddrOne.Text;
            _programSettings.AddressTwo = tbAddrTwo.Text;

            _programSettings.Default.ConnectionString = tbConnectionString.Text;
        }

        /// <summary>
        ///     Коррекция правильного написания названия города
        /// </summary>
        /// <param name="addressSfy"></param>
        /// <returns></returns>
        private string CorrectRightSpellCityName(string addressSfy)
        {
            foreach (var correctSpellCitiesName in CorrectSpellCitiesNames)
            {
                addressSfy = addressSfy.Replace(correctSpellCitiesName.Key, correctSpellCitiesName.Value);
            }

            return addressSfy;
        }

        private void btMigrateDb_Click(object sender, EventArgs e)
        {
            btMigrateDb.Enabled = false;
            btSync.Enabled = false;

            var connectionString = tbConnectionString.Text;
            if (connectionString.IsNullOrEmpty())
            {
                _log.Info("Не указана строка подключения!");
                return;
            }

            _programSettings.Default.ConnectionString = connectionString;
            var dbType = connectionString.DeterminateDbTypeFromConnectionString();

            ThreadMachine.Create(1).RunInMultiThreadsWithoutWaiting(() => {
                try
                {
                    _log.Info("Запуск миграции модельной БД...");
                    using (var dbContext = UniverseFiasDbContextFactory.Initialize.Create(connectionString, dbType))
                    {
                        dbContext.Migrate();
                    }
                    _log.Info("Успешно завершена миграция модельной БД!");

                    _log.Info("Запуск \"контрольной\" миграции модельной БД...");
                    using (var dbContext = UniverseFiasDbContextFactory.Initialize.Create(connectionString, dbType))
                    {
                        dbContext.Migrate();
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

        private void btSync_Click(object sender, EventArgs e)
        {
            var addressSfyFirst = tbAddrOne.Text;
            var addressSfySecond = tbAddrTwo.Text;

            btSync.Enabled = false;
            btCancel.Enabled = true;

            var connectionString = tbConnectionString.Text;
            if (connectionString.IsNullOrEmpty())
            {
                _log.Info("Не указана строка подключения!");
                return;
            }

            _programSettings.Default.ConnectionString = connectionString;
            var dbType = connectionString.DeterminateDbTypeFromConnectionString();

            ThreadMachine = ThreadMachine.Create(1).RunInMultiThreadsWithoutWaiting(() =>
            {
                try
                {
                    _log.Info("Запущена процедура сравнения двух адресов...");

                    var container = UnityConfig.Container;
                    var settings = new AppSettings();

                    settings.ConnectionString =
                        connectionString;

                    var resolver = new AppPrincipalResolver();
                    var scope = new UniverseFiasScope(resolver, settings, container, dbType);

                    using (var watcher = new RunningTimeWatcher())
                    {
                        var addressResolver = new FiasFieldValueResolver(scope, _log);
                        var normalizeSettings = NormalizeAddressSettings.IgnoreActualAndHouses;

                        addressSfyFirst = CorrectRightSpellCityName(addressSfyFirst);
                        addressSfySecond = CorrectRightSpellCityName(addressSfySecond);

                        var areEqual = CompareAddresses(addressResolver, addressSfyFirst, normalizeSettings, addressSfySecond);
                        if (!areEqual)
                        {
                            _log.Info($"Первое сравнение завершено. Время выполнения: {watcher.TakeRunningTime()}.");
                            _log.Info($"Производится дополнительное сравнение т.к требуется дополнительная проверка...");

                            normalizeSettings = NormalizeAddressSettings.StreetNamesMatchesButIgnoreHouses;
                            normalizeSettings.ActualIgnore = true;

                            areEqual = CompareAddresses(addressResolver, addressSfyFirst, normalizeSettings, addressSfySecond);
                        }

                        _log.Info(areEqual
                            ? $"Адрес '{addressSfyFirst}' эквивалентен адресу '{addressSfyFirst}'."
                            : $"Адрес '{addressSfyFirst}' не эквивалентен адресу '{addressSfySecond}'.");

                        watcher.FreezeTime();
                        _log.Info($"Сравнение завершено. Время выполнения: {watcher.TakeRunningTime()}.");
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

        private bool CompareAddresses(FiasFieldValueResolver addressResolver, string addressSfyFirst,
            NormalizeAddressSettings normalizeSettings, string addressSfySecond)
        {
            var codesFirst = addressResolver.NormalizeAddress(addressSfyFirst, normalizeSettings);
            var codesFirstSfy = JsonConvert.SerializeObject(codesFirst, Formatting.Indented);
            _log.Info($"Построены следующие коды у первого адреса: {codesFirstSfy}");

            var codesSecond = addressResolver.NormalizeAddress(addressSfySecond, normalizeSettings);
            var codesSecondSfy = JsonConvert.SerializeObject(codesSecond, Formatting.Indented);
            _log.Info($"Построены следующие коды у второго адреса: {codesSecondSfy}");

            var areEqual = addressResolver.CompareAddressCodesWithNullCheck(codesFirst, codesSecond);

            // Сбрасываем состояние сравнения, если код городов пуст
            // или если код улицы пуст, чтобы ложных срабатываний не было
            if (codesFirst.CityCode.IsNullOrEmpty() && codesSecond.CityCode.IsNullOrEmpty() ||
                codesFirst.StreetCode.IsNullOrEmpty() && codesSecond.StreetCode.IsNullOrEmpty())
            {
                areEqual = false;
            }

            return areEqual;
        }
    }
}
