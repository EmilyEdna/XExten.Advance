using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synctool.InternalFramework.Express.Common
{
    /// <summary>
    ///
    /// </summary>
    public enum QType
    {
        /// <summary>
        /// Like
        /// </summary>
        Like = 1,

        /// <summary>
        /// NotLike
        /// </summary>
        NotLike = 2,

        /// <summary>
        /// Equals
        /// </summary>
        Equals = 3,

        /// <summary>
        /// NotEquals
        /// </summary>
        NotEquals = 4,

        /// <summary>
        /// GreaterThan
        /// </summary>
        GreaterThan = 5,

        /// <summary>
        /// GreaterThanOrEqual
        /// </summary>
        GreaterThanOrEqual = 6,

        /// <summary>
        /// LessThan
        /// </summary>
        LessThan = 7,

        /// <summary>
        /// LessThanOrEqual
        /// </summary>
        LessThanOrEqual = 8
    }
}
