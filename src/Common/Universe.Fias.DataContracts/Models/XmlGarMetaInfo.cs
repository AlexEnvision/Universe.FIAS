namespace Universe.Fias.DataContracts.Models
{
    public class XmlGarMetaInfo
    {
        public class ACTSTAT : AsRecord
        {
            public string ACTSTATID { get; set; }

            public string NAME { get; set; }
        }

        public class ESTSTAT : AsRecord
        {
            public string ESTSTATID { get; set; }

            public string NAME { get; set; }

            public string SHORTNAME { get; set; }
        }

        public class CENTERST : AsRecord
        {
            public string CENTERSTID { get; set; }

            public string NAME { get; set; }
        }

        public class OPERSTAT : AsRecord
        {
            public string OPERSTATID { get; set; }

            public string NAME { get; set; }
        }

        public class SOCRBASE : AsRecord
        {
            public string LEVEL { get; set; }

            public string SOCRNAME { get; set; }

            public string SCNAME { get; set; }

            public string KOD_T_ST { get; set; }
        }

        public class STRSTAT : AsRecord
        {
            public string STRSTATID { get; set; }

            public string NAME { get; set; }

            public string SHORTNAME { get; set; }
        }

        public class ADDROB : AsRecord
        {
            public string AOID { get; set; }

            public string PREVID { get; set; }

            public string NEXTID { get; set; }

            public string AOGUID { get; set; }

            public string PARENTGUID { get; set; }

            public string DIVTYPE { get; set; }

            public string AOLEVEL { get; set; }

            public string SHORTNAME { get; set; }

            public string FORMALNAME { get; set; }

            public string OFFNAME { get; set; }

            public string CENTSTATUS { get; set; }

            public string CODE { get; set; }

            public string PLAINCODE { get; set; }

            public string POSTALCODE { get; set; }

            public string OKATO { get; set; }

            public string OKTMO { get; set; }

            public string LIVESTATUS { get; set; }

            public string ACTSTATUS { get; set; }

            public string OPERSTATUS { get; set; }

            public string UPDATEDATE { get; set; }

            public string STARTDATE { get; set; }

            public string ENDDATE { get; set; }
        }

        public class HOUSE : AsRecord
        {
            public string HOUSEID { get; set; }

            public string HOUSEGUID { get; set; }

            public string AOGUID { get; set; }

            public string DIVTYPE { get; set; }

            public string HOUSENUM { get; set; }

            public string ESTSTATUS { get; set; }

            public string BUILDNUM { get; set; }

            public string STRUCNUM { get; set; }

            public string STRSTATUS { get; set; }

            public string POSTALCODE { get; set; }

            public string OKATO { get; set; }

            public string OKTMO { get; set; }

            public string UPDATEDATE { get; set; }

            public string STARTDATE { get; set; }

            public string ENDDATE { get; set; }
        }

        public class DHOUSE : AsRecord
        {
            public string HOUSEID { get; set; }

            public string HOUSEGUID { get; set; }

            public string AOGUID { get; set; }

            public string DIVTYPE { get; set; }

            public string HOUSENUM { get; set; }

            public string ESTSTATUS { get; set; }

            public string BUILDNUM { get; set; }

            public string STRUCNUM { get; set; }

            public string STRSTATUS { get; set; }

            public string POSTALCODE { get; set; }

            public string OKATO { get; set; }

            public string OKTMO { get; set; }

            public string UPDATEDATE { get; set; }

            public string STARTDATE { get; set; }

            public string ENDDATE { get; set; }
        }

        public class DADDROB : AsRecord
        {
            public string AOID { get; set; }

            public string PREVID { get; set; }

            public string NEXTID { get; set; }

            public string AOGUID { get; set; }

            public string PARENTGUID { get; set; }

            public string DIVTYPE { get; set; }

            public string AOLEVEL { get; set; }

            public string SHORTNAME { get; set; }

            public string FORMALNAME { get; set; }

            public string OFFNAME { get; set; }

            public string CENTSTATUS { get; set; }

            public string CODE { get; set; }

            public string PLAINCODE { get; set; }

            public string POSTALCODE { get; set; }

            public string OKATO { get; set; }

            public string OKTMO { get; set; }

            public string LIVESTATUS { get; set; }

            public string ACTSTATUS { get; set; }

            public string OPERSTATUS { get; set; }

            public string UPDATEDATE { get; set; }

            public string STARTDATE { get; set; }

            public string ENDDATE { get; set; }
        }
    }
}