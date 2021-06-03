using System.ComponentModel;
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
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;

        private readonly IUserRepository _repository;

        public UserController(ILogger<UserController> logger,
                                 IUserRepository r)
        {
            _logger = logger;
            _repository = r;
        }

        [Route("GetAll")]
        [HttpGet]
        public async Task<IEnumerable<SqlrhUser>> GetAll()
        {
            return await _repository.GetAll();
        }

        [Route("AddNew")]
        [HttpPost]
        public async Task<IActionResult> AddNew(SqlrhUser n)
        {
            return new  CreatedResult(n.Name,
                         await _repository.Add(n));
        }

        [Route("Update")]
        [HttpPost]
        public async Task<IActionResult> Update(SqlrhUser u)
        {
            if (await _repository.Contains(u.Id))
            {
                return new  CreatedResult(u.Name,
                                await _repository.Update(u));
            } else{
                _logger.LogError($"User with id = '{u.Id}' not found");
                return new  NotFoundResult();
            }
        }


        [Route("Delete")]
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _repository.Contains(id))
            {
                await _repository.Delete(id);
                return new  OkResult();
            } else
            {
                _logger.LogError($"User with id = '{id}' not found");
                return new  NotFoundResult();
            }
        }
    }
}
