using System.Collections.Generic;

namespace CastmExcel
{
    public class ExcelDomain
    {
        public string LocalPath { get; set; }
        public string FileName { get; set; }
        public SheetDomain Sheet { get; set; }
        public List<TitleDomain> TitleList { get; set; }
        public BodyDomain BodyDomain { get; set; }
        public bool ErrorFlag { get; set; }
        public string FullPath
        {
            get
            {
                return this.LocalPath + "\\" + this.FileName + ".xlsx";
            }
        }
    }

    public class TitleDomain
    {
        public string Title { get; set; }
        public bool IsMerge { get; set; }
        public int RowNumber { get; set; }
        public int StartPoint { get; set; }
        public int EndPoint { get; set; }
        public StyleDomain StyleDomain { get; set; }
    }

    public class BodyDomain
    {
        public List<RowDomain> DataList { get; set; }
        public StyleDomain StyleDomain { get; set; }
        public  List<int> CannotReadRowNumber { get; set; }
    }

    public class RowDomain
    {
        public int RowNumber { get; set; }
        public List<ColumnDomain> ColumnList { get; set; }
        public List<ErrorDomain> ErrorList { get; set; }
    }

    public class ColumnDomain
    {
        public int ColumnNumber { get; set; }
        public string Value { get; set; }
    }

    public class SheetDomain
    {
        public string Name { get; set; }
        // TODO => others
    }

    public class StyleDomain
    {
        public bool Flag { get; set; }
        public string BackgroundColor { get; set; }
        public string FontColor { get; set; }
        public int FontSize { get; set; }
        public bool IsBold { get; set; }
        // TODO => others
    }

    public class ErrorDomain
    {
        public string ErrorField { get; set; }
        public string Description { get; set; }
        public int RowNumber { get; set; }
        public int ClomnNumber { get; set; }
    }
}