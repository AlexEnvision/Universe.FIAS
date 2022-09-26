using System;

namespace Universe.Fias.DataAccess.Models.Base
{
    /// <summary>
    ///     Интерфейс сущности в БД взаимодействующей с файловыми потоками.
    /// <author>Alex Envision</author>
    /// </summary>
    public interface IUEntity
    {
        Guid Id { get; set; }
    }
}