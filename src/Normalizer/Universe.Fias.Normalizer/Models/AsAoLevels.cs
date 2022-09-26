namespace Universe.Fias.Normalizer.Models
{
    /// <summary>
    /// Address system levels.
    /// </summary>
    public enum AsAoLevels
    {
        /// <summary>
        /// 1 – уровень региона
        /// </summary>
        Region = 1,

        /// <summary>
        /// 2 – уровень автономного округа (устаревшее)
        /// </summary>
        Level2 = 1,

        /// <summary>
        /// 3 – уровень района
        /// </summary>
        District = 3,

        /// <summary>
        /// 35 – уровень городских и сельских поселений
        /// </summary>
        Level35 = 35,

        /// <summary>
        /// 4 – уровень города
        /// </summary>
        City = 4,

        /// <summary>
        /// 5 – уровень внутригородской территории (устаревшее)
        /// </summary>
        Level5 = 5,

        /// <summary>
        /// 6 – уровень населенного пункта
        /// </summary>
        Location = 6,

        /// <summary>
        /// 65 – планировочная структура
        /// </summary>
        Level65 = 65,

        /// <summary>
        /// 7 – уровень улицы
        /// </summary>
        Street = 7,

        /// <summary>
        /// 75 – земельный участок
        /// </summary>
        Level75 = 75,

        /// <summary>
        /// 8 – здания, сооружения, объекта незавершенного строительства
        /// </summary>
        Level8 = 8,

        /// <summary>
        /// 9 – уровень помещения в пределах здания, сооружения
        /// </summary>
        Level9 = 9,

        /// <summary>
        /// 90 – уровень дополнительных территорий (устаревшее)
        /// </summary>
        Level90 = 90,

        /// <summary>
        /// 91 – уровень объектов на дополнительных территориях (устаревшее)
        /// </summary>
        Level91 = 91
    }
}