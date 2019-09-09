using System;
using IdentityServer4.Models;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using IdentityServer4;
using IdentityServer4.Saml;
using IdentityServer4.Saml.Models;

namespace idp
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            return new ApiResource[]
            {
                new ApiResource("api1", "My API #1")
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new[]
            {
                new Client
                {
                    ClientId = "http://localhost:5002/saml",
                    ClientName = "pkcs1",
                    ProtocolType = IdentityServerConstants.ProtocolTypes.Saml2p,
                    AllowedScopes = {"openid", "profile"}
                },
                new Client
                {
                    ClientId = "http://localhost:5002/saml-oaep",
                    ClientName = "oaep",
                    ProtocolType = IdentityServerConstants.ProtocolTypes.Saml2p,
                    AllowedScopes = {"openid", "profile"}
                }
            };
        }
        public static IEnumerable<ServiceProvider> GetServiceProviders()
        {
            return new[]
            {
                new ServiceProvider
                {
                    EntityId = "http://localhost:5002/saml",
                    AssertionConsumerServices =
                        {new Service(SamlConstants.BindingTypes.HttpPost, "http://localhost:5002/signin-saml-pkcs1")},
                    EncryptionCertificate = new X509Certificate2(Convert.FromBase64String("MIICtDCCAZygAwIBAgIIT1pFK6+584gwDQYJKoZIhvcNAQENBQAwGTEXMBUGA1UEAwwOSWRlbnRpdHlTZXJ2ZXIwIBcNMTkwOTA2MDAwMDAwWhgPMjIxOTA5MDYwMDAwMDBaMBkxFzAVBgNVBAMMDklkZW50aXR5U2VydmVyMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEArQ9hBxuFLbualoKNBrr4V7Ng6tXMw/2Mc6dzWt3nFQGxLAccsUUwfPQIbwGn+wA8yQQa6rgh5Zp1YwVDZRguP+cOlhg25rqGFdUrIvexJW7P3Gf4KWYQ9lBw46o0eZPOmML9i2BkrxzIWtM9iHzyTVDToqMkQGPSL2RQeSy0k3QQMQMk/1cv+Z1+mK8bgPO4tLTKyeTTyAec18y3qKZycWwk/h6b6PCtjRE80Et/yZqT8PNCXhNTrqzZx1X7J7UfzKCR8cNJ/ysEcMwtnZyBnA3Fv1LzujmiYcNPFl6OEiuiL0ghOIjW2hlw305hCs1TaTnUIBZz4jHYkZKyGr2DYwIDAQABMA0GCSqGSIb3DQEBDQUAA4IBAQAICu8lyJCzauuV1Yr/KRr2RfSd8s9oACxslLpyP++bP15R6rnppYas6yflQljH91xnv+z8cZYoq7ZEuTWhh3dh+ImaSbnkXag6VcnZxGTbz1B+CCp5vjAnq9REsjAzMnXOO6+idzGO8tTR8XMhfCCOzGdlGgnqs/wMOY32XlnGXDz3fgqBqqUtk6+QIhJ1Q8z4cpJ/Qp26YgkNy3X9I9Cti6Q1sqmUHK3bOzbwZ3HHh58vnmd66m3xdzk/09PJjY3lrGrrPrbHuM+rYWKOa/z6S9dPAc5tF6OBq9XvVNZdtyOSggC8DzSjzBANlPf7CUJLesnLdDSavOs8eEEetalb")),
                    SigningCertificates = new List<X509Certificate2>{new X509Certificate2(Convert.FromBase64String("MIICtDCCAZygAwIBAgIIGGGy3fqoj8wwDQYJKoZIhvcNAQENBQAwGTEXMBUGA1UEAwwOSWRlbnRpdHlTZXJ2ZXIwIBcNMTkwOTA2MDAwMDAwWhgPMjIxOTA5MDYwMDAwMDBaMBkxFzAVBgNVBAMMDklkZW50aXR5U2VydmVyMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAqazqX+dS0S+aiz3guDAlkYeHqSoikykwXmYl9J6IcTj5+kz2C61v0jAxv397IuM1Gb1gr32Ii5326iwM241XrNNRbhSFXKlyNNQ5/dg0aIWsHoY4H6GtkENCMxe7e9x3GrjZFbib6Lp+/uYkZj5wfUIc70VjP5GxtmtUXXKt8Qtun+uaOTU+qJ7h8/aYynoc18FdiU/wAJLzYRHeCgo9QzkDmdrVBTU1Zgvwu4va8QA8Arukri7UWvJUvQu8hBDGuf8bd8EogU3a6oDdfD51mmaViGRaI6+3/2+bXZGqm9Bwmi6M9fE5jEZfBJL4VY9yPaRwgzopEkEd8zFhH5X/4QIDAQABMA0GCSqGSIb3DQEBDQUAA4IBAQALC5qPKUue8LWrWa+2YDaeiyZfFIxfamUgQLkgieh9d6C7Pm738f4c2vtQVguS9v2A6VTVR2fFGWw7ZglGbZYB5VbDVGBJ7tTSBHtacTUeiiqm7STV7yGCxe9tNEx3Zsp58bJEU0+Uwwwb91vxeX4twKDVaD2B5U0ZMDQ7aRb/lbQWJS4h1LgZLWLxfo4yfpC9LJjMX12mnpMr/1tsq7QY6kWOlfcCUQt1z2XUhr4qfL1S2MoBuk1AAs7z7T9/rsYwo79YOzSgWKrHZ8C+mSwAoqWg6axwVPff5MnhOEdXSreByyCeO/QNX3jf+VEwFysxhDCdM/oflMIOXprdkfOx")) },
                    EncryptAssertions = true,
                    SignAssertions = true,
                    ClaimsMapping = new Dictionary<string, string>{ {"email", "email" }, { "name", "name" }, { "sub", "sub" }, { "urn:oasis:names:tc:SAML:1.1:nameid-format:emailAddress", "sub" }, { "given_name", "given_name" }, { "family_name", "family_name" }  }

                },
                new ServiceProvider
                {
                    EntityId = "http://localhost:5002/saml-oaep",
                    AssertionConsumerServices =
                        {new Service(SamlConstants.BindingTypes.HttpPost, "http://localhost:5002/signin-saml-oaep")},
                    EncryptionCertificate = new X509Certificate2(Convert.FromBase64String("MIICtDCCAZygAwIBAgIIT1pFK6+584gwDQYJKoZIhvcNAQENBQAwGTEXMBUGA1UEAwwOSWRlbnRpdHlTZXJ2ZXIwIBcNMTkwOTA2MDAwMDAwWhgPMjIxOTA5MDYwMDAwMDBaMBkxFzAVBgNVBAMMDklkZW50aXR5U2VydmVyMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEArQ9hBxuFLbualoKNBrr4V7Ng6tXMw/2Mc6dzWt3nFQGxLAccsUUwfPQIbwGn+wA8yQQa6rgh5Zp1YwVDZRguP+cOlhg25rqGFdUrIvexJW7P3Gf4KWYQ9lBw46o0eZPOmML9i2BkrxzIWtM9iHzyTVDToqMkQGPSL2RQeSy0k3QQMQMk/1cv+Z1+mK8bgPO4tLTKyeTTyAec18y3qKZycWwk/h6b6PCtjRE80Et/yZqT8PNCXhNTrqzZx1X7J7UfzKCR8cNJ/ysEcMwtnZyBnA3Fv1LzujmiYcNPFl6OEiuiL0ghOIjW2hlw305hCs1TaTnUIBZz4jHYkZKyGr2DYwIDAQABMA0GCSqGSIb3DQEBDQUAA4IBAQAICu8lyJCzauuV1Yr/KRr2RfSd8s9oACxslLpyP++bP15R6rnppYas6yflQljH91xnv+z8cZYoq7ZEuTWhh3dh+ImaSbnkXag6VcnZxGTbz1B+CCp5vjAnq9REsjAzMnXOO6+idzGO8tTR8XMhfCCOzGdlGgnqs/wMOY32XlnGXDz3fgqBqqUtk6+QIhJ1Q8z4cpJ/Qp26YgkNy3X9I9Cti6Q1sqmUHK3bOzbwZ3HHh58vnmd66m3xdzk/09PJjY3lrGrrPrbHuM+rYWKOa/z6S9dPAc5tF6OBq9XvVNZdtyOSggC8DzSjzBANlPf7CUJLesnLdDSavOs8eEEetalb")),
                    SigningCertificates = new List<X509Certificate2>{new X509Certificate2(Convert.FromBase64String("MIICtDCCAZygAwIBAgIIGGGy3fqoj8wwDQYJKoZIhvcNAQENBQAwGTEXMBUGA1UEAwwOSWRlbnRpdHlTZXJ2ZXIwIBcNMTkwOTA2MDAwMDAwWhgPMjIxOTA5MDYwMDAwMDBaMBkxFzAVBgNVBAMMDklkZW50aXR5U2VydmVyMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAqazqX+dS0S+aiz3guDAlkYeHqSoikykwXmYl9J6IcTj5+kz2C61v0jAxv397IuM1Gb1gr32Ii5326iwM241XrNNRbhSFXKlyNNQ5/dg0aIWsHoY4H6GtkENCMxe7e9x3GrjZFbib6Lp+/uYkZj5wfUIc70VjP5GxtmtUXXKt8Qtun+uaOTU+qJ7h8/aYynoc18FdiU/wAJLzYRHeCgo9QzkDmdrVBTU1Zgvwu4va8QA8Arukri7UWvJUvQu8hBDGuf8bd8EogU3a6oDdfD51mmaViGRaI6+3/2+bXZGqm9Bwmi6M9fE5jEZfBJL4VY9yPaRwgzopEkEd8zFhH5X/4QIDAQABMA0GCSqGSIb3DQEBDQUAA4IBAQALC5qPKUue8LWrWa+2YDaeiyZfFIxfamUgQLkgieh9d6C7Pm738f4c2vtQVguS9v2A6VTVR2fFGWw7ZglGbZYB5VbDVGBJ7tTSBHtacTUeiiqm7STV7yGCxe9tNEx3Zsp58bJEU0+Uwwwb91vxeX4twKDVaD2B5U0ZMDQ7aRb/lbQWJS4h1LgZLWLxfo4yfpC9LJjMX12mnpMr/1tsq7QY6kWOlfcCUQt1z2XUhr4qfL1S2MoBuk1AAs7z7T9/rsYwo79YOzSgWKrHZ8C+mSwAoqWg6axwVPff5MnhOEdXSreByyCeO/QNX3jf+VEwFysxhDCdM/oflMIOXprdkfOx")) },
                    EncryptAssertions = true,
                    SignAssertions = true,
                    ClaimsMapping = new Dictionary<string, string>{ {"email", "email" }, { "name", "name" }, { "sub", "sub" }, { "urn:oasis:names:tc:SAML:1.1:nameid-format:emailAddress", "sub" }, { "given_name", "given_name" }, { "family_name", "family_name" }  }

                }
            };
        }
    }
}