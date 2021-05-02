using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ValiTest.Controllers
{
    [ApiController]
    [Route("Api/[controller]/[action]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public string SystemVali(string name)
        {
            return name;
        }
        [HttpGet]
        public (string, int) ModelVali(TestClass input)
        {
            return (input.Name, input.Age);
        }
    }
}
