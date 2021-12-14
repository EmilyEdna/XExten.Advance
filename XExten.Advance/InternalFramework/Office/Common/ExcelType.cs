using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XExten.Advance.InternalFramework.Office.Common
{
    /// <summary>
    /// Excel 类型
    /// </summary>
    public enum ExcelType
    {
        /// <summary>
        /// Excel2003
        /// </summary>
        [Description("Excel2003")]
        xls = 2003,
        /// <summary>
        /// Excel2007+
        /// </summary>
        [Description("Excel2007+")]
        xlsx = 2007
    }
}
