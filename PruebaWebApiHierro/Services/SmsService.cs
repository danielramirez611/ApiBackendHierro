using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace PruebaWebApiHierro.Services
{
    public class SmsService
    {
        private readonly IConfiguration _config;

        public SmsService(IConfiguration config)
        {
            _config = config;
            TwilioClient.Init(
                _config["Twilio:AccountSID"],
                _config["Twilio:AuthToken"]
            );
        }

        public async Task<bool> SendVerificationCode(string phone, string code)
        {
            var message = await MessageResource.CreateAsync(
                body: $"Tu código de verificación es: {code}",
                from: new PhoneNumber(_config["Twilio:FromPhone"]),
                to: new PhoneNumber(phone)
            );
            return message.ErrorCode == null;
        }
    }
}
