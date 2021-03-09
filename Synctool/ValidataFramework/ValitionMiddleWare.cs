using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Synctool.LinqFramework;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyModel;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.IO;

namespace Synctool.ValidataFramework
{
    /// <summary>
    /// 验证中间件
    /// </summary>
    public class ValitionMiddleWare
    {
        private readonly RequestDelegate RequestDelegate;
        private static readonly List<Assembly> Assemblies = new List<Assembly>();
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="RequestDelegates"></param>
        public ValitionMiddleWare(RequestDelegate RequestDelegates)
        {
            RequestDelegate = RequestDelegates;
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="Context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext Context)
        {
            var Request = Context.Request;
            var Routes = Request.Path.ToString().Split("/")
                  .Where(t => !t.IsNullOrEmpty())
                  .Where(t => !t.ToUpper().StartsWith("API")).ToArray();
            var Ctrl = $"{Routes.FirstOrDefault()}controller".ToUpper();
            var TargetCtrl = GetAssembly().Select(t => t.GetTypes().Where(x => x.Name.ToUpper().Equals(Ctrl)).FirstOrDefault()).FirstOrDefault();
            MethodInfo? TargetMethod = TargetCtrl?.GetMethod(Routes.LastOrDefault().ToLower(), BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
            if (TargetMethod != null)
            {
                var Rules = GetAssembly().SelectMany(t => t.ExportedTypes.Where(x => x.BaseType == typeof(ValitionBaseRule))).ToList();
                Type Rule;
                if (Rules.Count > 1)
                    Rule = Rules.Where(t => !t.Name.Equals("ValitionRule")).LastOrDefault();
                else
                    Rule = Rules.FirstOrDefault();
                var Instance = Activator.CreateInstance(Rule) as ValitionBaseRule;
                Dictionary<string, string> KeyValues;
                var temp = GetQueryString(Request.QueryString.ToString());
                KeyValues = temp.Count > 0 ? temp : GetQueryString(Request);
                var result = await Instance.ValitionRules(KeyValues, TargetMethod.GetParameters());
                if (!result.Success)
                {
                    Context.Response.ContentType = "application/json";
                    await Context.Response.WriteAsync(result.ToJson(), Encoding.UTF8);
                    return;
                }
            }
            await RequestDelegate(Context);
        }
        private static List<Assembly> GetAssembly()
        {
            if (Assemblies.Count > 0)
                return Assemblies;
            var lib = DependencyContext.Default;
            var libs = lib.CompileLibraries.Where(t => !t.Serviceable).Where(t => t.Type == "project").ToList();
            foreach (var item in libs)
            {
                Assembly assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(item.Name));
                Assemblies.Add(assembly);
            }
            return Assemblies;
        }
        private Dictionary<string, string> GetQueryString(string url)
        {
            Dictionary<string, string> Result = new Dictionary<string, string>();
            Regex reg = new Regex(@"(^|&)?(\w+)=([^&]+)(&|$)?", RegexOptions.Compiled);
            MatchCollection mc = reg.Matches(url);
            foreach (Match m in mc)
            {
                Result.Add(m.Result("$2"), m.Result("$3"));
            }
            return Result;
        }
        private Dictionary<string, string> GetQueryString(HttpRequest Request)
        {
            if (Request.ContentType == "application/x-www-form-urlencoded")
            {
                Dictionary<string, string> Result = new Dictionary<string, string>();
                Request.Form.ToList().ForEach(t =>
                {
                    Result.Add(t.Key, t.Value.ToString());
                });
                return Result;
            }
            else if (Request.ContentType== "application/json")
            {
                StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8);
                return reader.ReadToEndAsync().Result.ToModel<Dictionary<string, string>>();
            }
            return new Dictionary<string, string>();
        }
    }
}
