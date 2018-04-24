using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace CastmExcel
{
    public class ExcelUtil
    {

        public ExcelUtil() { }

        public ExcelUtil(ExcelDomain excelDomain)
        {
            this.ExcelDomain = excelDomain;
        }

        

        public ExcelDomain ExcelDomain { get; set; }

        public void DownloadTemplate(bool isErrorDownload = false)
        {
            try
            {
                Excel.Application excel;
                Excel.Workbook excelworkBook;
                Excel.Worksheet excelSheet;
                Dictionary<StyleDomain, List<Excel.Range>> rangeDic = new Dictionary<StyleDomain, List<Excel.Range>>();
                List<Excel.Range> borderRanges = new List<Excel.Range>();

                if (File.Exists(this.ExcelDomain.FullPath))
                {
                    File.Delete(this.ExcelDomain.FullPath);
                }

                // create excle file to local path.
                excel = new Excel.Application();
                excel.Visible = false;
                excel.DisplayAlerts = false;
                excelworkBook = excel.Workbooks.Add(Type.Missing);
                excelSheet = (Excel.Worksheet)excelworkBook.ActiveSheet;
                excelSheet.Name = this.ExcelDomain.Sheet.Name ?? "sheet template";

                var titleGroups = this.ExcelDomain.TitleList.GroupBy(p => p.RowNumber);
                foreach (var titleGroup in titleGroups)
                {
                    var rowNumber = titleGroup.Key;
                    foreach (var titleInfo in titleGroup.ToList())
                    {
                        Excel.Range titleRange;
                        excelSheet.Cells[rowNumber, titleInfo.StartPoint] = titleInfo.Title;
                        excelSheet.Cells[rowNumber, titleInfo.StartPoint].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        if (titleInfo.IsMerge)
                        {
                            excelSheet.Range[excelSheet.Cells[rowNumber, titleInfo.StartPoint], excelSheet.Cells[rowNumber, titleInfo.EndPoint]].Merge(true);
                            titleRange = excelSheet.Range[excelSheet.Cells[rowNumber, titleInfo.StartPoint], excelSheet.Cells[rowNumber, titleInfo.EndPoint]];

                        }
                        else
                        {
                            titleRange = excelSheet.Range[excelSheet.Cells[rowNumber, titleInfo.StartPoint], excelSheet.Cells[rowNumber, titleInfo.StartPoint]];
                        }
                        if (!rangeDic.ContainsKey(titleInfo.StyleDomain))
                        {
                            rangeDic.Add(titleInfo.StyleDomain, new List<Excel.Range> { titleRange });
                        }
                        else
                        {
                            rangeDic[titleInfo.StyleDomain].Add(titleRange);
                        }
                        borderRanges.Add(titleRange);
                    }
                }

                foreach (var dataInfo in this.ExcelDomain.BodyDomain.DataList)
                {
                    foreach (var cellInfo in dataInfo.ColumnList)
                    {
                        excelSheet.Cells[dataInfo.RowNumber, cellInfo.ColumnNumber] = cellInfo.Value;
                    }
                }
                if (isErrorDownload)
                {
                    var lastRowNumber = 3;
                    var lastColumnNumber = 1;
                    if (this.ExcelDomain.BodyDomain.DataList.Any())
                    {
                        lastColumnNumber = this.ExcelDomain.BodyDomain.DataList.LastOrDefault().RowNumber;
                    }

                    var cannotReadRow = this.ExcelDomain.BodyDomain.CannotReadRowNumber;
                    var errorStr = string.Empty;
                    this.ExcelDomain.BodyDomain.CannotReadRowNumber.ForEach(p => {
                        errorStr += p + ";";
                    });
                    if (string.IsNullOrEmpty(errorStr))
                    {
                        excelSheet.Cells[lastRowNumber, lastColumnNumber] = errorStr;
                    }
                    this.ExcelDomain.FileName += "_Error";
                }

                var startRow = this.ExcelDomain.BodyDomain.DataList.FirstOrDefault().RowNumber;
                var startColumn = this.ExcelDomain.BodyDomain.DataList.FirstOrDefault().ColumnList.FirstOrDefault().ColumnNumber;
                var endRow = this.ExcelDomain.BodyDomain.DataList.Last().RowNumber;
                var endColumn = this.ExcelDomain.BodyDomain.DataList.Last().ColumnList.Last().ColumnNumber;
                Excel.Range bodyRange = excelSheet.Range[excelSheet.Cells[startRow, startColumn], excelSheet.Cells[endRow, endColumn]];
                rangeDic.Add(this.ExcelDomain.BodyDomain.StyleDomain, new List<Excel.Range> { bodyRange });

                // add style to excel
                foreach (var dic in rangeDic)
                {
                    var styleInfo = dic.Key;
                    foreach (var range in dic.Value)
                    {
                        this.FormattingExcelCells(range, styleInfo);
                    }
                }

                // add border to excel
                foreach (var borderRange in borderRanges)
                {
                    this.AddBorderToCells(borderRange);
                }

                // add width
                var defaultOne = rangeDic.FirstOrDefault(p => p.Key.Flag == true).Value.FirstOrDefault();
                defaultOne.EntireColumn.ColumnWidth = 40;

                // save excel
                excelworkBook.SaveAs(this.ExcelDomain.FullPath);
                excelworkBook.Close(true, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
                excel.Quit();
                Marshal.ReleaseComObject(excelSheet);
                Marshal.ReleaseComObject(excelworkBook);
                Marshal.ReleaseComObject(excel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void FormattingExcelCells(Excel.Range range, StyleDomain styleDomain)
        {
            range.Interior.Color = ColorTranslator.FromHtml(styleDomain.BackgroundColor);
            range.Font.Color = ColorTranslator.FromHtml(styleDomain.FontColor);
            range.Font.Size = styleDomain.FontSize;
            if (styleDomain.IsBold)
            {
                range.Font.Bold = true;
            }
        }

        private void AddBorderToCells(Excel.Range range)
        {
            Excel.Borders border = range.Borders;
            border.LineStyle = Excel.XlLineStyle.xlContinuous;
            border.Weight = 3d;
            border.Color = ColorTranslator.FromHtml("#000000");
        }


        public ExcelDomain ImportTemplate(string path)
        {
            var fileName = this.GetFileName(path);
            this.ExcelDomain = new ExcelDomain
            {
                LocalPath = path.Substring(0, path.LastIndexOf("\\")),
                FileName = fileName,
                Sheet = new SheetDomain
                {
                    Name = fileName
                },
                BodyDomain = new BodyDomain
                {
                    DataList = new List<RowDomain>(),
                    StyleDomain = GetStyleBody(),
                    CannotReadRowNumber = new List<int>()
                },
                TitleList = new List<TitleDomain>(),
                ErrorFlag = false
            };
            Excel.Application excelApplication = new Excel.Application();
            Excel.Workbook excelBook = excelApplication.Workbooks.Open(path, 0, true, 5, "", "", true, Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            Excel.Worksheet excelSheet = excelBook.Worksheets.get_Item(1);
            Excel.Range excelRange = excelSheet.UsedRange;
            //excelSheet.get_Range();

            // read title
            string title = excelRange.Cells[1, 1].Value2;
            this.ReadTitle(title);

            // read column name
            int rowCount = 0;
            int columnCount = 0;
            for (columnCount = 1; columnCount <= excelRange.Columns.Count; columnCount++)
            {
                string columnName = excelRange.Cells[2, columnCount].Value2;
                this.ReadColumnName(columnCount, columnName);
            }

            List<ErrorDomain> errorList = new List<ErrorDomain>();
            // read body
            for (rowCount = 3; rowCount <= excelRange.Rows.Count; rowCount++)
            {

                var rowDomain = this.ReadRow(rowCount);
                for (columnCount = 1; columnCount < excelRange.Columns.Count; columnCount++)
                {
                    try
                    {
                        var cellValue = excelRange.Cells[rowCount, columnCount].Value2;
                        this.ReadCell(columnCount, cellValue, rowDomain);
                    }
                    catch (Exception)
                    {
                        this.ExcelDomain.BodyDomain.CannotReadRowNumber.Add(rowCount);
                        continue;
                    }
                }
                this.ReadBody(rowDomain);
            }
            return this.ExcelDomain;
        }

        private void ReadTitle(string value)
        {
            this.ExcelDomain.TitleList.Add(new TitleDomain
            {
                StartPoint = 1,
                EndPoint = 4,
                RowNumber = 1,
                IsMerge = true,
                Title = value,
                StyleDomain = GetStyleTitle()
            });
        }

        private void ReadColumnName(int startPoint, string value)
        {
            this.ExcelDomain.TitleList.Add(new TitleDomain
            {
                StartPoint = startPoint,
                EndPoint = startPoint,
                RowNumber = 2,
                IsMerge = false,
                Title = value,
                StyleDomain = GetStyleColumnName()
            });
        }

        private void ReadBody(RowDomain rowDomain)
        {
            this.ExcelDomain.BodyDomain.DataList.Add(rowDomain);
        }

        private RowDomain ReadRow(int number)
        {
            var rowDomain = new RowDomain
            {
                ColumnList = new List<ColumnDomain>(),
                RowNumber = number
            };
            return rowDomain;
        }

        private RowDomain ReadCell(int number, string value, RowDomain rowDomain)
        {
            var result = new ColumnDomain()
            {
                ColumnNumber = number,
                Value = value
            };
            rowDomain.ColumnList.Add(result);
            return rowDomain;
        }

        private string GetFileName(string path)
        {
            var fileName = path.Substring(path.LastIndexOf("\\") + 1, path.LastIndexOf('.') -1 - path.LastIndexOf("\\"));
            return fileName;
        }

        public static StyleDomain GetStyleTitle()
        {
            var style1 = new StyleDomain
            {
                BackgroundColor = "#CD853F",
                FontColor = "#CCCC00",
                FontSize = 22,
                IsBold = true,
                Flag = true
            };
            return style1;
        }

        public static StyleDomain GetStyleColumnName()
        {
            var style2 = new StyleDomain
            {
                BackgroundColor = "#CD853F",
                FontColor = "#CCCC00",
                FontSize = 18,
                IsBold = true
            };
            return style2;
        }

        public static StyleDomain GetStyleBody()
        {
            var style3 = new StyleDomain
            {
                BackgroundColor = "#DDDDDD",
                FontColor = "#000000",
                FontSize = 14,
                IsBold = false
            };
            return style3;
        }
    }
}
