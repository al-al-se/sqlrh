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
        public async Task<IEnumerable<Report>> GetAll()
        {
            return await _repository.GetAll();
        }

        [Route("AddNew")]
        [HttpPost]
        public async Task<IActionResult> AddNew(string name)
        {
            return new  CreatedResult(name, await _repository.Add(name));
        }

        [Route("LoadFile")]
        [HttpPost]
        public async Task<IActionResult> LoadFile(int id, IFormFile uploadingFile)
        {
            if (uploadingFile != null)
            {
                string path =   System.IO.Path.Combine(,uploadingFile.FileName);
                    
                using (var fileStream = 
                    new FileStream(path, FileMode.Create))
                {
                   await  uploadingFile.CopyToAsync(fileStream);
                }

                var r =  await _repository.LoadFile(id,path);

                return new  CreatedResult(r.Name, r);
            } else {
                return new  BadRequestResult();
            }
        }
    }
}
