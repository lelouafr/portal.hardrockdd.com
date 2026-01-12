using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelDataReader;

namespace DB.Infrastructure.ViewPointDB.Data
{
    public partial class WPExcelImport
    {

        public VPContext _db;

        public VPContext db
        {
            set
            {
                _db = value;
            }
            get
            {
                _db ??= VPContext.GetDbContextFromEntity(this);
                return _db;
            }
        }
        public static string BaseTableName { get { return "budWPEI"; } }

        public WPExcelSheet AddSheet(string sheetName)
        {
            var sheet = Sheets.FirstOrDefault(f => f.SheetName == sheetName);
            if (sheet == null)
            {
                sheet = new WPExcelSheet()
                {
                    db = db,
                    Import = this,
                    SheetId = this.Sheets.DefaultIfEmpty().Max(max => max == null ? 0 : max.SheetId) + 1,
                    ImportId = this.ImportId,
                    SheetName = sheetName,

                };
                Sheets.Add(sheet);
            }

            return sheet;

        }

        public static void ImportExcelFile(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            LoadExcelFile(fileInfo);
        }

        private static void LoadExcelFile(FileInfo fileInfo)
        {
            using var db = new VPContext();
            var import = db.WPExcelImports.FirstOrDefault(f => f.FilePath == fileInfo.FullName);
            if (import == null)
                import = db.AddExcelImport(fileInfo);
            var headerRowIndex = 0;
            if (!import.Sheets.Any() || import.LastModifiedDate != fileInfo.LastWriteTime)
            {
                if (import.ImportTypeId != 1)
                    import.ImportTypeId = 1;
                if (import.LastModifiedDate != fileInfo.LastWriteTime)
                    import.LastModifiedDate = fileInfo.LastWriteTime;

                FileStream stream = System.IO.File.Open(fileInfo.FullName, FileMode.Open, FileAccess.Read);
                IExcelDataReader excelReader;
                //try
                //{
                excelReader = ExcelReaderFactory.CreateReader(stream);

                DataSet result = excelReader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                    {
                        EmptyColumnNamePrefix = "Column ",
                        UseHeaderRow = true,
                        ReadHeaderRow = (reader) =>
                        {
                            bool empty = true;
                            headerRowIndex = 0;

                            while (empty)
                            {
                                var fldCnt = reader.FieldCount >= 50 ? 50 : reader.FieldCount;

                                for (var i = 0; i < fldCnt && empty; i++)
                                {
                                    var val = reader[i]?.ToString();
                                    empty = string.IsNullOrWhiteSpace(val);
                                }

                                if (empty)
                                {
                                    empty = reader.Read(); // Only continue if more content is available
                                    headerRowIndex++; // Keep track of the first row position.
                                }
                            }
                        },
                        FilterColumn = (reader, index) =>
                        {
                            bool empty = false;
                            string sheet = reader.Name;
                            if (index >= 50)
                                return false;

                            // Start reading the table from the beginning
                            reader.Reset();

                            // EDIT: Head over the our current excel sheet
                            while (reader.Name != sheet)
                                if (!reader.NextResult())
                                    break;


                            // Head to the first row with content
                            int rowIndex = 0;
                            while (rowIndex < headerRowIndex)
                            {
                                reader.Read();
                                rowIndex++;
                            }

                            var sampleRow = 0;
                            while (reader.Read())
                            {
                                sampleRow++;
                                if (sampleRow >= 50)
                                {
                                    break;
                                }
                                // Decide if the current column is empty
                                if (reader[index] == null || string.IsNullOrEmpty(reader[index].ToString()))
                                    continue;

                                empty = true;
                                break;
                            }

                            // Start over again (This allows the reader to automatically read the rest of the content itself)
                            reader.Reset();

                            // EDIT: Head over the our current excel sheet
                            while (reader.Name != sheet)
                                if (!reader.NextResult())
                                    break;

                            reader.Read();

                            // Head over to the first row with content
                            rowIndex = 0;
                            while (rowIndex < headerRowIndex)
                            {
                                reader.Read();
                                rowIndex++;
                            }

                            // Return info on whether this column should be ignored or not.
                            return empty;
                        }
                    }
                });

                var collection = result.Tables;
                foreach (DataTable table in result.Tables)
                {
                    var sheet = import.AddSheet(table.TableName);
                    Console.WriteLine(string.Format("{0}, sheet {1}, rows: {2},", import.FileName, sheet.SheetName, table.Columns.Count));
                    var json = JsonConvert.SerializeObject(table);

                    var data = Encoding.UTF8.GetBytes(json);

                    sheet.JsonData = data;
                }
                excelReader.Dispose();

                //}
                //catch (Exception)
                //{

                //}
                stream.Dispose();
            }

            db.BulkSaveChanges();

        }
    }


    public static class WPExcelImportExt
    {
        public static WPExcelImport AddExcelImport(this VPContext db, FileInfo fileInfo)
        {
            var import = db.WPExcelImports.FirstOrDefault(f => f.FilePath == fileInfo.FullName);
            if (import== null)
                import = db.WPExcelImports.FirstOrDefault(f => f.FileName == fileInfo.Name);
            if (import == null)
            {
                import = new WPExcelImport()
                {
                    //Locate = this,
                    db = db,
                    ImportId = db.GetNextId(WPExcelImport.BaseTableName),
                    FilePath = fileInfo.FullName,
                    FileName = fileInfo.Name,
                    LastModifiedDate = fileInfo.LastWriteTime,
                    CreatedDate = fileInfo.CreationTime,
                    Processed = false,
                };
                db.WPExcelImports.Add(import);
            }

            return import;
        }
        public static WPExcelImport AddExcelImport(this VPContext db, FileInfo fileInfo, int nextId)
        {
            var import = new WPExcelImport()
            {
                //Locate = this,
                db = db,
                ImportId = nextId,
                FilePath = fileInfo.FullName,
                FileName = fileInfo.Name,
                LastModifiedDate = fileInfo.LastWriteTime,
                CreatedDate = fileInfo.CreationTime,
                Processed = false,
            };
            db.WPExcelImports.Add(import);

            return import;
        }

        public static void LoadExcelLocateFile(FileInfo fileInfo)
        {
            using var db = new VPContext();
            var import = db.AddExcelImport(fileInfo);
            //var import = db.WPExcelImports.FirstOrDefault(f => f.FilePath == fileInfo.FullName);
            //if (import == null)
            var headerRowIndex = 0;
            if (!import.Sheets.Any() || import.LastModifiedDate != fileInfo.LastWriteTime)
            {
                if (import.ImportTypeId != 1)
                    import.ImportTypeId = 1;
                if (import.LastModifiedDate != fileInfo.LastWriteTime)
                    import.LastModifiedDate = fileInfo.LastWriteTime;

                FileStream stream = System.IO.File.Open(fileInfo.FullName, FileMode.Open, FileAccess.Read);
                IExcelDataReader excelReader = ExcelReaderFactory.CreateReader(stream);

                DataSet result = excelReader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                    {

                        EmptyColumnNamePrefix = "Column ",
                        UseHeaderRow = true,
                        ReadHeaderRow = (reader) =>
                        {
                            bool empty = true;
                            headerRowIndex = 0;

                            while (empty)
                            {
                                var fldCnt = reader.FieldCount >= 50 ? 50 : reader.FieldCount;

                                for (var i = 0; i < fldCnt && empty; i++)
                                {
                                    var val = reader[i]?.ToString();
                                    empty = string.IsNullOrWhiteSpace(val);
                                }

                                if (empty)
                                {
                                    empty = reader.Read(); // Only continue if more content is available
                                    headerRowIndex++; // Keep track of the first row position.
                                }
                            }
                        },
                        FilterRow = (reader) =>
                        {
                            bool empty = true;
                            var fldCnt = reader.FieldCount >= 50 ? 50 : reader.FieldCount;

                            for (var i = 0; i < fldCnt && empty; i++)
                            {
                                var val = reader[i]?.ToString();
                                empty = string.IsNullOrWhiteSpace(val);
                            }

                            return !empty;
                        },
                        FilterColumn = (reader, index) =>
                        {
                            bool empty = false;
                            string sheet = reader.Name;
                            if (index >= 50)
                                return false;

                            // Start reading the table from the beginning
                            reader.Reset();

                            // EDIT: Head over the our current excel sheet
                            while (reader.Name != sheet)
                                if (!reader.NextResult())
                                    break;


                            // Head to the first row with content
                            int rowIndex = 0;
                            while (rowIndex < headerRowIndex)
                            {
                                reader.Read();
                                rowIndex++;
                            }

                            var sampleRow = 0;
                            while (reader.Read())
                            {
                                sampleRow++;
                                if (sampleRow >= 50)
                                {
                                    break;
                                }
                                // Decide if the current column is empty
                                if (reader[index] == null || string.IsNullOrEmpty(reader[index].ToString()))
                                    continue;

                                empty = true;
                                break;
                            }

                            // Start over again (This allows the reader to automatically read the rest of the content itself)
                            reader.Reset();

                            // EDIT: Head over the our current excel sheet
                            while (reader.Name != sheet)
                                if (!reader.NextResult())
                                    break;

                            reader.Read();

                            // Head over to the first row with content
                            rowIndex = 0;
                            while (rowIndex < headerRowIndex)
                            {
                                reader.Read();
                                rowIndex++;
                            }

                            // Return info on whether this column should be ignored or not.
                            return empty;
                        }
                    }
                });

                var collection = result.Tables;
                foreach (DataTable table in result.Tables)
                {
                    var colIdx = 1;
                    foreach (DataColumn col in table.Columns)
                    {
                        var name = col.ColumnName.Trim();

                        if (name.ToLower() == "texas 811" ||
                            name.ToLower() == "column 0" ||
                            name.ToLower() == "cbud #" ||
                            (name.ToLower() == "" && colIdx == 1))
                            name = "RefId";
                        else if (name.ToLower() == "" && colIdx == 11)
                            name = "Project Name";
                        else if (name.ToLower() == "expire date")
                            name = "Expiration";
                        else if (name.ToLower() == "job number" ||
                                 name.ToLower() == "job#")
                            name = "JobId";
                        else if (name == "")
                            name = string.Format("Column {0}", colIdx);

                        col.ColumnName = name;
                        colIdx++;
                    }
                    var sheet = import.AddSheet(table.TableName);
                    Console.WriteLine(string.Format("{0}, sheet {1}, rows: {2},", import.FileName, sheet.SheetName, table.Rows.Count));
                    var json = JsonConvert.SerializeObject(table);

                    var data = Encoding.UTF8.GetBytes(json);

                    sheet.JsonData = data;
                }
                excelReader.Dispose();
                stream.Dispose();
            }
            db.BulkSaveChanges();


        }
    }
}
