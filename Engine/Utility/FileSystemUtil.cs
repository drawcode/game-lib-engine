using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class FileSystemUtil {

    public static void CreateDirectoryIfNeededAndAllowed(string path) {

#if !UNITY_WEBPLAYER
        
        Debug.Log("FileSystemUtil::CreateDirectoryIfNeededAndAllowed:path:" + path);

        if(!Directory.Exists(path)) {

            Debug.Log("FileSystemUtil::CreateDirectoryIfNeededAndAllowed:pathnotexists:" + path);

            if(DirectoryAllowed(path)) {

                Debug.Log("CreateDirectoryIfNeededAndAllowed:" + path);

                path = path.TrimEnd('/');

                Debug.Log("CreateDirectoryIfNeededAndAllowed:trimmed:path:" + path);

                //Directory.CreateDirectory(path);

                DirectoryInfo dir = new DirectoryInfo(path);

                if(!dir.Exists) {
                    dir.Create();
                    Debug.Log("CreateDirectoryIfNeededAndAllowed:info:path:" + path);
                }
            }
        }
#endif
    }

    public static bool DirectoryAllowed(string path) {
        bool allowCreate = true;

#if !UNITY_WEBPLAYER
        if(path.IndexOf(Application.persistentDataPath) == -1
            && !Application.isEditor) {
            allowCreate = false;
        }
#endif
        return allowCreate;
    }

    public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs, bool versioned) {

#if !UNITY_WEBPLAYER
        FileSystemUtil.EnsureDirectory(sourceDirName, false);
        FileSystemUtil.EnsureDirectory(destDirName, false);

        CreateDirectoryIfNeededAndAllowed(sourceDirName);

        DirectoryInfo dir = new DirectoryInfo(sourceDirName);
        DirectoryInfo[] dirs = dir.GetDirectories();

        if(!dir.Exists) {
            throw new DirectoryNotFoundException(
                "Source directory does not exist or could not be found: "
                + sourceDirName);
        }

        CreateDirectoryIfNeededAndAllowed(destDirName);

        FileInfo[] files = dir.GetFiles();

        LogUtil.Log("Directory Files: directory: " + destDirName);
        LogUtil.Log("files.Count:", files.Count());

        //int curr = 0;

        foreach(FileInfo file in files) {

            if(file.Extension != ".meta"
                && file.Extension != ".DS_Store") {

                string temppath = PathUtil.Combine(destDirName, file.Name);


                if(!CheckFileExists(temppath) || Application.isEditor) {

                    LogUtil.Log("copying ship file: " + file.FullName);
                    LogUtil.Log("copying ship file to cache: " + temppath);

                    file.CopyTo(temppath, true);
                    ////SystemHelper.SetNoBackupFlag(temppath);
                }
            }
        }

        if(copySubDirs) {

            foreach(DirectoryInfo subdir in dirs) {

                string temppath = PathUtil.Combine(destDirName, subdir.Name);
                LogUtil.Log("Copying Directory: " + temppath);
                DirectoryCopy(subdir.FullName, temppath, copySubDirs, versioned);
            }
        }
#endif      
    }

    public static void EnsureDirectory(string filePath) {
        EnsureDirectory(filePath, true);
    }

    public static void EnsureDirectory(string filePath, bool filterFileName) {

        //LogUtil.Log("filePath:" + filePath);

        Debug.Log("FileSystemUtil::EnsureDirectory:filePath:" + filePath);

        string directory = filePath;

        if(filePath.IndexOf('.') > -1 && filterFileName) {

            directory = filePath.Replace(Path.GetFileName(filePath), "");

            Debug.Log("FileSystemUtil::EnsureDirectory:directory:" + directory);

        }
        
        Debug.Log("FileSystemUtil::EnsureDirectory:directory:" + directory);

        //LogUtil.Log("directory:" + directory);

        CreateDirectoryIfNeededAndAllowed(directory);
    }

    public static string GetFileLocalPath(string path) {
        if(!path.Contains("file://")) {

            if(!path.StartsWith("/")) {
                path = "/" + path;
            }

            path = "file://" + path;
        }
        return path;
    }

    public static bool CheckFileExists(string path) {

        bool exists = false;

        Debug.Log("CheckFileExists: path:" + path);
        Debug.Log("CheckFileExists: Application.streamingAssetsPath:" + Application.streamingAssetsPath);
        Debug.Log("CheckFileExists: path.Contains(Application.streamingAssetsPath):" + path.Contains(Application.streamingAssetsPath));

#if UNITY_ANDROID
        if(!exists) {// && path.Contains(Application.streamingAssetsPath)) {
                     // android stores streamingassets in a compressed file, 
                     // must use WWW to check if you can access it

            path = GetFileLocalPath(path);


            UnityWebRequest www = new UnityWebRequest();
            www.url = path;

            UnityWebRequestAsyncOperation asyncOp = www.SendWebRequest();

            //WWW file = new WWW(dataFilePath);

            float currentTime = Time.time;
            float endTime = currentTime + 6f; // only allow some seconds for file check

            while (!asyncOp.isDone && currentTime < endTime) {
                currentTime = Time.time;
            };

            if (www.isNetworkError || www.isHttpError) {
                Debug.LogWarning($"Network error whilst downloading [{path}] Error: [{www.error}]");
                //Debug.Log(www.error);
            }
            else {
                //int length = file.bytes.Length;
                int length = www.downloadHandler.data.Length;

                Debug.Log("CheckFileExists: Android: path:" + path);
                Debug.Log("CheckFileExists: Android: file.bytes.length:" + length);

                if(www.downloadHandler.data.Length > 0) {
                    exists = true;
                }
            }
        }

        if(!exists) {
            exists = File.Exists(path);
        }

#elif UNITY_WEBPLAYER
        if(SystemPrefUtil.HasLocalSetting(path)) {
            exists = true;
        }
#else
        exists = File.Exists(path);
#endif

        return exists;
    }

    public static void CopyFile(string dataFilePath, string persistenceFilePath) {
        CopyFile(dataFilePath, persistenceFilePath, false);
    }

    public static void CopyFile(string dataFilePath, string persistenceFilePath, bool force) {

#if !UNITY_WEBPLAYER

        EnsureDirectory(dataFilePath);
        EnsureDirectory(persistenceFilePath);

        Debug.Log("dataFilePath: " + dataFilePath);
        Debug.Log("persistenceFilePath: " + persistenceFilePath);
        Debug.Log("Application.streamingAssetsPath: " + Application.streamingAssetsPath);

        Debug.Log("CheckFileExists(dataFilePath): " + CheckFileExists(dataFilePath));
        Debug.Log("!CheckFileExists(persistenceFilePath): " + !CheckFileExists(persistenceFilePath));
        Debug.Log("force: " + force);

        if(CheckFileExists(dataFilePath) && (!CheckFileExists(persistenceFilePath) || force)) {

#if UNITY_ANDROID
            if(dataFilePath.Contains(Application.streamingAssetsPath)) {
                // android stores streamingassets in a compressed file, 
                // must use WWW to copy contents if you can access it

                dataFilePath = GetFileLocalPath(dataFilePath);

                //using (UnityWebRequest www = UnityWebRequest.Get(dataFilePath)) {
                //    UnityWebRequestAsyncOperation asyncOp = www.SendWebRequest();
                //    while (asyncOp.isDone == false) {
                //        await Task.Delay(30);
                //    }
                //    if (www.isNetworkError || www.isHttpError) {
                //        Debug.LogWarning($"Network error whilst downloading [{url}] Error: [{www.error}]");
                //        return null;
                //    }
                //}

                UnityWebRequest www = new UnityWebRequest();
                www.url = dataFilePath;

                UnityWebRequestAsyncOperation asyncOp = www.SendWebRequest();

                //WWW file = new WWW(dataFilePath);

                float currentTime = Time.time;
                float endTime = currentTime + 6f; // only allow some seconds for file check

                while(!asyncOp.isDone && currentTime < endTime) {
                    currentTime = Time.time;
                };

                if (www.isNetworkError || www.isHttpError) {
                    Debug.LogWarning($"Network error whilst downloading [{dataFilePath}] Error: [{www.error}]");
                    //Debug.Log(www.error);
                }
                else {
                    //int length = file.bytes.Length;
                    int length = www.downloadHandler.data.Length;

                    Debug.Log("CopyFile: Android: dataFilePath:" + dataFilePath);
                    Debug.Log("CopyFile: Android: persistenceFilePath:" + persistenceFilePath);
                    Debug.Log("CopyFile: Android: file.bytes.length:" + length);

                    //if(file.bytes.Length > 0) {
                    //    // Save file contents to new location                   
                    //    FileSystemUtil.WriteAllBytes(persistenceFilePath, file.bytes);
                    //}

                    if (www.downloadHandler.data.Length > 0) {
                        // Save file contents to new location                   
                        FileSystemUtil.WriteAllBytes(persistenceFilePath, www.downloadHandler.data);
                    }
                }
            }
            else {
                File.Copy(dataFilePath, persistenceFilePath, true);
            }
#else
            File.Copy(dataFilePath, persistenceFilePath, true);
#endif  
            ////SystemHelper.SetNoBackupFlag(persistenceFilePath);
        }
#endif
    }



    public static List<string> GetFilesLikeRecursive(
        string dirInfoCurrent
        ) {

        string filter = "*";
        List<string> excludeExts = new List<string>();
        excludeExts.Add(".DS_Store");
        excludeExts.Add(".meta");

        return GetFilesLikeRecursive(dirInfoCurrent, filter, excludeExts);
    }

    public static List<string> GetFilesLikeRecursive(
        string dirInfoCurrent,
        string filter,
        List<string> excludeExts) {

        List<string> files = new List<string>();

#if !UNITY_WEBPLAYER
        if(Directory.Exists(dirInfoCurrent)) {
            DirectoryInfo info = new DirectoryInfo(dirInfoCurrent);

            files = GetFilesLikeRecursive(info, filter, excludeExts);
        }
#endif

        return files;
    }

    public static List<string> GetFilesLikeRecursive(
        DirectoryInfo dirInfoCurrent,
        string filter,
        List<string> excludeExts) {

        List<string> files = new List<string>();

        return GetFilesLikeRecursive(dirInfoCurrent, files, filter, excludeExts);
    }

    public static List<string> GetFilesLikeRecursive(
        DirectoryInfo dirInfoCurrent,
        List<string> files,
        string filter,
        List<string> excludeExts) {

#if !UNITY_WEBPLAYER
        foreach(FileInfo fileInfo in dirInfoCurrent.GetFiles()) {
            string fileTo = fileInfo.FullName;
            if(fileTo.Contains(filter)
                || filter == "*") {
                if(!CheckFileExtention(fileTo, excludeExts)) {
                    if(!files.Contains(fileTo)) {
                        files.Add(fileTo);
                    }
                }
            }
        }

        foreach(DirectoryInfo dirInfoItem in dirInfoCurrent.GetDirectories()) {
            files = GetFilesLikeRecursive(dirInfoItem, files, filter, excludeExts);

        }
#endif

        return files;
    }

    public static void MoveFile(string dataFilePath, string persistenceFilePath) {
        MoveFile(dataFilePath, persistenceFilePath, false);
    }

    public static void MoveFile(string dataFilePath, string persistenceFilePath, bool force) {

#if !UNITY_WEBPLAYER
        EnsureDirectory(dataFilePath);
        EnsureDirectory(persistenceFilePath);
        //LogUtil.Log("dataFilePath: " + dataFilePath);
        //LogUtil.Log("persistenceFilePath: " + persistenceFilePath);
        if(CheckFileExists(dataFilePath) && (!CheckFileExists(persistenceFilePath) || force)) {

            //LogUtil.Log("fileMoved: " + persistenceFilePath);
#if UNITY_ANDROID       
            if(dataFilePath.Contains(Application.streamingAssetsPath)) {
                // android stores streamingassets in a compressed file, 
                // must use WWW to copy contents if you can access it

                dataFilePath = GetFileLocalPath(dataFilePath);

                UnityWebRequest www = new UnityWebRequest();
                www.url = dataFilePath;

                UnityWebRequestAsyncOperation asyncOp = www.SendWebRequest();

                //WWW file = new WWW(dataFilePath);

                float currentTime = Time.time;
                float endTime = currentTime + 6f; // only allow some seconds for file check

                while (!asyncOp.isDone && currentTime < endTime) {
                    currentTime = Time.time;
                };

                if (www.isNetworkError || www.isHttpError) {
                    Debug.LogWarning($"Network error whilst downloading [{dataFilePath}] Error: [{www.error}]");
                    //Debug.Log(www.error);
                }
                else {

                    int length = www.downloadHandler.data.Length;

                    LogUtil.Log("CopyFile: Android: dataFilePath:" + dataFilePath);
                    LogUtil.Log("CopyFile: Android: persistenceFilePath:" + persistenceFilePath);
                    LogUtil.Log("CopyFile: Android: file.bytes.length:" + length);

                    if (www.downloadHandler.data.Length > 0) {
                        // Save file contents to new location                   
                        FileSystemUtil.WriteAllBytes(persistenceFilePath, www.downloadHandler.data);
                    }
                }
            }
            else {
                File.Move(dataFilePath, persistenceFilePath);
            }
#else
            File.Move(dataFilePath, persistenceFilePath);
#endif  

            //SystemHelper.SetNoBackupFlag(persistenceFilePath);
        }
#endif
    }

    public static byte[] ReadAllBytes(string fileName) {

#if !UNITY_WEBPLAYER        
        return File.ReadAllBytes(fileName);
#else
        return System.Text.Encoding.UTF8.GetBytes(SystemPrefUtil.GetLocalSettingString(fileName));
#endif
    }

    public static void WriteAllBytes(string fileName, byte[] buffer) {

#if !UNITY_WEBPLAYER                
        EnsureDirectory(fileName);
        File.WriteAllBytes(fileName, buffer);
        ////SystemHelper.SetNoBackupFlag(fileName);
#else
        SystemPrefUtil.SetLocalSettingString(fileName, System.Text.Encoding.UTF8.GetString(buffer));        
        SystemPrefUtil.Save();
#endif
    }

    public static byte[] ReadStream(string fileName) {
#if !UNITY_WEBPLAYER

        byte[] buffer = null;
        if(CheckFileExists(fileName)) {
            FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            long length = new FileInfo(fileName).Length;
            buffer = br.ReadBytes((int)length);
            br.Close();
            fs.Close();
        }
        return buffer;
#else
        return System.Text.Encoding.UTF8.GetBytes(SystemPrefUtil.GetLocalSettingString(fileName));
#endif      
    }

    public static void WriteStream(string fileName, byte[] data) {
#if !UNITY_WEBPLAYER            
        EnsureDirectory(fileName);
        StreamWriter sw = new StreamWriter(fileName, false, Encoding.ASCII);
        sw.Write(data);
        sw.Flush();
        sw.Close();
        ////SystemHelper.SetNoBackupFlag(fileName);
#else
        SystemPrefUtil.SetLocalSettingString(fileName, System.Text.Encoding.UTF8.GetString(data));
        SystemPrefUtil.Save();
#endif
    }

    public static string ReadString(string fileName) {
        string contents = "";
        if(CheckFileExists(fileName)) {
#if UNITY_WEBPLAYER
        contents = SystemPrefUtil.GetLocalSettingString(fileName);
#else
            StreamReader sr = new StreamReader(fileName, true);
            contents = sr.ReadToEnd();
            sr.Close();
#endif
        }
        return contents;
    }

    public static void WriteString(string fileName, string data) {
        WriteString(fileName, data, false);
    }

    public static void WriteString(string fileName, string data, bool append) {
#if UNITY_WEBPLAYER
        SystemPrefUtil.SetLocalSettingString(fileName, data);
        SystemPrefUtil.Save();
#else
        Debug.Log("FileSystemUtil::WriteString:EnsureDirectory:fileName:" + fileName);

        EnsureDirectory(fileName);

        StreamWriter sw = new StreamWriter(fileName, append);
        sw.Write(data);
        sw.Flush();
        sw.Close();
        ////SystemHelper.SetNoBackupFlag(fileName);
#endif
    }

    public static void RemoveFile(string file) {
        if(CheckFileExists(file)) {
#if UNITY_WEBPLAYER
        SystemPrefUtil.SetLocalSettingString(file, "");
        SystemPrefUtil.Save();
#else           
            File.Delete(file);
#endif
        }
    }

    public static void RemoveFilesLikeRecursive(DirectoryInfo dirInfo, string fileKey) {

#if !UNITY_WEBPLAYER
        foreach(FileInfo fileInfo in dirInfo.GetFiles()) {
            if(fileInfo.FullName.Contains(fileKey)) {
                File.Delete(fileInfo.FullName);
            }
        }

        foreach(DirectoryInfo dirInfoItem in dirInfo.GetDirectories()) {
            RemoveFilesLikeRecursive(dirInfoItem, fileKey);
        }
#endif
    }

    public static void CopyFilesLikeRecursive(
        DirectoryInfo dirInfoCurrent,
        DirectoryInfo dirInfoFrom,
        DirectoryInfo dirInfoTo,
        string filter,
        List<string> excludeExts) {

#if !UNITY_WEBPLAYER
        foreach(FileInfo fileInfo in dirInfoCurrent.GetFiles()) {
            if(fileInfo.FullName.Contains(filter)) {
                string fileTo = fileInfo.FullName.Replace(dirInfoFrom.FullName, dirInfoTo.FullName);
                if(!CheckFileExtention(fileTo, excludeExts)) {
                    string directoryTo = Path.GetDirectoryName(fileTo);

                    if(!Directory.Exists(directoryTo)) {
                        Directory.CreateDirectory(directoryTo);
                    }

                    File.Copy(fileInfo.FullName, fileTo, true);
                }
            }
        }

        foreach(DirectoryInfo dirInfoItem in dirInfoCurrent.GetDirectories()) {
            CopyFilesLikeRecursive(dirInfoItem, dirInfoFrom, dirInfoTo, filter, excludeExts);
        }
#endif
    }

    public static bool CheckFileExtention(string path, List<string> extensions) {
        foreach(string ext in extensions) {
            if(path.ToLower().EndsWith(ext.ToLower())) {
                return true;
            }
        }
        return false;
    }

    public static void MoveFilesLikeRecursive(
        DirectoryInfo dirInfoCurrent,
        DirectoryInfo dirInfoFrom,
        DirectoryInfo dirInfoTo,
        string filter,
        List<string> excludeExts) {
#if !UNITY_WEBPLAYER
        foreach(FileInfo fileInfo in dirInfoCurrent.GetFiles()) {
            if(fileInfo.FullName.Contains(filter)) {
                string fileTo = fileInfo.FullName.Replace(dirInfoFrom.FullName, dirInfoTo.FullName);
                if(!CheckFileExtention(fileTo, excludeExts)) {
                    string directoryTo = Path.GetDirectoryName(fileTo);

                    if(!Directory.Exists(directoryTo)) {
                        Directory.CreateDirectory(directoryTo);
                    }

                    LogUtil.Log("fileTo:" + fileTo);

                    if(CheckFileExists(fileTo)) {
                        File.Delete(fileTo);
                    }

                    File.Move(fileInfo.FullName, fileTo);
                }
            }
        }

        foreach(DirectoryInfo dirInfoItem in dirInfoCurrent.GetDirectories()) {
            MoveFilesLikeRecursive(dirInfoItem, dirInfoFrom, dirInfoTo, filter, excludeExts);
        }
#endif
    }

    public static void RemoveDirectoriesLikeRecursive(
        DirectoryInfo dirInfoCurrent,
        string filterLike,
        string filterNotLike) {

#if !UNITY_WEBPLAYER
        foreach(DirectoryInfo dirInfoItem in dirInfoCurrent.GetDirectories()) {
            RemoveDirectoriesLikeRecursive(dirInfoItem, filterLike, filterNotLike);
        }

        if(dirInfoCurrent.FullName.Contains(filterLike)
            && !dirInfoCurrent.FullName.Contains(filterNotLike)) {
            Directory.Delete(dirInfoCurrent.FullName, true);
        }
#endif
    }

    public static bool CheckSignatureFile(string filepath, int signatureSize, string expectedSignature) {

#if !UNITY_WEBPLAYER
        if(String.IsNullOrEmpty(filepath))
            throw new ArgumentException("Must specify a filepath");
        if(String.IsNullOrEmpty(expectedSignature))
            throw new ArgumentException("Must specify a value for the expected file signature");
        using(FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
            if(fs.Length < signatureSize)
                return false;
            byte[] signature = new byte[signatureSize];
            int bytesRequired = signatureSize;
            int index = 0;
            while(bytesRequired > 0) {
                int bytesRead = fs.Read(signature, index, bytesRequired);
                bytesRequired -= bytesRead;
                index += bytesRead;
            }
            string actualSignature = BitConverter.ToString(signature);
            if(actualSignature == expectedSignature)
                return true;
            else
                return false;
        }
#else 
        return false;
        
#endif
    }

    public static bool CheckSignatureString(string data, int signatureSize, string expectedSignature) {

#if !UNITY_WEBPLAYER
        byte[] datas = Encoding.ASCII.GetBytes(data);
        return CheckSignature(datas, signatureSize, expectedSignature);
#else 
        return false;

#endif
    }


    public static bool CheckSignature(byte[] datas, int signatureSize, string expectedSignature) {

#if !UNITY_WEBPLAYER
        using(MemoryStream ms = new MemoryStream(datas)) {
            if(ms.Length < signatureSize)
                return false;
            byte[] signature = new byte[signatureSize];
            int bytesRequired = signatureSize;
            int index = 0;
            while(bytesRequired > 0) {
                int bytesRead = ms.Read(signature, index, bytesRequired);
                bytesRequired -= bytesRead;
                index += bytesRead;
            }
            string actualSignature = BitConverter.ToString(signature);
            if(actualSignature == expectedSignature)
                return true;
            else
                return false;
        }
#else
        return false;
        
#endif
    }
}