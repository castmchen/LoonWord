using CastmExcel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordTool
{
    public class TemplateInfo
    {
        public static ExcelDomain CreateTemplateForExport(string localPath, string fileName)
        {
            var style1 = ExcelUtil.GetStyleTitle();
            var style2 = ExcelUtil.GetStyleColumnName();
            var style3 = ExcelUtil.GetStyleBody();
            var domain = new ExcelDomain
            {
                LocalPath = localPath,
                FileName = fileName,
                Sheet = new SheetDomain
                {
                    Name = fileName,
                },
                TitleList = new List<TitleDomain>
                {
                    new TitleDomain
                    {
                        Title = fileName,
                        IsMerge = true,
                        StartPoint = 1,
                        EndPoint = 4,
                        RowNumber = 1,
                        StyleDomain = style1
                    },
                    new TitleDomain
                    {
                        Title = "Word",
                        RowNumber = 2,
                        IsMerge = false,
                        StartPoint = 1,
                        EndPoint = 1,
                        StyleDomain = style2
                    },
                    new TitleDomain
                    {
                        Title = "Translation",
                        RowNumber = 2,
                        IsMerge = false,
                        StartPoint = 2,
                        EndPoint = 2,
                        StyleDomain = style2
                    },
                    new TitleDomain
                    {
                        Title = "Phonetic ",
                        RowNumber = 2,
                        IsMerge = false,
                        StartPoint = 3,
                        EndPoint = 3,
                        StyleDomain = style2
                    },
                    new TitleDomain
                    {
                        Title = "Voice ",
                        RowNumber = 2,
                        IsMerge = false,
                        StartPoint = 4,
                        EndPoint = 4,
                        StyleDomain = style2
                    }
                },
                BodyDomain = new BodyDomain
                {
                    StyleDomain = style3,
                    DataList = new List<RowDomain>
                    {
                        new RowDomain
                        {
                            RowNumber=3,
                            ColumnList = new List<ColumnDomain>
                            {
                                new ColumnDomain
                                {
                                    ColumnNumber = 1,
                                    Value = "你好"
                                },
                                new ColumnDomain
                                {
                                    ColumnNumber = 2,
                                    Value = "hello"
                                },
                                new ColumnDomain
                                {
                                    ColumnNumber = 3,
                                    Value = "hel·lo"
                                },
                                new ColumnDomain
                                {
                                    ColumnNumber = 4,
                                    Value = string.Empty
                                }
                            }
                        }
                    }
                }
            };
            return domain;
        }
    }
}
