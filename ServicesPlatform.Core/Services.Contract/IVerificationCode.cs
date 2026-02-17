using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Core.Services.Contract
{
    public interface IVerificationCode
    {
        Task<bool> SendVerificationCode(string recipientEmail, string subject, string body);
    }
}
