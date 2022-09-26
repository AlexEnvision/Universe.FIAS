using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Universe.Algorithm.MultiThreading;
using Universe.CQRS.Dal.Queries;
using Universe.CQRS.Extensions;
using Universe.CQRS.Infrastructure;
using Universe.CQRS.Models.Base;
using Universe.Diagnostic;
using Universe.Diagnostic.Logger;
using Universe.Fias.Core.Infrastructure;
using Universe.Fias.DataAccess.Models;
using Universe.Fias.Normalizer.ConsoleApp.Senders;
using Universe.Fias.Normalizer.ConsoleApp.Tests.Base;
using Universe.Fias.Normalizer.ConsoleApp.Types;
using Universe.IO.CsvSerialization;
using Universe.Types.Collection;

namespace Universe.Fias.Normalizer.ConsoleApp.Tests
{
    /// <summary>
    ///     Выполняет поиск корректных адресов, которые взаимосвязаны через ParentAoId и выводит отчёт в файл.
    /// </summary>
    internal class FindCorrectAddresses : BaseTest
    {
        private UniverseFiasScope _scope;

        public FindCorrectAddresses(IUniverseLogger log) : base(log)
        {
            try
            {
                var container = UnityConfig.Container;
                var settings = new AppSettings();

                settings.ConnectionString =
                    "data source=localhost;initial catalog=FIAS-DB;integrated security=True;MultipleActiveResultSets=True;App=UniverseFiasApp;Connection Timeout=3600";

                var resolver = new AppPrincipalResolver();
                _scope = new UniverseFiasScope(resolver, settings, container);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
        }

        public void Run()
        {
            RunUnderKickAss();
        }

        private void RunUnderKickAss()
        {
            var sendKeys = true;
            var tm = ThreadMachine.Create
                (1).RunInMultiThreadsWithoutWaiting(() =>
            {
                while (sendKeys)
                {
                    var processName = Process.GetCurrentProcess().ProcessName;
                    WindowSender.SetForegroundWindowByName(processName);
                    KeyboardSender.keybd_event(Keys.Enter, 0x45, KeyboardSender.KEYEVENTF_EXTENDEDKEY | 0, IntPtr.Zero);
                    KeyboardSender.keybd_event(Keys.Enter, 0x45, KeyboardSender.KEYEVENTF_EXTENDEDKEY | KeyboardSender.KEYEVENTF_KEYUP, (IntPtr)0);
                    Thread.Sleep(30000);
                }
            });

            RunInternal();

            sendKeys = false;
            Thread.Sleep(1000);
            tm.CancelAllThreads(true);
            Thread.Sleep(1000);
            tm.CancelAllThreads(true);
        }

        private void RunInternal()
        {
            var correctAddresses = new MatList<CorrectAddress>();

            using (var watcher = new RunningTimeWatcher())
            {
                var housesResult = _scope.GetQuery<SelectEntitiesQuery<AsHouse, AsHouseDto>>().Execute(
                    EntityReqHelper.GetAnyReq(
                        MetaInfoHelper.FieldMap(
                            MetaInfoHelper.MapRule<AsHouseDto>(nameof(AsHouseDto.Id), x => x.Id)),
                        allItemsAsOnePage: true
                    ), house => new AsHouseDto
                    {
                        Id = house.Id,
                        HouseId = house.HouseId,
                        AoId = house.AoId,
                        HouseNum = house.HouseNum
                    });
                var housesCount = housesResult.Items.Count;

                Log.Info($"Частично выгружены записи домов в количестве: {housesCount}");

                var getHousesProcessingTime = watcher.TakeRunningTime();
                watcher.Reset();
                watcher.Continue();
                Log.Info($"Время обработки: {getHousesProcessingTime}");

                var addressObjctsResult = _scope.GetQuery<SelectEntitiesQuery<AsAddrObj, AsAddrObjDto>>().Execute(
                    EntityReqHelper.GetAnyReq(
                        MetaInfoHelper.FieldMap(
                            MetaInfoHelper.MapRule<AsAddrObjDto>(nameof(AsAddrObjDto.Id), x => x.Id)),
                        allItemsAsOnePage: true
                    ), obj => new AsAddrObjDto
                    {
                        Id = obj.Id,
                        AoId = obj.AoId,
                        ParentAoId = obj.ParentAoId,
                        Name = obj.Name,
                        PlainCode = obj.PlainCode,
                        TypeShortName = obj.TypeShortName
                    });
                var addressesCount = addressObjctsResult.Items.Count;

                Log.Info($"Частично выгружены записи адресных объектов в количестве: {addressesCount}");
                var getAddresesProcessingTime = watcher.TakeRunningTime();
                watcher.Reset();
                watcher.Continue();
                Log.Info($"Время обработки: {getAddresesProcessingTime}");

                var indexedAddresses = addressObjctsResult.Items.ToDictionary(x => x.AoId);

                foreach (var asHouse in housesResult.Items)
                {
                    if (!indexedAddresses.TryGetValue(asHouse.AoId, out var asStreet))
                        continue;

                    if (!asStreet.ParentAoId.HasValue)
                        continue;

                    if (!indexedAddresses.TryGetValue(asStreet.ParentAoId.Value, out var asSettlement))
                        continue;

                    if (!asSettlement.ParentAoId.HasValue)
                        continue;

                    if (!indexedAddresses.TryGetValue(asSettlement.ParentAoId.Value, out var asRegionDistrict))
                        continue;

                    var correctAddress = new CorrectAddress
                    {
                        House = asHouse,
                        Street = asStreet,
                        Settlement = asSettlement,
                        RegionDistrict = asRegionDistrict
                    };
                    correctAddresses += correctAddress;
                }

                var buildAddresesProcessingTime = watcher.TakeRunningTime();
                Log.Info($"Сформированы корректные адреса. Время обработки: {buildAddresesProcessingTime}");

                //var housesPageIndex = 0;
                //RequestedPage<AsHouseDto> result;

                //var countPage = 2000;

                //    do
                //    {
                //        housesPageIndex++;

                //        if (housesPageIndex > 128 && correctAddresses.Count > 0)
                //            break;

                //        Log.Info($"Производится проверка {housesPageIndex}-й {countPage} домов...");

                //        result = _scope.GetQuery<SelectEntitiesQuery<AsHouse, AsHouseDto>>().Execute(
                //            EntityReqHelper.GetAnyReq(
                //                MetaInfoHelper.FieldMap(
                //                    MetaInfoHelper.MapRule<AsHouseDto>(nameof(AsHouseDto.Id), x => x.Id)),
                //                countOnPage: countPage,
                //                pageIndex: housesPageIndex
                //            ),
                //            house =>
                //                new AsHouseDto
                //                {
                //                    Id = house.Id,
                //                    HouseId = house.HouseId,
                //                    AoId = house.AoId,
                //                    HouseNum = house.HouseNum
                //                });

                //        var houses = result.Items;
                //        var parentStreetsAoIds = houses.Select(x => x.AoId.ToString()).ToList();

                //        var streets = _scope.GetQuery<SelectEntitiesQuery<AsAddrObj, AsAddrObjDto>>().Execute(
                //            EntityReqHelper.GetInReq(
                //                MetaInfoHelper.FieldMap(
                //                    MetaInfoHelper.MapRule<AsAddrObjDto>(nameof(AsAddrObjDto.Id), x => x.Id),
                //                    MetaInfoHelper.MapRule<AsAddrObjDto>(nameof(AsAddrObjDto.AoId), x => x.AoId)),
                //                countOnPage: countPage,
                //                pageIndex: 1,
                //                searchfieldName: nameof(AsAddrObjDto.AoId),
                //                searchvalues: parentStreetsAoIds
                //            ),
                //            obj => new AsAddrObjDto
                //            {
                //                Id = obj.Id,
                //                AoId = obj.AoId,
                //                ParentAoId = obj.ParentAoId,
                //                Name = obj.Name,
                //                PlainCode = obj.PlainCode,
                //                TypeShortName = obj.TypeShortName
                //            });

                //        if (streets.Items.Count == 0)
                //            continue;

                //        var parentSettlementsAoIds = streets.Items
                //            .Where(x => x.ParentAoId != null)
                //            .Select(x => x.ParentAoId.ToString()).ToList();

                //        var settlements = _scope.GetQuery<SelectEntitiesQuery<AsAddrObj, AsAddrObjDto>>().Execute(
                //            EntityReqHelper.GetInReq(
                //                MetaInfoHelper.FieldMap(
                //                    MetaInfoHelper.MapRule<AsAddrObjDto>(nameof(AsAddrObjDto.Id), x => x.Id),
                //                    MetaInfoHelper.MapRule<AsAddrObjDto>(nameof(AsAddrObjDto.AoId), x => x.AoId)),
                //                countOnPage: countPage,
                //                pageIndex: 1,
                //                searchfieldName: nameof(AsAddrObjDto.AoId),
                //                searchvalues: parentSettlementsAoIds
                //            ),
                //            obj => new AsAddrObjDto
                //            {
                //                Id = obj.Id,
                //                AoId = obj.AoId,
                //                ParentAoId = obj.ParentAoId,
                //                Name = obj.Name,
                //                PlainCode = obj.PlainCode,
                //                TypeShortName = obj.TypeShortName
                //            });

                //        if (settlements.Items.Count == 0)
                //            continue;

                //        var parentDistrictsOrRegions = settlements.Items
                //            .Where(x => x.ParentAoId != null)
                //            .Select(x => x.ParentAoId.ToString()).ToList();

                //        var districtsOrRegions = _scope.GetQuery<SelectEntitiesQuery<AsAddrObj, AsAddrObjDto>>().Execute(
                //            EntityReqHelper.GetInReq(
                //                MetaInfoHelper.FieldMap(
                //                    MetaInfoHelper.MapRule<AsAddrObjDto>(nameof(AsAddrObjDto.Id), x => x.Id),
                //                    MetaInfoHelper.MapRule<AsAddrObjDto>(nameof(AsAddrObjDto.AoId), x => x.AoId)),
                //                countOnPage: countPage,
                //                pageIndex: 1,
                //                searchfieldName: nameof(AsAddrObjDto.AoId),
                //                searchvalues: parentDistrictsOrRegions
                //            ),
                //            obj => new AsAddrObjDto
                //            {
                //                Id = obj.Id,
                //                AoId = obj.AoId,
                //                ParentAoId = obj.ParentAoId,
                //                Name = obj.Name,
                //                PlainCode = obj.PlainCode,
                //                TypeShortName = obj.TypeShortName
                //            });

                //        if (districtsOrRegions.Items.Count == 0)
                //            continue;

                //        var streetsIndexed = streets.Items.ToDictionary(x => x.AoId);
                //        var settlementsIndexed = settlements.Items.ToDictionary(x => x.AoId);
                //        var regionDistrictIndexed = districtsOrRegions.Items.ToDictionary(x => x.AoId);

                //        foreach (var asHouse in houses)
                //        {
                //            if (!streetsIndexed.TryGetValue(asHouse.AoId, out var asStreet))
                //                continue;

                //            if (!asStreet.ParentAoId.HasValue)
                //                continue;

                //            if (!settlementsIndexed.TryGetValue(asStreet.ParentAoId.Value, out var asSettlement))
                //                continue;

                //            if (!asSettlement.ParentAoId.HasValue)
                //                continue;

                //            if (!regionDistrictIndexed.TryGetValue(asSettlement.ParentAoId.Value, out var asRegionDistrict))
                //                continue;

                //            var correctAddress = new CorrectAddress
                //            {
                //                House = asHouse,
                //                Street = asStreet,
                //                Settlement = asSettlement,
                //                RegionDistrict = asRegionDistrict
                //            };
                //            correctAddresses += correctAddress;
                //        }
                //    } while (result.NextPageHavingItems);

                var records = correctAddresses.Select(x => new CorrectAddressNames
                {
                    RegionDistrict = (x.RegionDistrict?.Name + ' ' + x.RegionDistrict?.TypeShortName).Trim(),
                    Settlement = x.Settlement.Name + ' ' + x.Settlement.TypeShortName,
                    Street = x.Street.Name + ' ' + x.Street.TypeShortName,
                    House = "д. " + x.House.HouseNum,
                }).ToList();

                Log.Info($"Найдено {records.Count} корретных записей адресов.");

                var directory = Directory.GetCurrentDirectory();
                var filename = "CorrectAddresses.csv";
                filename = Path.Combine(directory, filename);

                using (var fs = File.Create(filename))
                {
                    var serializer = new CsvSerializer<CorrectAddressNames>
                    {
                        Separator = ',',
                        UseTextQualifier = false
                    };
                    serializer.Serialize(fs, records);
                }

                Log.Info($"Файл с записями сохранён по адресу: {filename}");
                Log.Info($"Поиск завершён за {getAddresesProcessingTime + getHousesProcessingTime + watcher.TakeRunningTime()}.");
            }
        }

        private class CorrectAddress
        {
            public AsHouseDto House { get; set; }

            public AsAddrObjDto Street { get; set; }

            public AsAddrObjDto Settlement { get; set; }

            public AsAddrObjDto RegionDistrict { get; set; }
        }

        public class CorrectAddressNames
        {
            public string RegionDistrict { get; set; }

            public string Settlement { get; set; }

            public string Street { get; set; }

            public string House { get; set; }
        }

        public class AsHouseDto : EntityDto
        {
            public Guid HouseId { get; set; }

            public Guid AoId { get; set; }

            public string HouseNum { get; set; }
        }

        public class AsAddrObjDto : EntityDto
        {
            public Guid AoId { get; set; }

            public Guid? ParentAoId { get; set; }

            public string TypeShortName { get; set; }

            public string Name { get; set; }

            public string Code { get; set; }

            public string PlainCode { get; set; }
        }
    }
}
