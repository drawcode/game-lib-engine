//#define ZIP_USE_SYSTEM
//#define ZIP_USE_IONIC
#define ZIP_USE_SHARPZIPLIB

using System;
using System.IO;
#if ZIP_USE_SYSTEM
//using System.IO.Compression;
#elif ZIP_USE_IONIC
using Ionic.Zlib;
#elif ZIP_USE_SHARPZIPLIB
using ICSharpCode.SharpZipLib.GZip;
#endif
using System.Text;

#if !ZIP_USE_SHARPZIPLIB

#endif
    
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

        if(src == null) {
            return;
        }

        if(!src.CanRead) {
            return;
        }
        
        while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0) {
            dest.Write(bytes, 0, cnt);
        }
    }

#if ZIP_USE_SHARPZIPLIB

    /*
    public static string ZipString(string sBuffer)
    {
        MemoryStream mso = null;
        GZipOutputStream gzipOutout = null;
        string result;
        try
        {
            mso = new MemoryStream();
            //Int32 size = sBuffer.Length;
            // Prepend the compressed data with the length of the uncompressed data (firs 4 bytes)
            //
            using (BinaryWriter writer = new BinaryWriter(mso, System.Text.Encoding.ASCII))
            {
                //writer.Write( size );
                
                gzipOutout = new GZipOutputStream(mso);
                gzipOutout.Write(Encoding.ASCII.GetBytes(sBuffer), 0, sBuffer.Length);
                
                gzipOutout.Close();
                result = Convert.ToBase64String(mso.ToArray());
                gzipOutout.Close();
                
                writer.Close();
            }
        }
        finally
        {
            if (gzipOutout != null)
            {
                gzipOutout.Dispose();
            }
            if (mso != null)
            {
                mso.Dispose();
            }
        }
        return result;
    }
    
    public static string UnzipString(string compbytes)
    {
        string result;
        MemoryStream msi = null;
        GZipInputStream gzipInput = null;
        try
        {
            msi = new MemoryStream(Convert.FromBase64String(compbytes));
            // read final uncompressed string size stored in first 4 bytes
            //
            using (BinaryReader reader = new BinaryReader(msi, System.Text.Encoding.ASCII))
            {
               // Int32 size = reader.ReadInt32();
                
                gzipInput = new BZip2InputStream(msi);
                byte[] bytesUncompressed = new byte[size];
                gzipInput.Read(bytesUncompressed, 0, bytesUncompressed.Length);
                gzipInput.Close();
                gzipInput.Close();
                
                result = Encoding.ASCII.GetString(bytesUncompressed);
                
                reader.Close();
            }
        }
        finally
        {
            if (m_isBZip2 != null)
            {
                m_isBZip2.Dispose();
            }
            if (m_msBZip2 != null)
            {
                m_msBZip2.Dispose();
            }
        }
        return result;
    }
    */
    
    public static byte[] Zip(string str) {
        var bytes = Encoding.UTF8.GetBytes(str);
        
        using (var msi = new MemoryStream(bytes))
        using (var mso = new MemoryStream()) {
            using (var gs = new GZipOutputStream(mso)) {
                //msi.CopyTo(gs);
                CopyTo(msi, gs);
            }
            
            return mso.ToArray();
        }
    }
    
    public static string Unzip(byte[] bytes) {
        using (var msi = new MemoryStream(bytes))
        using (var mso = new MemoryStream()) {
            using (var gs = new GZipInputStream(msi)) {
                //gs.CopyTo(mso);
                CopyTo(gs, mso);
            }
            
            return Encoding.UTF8.GetString(mso.ToArray());
        }
    }

#else
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

#endif
        
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