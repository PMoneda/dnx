// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Security.Cryptography;
using System.Text;

namespace NuGet
{
    public static class EncryptionUtility
    {
        private static readonly byte[] _entropyBytes = Encoding.UTF8.GetBytes("NuGet");

        internal static string EncryptString(string value)
        {
            var decryptedByteArray = Encoding.UTF8.GetBytes(value);
            var encryptedByteArray = ProtectedData.Protect(decryptedByteArray, _entropyBytes, DataProtectionScope.CurrentUser);
            var encryptedString = Convert.ToBase64String(encryptedByteArray);
            return encryptedString;
        }

        internal static string DecryptString(string encryptedString)
        {
            var encryptedByteArray = Convert.FromBase64String(encryptedString);
            var decryptedByteArray = ProtectedData.Unprotect(encryptedByteArray, _entropyBytes, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(decryptedByteArray, 0, decryptedByteArray.Length);
        }

        public static string GenerateUniqueToken(string caseInsensitiveKey)
        {
            // SHA256 is case sensitive; given that our key is case insensitive, we upper case it
            var pathBytes = Encoding.UTF8.GetBytes(caseInsensitiveKey.ToUpperInvariant());
            var hashProvider = new CryptoHashProvider();

            return Convert.ToBase64String(hashProvider.CalculateHash(pathBytes)).ToUpperInvariant();
        }
    }
}
