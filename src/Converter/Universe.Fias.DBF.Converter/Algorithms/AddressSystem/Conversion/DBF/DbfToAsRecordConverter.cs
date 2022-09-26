using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using Universe.Fias.DataContracts.Models;
using Universe.Helpers.Extensions;
using Universe.IO.CsvSerialization;

namespace Universe.Fias.DBF.Converter.x64.Algorithms.AddressSystem.Conversion.DBF
{
    public class DbfToAsRecordConverter
    {
        private string _dbfFolderPath;

        private readonly string _csvFolderPath;

        private const int BatchSize = 5000;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbfToAsRecordConverter"/> class.
        /// </summary>
        /// <param name="dbfFolderPath">The DBF folder path.</param>
        /// <param name="csvFolderPath"></param>
        /// <exception cref="System.ArgumentNullException">
        /// dbfFolderPath
        /// or
        /// db
        /// </exception>
        public DbfToAsRecordConverter(string dbfFolderPath, string csvFolderPath)
        {
            if (dbfFolderPath.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(dbfFolderPath));
            if (csvFolderPath.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(csvFolderPath));

            _dbfFolderPath = dbfFolderPath;
            _csvFolderPath = csvFolderPath;
        }

        public IList<T> Convert<T>(string dbfFileNamePattern)
            where T : AsRecord, new()
        {
            if (dbfFileNamePattern.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(dbfFileNamePattern));

            var dbfConnString = $@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={_dbfFolderPath.TrimEnd('\\')};Extended Properties='DBASE IV';OLE DB Services=0;";

            string dbfToConvert = dbfFileNamePattern;
            var csvfileName = dbfToConvert.Replace(".dbf", ".csv", StringComparison.InvariantCultureIgnoreCase);
            var csvfileFullname = Path.Combine(_csvFolderPath, csvfileName);
            ConvertDbf(dbfConnString, dbfToConvert, csvfileFullname);

            IList<T> records = new List<T>();

            // Отключаем десериализацию полученного файла в целях экономии памяти
            //using (var fs = File.Open(csvfileFullname, FileMode.Open))
            //{
            //    var serializer = new CsvSerializer<T>
            //    {
            //        Separator = '|',
            //        UseTextQualifier = true,
            //        IgnoreReferenceTypesExceptString = true
            //    };

            //    records = serializer.Deserialize(fs);
            //}

            return records;
        }

        private void DataTableToCsv(DataTable dt, string csvFile)
        {
            StringBuilder sb = new StringBuilder();
            var columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToArray();
            sb.AppendLine(string.Join("|", columnNames));
            foreach (DataRow row in dt.Rows)
            {
                var fields = row.ItemArray.Select(field => field.ToString()).ToArray();
                for (int i = 0; i < fields.Length; i++)
                {
                    var val = fields[i].Trim();
                    //var decode = Decode(val);

                    sb.Append("\"" + val);
                    sb.Append((i != fields.Length - 1) ? "\"|" : "\"");
                }

                sb.Append("\r\n");
            }

            if (File.Exists(csvFile))
            {
                File.AppendAllText(csvFile, sb.ToString());
            }
            else
            {
                File.WriteAllText(csvFile, sb.ToString());
            }
        }

        private string Decode(string input)
        {
            //Encoding fromEncoding = Encoding.GetEncoding(866);
            //Encoding toEncoding = Encoding.UTF8;

            Encoding fromEncoding = Encoding.GetEncoding("cp850");
            Encoding toEncoding = Encoding.GetEncoding(866);

            //Encoding fromEncoding = Encoding.GetEncoding(1252);
            //Encoding toEncoding = Encoding.UTF8;

            string returnValue = toEncoding.GetString(fromEncoding.GetBytes(input));

            return returnValue;
        }

        private int GetCount(DataTable dt)
        {
            var recordsCount = 0;
            foreach (DataRow row in dt.Rows)
            {
                var fields = row.ItemArray.Select(field => field.ToString()).ToArray();
                var field = fields.FirstOrDefault();

                if (field != null)
                    if (int.TryParse(field, out recordsCount))
                        return recordsCount;
            }

            return recordsCount;
        }

        private string GetFirstColumn(DataTable dt)
        {
            var columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToArray();
            return columnNames.FirstOrDefault();
        }

        private void ConvertDbf(string connectionString, string dbfFile, string csvFile)
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                //int recordsCount;

                //string sqlCount = string.Format("SELECT COUNT(*) FROM {0}", dbfFile);
                //using (OleDbDataAdapter da = new OleDbDataAdapter(sqlCount, connection))
                //{
                //    DataSet ds = new DataSet();
                //    da.Fill(ds);
                //    recordsCount = GetCount(ds.Tables[0]);
                //}

                //string firstColumn;

                //string sqlTopRow = string.Format("SELECT TOP 1 * FROM {0}", dbfFile);
                //using (OleDbDataAdapter da = new OleDbDataAdapter(sqlTopRow, connection))
                //{
                //    DataSet ds = new DataSet();
                //    da.Fill(ds);
                //    firstColumn = GetFirstColumn(ds.Tables[0]);
                //}

                //for (int i = 0; i < recordsCount; i+= BatchSize)
                //{
                //    var rowsNext = recordsCount >= BatchSize ? BatchSize : recordsCount;
                //    var offset = i * BatchSize;

                string sqlSelect = $"SELECT * FROM {dbfFile}"; // ORDER BY {firstColumn}"; // OFFSET {offset} ROWS FETCH NEXT {rowsNext} ROWS ONLY";
                using (OleDbDataAdapter da = new OleDbDataAdapter(sqlSelect, connection))
                {
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    DataTableToCsv(ds.Tables[0], csvFile);
                }
                //}
            }
        }
    }
}