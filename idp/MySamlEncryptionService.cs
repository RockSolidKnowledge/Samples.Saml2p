using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using System.Xml;
using IdentityServer4.Saml.Services.Interfaces;

namespace idp
{
    public class MySamlEncryptionService : ISamlEncryptionService
    {
        private const string Callback_Path_For_Pkcs1_Federation = "http://localhost:5002/signin-saml-pkcs1";

        public Task<XmlElement> CreateEncryptedAssertion(XmlElement assertionElement, X509Certificate2 certificate)
        {
            // selection of padding based on predefined 
            var enforceOAEP = IsThisOAEPFederation(assertionElement);
                
            using (var sessionKey = new RijndaelManaged())
            {
                var ek = new EncryptedKey { EncryptionMethod = new EncryptionMethod(EncryptedXml.XmlEncRSA15Url) };
                var algorithm = (RSA)certificate.PublicKey.Key;
                var encryptKey = EncryptedXml.EncryptKey(sessionKey.Key, algorithm, enforceOAEP);
                ek.CipherData = new CipherData(encryptKey);
                
                var edElement = new EncryptedData();
                edElement.Type = EncryptedXml.XmlEncElementUrl;
                edElement.EncryptionMethod = new EncryptionMethod(EncryptedXml.XmlEncAES256Url);
                edElement.CipherData.CipherValue = new EncryptedXml().EncryptData(assertionElement, sessionKey, false);
                edElement.KeyInfo.AddClause(new KeyInfoEncryptedKey(ek));
                return Task.FromResult(edElement.GetXml());
            }
        }

        private static bool IsThisOAEPFederation(XmlElement assertionElement) => !assertionElement.InnerXml.Contains(Callback_Path_For_Pkcs1_Federation);
    }
}