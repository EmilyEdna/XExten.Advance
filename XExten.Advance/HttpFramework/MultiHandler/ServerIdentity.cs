using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace XExten.Advance.HttpFramework.MultiHandler
{
    /// <summary>
    /// 证书验证
    /// </summary>
    public abstract class ServerIdentity
    {
        /// <summary>
        /// 证书验证
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <param name="x509"></param>
        /// <param name="chain"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public abstract bool ServerCertificate(HttpRequestMessage httpRequest, X509Certificate2? x509, X509Chain? chain, SslPolicyErrors err);
    }
}
