using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;

public class CryptoUtil {
        
    private static byte[] _salt = Encoding.ASCII.GetBytes(AppConfigs.cryptoSharedSecret);
    private static string _sharedSecret = AppConfigs.cryptoSharedSecret;

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
        return EncryptStringAES(plainText, _sharedSecret);
    }
    
    public static string DecryptStringAES(string cipher) {
        return DecryptStringAES(cipher, _sharedSecret);
    }
        
    /// <summary>
    /// Encrypt the given string using AES.  The string can be decrypted using 
    /// DecryptStringAES().  The sharedSecret parameters must match.
    /// </summary>
    /// <param name="plainText">The text to encrypt.</param>
    /// <param name="sharedSecret">A password used to generate a key for encryption.</param>
    public static string EncryptStringAES(string plainText, string sharedSecret) {
        if (string.IsNullOrEmpty(plainText))
            throw new ArgumentNullException("plainText");
        if (string.IsNullOrEmpty(sharedSecret))
            throw new ArgumentNullException("sharedSecret");
            
        string outStr = plainText;                       // Encrypted string to return
        RijndaelManaged aesAlg = null;              // RijndaelManaged object used to encrypt the data.
            
        try {
            // generate the key from the shared secret and the salt
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _salt);
                
            // Create a RijndaelManaged object
            aesAlg = new RijndaelManaged();
            aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                
            // Create a decryptor to perform the stream transform.
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                
            // Create the streams used for encryption.
            using (MemoryStream msEncrypt = new MemoryStream()) {
                // prepend the IV
                msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
                msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)) {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt)) {
                        //Write all data to the stream.
                        swEncrypt.Write(plainText);
                    }
                }
                outStr = Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
        finally {
            // Clear the RijndaelManaged object.
            if (aesAlg != null)
                aesAlg.Clear();
        }
            
        // Return the encrypted bytes from the memory stream.
        return outStr;
    }
        
    /// <summary>
    /// Decrypt the given string.  Assumes the string was encrypted using 
    /// EncryptStringAES(), using an identical sharedSecret.
    /// </summary>
    /// <param name="cipherText">The text to decrypt.</param>
    /// <param name="sharedSecret">A password used to generate a key for decryption.</param>
    public static string DecryptStringAES(string cipherText, string sharedSecret) {
        
        if (!cipherText.IsBase64()) {
            return cipherText;
        }

        if (string.IsNullOrEmpty(cipherText))
            throw new ArgumentNullException("cipherText");
        if (string.IsNullOrEmpty(sharedSecret))
            throw new ArgumentNullException("sharedSecret");
            
        // Declare the RijndaelManaged object
        // used to decrypt the data.
        RijndaelManaged aesAlg = null;
            
        // Declare the string used to hold
        // the decrypted text.
        string plaintext = cipherText;
            
        try {
            // generate the key from the shared secret and the salt
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _salt);

            // Create the streams used for decryption.                
            byte[] bytes = Convert.FromBase64String(cipherText);

            using (MemoryStream msDecrypt = new MemoryStream(bytes)) {
                // Create a RijndaelManaged object
                // with the specified key and IV.
                aesAlg = new RijndaelManaged();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                // Get the initialization vector from the encrypted stream
                aesAlg.IV = ReadByteArray(msDecrypt);
                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)) {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                        plaintext = srDecrypt.ReadToEnd();
                }
            }
        }
        finally {
            // Clear the RijndaelManaged object.
            if (aesAlg != null)
                aesAlg.Clear();
        }
            
        return plaintext;
    }
        
    private static byte[] ReadByteArray(Stream s) {
        byte[] rawLength = new byte[sizeof(int)];
        if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length) {
            throw new SystemException("Stream did not contain properly formatted byte array");
        }
            
        byte[] buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
        if (s.Read(buffer, 0, buffer.Length) != buffer.Length) {
            throw new SystemException("Did not read byte array properly");
        }
            
        return buffer;
    }

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