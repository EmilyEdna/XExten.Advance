using System;
using System.Collections.Generic;
using System.Text;
using XExten.Advance.RestHttpFramewor.Options;

namespace XExten.Advance.RestHttpFramewor
{
    public interface IRestHttpClient
    {
        static IRestHttpClient Rest => new Lazy<RestHttpClient>().Value;

        IRestHttpClient UseProxy(RestProxy Proxy);

        IRestHttpClient UseCookie(Action<RestCookie> action);
    }
}
