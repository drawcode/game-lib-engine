using System;
using System.IO;
using System.IO.Compression;
using System.Text;
    
public static class Compress {
        
    /*
    public static string ToCompressed(this string val) {

        if (string.IsNullOrEmpty(val)) {
            return val;
        }

        if (!IsStringCompressed(val)) {
            return CompressString(val);
        }

        return val;
    }
        
    public static string ToDecompressed(this string val) {
        
        if (string.IsNullOrEmpty(val)) {
            return val;
        }

        if (IsStringCompressed(val) || val.IsBase64()) {
            return DecompressString(val);
        }

        return val;
    }
    */
        
    /*
    public static string CompressString(string text) {
        byte[] buffer = Encoding.UTF8.GetBytes(text);
        var memoryStream = new MemoryStream();
        using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true)) {
            gZipStream.Write(buffer, 0, buffer.Length);
        }
            
        memoryStream.Position = 0;
            
        var compressedData = new byte[memoryStream.Length];
        memoryStream.Read(compressedData, 0, compressedData.Length);
            
        var gZipBuffer = new byte[compressedData.Length + 4];
        Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
        Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
        return Convert.ToBase64String(gZipBuffer);
    }
        
    public static string DecompressString(string compressedText) {
        byte[] gZipBuffer = Convert.FromBase64String(compressedText);
        using (var memoryStream = new MemoryStream()) {
            int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
            memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);
                
            var buffer = new byte[dataLength];
                
            memoryStream.Position = 0;
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress)) {
                gZipStream.Read(buffer, 0, buffer.Length);
            }
                
            return Encoding.UTF8.GetString(buffer);
        }
    }
    */

    public static string CompressString(string s) {
        return Convert.ToBase64String(Zip(s));
    }
    
    public static string DecompressString(string s) {
        var bytes = Convert.FromBase64String(s);
        return Unzip(bytes);
    }

    public static void CopyTo(Stream src, Stream dest) {
        byte[] bytes = new byte[4096];
        
        int cnt;
        
        while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0) {
            dest.Write(bytes, 0, cnt);
        }
    }
    
    public static byte[] Zip(string str) {
        var bytes = Encoding.UTF8.GetBytes(str);
        
        using (var msi = new MemoryStream(bytes))
        using (var mso = new MemoryStream()) {
            using (var gs = new GZipStream(mso, CompressionMode.Compress)) {
                //msi.CopyTo(gs);
                CopyTo(msi, gs);
            }
            
            return mso.ToArray();
        }
    }
    
    public static string Unzip(byte[] bytes) {
        using (var msi = new MemoryStream(bytes))
        using (var mso = new MemoryStream()) {
            using (var gs = new GZipStream(msi, CompressionMode.Decompress)) {
                //gs.CopyTo(mso);
                CopyTo(gs, mso);
            }
            
            return Encoding.UTF8.GetString(mso.ToArray());
        }
    }
        
    public static bool IsStringCompressed(string data) {

        byte[] datas = Encoding.UTF8.GetBytes(data);

        if (data.IsBase64()) {
            datas = data.FromBase64Bytes();
        }

        data = null;

        return IsCompressed(datas);
    }

    public static bool IsCompressed(byte[] data) {
                
        if (IsCompressedGZip(data) 
            || IsCompressedPKZip(data)) {
            data = null;
            return true;
        }
        data = null;
        return false;
    }
    
    public static bool IsCompressedGZip(byte[] data) {
        return FileSystemUtil.CheckSignature(data, 3, "1F-8B-08");
    }
    
    public static bool IsCompressedPKZip(byte[] data) {
        return FileSystemUtil.CheckSignature(data, 4, "50-4B-03-04");
    }

    public static bool IsStringCompressedGZip(string data) {
        return FileSystemUtil.CheckSignatureString(data, 3, "1F-8B-08");
    }
        
    public static bool IsStringCompressedPKZip(string data) {
        return FileSystemUtil.CheckSignatureString(data, 4, "50-4B-03-04");
    }
}