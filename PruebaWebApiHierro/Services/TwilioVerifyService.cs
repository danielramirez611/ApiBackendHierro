using Twilio;
using Twilio.Rest.Verify.V2.Service;

namespace PruebaWebApiHierro.Services
{
    public class TwilioVerifyService
    {
        private readonly string _serviceSid;

        public TwilioVerifyService(IConfiguration config)
        {
            TwilioClient.Init(config["Twilio:AccountSID"], config["Twilio:AuthToken"]);
            _serviceSid = config["TwilioVerify:ServiceSid"];
            Console.WriteLine("Service SID usado: " + _serviceSid); // Agrega esto para verificar

        }

        public async Task<bool> SendVerificationCode(string phone)
        {
            var result = await VerificationResource.CreateAsync(
                to: $"+{phone}",
                channel: "sms",
                pathServiceSid: _serviceSid
            );

            return result.Status == "pending";
        }

        public async Task<bool> CheckVerificationCode(string phone, string code)
        {
            var result = await VerificationCheckResource.CreateAsync(
                to: $"+{phone}",
                code: code,
                pathServiceSid: _serviceSid
            );

            return result.Status == "approved";
        }
    }
}
