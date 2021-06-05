using System.Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace sqlrh_server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly ILogger<ReportsController> _logger;

        private readonly IReportRepository _repository;

        private readonly IUserRepository _users;

        private readonly IReportBuilderService _builder;

        public ReportsController(ILogger<ReportsController> logger,
                                 IReportRepository r,
                                 IUserRepository u,
                                 IReportBuilderService b)
        {
            _logger = logger;
            _repository = r;
            _builder = b;
            _users = u;
        }

        [Route("GetAll")]
        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<Report>> GetAll()
        {
            return await _repository.GetAll(User.Identity.Name);
        }

        [Route("AddNew")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddNew(string name)
        {
            if (!await _users.IsUserAdmin(User.Identity.Name)) 
                return new UnauthorizedResult();
            return new  CreatedResult(name, await _repository.Add(name));
        }

        [Route("LoadFile")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> LoadFile(int id, IFormFile uploadingFile)
        {
            if (!await _users.IsUserAdmin(User.Identity.Name)) 
                return new UnauthorizedResult();
            
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
                } else
                {
                    _logger.LogError($"Report id = {id} not found");
                    return new NotFoundResult();
                }
            } 

            return new  BadRequestResult();
        }

        [Route("Execute")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Execute(int id)
        {
            if (!await _repository.IsReportAvailableToUser(id, User.Identity.Name)) 
                        return new UnauthorizedResult();
            
            if (await _repository.ContainsId(id))
            {
                var r = await _repository.GetReport(id);
                var dest = await _builder.StartReportBuilding(r.FilePath,User.Identity.Name);

                return new  CreatedResult(r.Name, dest);
            } else
            {
                _logger.LogError($"Report id = {id} not found");
                return new  NotFoundResult();
            }
        }

        string MimeType(string path)
        {
            string ext = System.IO.Path.GetExtension(path);

            switch(ext)
            {
                case ".odt" : return "application/vnd.oasis.opendocument.text";
                case ".docx" : return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".html" : return "text/html";
                case ".md" : return "text/markdown";
                default: return "text/plain";
            }
        }

        [Route("GetBuildedReport")]
        [HttpPost]
        [Authorize]
        public IActionResult GetBuildedReport(string path)
        {
            if (!_builder.CheckUserAccess(path, User.Identity.Name))
                return new UnauthorizedResult();

            if (!_builder.CheckReportStartBuilding(path))
                return new NotFoundResult();
            
            if (!_builder.CheckReportFinished(path))
                return new StatusCodeResult(102);//102 Processing («идёт обработка»);
            
            return new PhysicalFileResult(path,MimeType(path));    
        }

        [Route("DeleteReport")]
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _users.IsUserAdmin(User.Identity.Name)) 
                return new UnauthorizedResult();
            
            if (await _repository.ContainsId(id))
            {
                await _repository.Delete(id);
                return new  OkResult();
            } else
            {
                _logger.LogError($"Report id = {id} not found");
                return new  NotFoundResult();
            }
        }

        [Route("AllowUser")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AllowUser(int id, string login)
        {
            if (! await _users.IsUserAdmin(User.Identity.Name))
                 return new UnauthorizedResult();
            
            if (! await _repository.ContainsId(id)) return new NotFoundResult();
            
            if (! await _users.Contains(login)) return new NotFoundResult();

            return new CreatedResult("The user is allowed to the report",
                         _repository.Allow(id,login));
        }

        [Route("AllowUser")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DisallowUser(int id, string login)
        {
            if (! await _users.IsUserAdmin(User.Identity.Name))
                 return new UnauthorizedResult();
            
            if (! await _repository.ContainsId(id)) return new NotFoundResult();
            
            if (! await _users.Contains(login)) return new NotFoundResult();

            return new CreatedResult("The user is not allowed to the report",
                         _repository.Disallow(id,login));
        }
    }
}
