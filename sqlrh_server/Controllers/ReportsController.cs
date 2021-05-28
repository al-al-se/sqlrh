using System.Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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


        [HttpGet]
        public IEnumerable<Report> Get()
        {
            return _repository.All;
        }
    }
}
