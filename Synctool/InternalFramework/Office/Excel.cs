using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using Synctool.InternalFramework.Office.Common;
using Synctool.LinqFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Synctool.InternalFramework.Office
{
    internal partial class Excel
    {
        #region NPOI
        private IWorkbook Workbook;
        private ISheet Sheet;
        private IRow Row;
        private ICell Cell;
        private ICellStyle HStyle;
        private ICellStyle BStyle;
        private ICellStyle FStyle;
        private ExcelType EnumType;
        private string DateFormat;
        private ArrayList Table;
        private List<int> FullCols;
        private Stream stream;
        private bool HasFooter;
        #endregion

        #region 获取对象
        /// <summary>
        /// 获取工作薄
        /// </summary>
        /// <returns></returns>
        internal IWorkbook GetWorkbook() => Workbook;
        /// <summary>
        /// 获取工作表
        /// </summary>
        /// <returns></returns>
        internal ISheet GetSheet() => Sheet;
        /// <summary>
        /// 获取行
        /// </summary>
        /// <returns></returns>
        internal IRow GetRow() => Row;
        /// <summary>
        /// 获取列
        /// </summary>
        /// <returns></returns>
        internal ICell GetCell() => Cell;
        #endregion

        #region 导出
        /// <summary>
        /// 导出构造
        /// </summary>
        /// <param name="type"></param>
        /// <param name="Format"></param>
        internal Excel(ExcelType type, string Format = "yyyy-MM-dd")
        {
            EnumType = type;
            DateFormat = Format;
        }
        /// <summary>
        /// 创建工作薄
        /// </summary>
        /// <returns></returns>
        internal Excel CreateExportWorkBook()
        {
            if (Workbook != null) return this;
            if (EnumType == ExcelType.xls)
                Workbook = new HSSFWorkbook();
            else
                Workbook = new XSSFWorkbook();
            return this;
        }
        /// <summary>
        /// 创建工作表
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal Excel CreateExportSheet(string name)
        {
            CheckWorkBook();
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("参数不能为空");
            Sheet = Workbook.GetSheet(name) ?? Workbook.CreateSheet(name);
            return this;
        }
        /// <summary>
        /// 创建行
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal Excel CreateExportRows(int index)
        {
            Row = Sheet.GetRow(index) ?? Sheet.CreateRow(index);
            return this;
        }
        /// <summary>
        /// 创建单元格
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        internal Excel CreateExportCells(int index, object value)
        {
            Cell = Row.GetCell(index) ?? Row.CreateCell(index);
            if (value.GetType() == typeof(DateTime))
            {
                Cell.SetCellValue(((DateTime)value).ToString(DateFormat));
                Cell.CellStyle.DataFormat = Workbook.CreateDataFormat().GetFormat(DateFormat);
            }
            Cell.SetCellValue(value.ToString());
            return this;
        }
        /// <summary>
        /// 表头样式
        /// </summary>
        /// <param name="EndCol"></param>
        /// <returns></returns>
        internal Excel HeadExportStyle(int EndCol)
        {
            HStyle = Workbook.CreateCellStyle();
            SetBorder(HStyle);
            SetBorderColor(HStyle);
            SetAlignment(HStyle);
            SetFont(HStyle);
            SetStyle(0, 0, 0, EndCol, HStyle);
            return this;
        }
        /// <summary>
        /// 内容样式
        /// </summary>
        /// <param name="EndRow"></param>
        /// <param name="EndCol"></param>
        /// <returns></returns>
        internal Excel BodyExportStyle(int EndRow, int EndCol)
        {
            BStyle = Workbook.CreateCellStyle();
            SetBorder(BStyle);
            SetBorderColor(BStyle, false);
            SetAlignment(BStyle);
            SetFont(BStyle, false);
            SetStyle(1, EndRow, 0, EndCol, BStyle);
            return this;
        }
        /// <summary>
        /// 页脚样式
        /// </summary>
        /// <param name="EndRow"></param>
        /// <param name="EndCol"></param>
        /// <returns></returns>
        internal Excel FootExportStyle(int EndRow, int EndCol)
        {
            FStyle = Workbook.CreateCellStyle();
            SetBorder(FStyle);
            SetBorderColor(FStyle, false);
            SetAlignment(FStyle);
            SetFont(FStyle, false);
            SetStyle(EndRow, EndRow, 0, EndCol, FStyle);
            return this;
        }
        /// <summary>
        /// 合并单元格
        /// </summary>
        /// <param name="SRI">起始行</param>
        /// <param name="ERI">结束行</param>
        /// <param name="SCI">起始列</param>
        /// <param name="ECI">结束列</param>
        /// <returns></returns>
        internal Excel MergeExportCell(int SRI, int ERI, int SCI, int ECI)
        {
            Sheet.AddMergedRegion(new CellRangeAddress(SRI, ERI, SCI, ECI));
            return this;
        }
        /// <summary>
        ///  写入文件流
        /// </summary>
        /// <param name="st"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        internal Excel WriteExportStream(Stream st, Action<Stream> action = null)
        {
            Workbook.Write(st);
            action?.Invoke(st);
            return this;
        }
        #endregion

        #region 导入
        /// <summary>
        /// 导入构造
        /// </summary>
        /// <param name="st"></param>
        /// <param name="type"></param>
        /// <param name="HasPageFooter"></param>
        internal Excel(Stream st, ExcelType type, bool HasPageFooter)
        {
            stream = st;
            EnumType = type;
            HasFooter = HasPageFooter;
            Table = new ArrayList();
        }
        /// <summary>
        /// 创建导入工作薄
        /// </summary>
        /// <returns></returns>
        internal Excel CreateImportWorkBook()
        {
            if (stream == null) return this;
            if (EnumType == ExcelType.xls)
                Workbook = new HSSFWorkbook(stream);
            else
                Workbook = new XSSFWorkbook(stream);
            return this;
        }
        /// <summary>
        /// 获取导入的工作表
        /// </summary>
        /// <param name="Index"></param>
        /// <returns></returns>
        internal Excel CreateImportSheet(int Index)
        {
            Sheet = Workbook.GetSheetAt(Index);
            return this;
        }
        /// <summary>
        /// 获取导入表格头
        /// </summary>
        /// <returns></returns>
        internal Excel CreateImportHead<T>()
        {
            Row = Sheet.GetRow(Sheet.FirstRowNum);
            Dictionary<string, string> FieldNames = new Dictionary<string, string>();
            FullCols = new List<int>();
            typeof(T).GetProperties().ForArrayEach<PropertyInfo>(item =>
            {
                var AttrField = (item.GetCustomAttribute(typeof(OfficeAttribute), false) as OfficeAttribute).MapperField;
                FieldNames.Add(AttrField, item.Name);
            });
            for (int index = 0; index < Row.LastCellNum; index++)
            {
                var HeaderKey = Row.GetCell(index).StringCellValue;
                if (!FieldNames.ContainsKey(HeaderKey))
                    FieldNames.Remove(HeaderKey);
                Table.Add(FieldNames[HeaderKey]);
                FullCols.Add(index);
            }
            return this;
        }
        /// <summary>
        /// 创建导入内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal List<T> CreateImportBody<T>() where T : new()
        {
            List<T> Entitise = new List<T>();
            var SecondRow = Sheet.FirstRowNum + 1;
            var LastRow = HasFooter ? Sheet.LastRowNum - 1 : Sheet.LastRowNum;//排除页脚
            for (int index = SecondRow; index <= LastRow; index++)
            {
                if (Sheet.GetRow(index) == null)
                    return null;
                T Entity = new T();
                foreach (var item in FullCols)
                {
                    var Office = typeof(T).GetProperty(Table[item].ToString()).GetCustomAttribute(typeof(OfficeAttribute), false);
                    if (Office != null)
                    {
                        var WasEnum = (Office as OfficeAttribute).Enum;
                        if (WasEnum)
                        {
                            var Result = Enum.Parse(typeof(T).GetProperty(Table[item].ToString()).PropertyType, typeof(T).GetProperty(Table[item].ToString()).PropertyType.GetFields()
                                  .Where(Item => Item.GetCustomAttribute(typeof(DescriptionAttribute), false) != null)
                                  .Where(Item => ((DescriptionAttribute)Item.GetCustomAttribute(typeof(DescriptionAttribute), false)).Description == Sheet.GetRow(index).GetCell(item).StringCellValue)
                                  .FirstOrDefault().Name);
                            Entity.GetType().GetProperty(Table[item].ToString()).SetValue(Entity, Result);
                        }
                        else
                        {
                            var Result = Sheet.GetRow(index).GetCell(item).StringCellValue;
                            Type EnumType = (Entity.GetType().GetProperty(Table[item].ToString()).GetCustomAttribute(typeof(OfficeAttribute)) as OfficeAttribute)?.BoolEnum;
                            if (EnumType != null)
                            {
                                var FieldName = EnumType.GetFields().Where(Item => Item.GetCustomAttribute(typeof(DescriptionAttribute), false) != null)
                                    .Where(Item => ((DescriptionAttribute)Item.GetCustomAttribute(typeof(DescriptionAttribute), false)).Description == Result).FirstOrDefault()?.Name;
                                if (!FieldName.IsNullOrEmpty())
                                {
                                    if (bool.TryParse(FieldName, out bool BoolRes))
                                        Entity.GetType().GetProperty(Table[item].ToString()).SetValue(Entity, BoolRes);
                                    else
                                        Entity.GetType().GetProperty(Table[item].ToString()).SetValue(Entity, Result);
                                }
                            }
                            else
                                Entity.GetType().GetProperty(Table[item].ToString()).SetValue(Entity, Result);
                        }
                    }
                }
                Entitise.Add(Entity);
            }
            return Entitise;
        }
        #endregion
    }
}
