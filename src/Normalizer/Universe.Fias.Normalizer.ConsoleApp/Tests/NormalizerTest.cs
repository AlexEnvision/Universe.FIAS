using System;
using Newtonsoft.Json;
using Universe.Diagnostic;
using Universe.Diagnostic.Logger;
using Universe.Fias.Core.Infrastructure;
using Universe.Fias.Normalizer.Algorithms.AddressSystem;
using Universe.Fias.Normalizer.Algorithms.AddressSystem.Settings;
using Universe.Fias.Normalizer.ConsoleApp.Tests.Base;

namespace Universe.Fias.Normalizer.ConsoleApp.Tests
{
    internal class NormalizerTest : BaseTest
    {
        private FiasFieldValueResolver _addressResolver;

        public NormalizerTest(IUniverseLogger log) : base(log)
        {
            try
            {
                var container = UnityConfig.Container;
                var settings = new AppSettings();

                settings.ConnectionString =
                    "data source=localhost;initial catalog=FIAS-DB;integrated security=True;MultipleActiveResultSets=True;App=UniverseFiasApp;Connection Timeout=3600";

                var resolver = new AppPrincipalResolver();
                var scope = new UniverseFiasScope(resolver, settings, container);

                var addressResolver = new FiasFieldValueResolver(scope, Log);
                _addressResolver = addressResolver;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
        }

        public void Run()
        {
            CompareAddresses();
            ResolveCity();
            ResolveLocality();
            ResolveAddress();
        }

        private void CompareAddresses()
        {
            var addressPairs = new[]
            {
                new {
                    AddressSfyFirst = "103274, Москва, ул. Орджоникидзе, д. 13",
                    AddressSfySecond = "г. Москва, Орджоникидзе ул., дом 13",
                },
                new {
                    AddressSfyFirst = "ул. Скаковая, д. 17, Москва, стр. 2, этаж 6, Москва, Россия, 125040",
                    AddressSfySecond = "Россия, 125040, Москва, ул. Скаковая, д. 17, стр. 2, этаж 6",
                },
                new {
                    AddressSfyFirst = "124489, Москва г., Зеленоград, Панфиловский п-кт, д. 10, стр. 3",
                    AddressSfySecond = " Москва, г Зеленоград, просп Панфиловский, д 10, корп 3",
                },
                new {
                    AddressSfyFirst = "Москва, ИЦ «Сколково», Большой бульвар, д. 40 сектор «С»",
                    AddressSfySecond = "Москва, ИЦ «Сколково», Большой бульвар, д. 40 сектор «С», 4 этаж",
                },
                new {
                    AddressSfyFirst = "Москва, проезд Электролитный, д. 1",
                    AddressSfySecond = "115230, г. Москва\r\nЭлектролитный пр., д. 1, стр.1",
                },
                new {
                    AddressSfyFirst = "Севастополь г,ТСН СНТ Наука тер. ТСН,Фиолентовское ш,д. 37/1-199",
                    AddressSfySecond = "Севастополь г, ТСН СНТ Наука тер. ТСН, Фиолентовское ш, д. 37/1-199",
                }
            };

            try
            {
                foreach (var addressPair in addressPairs)
                {
                    var addressSfyFirst = addressPair.AddressSfyFirst;
                    var addressSfySecond = addressPair.AddressSfySecond;

                    using (var watcher = new RunningTimeWatcher())
                    {
                        var settings = NormalizeAddressSettings.HouseNumberMatches;

                        var codesFirst = _addressResolver.NormalizeAddress(addressSfyFirst, settings);
                        var codesFirstSfy = JsonConvert.SerializeObject(codesFirst, Formatting.Indented);
                        Log.Info($"Построены следующие коды у первого адреса: {codesFirstSfy}");

                        var codesSecond = _addressResolver.NormalizeAddress(addressSfySecond, settings);
                        var codesSecondSfy = JsonConvert.SerializeObject(codesSecond, Formatting.Indented);
                        Log.Info($"Построены следующие коды у второго адреса: {codesSecondSfy}");

                        var areEqual = _addressResolver.CompareAddressCodesWithNullCheck(codesFirst, codesSecond);
                        Log.Info(areEqual
                            ? $"Адрес '{addressSfyFirst}' эквивалентен адресу '{addressSfySecond}'."
                            : $"Адрес '{addressSfyFirst}' не эквивалентен адресу '{addressSfySecond}'.");
                        Log.Info($"Сравнение выполнено за: {watcher.TakeRunningTime()}");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
        }

        private void ResolveCity()
        {
            var city = "ООО «ПРОМ-ТЭК» Россия, г.Санкт-Петербург, 26-ая линия В. О., д. 15";

            try
            {
                using (var watcher = new RunningTimeWatcher())
                {
                    var result = _addressResolver.ResolveSettlement(city);
                    var cityCodesSfy = JsonConvert.SerializeObject(result.Codes, Formatting.Indented);
                    Log.Info($"Построены следующие коды у города: {cityCodesSfy}");

                    var fullAddress = result.Result?.GetFullAddress();
                    Log.Info($"Город: {fullAddress}");

                    Log.Info($"Перестроение выполнено за: {watcher.TakeRunningTime()}");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
        }

        private void ResolveLocality()
        {
            var locality = "Сармановский район, п. Джалиль";

            try
            {
                using (var watcher = new RunningTimeWatcher())
                {
                    var result = _addressResolver.ResolveSettlement(locality);
                    var localityCodesSfy = JsonConvert.SerializeObject(result.Codes, Formatting.Indented);
                    Log.Info($"Построены следующие коды у населённого пункта: {localityCodesSfy}");

                    var fullAddress = result.Result?.GetFullAddress();
                    Log.Info($"Населённый пункт: {fullAddress}");

                    Log.Info($"Перестроение выполнено за: {watcher.TakeRunningTime()}");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
        }

        private void ResolveAddress()
        {
            var address = "124489, Москва г., Зеленоград, Панфиловский п-кт, д. 10, стр. 3";

            try
            {
                using (var watcher = new RunningTimeWatcher())
                {
                    var result = _addressResolver.ResolveAddress(address, NormalizeAddressSettings.HouseNumberMatches);
                    var addressCodesSfy = JsonConvert.SerializeObject(result.Codes, Formatting.Indented);
                    Log.Info($"Построены следующие коды у адреса: {addressCodesSfy}");

                    var fullAddress = result.Result?.GetFullAddress();
                    Log.Info($"Полный адрес: {fullAddress}");

                    Log.Info($"Перестроение выполнено за: {watcher.TakeRunningTime()}");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
        }
    }
}