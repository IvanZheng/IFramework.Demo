using System.ComponentModel;

namespace Demo.Domain
{
    public enum Error
    {
        NoError = 0,
        [Description("帐号({0})不存在")]
        AccountNotExists = 1000,
        [Description("用户({0})不存在")]
        UserNotExists,
        [Description("用户名({0})已存在")]
        UserNameAlreadyExists,
        [Description("错误的用户名或密码")]
        WrongUserNameOrPassword,
        [Description("无效的通用状状态({0})")]
        InvalidCommonStatus,
        [Description("用户不可用")]
        UserUnavailable
    }
}