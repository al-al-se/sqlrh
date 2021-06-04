using System.ComponentModel;
using System.Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace sqlrh_server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;

        private readonly IUserRepository _repository;

        IPasswordHasher<SqlrhUser> _PasswordHasher;

        public UserController(ILogger<UserController> logger,
                                 IUserRepository r,
                                 IPasswordHasher<SqlrhUser> p)
        {
            _logger = logger;
            _repository = r;
            _PasswordHasher = p;
        }

        [Route("GetAll")]
        [HttpGet]
        public async Task<IEnumerable<SqlrhUser>> GetAll()
        {
            return await _repository.GetAll();
        }

        [Route("Update")]
        [HttpPost]
        public async Task<IActionResult> Update(SqlrhUser u)
        {
            if (await _repository.Contains(u.Login))
            {
                return new  CreatedResult(u.Login,
                                await _repository.Update(u));
            } else{
                _logger.LogError($"User login '{u.Login}' not found");
                return new  NotFoundResult();
            }
        }


        [Route("Delete")]
        [HttpDelete]
        public async Task<IActionResult> Delete(string login)
        {
            if (await _repository.Contains(login))
            {
                await _repository.Delete(login);
                return new  OkResult();
            } else
            {
                _logger.LogError($"User login '{login}' not found");
                return new  NotFoundResult();
            }
        }

        [Route("Login")]
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.User.Claims.Count() == 0)
            {
                    return new ContentResult 
                    {
                        ContentType = "text/html",
                        Content = "<div>Hello World</div>"
                    };
            }
            return Content(HttpContext.User.FindFirstValue(ClaimsIdentity.DefaultNameClaimType));
        }

        [Route("Login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string login, string password)
        {
            if (String.IsNullOrEmpty(login) && String.IsNullOrEmpty(password))
            {
                if (await _repository.Contains(login))
                {
                    SqlrhUser user = await _repository.Get(login);
                    return await Authenticate(user, password);
                } else{
                    return new NotFoundResult();
                }
            } else{
                return new BadRequestResult();
            }
        }

        [Route("Register")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string login, string password)
        {
            if (String.IsNullOrEmpty(login) && String.IsNullOrEmpty(password))
            {
                if (await _repository.Contains(login))
                {
                    return new ConflictResult();
                }
                var n = new SqlrhUser(login);
                n.PasswordHash =  _PasswordHasher.HashPassword(n, password);
                new  CreatedResult(n.Name,
                         await _repository.Add(n));              
            }
            return new BadRequestResult();
        }
 
        private async Task<IActionResult> Authenticate(SqlrhUser user, string password)
        {
            switch (_PasswordHasher.VerifyHashedPassword(user,user.PasswordHash,password))
            {
                case PasswordVerificationResult.Failed:
                    return new NotFoundResult();
                case PasswordVerificationResult.SuccessRehashNeeded:
                    return Content($"Need change password");
                case PasswordVerificationResult.Success:
                {
                    // создаем один claim
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login)
                    };
                    // создаем объект ClaimsIdentity
                    ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                    // установка аутентификационных куки
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
                    return new OkResult();
                }
            }
            return new BadRequestResult();
        }
 
        [Route("Logout")]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
