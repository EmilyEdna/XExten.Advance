using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NPOI.HPSF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using XExten.Advance.InternalFramework.Office.Common;
using XExten.Advance.StaticFramework;

namespace ValiTest.Controllers
{
    [ApiController]
    [Route("Api/[controller]/[action]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public string SystemVali(string name)
        {
            List<Test1> d = new List<Test1>() {
                new Test1{
                    Datas= Datas.X1,
                    ISDel= true,
                    Name="张三"
                 }
            };
            SyncStatic.ExportExcel(d, ExcelType.xlsx, "test", stream =>
            {
                //using FileStream fs = new FileStream("D:\\Test.xls", FileMode.Create);

                //stream.CopyTo(fs);

                // 把 Stream 转换成 byte[]
                stream.Seek(0, SeekOrigin.Begin);

                byte[] bytes = new byte[stream.Length];

                stream.Read(bytes, 0, bytes.Length);

                // 设置当前流的位置为流的开始

                stream.Seek(0, SeekOrigin.Begin);

                // 把 byte[] 写入文件
                FileStream fs = new FileStream("D:\\Test.xlsx", FileMode.Create);


                BinaryWriter bw = new BinaryWriter(fs);

                bw.Write(bytes);

                bw.Close();

                fs.Close();
            });
            return name;
        }
        [HttpGet]
        public (string, int) ModelVali(TestClass input)
        {
            return (input.Name, input.Age);
        }
    }
    public class Test1
    {
        [Office(MapperField = "姓名")]
        public string Name { get; set; }
        [Office(MapperField = "是否删除", BoolEnum = typeof(Tb))]
        public bool ISDel { get; set; }
        [Office(MapperField = "值", Enum = true)]
        public Datas Datas { get; set; }

    }

    public enum Datas
    {
        [Description("X1")]
        X1,
        [Description("X2")]
        X2
    }
    public enum Tb
    {
        [Description("是")]
        TRUE,
        [Description("否")]
        FALSE
    }
}
