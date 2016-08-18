# PartnerApiSoapExample

The SOAP service can be found here: https://partner-api-soap-proxy.azurewebsites.net/PartnerApiService.svc

## Authentication

The service requires two authentication headers to be specified. 

* ```X-DeveloperAgreement``` is the e-conomic developer agreement number.
* ```X-PartnerApiSecretToken``` is the authentication token.

## Signing and Encryption

If a public RSA key has been provided by the partner the authentication token has to be signed with the corresponding private key. This signature also has to be timestamped. In this case the header is expected on the format:

`<token>;<timestamp>;<lifetime>#<signature>`

The lifetime is in seconds and is optional. If no lifetime is specified, a default lifetime of 60 seconds is used.

The timestamp is expressed according to ISO 8601 and is in UTC.

This is done in the method ```EncodedHeader``` in ```Program.cs```
