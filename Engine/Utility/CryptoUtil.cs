using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;

public class CryptoUtil {

    const string defaultSharedSecret = "sharedsecrethere";

    // Per-app key from AppConfigs (was gated on a misspelled USE_CONFIGS_APP that no app defined,
    // so every app silently used the default). An app that leaves it empty keeps the default.
    // Changing an app's key makes data it already encrypted unreadable - no shipped game uses
    // encrypted storage as of 2026-10, so this was safe to switch.
#if USE_CONFIG_APP
    private static string _sharedSecretSource =
        string.IsNullOrEmpty(AppConfigs.cryptoSharedSecret) ? defaultSharedSecret : AppConfigs.cryptoSharedSecret;
#else
    private static string _sharedSecretSource = defaultSharedSecret;
#endif

    private static byte[] _salt = Encoding.ASCII.GetBytes(_sharedSecretSource);
    private static string _sharedSecret = _sharedSecretSource;

    public static string NewGuid() {
        string puid = "";
        for (int c = 0; c < 40; ++c)
            puid += String.Format("{0:x}", UnityEngine.Random.Range(0, 16));
        return puid;
    }

    public static string CalculateSHA1(string text, Encoding enc) {
        byte[] buf = enc.GetBytes(text);
        SHA1CryptoServiceProvider sha1 =
            new SHA1CryptoServiceProvider();
        return enc.GetString(sha1.ComputeHash(buf));
    }

    public static string CalculateBase64SHA1(string text, Encoding enc) {
        byte[] buf = enc.GetBytes(text);
        SHA1CryptoServiceProvider sha1 =
            new SHA1CryptoServiceProvider();
        return Convert.ToBase64String(sha1.ComputeHash(buf));
    }

    public static string CalculateBase64SHA1Trim(string text, Encoding enc) {
        return CalculateBase64SHA1(text, enc).Replace("=", "");
    }

    public static string CalculateSHA1ASCII(string text) {
        ASCIIEncoding enc = new ASCIIEncoding();
        return CalculateSHA1(text, enc);
    }

    public static string CalculateBase64SHA1ASCII(string text) {
        ASCIIEncoding enc = new ASCIIEncoding();
        return CalculateBase64SHA1(text, enc);
    }

    public static string CalculateBase64SHA1TrimASCII(string text) {
        ASCIIEncoding enc = new ASCIIEncoding();
        return CalculateBase64SHA1Trim(text, enc);
    }

    public static string StringToBase64(string value) {
        byte[] buf = System.Text.ASCIIEncoding.ASCII.GetBytes(value);
        return Convert.ToBase64String(buf);
    }

    public static string CalculateMD5HashFromFile(string fileName) {

        // Faster not secure needed hashing of files for coy process
        if (File.Exists(fileName)) {
            FileStream file = new FileStream(fileName, FileMode.Open);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(file);
            file.Close();

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < retVal.Length; i++) {
                sb.Append(retVal[i].ToString("x2"));
            }

            return sb.ToString();
        }
        else {
            return "";
        }
    }

    public static string EncryptStringAES(string plainText) {
        //return EncryptStringAES256CBC(plainText, _sharedSecret);
        //return EncryptStringAES(plainText, _sharedSecret);

        return EncryptAES256CBCNode(plainText, _sharedSecret);
    }

    public static string DecryptStringAES(string cipher) {
        return DecryptAES256CBCNode(cipher, _sharedSecret);
        //return DecryptStringAES256CBC(cipher, _sharedSecret);
        //return DecryptStringAES(cipher, _sharedSecret);
    }

#if USE_ENGINE_CRYPTO
    // AES lives in the opt-in Engine.Crypto assembly (Engine/Crypto/CryptoAes.cs). These keep the public
    // surface for apps that define USE_ENGINE_CRYPTO; without it they throw, and no cipher ships.

    public static string EncryptStringAES(string plainText, string sharedSecret) {
        return Engine.Crypto.CryptoAes.EncryptStringAES(plainText, sharedSecret, _salt);
    }

    public static string DecryptStringAES(string cipherText, string sharedSecret) {

        if (!cipherText.IsBase64()) {
            return cipherText;
        }

        return Engine.Crypto.CryptoAes.DecryptStringAES(cipherText, sharedSecret, _salt);
    }

    public static string DecryptAES256CBCNode(string cipherData, string keyString, string ivString) {
        return Engine.Crypto.CryptoAes.DecryptAES256CBCNode(cipherData, keyString, ivString);
    }

    public static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes) {
        return Engine.Crypto.CryptoAes.AES_Encrypt(bytesToBeEncrypted, passwordBytes);
    }

    public static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes) {
        return Engine.Crypto.CryptoAes.AES_Decrypt(bytesToBeDecrypted, passwordBytes);
    }

    public static string EncryptStringAES256CBC(string input, string password) {
        return Engine.Crypto.CryptoAes.EncryptStringAES256CBC(input, password);
    }

    public static string DecryptStringAES256CBC(string input, string password) {
        return Engine.Crypto.CryptoAes.DecryptStringAES256CBC(input, password);
    }

    public static string EncryptAES256CBCNode(string input, string passphrase = null) {
        return Engine.Crypto.CryptoAes.EncryptAES256CBCNode(input, passphrase);
    }

    public static string DecryptAES256CBCNode(string inputBase64, string passphrase = null) {
        return Engine.Crypto.CryptoAes.DecryptAES256CBCNode(inputBase64, passphrase);
    }
#else
    // Built without USE_ENGINE_CRYPTO: no AES implementation is compiled in. Encrypted storage
    // (AppConfigs/ProfileConfigs.useStorageEncryption) needs the define.

    static NotSupportedException NoCrypto() {
        return new NotSupportedException("AES is in the opt-in Engine.Crypto assembly; add USE_ENGINE_CRYPTO to the Scripting Define Symbols");
    }

    public static string EncryptStringAES(string plainText, string sharedSecret) { throw NoCrypto(); }

    public static string DecryptStringAES(string cipherText, string sharedSecret) {

        if (!cipherText.IsBase64()) {
            return cipherText;
        }

        throw NoCrypto();
    }

    public static string DecryptAES256CBCNode(string cipherData, string keyString, string ivString) { throw NoCrypto(); }

    public static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes) { throw NoCrypto(); }

    public static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes) { throw NoCrypto(); }

    public static string EncryptStringAES256CBC(string input, string password) { throw NoCrypto(); }

    public static string DecryptStringAES256CBC(string input, string password) { throw NoCrypto(); }

    public static string EncryptAES256CBCNode(string input, string passphrase = null) { throw NoCrypto(); }

    public static string DecryptAES256CBCNode(string inputBase64, string passphrase = null) { throw NoCrypto(); }
#endif

    //


    public static string GenerateRandomString(int length) {

        if (length < 8)
            length = 8;

        byte[] result = new byte[length];

        for (int index = 0; index < length; index++) {
            result[index] = (byte)new Random().Next(33, 126);
        }

        return System.Text.Encoding.ASCII.GetString(result);
    }

    // HASHING, only use MD5 for basic file or content compare, nothing secure (use minimum SHA-1 for that)

    // Used primarily for comparing last network user profile data against current to see if update needed

    public static bool VerifyMD5(string input, string hash) {
        using (MD5 md5Hash = MD5.Create()) {
            if (HashVerify(md5Hash, input, hash)) {
                return true;
            }
            else {
                return false;
            }
        }
    }

    public static string HashMD5(string input) {
        using (MD5 md5Hash = MD5.Create()) {
            string hash = Hash(md5Hash, input);
            return hash;
        }
    }

    static string Hash(MD5 md5Hash, string input) {

        byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

        StringBuilder sb = new StringBuilder();

        // Loop through each byte of the hashed data  
        // and format each one as a hexadecimal string. 
        for (int i = 0; i < data.Length; i++) {
            sb.Append(data[i].ToString("x2"));
        }

        return sb.ToString();
    }

    static bool HashVerify(MD5 md5Hash, string input, string hash) {
        string hashOfInput = Hash(md5Hash, input);
        StringComparer comparer = StringComparer.OrdinalIgnoreCase;

        if (0 == comparer.Compare(hashOfInput, hash)) {
            return true;
        }
        else {
            return false;
        }
    }

}
