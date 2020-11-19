using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.ConditionalFormatting;

namespace SmartWatering.Controllers
{
    [AllowAnonymous]
    [Route("ai/test")]
    public class AI : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
        //[HttpGet]
        //public ActionResult Test()
        //{
        //    var val = new
        //    {
        //        test = "Why so serious?"
        //    };
        //    return new JsonResult(val);
        //}
        [HttpPost]
        public IActionResult Add(Object t)
        {

            Console.WriteLine(t);

            return new JsonResult(t);
        }
    }
}
