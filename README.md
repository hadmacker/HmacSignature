# HMAC SHA256 Signature Implementation in .NET

## Overview

Given a shared secret key, creates a one-way verifiable HMAC SHA 256 hash.

## Quick Start

* Jump right into a working sender/receiver combined implementation here: QuickStart_BuildSignature.cs
  * Includes example E2E signature calculation + verification flow for both Base64 and Hex outputs.
* Library contains two important classes:
  * HMACSHA256SignatureCalculator
    * Creates an HMAC SHA256 hash based on a string input and key.
    * Implementation based on [HMACSHA256 Class](https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.hmacsha256?view=net-5.0)
    * Also provides an ISignatureCalculator interface for those seeking more control with Dependency Injection.
  * InstanceSignatureBuilder
    * Constructor accepts an ISignatureCalculator.
    * Computes a signature using `InstanceSignatureBuilder.Compute(object, key)`
    * Verifies a signature using `InstanceSignatureBuilder.Verify(object, apparent signature, key)`
    * Takes an object and shared secret key input and uses a set of rules to generate a signature using ISignatureCalculator.

## Rules for HMACSHA256SignatureCalculator

* This is an implementation of [HMACSHA256 Class](https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.hmacsha256?view=net-5.0)
* HMACSHA256 accepts keys of any size, and produces a hash sequence 256 bits in length
* Independent Online HMAC tester: https://www.devglan.com/online-tools/hmac-sha256-online
* `ISignatureCalculator.Calculate(string input, string key)` returns a `SignatureCalculation` response that can convert signature byte[] to _Hex_ or _Base64_ encoded string.
* SignatureCalculation also identifies the `Payload` string that was used to generate the signature. Key is not returned.
* Key used in HMAC calculation is a shared secret between parties. Please treat it as sensitive.

## Rules for InstanceSignatureBuilder

* Uses Reflection to identify properties to be included in signature calculation.
* All of the following criteria must be true for a property to be included in the signature calculation:
  * Propert must have a `get` accessor.
  * Property must be one of the following data types: string, bool, int, short, long, decimal, float, double, DateTime
  * Property must not have a `[Signature(Excluded = true)]` attribute associated to it.
* Some data types use specifically formatted values:
  * Bool uses values of "false" and "true".
  * DateTime uses output format `"O"` for ISO-8601 compatibility and `" "` is removed. [The round-trip ("O", "o") format specifier](https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings#the-round-trip-o-o-format-specifier)
  * Strings have `" "` removed.
  * Number types use `CultureInfo.InvariantCulture`. This impacts numeric values that include comma's and decimals.
* Properties are identified only using their `PropertyInfo.Name` and are not automatically renamed based on any data annotations.
* Properties can be removed from signature calculation by excluding them using the `[Signature(Excluded = true)]` attribute on any property. This attribute should be configured on the output signature property if present on the object being signed.

### Quote Attribution for Test Data

* All quotes are attributed to ["Epigrams in Programming"](http://www.cs.yale.edu/homes/perlis-alan/quotes.html) by Alan Perlis. Used without permission.

## Further Reference

The links below were helpful in building this library.

* https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.hmacsha256?view=net-5.0
* https://docs.github.com/en/developers/webhooks-and-events/securing-your-webhooks
* http://michaco.net/blog/HowToValidateGitHubWebhooksInCSharpWithASPNETCoreMVC
* https://gist.github.com/duncansmart/3169752