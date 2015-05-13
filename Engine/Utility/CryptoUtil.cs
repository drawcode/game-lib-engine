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
        //return EncryptStringAES256CBC(plainText, _sharedSecret);
        //return EncryptStringAES(plainText, _sharedSecret);
        
        return EncryptAES256CBCNode(plainText, _sharedSecret);
    }
    
    public static string DecryptStringAES(string cipher) {
        return DecryptAES256CBCNode(cipher, _sharedSecret);
        //return DecryptStringAES256CBC(cipher, _sharedSecret);
        //return DecryptStringAES(cipher, _sharedSecret);
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
            //aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                        
            aesAlg.KeySize = 256;
            aesAlg.BlockSize = 128;
            
            aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
            aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);
            
            aesAlg.Mode = CipherMode.CBC;
                
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
                                
                aesAlg.KeySize = 256;
                aesAlg.BlockSize = 128;

                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);
                
                aesAlg.Mode = CipherMode.CBC;


                //aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                // Get the initialization vector from the encrypted stream
                //aesAlg.IV = ReadByteArray(msDecrypt);
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

    public static string DecryptAES256CBCNode(string cipherData, string keyString, string ivString) {
        byte[] key = Encoding.UTF8.GetBytes(keyString);
        byte[] iv = Encoding.UTF8.GetBytes(ivString);
        
        try {
            using (var rijndaelManaged =
                   new RijndaelManaged {Key = key, IV = iv, Mode = CipherMode.CBC})
            using (var memoryStream = 
                       new MemoryStream(Convert.FromBase64String(cipherData)))
            using (var cryptoStream =
                           new CryptoStream(memoryStream,
                                     rijndaelManaged.CreateDecryptor(key, iv),
                                     CryptoStreamMode.Read)) {
                return new StreamReader(cryptoStream).ReadToEnd();
            }
        }
        catch (CryptographicException e) {
            Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
            return null;
        }
        // You may want to catch more exceptions here...
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

    // AES CBC 256

    public static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes) {
        byte[] encryptedBytes = null;
        
        // Set your salt here, change it to meet your flavor:
        // The salt bytes must be at least 8 bytes.
        byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        
        using (MemoryStream ms = new MemoryStream()) {
            using (RijndaelManaged AES = new RijndaelManaged()) {
                AES.KeySize = 256;
                AES.BlockSize = 128;
                
                var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                AES.Key = key.GetBytes(AES.KeySize / 8);
                AES.IV = key.GetBytes(AES.BlockSize / 8);
                
                AES.Mode = CipherMode.CBC;
                
                using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write)) {
                    cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                    cs.Close();
                }
                encryptedBytes = ms.ToArray();
            }
        }
        
        return encryptedBytes;
    }

    public static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes) {
        byte[] decryptedBytes = null;
        
        // Set your salt here, change it to meet your flavor:
        // The salt bytes must be at least 8 bytes.
        byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        
        using (MemoryStream ms = new MemoryStream()) {
            using (RijndaelManaged AES = new RijndaelManaged()) {
                AES.KeySize = 256;
                AES.BlockSize = 128;
                
                var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                AES.Key = key.GetBytes(AES.KeySize / 8);
                AES.IV = key.GetBytes(AES.BlockSize / 8);
                
                AES.Mode = CipherMode.CBC;
                
                using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write)) {
                    cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                    cs.Close();
                }
                decryptedBytes = ms.ToArray();
            }
        }
        
        return decryptedBytes;
    }

    public static string EncryptStringAES256CBC(string input, string password) {
        // Get the bytes of the string
        byte[] bytesToBeEncrypted = Encoding.UTF8.GetBytes(input);
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
        
        // Hash the password with SHA256
        passwordBytes = SHA256.Create().ComputeHash(passwordBytes);
        
        byte[] bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, passwordBytes);
        
        string result = Convert.ToBase64String(bytesEncrypted);
        
        return result;
    }

    public static string DecryptStringAES256CBC(string input, string password) {
        // Get the bytes of the string
        byte[] bytesToBeDecrypted = Convert.FromBase64String(input);
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
        passwordBytes = SHA256.Create().ComputeHash(passwordBytes);
        
        byte[] bytesDecrypted = AES_Decrypt(bytesToBeDecrypted, passwordBytes);
        
        string result = Encoding.UTF8.GetString(bytesDecrypted);
        
        return result;
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

    // NODE COMPAT ENCRYPT/DECRYPT

    /*  Wanting to stay compatible with NodeJS
     *  http://stackoverflow.com/questions/18502375/aes256-encryption-decryption-in-both-nodejs-and-c-sharp-net/
     *  http://stackoverflow.com/questions/12261540/decrypting-aes256-encrypted-data-in-net-from-node-js-how-to-obtain-iv-and-key
     *  http://stackoverflow.com/questions/8008253/c-sharp-version-of-openssl-evp-bytestokey-method
     *  
     * var cipher = crypto.createCipher('aes-256-cbc', 'passphrase');
     * var encrypted = cipher.update("test", 'utf8', 'base64') + cipher.final('base64');
     * 
     * var decipher = crypto.createDecipher('aes-256-cbc', 'passphrase');
     * var plain = decipher.update(encrypted, 'base64', 'utf8') + decipher.final('utf8');
     */
    
    public static string EncryptAES256CBCNode(string input, string passphrase = null) {
        byte[] key, iv;
        DeriveKeyAndIV(RawBytesFromString(passphrase), null, 1, out key, out iv);
        
        return Convert.ToBase64String(EncryptStringToBytes(input, key, iv));
    }
    
    public static string DecryptAES256CBCNode(string inputBase64, string passphrase = null) {
        byte[] key, iv;
        DeriveKeyAndIV(RawBytesFromString(passphrase), null, 1, out key, out iv);
        
        return DecryptStringFromBytes(Convert.FromBase64String(inputBase64), key, iv);
    }
    
    private static byte[] RawBytesFromString(string input) {
        var ret = new List<Byte>();
        
        foreach (char x in input) {
            var c = (byte)((ulong)x & 0xFF);
            ret.Add(c);
        }
        
        return ret.ToArray();
    }
    
    private static void DeriveKeyAndIV(byte[] data, byte[] salt, int count, out byte[] key, out byte[] iv) {
        List<byte> hashList = new List<byte>();
        byte[] currentHash = new byte[0];
        
        int preHashLength = data.Length + ((salt != null) ? salt.Length : 0);
        byte[] preHash = new byte[preHashLength];
        
        System.Buffer.BlockCopy(data, 0, preHash, 0, data.Length);
        if (salt != null)
            System.Buffer.BlockCopy(salt, 0, preHash, data.Length, salt.Length);
        
        MD5 hash = MD5.Create();
        currentHash = hash.ComputeHash(preHash);
        
        for (int i = 1; i < count; i++) {
            currentHash = hash.ComputeHash(currentHash);
        }
        
        hashList.AddRange(currentHash);
        
        while (hashList.Count < 48) { // for 32-byte key and 16-byte iv
            preHashLength = currentHash.Length + data.Length + ((salt != null) ? salt.Length : 0);
            preHash = new byte[preHashLength];
            
            System.Buffer.BlockCopy(currentHash, 0, preHash, 0, currentHash.Length);
            System.Buffer.BlockCopy(data, 0, preHash, currentHash.Length, data.Length);
            if (salt != null)
                System.Buffer.BlockCopy(salt, 0, preHash, currentHash.Length + data.Length, salt.Length);
            
            currentHash = hash.ComputeHash(preHash);
            
            for (int i = 1; i < count; i++) {
                currentHash = hash.ComputeHash(currentHash);
            }
            
            hashList.AddRange(currentHash);
        }
        hash.Clear();
        key = new byte[32];
        iv = new byte[16];
        hashList.CopyTo(0, key, 0, 32);
        hashList.CopyTo(32, iv, 0, 16);
    }
    
    static byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV) {
        // Check arguments. 
        if (plainText == null || plainText.Length <= 0)
            throw new ArgumentNullException("plainText");
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException("Key");
        if (IV == null || IV.Length <= 0)
            throw new ArgumentNullException("Key");
        byte[] encrypted;
        // Create an RijndaelManaged object 
        // with the specified key and IV. 
        using (RijndaelManaged cipher = new RijndaelManaged()) {
            cipher.Key = Key;
            cipher.IV = IV;
            //cipher.Mode = CipherMode.CBC;
            //cipher.Padding = PaddingMode.PKCS7;
            
            // Create a decrytor to perform the stream transform.
            ICryptoTransform encryptor = cipher.CreateEncryptor(cipher.Key, cipher.IV);
            
            // Create the streams used for encryption. 
            using (MemoryStream msEncrypt = new MemoryStream()) {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)) {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt)) {
                        
                        //Write all data to the stream.
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
        }
        
        
        // Return the encrypted bytes from the memory stream. 
        return encrypted;
        
    }
    
    static string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV) {
        // Check arguments. 
        if (cipherText == null || cipherText.Length <= 0)
            throw new ArgumentNullException("cipherText");
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException("Key");
        if (IV == null || IV.Length <= 0)
            throw new ArgumentNullException("Key");
        
        // Declare the string used to hold 
        // the decrypted text. 
        string plaintext = null;
        
        // Create an RijndaelManaged object 
        // with the specified key and IV. 
        using (var cipher = new RijndaelManaged()) {
            cipher.Key = Key;
            cipher.IV = IV;
            //cipher.Mode = CipherMode.CBC;
            //cipher.Padding = PaddingMode.PKCS7;
            
            // Create a decrytor to perform the stream transform.
            ICryptoTransform decryptor = cipher.CreateDecryptor(cipher.Key, cipher.IV);
            
            // Create the streams used for decryption. 
            using (MemoryStream msDecrypt = new MemoryStream(cipherText)) {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)) {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt)) {
                        
                        // Read the decrypted bytes from the decrypting stream 
                        // and place them in a string.
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
            
        }
        
        return plaintext;
        
    }
}