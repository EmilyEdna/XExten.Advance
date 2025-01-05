using System.IO.Ports;

namespace XExten.Advance.CommunicationFramework.Model
{
    /// <summary>
    /// 连接参数
    /// </summary>
    public class CommunicationParams
    {
        /// <summary>
        /// COM口/IP
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 绑定端口
        /// </summary>
        public int BindPort { get; set; }

        /// <summary>
        /// 奇偶校验
        /// </summary>
        public Parity Parity { get; set; } = Parity.None;

        /// <summary>
        /// 数据位
        /// </summary>
        public int DataBits { get; set; } = 8;

        /// <summary>
        /// 启停位
        /// </summary>
        public StopBits StopBits { get; set; } = StopBits.One;

        /// <summary>
        /// 波特率
        /// </summary>
        public int BaudRate { get; set; } = 9600;

        /// <summary>
        /// 日志头
        /// </summary>
        public string LogHead {  get; set; }

        /// <summary>
        /// 解码数据写日志
        /// </summary>
        public bool IsDecodeWriteLog { get; set; } = false;

        /// <summary>
        /// 接收数据线程Key
        /// </summary>
        public string ThreadKey {  get; set; }
    }
}
