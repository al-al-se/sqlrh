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
    public class ExternalDataBaseController : ControllerBase
    {
        private readonly ILogger<ExternalDataBaseController> _logger;

        private readonly IExternalDataBaseRepository _repository;

        public ExternalDataBaseController(ILogger<ExternalDataBaseController> logger,
                                 IExternalDataBaseRepository r)
        {
            _logger = logger;
            _repository = r;
        }

        [Route("GetAll")]
        [HttpGet]
        public async Task<IEnumerable<ExternalDatabase>> GetAll()
        {
            return await _repository.GetAll();
        }

        [Route("AddNew")]
        [HttpPost]
        public async Task<IActionResult> AddNew(string alias, string connectionString)
        {
            return new  CreatedResult(alias,
                         await _repository.Add(alias,connectionString));
        }

        [Route("Execute")]
        [HttpPost]
        public async Task<IActionResult> ChangeConnection(string alias, string connectionString)
        {
            if (await _repository.Contains(alias))
            {
                return new  CreatedResult(alias,
                                await _repository.Change(alias,connectionString));
            }
            return new  NotFoundResult();
        }
    }
}
