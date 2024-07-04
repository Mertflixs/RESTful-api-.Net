using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using FluentValidation.Results;
using Ppr.Rest.Api.Models;
using Ppr.Rest.Api.Validators;

namespace Ppr.Rest.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IValidator<User> _validator;
        private static List<User> users = new List<User>();

        public UsersController(IValidator<User> validator)
        {
            _validator = validator;
        }

        //Tüm kullanıcıları getiren GET endpointi
        [HttpGet]
        public ActionResult<ApiResponse<List<User>>> Get()
        {
            return Ok(new ApiResponse<List<User>>(users));
        }
        //Belirli id deki kullanıcıyı getiren GET endpointi
        [HttpGet("{id}")]
        public ActionResult<ApiResponse<User>> Get(int id)
        {
            var user = users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound(new ApiResponse<User>("User not found."));

            return Ok(new ApiResponse<User>(user));
        }
        //Yeni bir kullanıcı eklemek için POST endpointi
        [HttpPost]
        public ActionResult<ApiResponse<User>> Post([FromBody] User user)
        {
            //burada eklenecek kullanıcı fluentValidation ile doğrulanır ve ekleme işşlemi devam eder
            ValidationResult result = _validator.Validate(user);

            if (!result.IsValid)
            {
                return BadRequest(result.ToApiResponse<User>());
            }

            user.Id = users.Count > 0 ? users.Max(u => u.Id) + 1 : 1;
            users.Add(user);
            return CreatedAtAction(nameof(Get), new { id = user.Id }, new ApiResponse<User>(user));
        }
        //Mevcut bir kullanıcıyı güncelleyen PUT endpointi
        [HttpPut("{id}")]
        public ActionResult<ApiResponse<User>> Put(int id, [FromBody] User user)
        {
            var existingUser = users.FirstOrDefault(u => u.Id == id);
            if (existingUser == null)
            {
                return NotFound(new ApiResponse<User>("User not found."));
            }

            ValidationResult result = _validator.Validate(user);

            if (!result.IsValid)
            {
                return BadRequest(result.ToApiResponse<User>());
            }
            //FluentValidation ile kontrol edildikten sonra bu kısımda güncelleme işlemi yapılır
            existingUser.Name = user.Name;
            existingUser.Email = user.Email;
            existingUser.Password = user.Password;
            return Ok(new ApiResponse<User>(existingUser));
        }
        //Mevcut kullanıcıyı kısmen güncelleyen PATCH endpointi
        [HttpPatch("{id}")]
        public ActionResult<ApiResponse<User>> Patch(int id, [FromBody] User user)
        {
            var existingUser = users.FirstOrDefault(u => u.Id == id);
            if (existingUser == null)
            {
                return NotFound(new ApiResponse<User>("User not found."));
            }
            //yalnızca dolu olan alanların güncelleme işlemi yapılır
            if (user.Name != null)
                existingUser.Name = user.Name;
            if (user.Email != null)
                existingUser.Email = user.Email;
            if (user.Password != null)
                existingUser.Password = user.Password;

            return Ok(new ApiResponse<User>(existingUser));
        }
        //mevcut kayıtlı kullanıcıyı silen DELETE endpointi
        [HttpDelete("{id}")]
        public ActionResult<ApiResponse<List<User>>> Delete(int id)
        {
            var user = users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound(new ApiResponse<List<User>>("User not found."));
            }

            users.Remove(user);
            return Ok(new ApiResponse<List<User>>(users));
        }
        //Kullanıcıları isme veya email e göre filtreleyen listeleme endpointi
        [HttpGet("list")]
        public ActionResult<ApiResponse<List<User>>> List([FromQuery] string? name, [FromQuery] string? email)
        {
            var filteredUsers = users.AsQueryable();
            if (!string.IsNullOrEmpty(name))
            {
                filteredUsers = filteredUsers.Where(u => u.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrEmpty(email))
            {
                filteredUsers = filteredUsers.Where(u => u.Email.Contains(email, StringComparison.OrdinalIgnoreCase));
            }
            return Ok(new ApiResponse<List<User>>(filteredUsers.ToList()));
        }
    }
}
