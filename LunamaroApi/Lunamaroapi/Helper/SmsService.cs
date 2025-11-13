using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Lunamaroapi.Helper
{
    public class SmsService
    {
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _fromNumber;
        public SmsService(IConfiguration config)
        {
            _accountSid = config["Twilio:AccountSid"];
            _authToken = config["Twilio:AuthToken"];
            _fromNumber = config["Twilio:FromPhoneNumber"];

            TwilioClient.Init(_accountSid, _authToken);
        }
       public void SendSms(string toPhoneNumber, string message)
        {
            try
            {
                var msg = MessageResource.Create(
                    body: message,
                    from: new Twilio.Types.PhoneNumber(_fromNumber),
                    to: new Twilio.Types.PhoneNumber(toPhoneNumber)
                );

                Console.WriteLine($"✅ SMS status: {msg.Status}");
                Console.WriteLine($"✅ SMS SID: {msg.Sid}");
                Console.WriteLine($"✅ To: {msg.To}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ SMS failed: {ex.Message}");
            }
        }
    }
    
}
