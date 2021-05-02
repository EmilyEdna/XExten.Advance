using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Model;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public string GetTest(string name)
        {
            return name;
        }
        [HttpGet]
        public (string, int) GetModelTest(TestClass input)
        {
            return (input.Name, input.Age);
        }
    }
}
