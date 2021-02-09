using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synctool.InternalFunc.Office.Common
{
    /// <summary>
    /// Excel 类型
    /// </summary>
    public enum ExcelType
    {
        [Description("Excel2003")]
        xls = 2003,
        [Description("Excel2007+")]
        xlsx = 2007
    }
}
