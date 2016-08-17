using System;
using System.Globalization;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using PartnerApiSoapExample.PartnerApiService;

namespace PartnerApiSoapExample
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using (var service = new PartnerApiServiceClient())
            using (var operationScope = new OperationContextScope(service.InnerChannel))
            {
                var requestMessage = new HttpRequestMessageProperty();
                requestMessage.Headers["X-DeveloperAgreement"] = Constants.Agreement;
                requestMessage.Headers["X-PartnerApiSecretToken"] = EncodedHeader(Constants.RawToken);
                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;

                // Fetch all apps on the agreement.
                var apps = service.Application_GetAll();
                
                foreach (var application in apps)
                {
                    Console.WriteLine($"App: {application.Name}");

                    var date = new DateTime(2015, 6, 9);

                    // Fetch all the grants that has been updated since the above date for this app.
                    var result = service.Grant_GetForApplicationSince(application.Id, date);

                    foreach (var grant in result)
                    {
                        Console.WriteLine($"Grant with id {grant.Id}.");
                        Console.WriteLine(DecryptToken(grant.AgreementGrantToken));
                    }

                }
                Console.ReadLine();
            }
            Main(args);
        }

        private static string EncodedHeader(string rawToken)
        {
            var rsa = new RSACryptoServiceProvider(1024);

            rsa.FromXmlString(Constants.PrivateKey);

            var message = $"{rawToken};{DateTime.Now.ToString(CultureInfo.InvariantCulture)};1";

            var tokenBytes = Encoding.UTF8.GetBytes(message);

            var signature = rsa.SignData(tokenBytes, "SHA256");

            var header = $"{message}#{Convert.ToBase64String(signature)}";

            var encodedHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes(header));
            return encodedHeader;
        }

        private static string DecryptToken(string token)
        {
            var rsa = new RSACryptoServiceProvider(1024);

            rsa.FromXmlString(Constants.PrivateKey);
            var tokenBytes = Convert.FromBase64String(token);

            var decryptToken = rsa.Decrypt(tokenBytes, true);

            return Encoding.UTF8.GetString(decryptToken);
        }
    }
}