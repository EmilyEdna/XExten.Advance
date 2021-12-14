using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace XExten.Advance.InternalFramework.Securities.Common
{
    /// <summary>
    /// 加解密类型
    /// </summary>
    public enum SecurityType
    {
        /// <summary>
        /// Base64
        /// </summary>
        [Description("Base64")]
        Base64,
        /// <summary>
        /// UTF16
        /// </summary>
        [Description("UTF16")]
        UTF16,
        /// <summary>
        /// EncodedURI
        /// </summary>
        [Description("EncodedURI")]
        EncodedURI,
        /// <summary>
        /// Uint8
        /// </summary>
        [Description("Uint8")]
        Uint8,
        /// <summary>
        /// Normal
        /// </summary>
        [Description("Normal")]
        Normal
    }
}
