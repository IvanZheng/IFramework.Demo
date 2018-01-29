using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Demo.DTO.RequestModels.Accounts
{
    /// <summary>
    /// 注册用户
    /// </summary>
    public class RegisterUserRequest
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required(ErrorMessage = "为必填项")]
        [MaxLength(200)]
        [RegularExpression("^[A-Za-z0-9]+$", ErrorMessage = "只能使用字母和数字")]
        [Display(Name = "用户名")]
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Password { get; set; }
    }
}
