using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;

public class CryptoUtil {

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
}