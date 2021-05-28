using System.Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace sqlrh_server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly ILogger<ReportsController> _logger;

        private readonly IReportRepository _repository;

        public ReportsController(ILogger<ReportsController> logger, IReportRepository r)
        {
            _logger = logger;
            _repository = r;
        }

        [Route("GetAll")]
        [HttpGet]
        public IEnumerable<Report> GetAll()
        {
            return _repository.All;
        }

        [Route("AddNew")]
        [HttpPost]
        public ActionResult AddNew(string name)
        {
            return new  CreatedResult(name, _repository.Add(name));
        }

        [Route("LoadFile")]
        [HttpPost]
        public ActionResult LoadFile(int id, IFormFile f)
        {
            string path =   "./Files/" + f.FileName;
                
            using (var fileStream = 
                new FileStream(path, FileMode.Create))
            {
                f.CopyTo(fileStream);
            }

            var r = _repository.LoadFile(id,path);

            return new  CreatedResult(r.Name, r);
        }
    }
}
