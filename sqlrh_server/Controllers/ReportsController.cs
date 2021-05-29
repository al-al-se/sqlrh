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

        private readonly IReportBuilderService _builder;

        public ReportsController(ILogger<ReportsController> logger,
                                 IReportRepository r,
                                 IReportBuilderService b)
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
                if (await _repository.ContainsId(id))
                {
                    string path = await _builder.SaveUploadingReportTemplate(uploadingFile);
                    if ( path != null)
                    {
                        var r =  await _repository.LoadFile(id,path);
                        return new  CreatedResult(r.Name, r);
                    }
                }
            } 

            return new  BadRequestResult();
        }

        [Route("Execute")]
        [HttpPost]
        public async Task<IActionResult> Execute(int id)
        {
            if (await _repository.ContainsId(id))
            {
                var r = await _repository.GetReport(id);
                var dest = _builder.StartReportBuilding(r.FilePath);

                return new  CreatedResult(r.Name, dest);
            }
            return new  NotFoundResult();
        }

        string MimeType(string path)
        {
            string ext = System.IO.Path.GetExtension(path);

            switch(ext)
            {
                default: return "text/plain";
            }
        }

        [Route("GetBuildedReport")]
        [HttpPost]
        public IActionResult GetBuildedReport(string path)
        {
            if (_builder.CheckReportStartBuilding(path))
            {
                if (_builder.CheckReportFinished(path))
                {
                    return new PhysicalFileResult(path,MimeType(path));
                }
                return new StatusCodeResult(102);//102 Processing («идёт обработка»);
            }
            return new NotFoundResult();
        }
    }
}
