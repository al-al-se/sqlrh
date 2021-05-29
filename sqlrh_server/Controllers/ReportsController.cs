using System.Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private readonly IReportBuilder _builder;

        public ReportsController(ILogger<ReportsController> logger,
                                 IReportRepository r,
                                 IReportBuilder b)
        {
            _logger = logger;
            _repository = r;
            _builder = b;
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
                string path = await _builder.SaveUploadingReportTemplate(uploadingFile);
                if ( path != null)
                {
                    var r =  await _repository.LoadFile(id,path);
                    return new  CreatedResult(r.Name, r);
                }
            } 

            return new  BadRequestResult();
        }
    }
}
