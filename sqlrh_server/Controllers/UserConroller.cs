using System.Reflection.PortableExecutable;
using System.Threading;
using System.Runtime.Serialization;
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
using System.IO;
using Microsoft.AspNetCore.Authorization;

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
        [Authorize]
        public async Task<IEnumerable<SqlrhUser>> GetAll()
        {
            string login = User.Identity.Name;
            var user = await _repository.Get(login);
            if (user.Admin)
            {
                return await _repository.GetAll();
            } else{
                return new List<SqlrhUser>();
            }
        }

        [Route("Update")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Update([FromForm]SqlrhUser u)
        {
            var user = await _repository.Get(User.Identity.Name);
            if (!(user.Admin || user.Login == u.Login))
            {
                return new UnauthorizedResult();
            }

            if (!user.Admin)
            {
                u.Admin = false;
            }

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
        [Authorize]
        public async Task<IActionResult> Delete(string login)
        {
            var user = await _repository.Get(User.Identity.Name);
            if (!user.Admin)
            {
                return new UnauthorizedResult();
            }
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

        [Route("LoginForm")]
        [HttpGet]
        public IActionResult LoginForm()
        {
                string curDir = Directory.GetCurrentDirectory();
                string loginFormPath = Path.Combine(Path.Combine(curDir,"Views"),"SimpleLoginForm.html");
                return new PhysicalFileResult(loginFormPath,"text/html");
        }

        [Route("Login")]
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.User.Claims.Count() == 0)
            {
                return Content("User is not authenticated");
            }
            return Content(HttpContext.User.FindFirstValue(ClaimsIdentity.DefaultNameClaimType));
        }

        [Route("Login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromForm]LoginData d)
        {
            if (!String.IsNullOrEmpty(d.Login) && !String.IsNullOrEmpty(d.Password))
            {
                if (await _repository.Contains(d.Login))
                {
                    SqlrhUser user = await _repository.Get(d.Login);
                    return await Authenticate(user, d.Password);
                } else{
                    return new NotFoundResult();
                }
            } else{
                return new BadRequestResult();
            }
        }

        public class LoginData
        {
            public string Login {get; set;}
            public string Password {get; set;}
        }

        [Route("Register")]
        [HttpPost]    
        public async Task<IActionResult> Register([FromForm]LoginData d)
        {
            if (!String.IsNullOrEmpty(d.Login) && !String.IsNullOrEmpty(d.Password))
            {
                if (await _repository.Contains(d.Login))
                {
                    return new ConflictResult();
                }
                var n = new SqlrhUser(d.Login);
                n.PasswordHash =  _PasswordHasher.HashPassword(n, d.Password);
                if ((await _repository.Count()) == 0)
                {
                    //create admin
                    n.Admin = true;
                }
                return new  CreatedResult(n.Login,
                         await _repository.Add(n));              
            }
            return new BadRequestResult();
        }

        [Route("Logout")]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "User");
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
    }
}
