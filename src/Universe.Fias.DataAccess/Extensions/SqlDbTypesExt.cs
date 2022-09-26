using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.SqlServer.Server;

namespace Universe.Fias.DataAccess.Extensions
{
    /// <summary>
    /// Extension for SqlDbTypes.
    /// </summary>
    public static class SqlDbTypesExt
    {
        /// <summary>
        /// Gets the SQL meta data for table valued parameter.
        /// </summary>
        /// <typeparam name="TDbContextExt">The type of the database context ext.</typeparam>
        /// <param name="db">The database.</param>
        /// <param name="procName">Name of the proc.</param>
        /// <param name="paramName">Name of the parameter.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// procName
        /// or
        /// paramName
        /// </exception>
        public static List<SqlMetaData> GetSqlMetaDataForTableValuedParameter<TDbContextExt>(TDbContextExt db, string procName, string paramName)
            where TDbContextExt : SqlDbContextExt
        {
            if (procName == null)
                throw new ArgumentNullException(nameof(procName));

            if (paramName == null)
                throw new ArgumentNullException(nameof(paramName));

            var collumns = db.ExecuteSqlReader(
                    @"select p.name
    ,tt.name
    ,c.name as ColName
    ,type_name(c.user_type_id) as ColType
    --,p.*
    --,tt.*
    --,c.*
from sys.parameters p
    inner join sys.table_types tt on tt.user_type_id = p.user_type_id
    inner join sys.columns c on c.object_id = tt.type_table_object_id
where p.object_id = object_id(@procName) and p.name = @paramName",
                    setParameters: pColl => {
                        pColl.AddWithValue("@procName", procName);
                        pColl.AddWithValue("@paramName", paramName);
                    },
                    converter: r => new {
                        ColName = r.GetString(2),
                        ColType = r.GetString(3)
                    })
                .ToList();

            var md = new List<SqlMetaData>(collumns.Count);
            foreach (var c in collumns)
            {
                var cn = c.ColName;
                //if (string.Compare(c.ColName, "Level", StringComparison.OrdinalIgnoreCase) == 0)
                //{
                //    cn = $"[{c.ColName}]";
                //}

                var sqlDbType = (SqlDbType)Enum.Parse(typeof(SqlDbType), c.ColType, true);
                if (sqlDbType == SqlDbType.NVarChar)
                {
                    md.Add(new SqlMetaData(cn, sqlDbType, -1));
                    continue;
                }

                md.Add(new SqlMetaData(cn, sqlDbType));
            }

            return md;
        }

        /// <summary>
        /// The get type by sql db type.
        /// </summary>
        /// <param name="sqlDbType">
        /// The sql db type.
        /// </param>
        /// <returns>
        /// The <see cref="Type"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public static Type GetTypeBySqlDbType(SqlDbType sqlDbType)
        {
            switch (sqlDbType)
            {
                case SqlDbType.BigInt:
                    return typeof(long);
                case SqlDbType.Binary:
                    return typeof(byte[]);
                case SqlDbType.Bit:
                    return typeof(bool);
                case SqlDbType.Char:
                    return typeof(string);
                case SqlDbType.DateTime:
                    return typeof(DateTime);
                case SqlDbType.Decimal:
                    return typeof(decimal);
                case SqlDbType.Float:
                    return typeof(double);
                case SqlDbType.Image:
                    return typeof(byte[]);
                case SqlDbType.Int:
                    return typeof(int);
                case SqlDbType.Money:
                    return typeof(decimal);
                case SqlDbType.NChar:
                    return typeof(string);
                case SqlDbType.NText:
                    return typeof(string);
                case SqlDbType.NVarChar:
                    return typeof(string);
                case SqlDbType.Real:
                    return typeof(float);
                case SqlDbType.UniqueIdentifier:
                    return typeof(Guid);
                case SqlDbType.SmallDateTime:
                    return typeof(DateTime);
                case SqlDbType.SmallInt:
                    return typeof(short);
                case SqlDbType.SmallMoney:
                    return typeof(decimal);
                case SqlDbType.Text:
                    return typeof(string);
                case SqlDbType.Timestamp:
                    return typeof(byte[]);
                case SqlDbType.TinyInt:
                    return typeof(byte);
                case SqlDbType.VarBinary:
                    return typeof(byte[]);
                case SqlDbType.VarChar:
                    return typeof(string);
                case SqlDbType.Variant:
                    return typeof(object);
                case SqlDbType.Xml:
                    return typeof(string);

                /*case SqlDbType.Udt: return typeof();*/
                /*case SqlDbType.Structured: return typeof();*/
                case SqlDbType.Date:
                    return typeof(DateTime);
                case SqlDbType.Time:
                    return typeof(TimeSpan);
                case SqlDbType.DateTime2:
                    return typeof(DateTime);
                case SqlDbType.DateTimeOffset:
                    return typeof(DateTimeOffset);
                default:
                    throw new NotSupportedException(sqlDbType.ToString());
            }

            /*
QL Server Database Engine type	.NET Framework type	SqlDbType enumeration	SqlDataReader SqlTypes typed accessor	DbType enumeration	SqlDataReader DbType typed accessor
bigint	Int64	BigInt	GetSqlInt64	Int64	GetInt64
binary	Byte[]	VarBinary	GetSqlBinary	Binary	GetBytes
bit	Boolean	Bit	GetSqlBoolean	Boolean	GetBoolean
char	String	Char	GetSqlString	AnsiStringFixedLength ,	GetString
					
	Char[]			String	GetChars
date	DateTime	Date	GetSqlDateTime	Date	GetDateTime
					
(SQL Server 2008 and later)					
datetime	DateTime	DateTime	GetSqlDateTime	DateTime	GetDateTime
datetime2	DateTime	DateTime2	None	DateTime2	GetDateTime
					
(SQL Server 2008 and later)					
datetimeoffset	DateTimeOffset	DateTimeOffset	none	DateTimeOffset	GetDateTimeOffset
					
(SQL Server 2008 and later)					
decimal	Decimal	Decimal	GetSqlDecimal	Decimal	GetDecimal
FILESTREAM attribute (varbinary(max))	Byte[]	VarBinary	GetSqlBytes	Binary	GetBytes
float	Double	Float	GetSqlDouble	Double	GetDouble
image	Byte[]	Binary	GetSqlBinary	Binary	GetBytes
int	Int32	Int	GetSqlInt32	Int32	GetInt32
money	Decimal	Money	GetSqlMoney	Decimal	GetDecimal
nchar	String	NChar	GetSqlString	StringFixedLength	GetString
					
	Char[]				GetChars
ntext	String	NText	GetSqlString	String	GetString
					
	Char[]				GetChars
numeric	Decimal	Decimal	GetSqlDecimal	Decimal	GetDecimal
nvarchar	String	NVarChar	GetSqlString	String	GetString
					
	Char[]				GetChars
real	Single	Real	GetSqlSingle	Single	GetFloat
rowversion	Byte[]	Timestamp	GetSqlBinary	Binary	GetBytes
smalldatetime	DateTime	DateTime	GetSqlDateTime	DateTime	GetDateTime
smallint	Int16	SmallInt	GetSqlInt16	Int16	GetInt16
smallmoney	Decimal	SmallMoney	GetSqlMoney	Decimal	GetDecimal
sql_variant	Object *	Variant	GetSqlValue *	Object	GetValue *
text	String	Text	GetSqlString	String	GetString
					
	Char[]				GetChars
time	TimeSpan	Time	none	Time	GetDateTime
					
(SQL Server 2008 and later)					
timestamp	Byte[]	Timestamp	GetSqlBinary	Binary	GetBytes
tinyint	Byte	TinyInt	GetSqlByte	Byte	GetByte
uniqueidentifier	Guid	UniqueIdentifier	GetSqlGuid	Guid	GetGuid
varbinary	Byte[]	VarBinary	GetSqlBinary	Binary	GetBytes
varchar	String	VarChar	GetSqlString	AnsiString , String	GetString
					
	Char[]				GetChars
xml	Xml	Xml	GetSqlXml	Xml	none
*/
        }
    }
}