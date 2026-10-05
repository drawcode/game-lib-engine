using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class FileSystemUtil {

    // Per-file-operation trace logging, OFF by default.
    //
    // These 39 call sites were plain Debug.Log, and they are the single most expensive thing this
    // class does. Measured on a 7-byte write: WriteString costs 5.06 ms with stack traces on and
    // 1.66 ms with them off, while the write itself is 0.20 ms and the Directory.Exists behind
    // EnsureDirectory is 0.007 ms. The gap is UnityEngine.Debug capturing a managed stack trace
    // per call — five calls per WriteString (one here, three in EnsureDirectory, one in
    // CreateDirectoryIfNeededAndAllowed), and CopyFile logs fifteen more per file.
    //
    // That is not an Editor artifact: StackTraceLogType.ScriptOnly is the DEFAULT for LogType.Log
    // in player builds too, so it shipped. GameState.SaveProfile writes ten blobs and paid ~34 ms
    // of it per save.
    //
    // Routing them through LogUtil.Log would NOT have fixed it — LogUtil.loggingEnabled defaults
    // true and the "default" key is active, so it runs LoadKeys() plus a key scan and then calls
    // Debug.Log anyway. Hence an explicit flag: set FileSystemUtil.logVerbose = true to get the
    // trace back when diagnosing a path or permissions problem.
    //
    // The File.Copy failure log is deliberately NOT gated — a real error must never go quiet.
    public static bool logVerbose = false;

    // Directories confirmed to exist this session — see EnsureDirectory. Session-scoped on
    // purpose: it is a cache of an observation, not of an intent, so the worst case if something
    // deletes a folder underneath us is one failed write rather than a silently wrong path.
    static readonly HashSet<string> ensuredDirectories = new HashSet<string>();

    static void LogVerbose(object message) {

        if (!logVerbose) {
            return;
        }

        Debug.Log(message);
    }

    public static void CreateDirectoryIfNeededAndAllowed(string path) {

#if !UNITY_WEBPLAYER

        LogVerbose("FileSystemUtil::CreateDirectoryIfNeededAndAllowed:path:" + path);

        if (!Directory.Exists(path)) {

            LogVerbose("FileSystemUtil::CreateDirectoryIfNeededAndAllowed:pathnotexists:" + path);

            if (DirectoryAllowed(path)) {

                LogVerbose("CreateDirectoryIfNeededAndAllowed:" + path);

                path = path.TrimEnd('/');

                LogVerbose("CreateDirectoryIfNeededAndAllowed:trimmed:path:" + path);

                //Directory.CreateDirectory(path);

                DirectoryInfo dir = new DirectoryInfo(path);

                if (!dir.Exists) {

                    dir.Create();

                    LogVerbose("CreateDirectoryIfNeededAndAllowed:info:path:" + path);
                }

            }
        }

#endif
    }

    public static bool DirectoryAllowed(string path) {
        bool allowCreate = true;

#if !UNITY_WEBPLAYER
        if (path.IndexOf(Application.persistentDataPath) == -1
            && !Application.isEditor) {
            allowCreate = false;
        }
#endif
        return allowCreate;
    }

    public static void CreateDirectoryHolderFile(
        string path, string filename = "dirinfo.json.txt", string filedata = "") {

        string filepath = StringUtil.Combine("/", path, filename);

        Dictionary<string, object> dirInfo = null;

        if (CheckFileExists(filepath)) {
            dirInfo = FileSystemUtil.ReadString(filepath).FromJsonToDict();
        }

        if (dirInfo == null) {

            dirInfo = new Dictionary<string, object>();
        }

        if (filedata.IsNullOrEmpty()) {
            filedata = "DIRECTORY HOLDER file for:" + path;
        }

        dirInfo.Set("path", path);
        dirInfo.Set("filename", filename);
        dirInfo.Set("data", filedata);

        string data = dirInfo.ToString();

        WriteString(filepath, data);

        LogVerbose("CreateDirectoryHolderFile:data:" + data);
    }

    public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs, bool versioned) {

#if !UNITY_WEBPLAYER
        FileSystemUtil.EnsureDirectory(sourceDirName, false);
        FileSystemUtil.EnsureDirectory(destDirName, false);

        CreateDirectoryIfNeededAndAllowed(sourceDirName);

        DirectoryInfo dir = new DirectoryInfo(sourceDirName);
        DirectoryInfo[] dirs = dir.GetDirectories();

        if (!dir.Exists) {
            throw new DirectoryNotFoundException(
                "Source directory does not exist or could not be found: "
                + sourceDirName);
        }

        CreateDirectoryIfNeededAndAllowed(destDirName);

        FileInfo[] files = dir.GetFiles();

        LogUtil.Log("Directory Files: directory: " + destDirName);
        LogUtil.Log("files.Count:", files.Count());

        //int curr = 0;

        foreach (FileInfo file in files) {

            if (file.Extension != ".meta"
                && file.Extension != ".DS_Store") {

                string temppath = PathUtil.Combine(destDirName, file.Name);


                if (!CheckFileExists(temppath) || Application.isEditor) {

                    LogUtil.Log("copying ship file: " + file.FullName);
                    LogUtil.Log("copying ship file to cache: " + temppath);

                    file.CopyTo(temppath, true);
                    ////SystemHelper.SetNoBackupFlag(temppath);
                }
            }
        }

        if (copySubDirs) {

            foreach (DirectoryInfo subdir in dirs) {

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

        LogVerbose("FileSystemUtil::EnsureDirectory:filePath:" + filePath);

        string directory = filePath;

        if (filePath.IndexOf('.') > -1 && filterFileName) {

            directory = filePath.Replace(Path.GetFileName(filePath), "");

            LogVerbose("FileSystemUtil::EnsureDirectory:directory:" + directory);

        }

        LogVerbose("FileSystemUtil::EnsureDirectory:directory:" + directory);

        //LogUtil.Log("directory:" + directory);

        // One directory serves every write that follows it — SaveProfile alone writes ten blobs
        // into the same folder — so confirm it once per session instead of once per file. Only
        // directories we have actually seen exist are remembered, and the remember happens AFTER
        // the create, so a first run still creates and a failed create is never cached.
        if (ensuredDirectories.Contains(directory)) {
            return;
        }

        CreateDirectoryIfNeededAndAllowed(directory);

        if (Directory.Exists(directory)) {
            ensuredDirectories.Add(directory);
        }

        //if(createFileEmptyFileInFolder) {
        //    CreateDirectoryHolderFile(directory);
        //}
    }

    public static bool CheckDirectoryExists(string filePath, bool filterFileName = true) {

        LogVerbose("FileSystemUtil::CheckDirectoryExists:filePath:" + filePath);

        string directory = filePath;

        if (filePath.IndexOf('.') > -1 && filterFileName) {

            directory = filePath.Replace(Path.GetFileName(filePath), "");

            LogVerbose("FileSystemUtil::CheckDirectoryExists:directory:" + directory);

        }

        LogVerbose("FileSystemUtil::CheckDirectoryExists:directory:" + directory);

        return Directory.Exists(directory);

    }

    public static string GetFileLocalPath(string path) {
        if (!path.Contains("file://")) {

            if (!path.StartsWith("/")) {
                path = "/" + path;
            }

            path = "file://" + path;
        }
        return path;
    }

    public static bool CheckFileExists(string path) {

        bool exists = false;

        LogVerbose("CheckFileExists: path:" + path);
        LogVerbose("CheckFileExists: Application.streamingAssetsPath:" + Application.streamingAssetsPath);
        LogVerbose("CheckFileExists: path.Contains(Application.streamingAssetsPath):" + path.Contains(Application.streamingAssetsPath));

#if UNITY_ANDROID
        if(!exists) {// && path.Contains(Application.streamingAssetsPath)) {
                     // android stores streamingassets in a compressed file, 
                     // must use WWW to check if you can access it

            path = GetFileLocalPath(path);


            UnityWebRequest www = new UnityWebRequest();
            www.downloadHandler = new DownloadHandlerBuffer();
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

                LogVerbose("CheckFileExists: Android: path:" + path);
                LogVerbose("CheckFileExists: Android: file.bytes.length:" + length);

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

        LogVerbose("dataFilePath: " + dataFilePath);
        LogVerbose("persistenceFilePath: " + persistenceFilePath);
        LogVerbose("Application.dataPath: " + Application.dataPath);
        LogVerbose("Application.persistentDataPath: " + Application.persistentDataPath);
        LogVerbose("Application.temporaryCachePath: " + Application.temporaryCachePath);
        LogVerbose("Application.streamingAssetsPath: " + Application.streamingAssetsPath);

        LogVerbose("CheckFileExists(dataFilePath): " + CheckFileExists(dataFilePath));
        LogVerbose("!CheckFileExists(persistenceFilePath): " + !CheckFileExists(persistenceFilePath));
        LogVerbose("CheckDirectoryExists(dataFilePath): " + CheckDirectoryExists(dataFilePath));
        LogVerbose("CheckDirectoryExists(dataPath): " + CheckDirectoryExists(Application.dataPath));
        LogVerbose("CheckDirectoryExists(Application.persistentDataPath): " + CheckDirectoryExists(Application.persistentDataPath));
        LogVerbose("CheckDirectoryExists(Application.persistentDataPath): " + CheckDirectoryExists(Application.persistentDataPath));
        LogVerbose("CheckDirectoryExists(Application.temporaryCachePath): " + CheckDirectoryExists(Application.temporaryCachePath));
        LogVerbose("CheckDirectoryExists(Application.streamingAssetsPath): " + CheckDirectoryExists(Application.streamingAssetsPath));


        LogVerbose("force: " + force);

        if (CheckFileExists(dataFilePath) && (!CheckFileExists(persistenceFilePath) || force)) {

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
                www.downloadHandler = new DownloadHandlerBuffer();
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

                    LogVerbose("CopyFile: Android: dataFilePath:" + dataFilePath);
                    LogVerbose("CopyFile: Android: persistenceFilePath:" + persistenceFilePath);
                    LogVerbose("CopyFile: Android: file.bytes.length:" + length);

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
            try {
                File.Copy(dataFilePath, persistenceFilePath, true);
            }
            catch (Exception e) {

                Debug.Log("ERROR File.Copy ERROR (dataFilePath,persistenceFilePath):  " + dataFilePath + " : " + persistenceFilePath);
                Debug.Log(e);
            }
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
        if (Directory.Exists(dirInfoCurrent)) {
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
        foreach (FileInfo fileInfo in dirInfoCurrent.GetFiles()) {
            string fileTo = fileInfo.FullName;
            if (fileTo.Contains(filter)
                || filter == "*") {
                if (!CheckFileExtention(fileTo, excludeExts)) {
                    if (!files.Contains(fileTo)) {
                        files.Add(fileTo);
                    }
                }
            }
        }

        foreach (DirectoryInfo dirInfoItem in dirInfoCurrent.GetDirectories()) {
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
        if (CheckFileExists(dataFilePath) && (!CheckFileExists(persistenceFilePath) || force)) {

            //LogUtil.Log("fileMoved: " + persistenceFilePath);
#if UNITY_ANDROID       
            if(dataFilePath.Contains(Application.streamingAssetsPath)) {
                // android stores streamingassets in a compressed file, 
                // must use WWW to copy contents if you can access it

                dataFilePath = GetFileLocalPath(dataFilePath);

                UnityWebRequest www = new UnityWebRequest();
                www.downloadHandler = new DownloadHandlerBuffer();
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
        if (CheckFileExists(fileName)) {
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
        if (CheckFileExists(fileName)) {
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
        LogVerbose("FileSystemUtil::WriteString:EnsureDirectory:fileName:" + fileName);

        EnsureDirectory(fileName);

        StreamWriter sw = new StreamWriter(fileName, append);
        sw.Write(data);
        sw.Flush();
        sw.Close();
        ////SystemHelper.SetNoBackupFlag(fileName);
#endif
    }

    public static void RemoveFile(string file) {
        if (CheckFileExists(file)) {
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
        foreach (FileInfo fileInfo in dirInfo.GetFiles()) {
            if (fileInfo.FullName.Contains(fileKey)) {
                File.Delete(fileInfo.FullName);
            }
        }

        foreach (DirectoryInfo dirInfoItem in dirInfo.GetDirectories()) {
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
        foreach (FileInfo fileInfo in dirInfoCurrent.GetFiles()) {
            if (fileInfo.FullName.Contains(filter)) {
                string fileTo = fileInfo.FullName.Replace(dirInfoFrom.FullName, dirInfoTo.FullName);
                if (!CheckFileExtention(fileTo, excludeExts)) {
                    string directoryTo = Path.GetDirectoryName(fileTo);

                    if (!Directory.Exists(directoryTo)) {
                        Directory.CreateDirectory(directoryTo);
                    }

                    File.Copy(fileInfo.FullName, fileTo, true);
                }
            }
        }

        foreach (DirectoryInfo dirInfoItem in dirInfoCurrent.GetDirectories()) {
            CopyFilesLikeRecursive(dirInfoItem, dirInfoFrom, dirInfoTo, filter, excludeExts);
        }
#endif
    }

    public static bool CheckFileExtention(string path, List<string> extensions) {
        foreach (string ext in extensions) {
            if (path.ToLower().EndsWith(ext.ToLower())) {
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
        foreach (FileInfo fileInfo in dirInfoCurrent.GetFiles()) {
            if (fileInfo.FullName.Contains(filter)) {
                string fileTo = fileInfo.FullName.Replace(dirInfoFrom.FullName, dirInfoTo.FullName);
                if (!CheckFileExtention(fileTo, excludeExts)) {
                    string directoryTo = Path.GetDirectoryName(fileTo);

                    if (!Directory.Exists(directoryTo)) {
                        Directory.CreateDirectory(directoryTo);
                    }

                    LogUtil.Log("fileTo:" + fileTo);

                    if (CheckFileExists(fileTo)) {
                        File.Delete(fileTo);
                    }

                    File.Move(fileInfo.FullName, fileTo);
                }
            }
        }

        foreach (DirectoryInfo dirInfoItem in dirInfoCurrent.GetDirectories()) {
            MoveFilesLikeRecursive(dirInfoItem, dirInfoFrom, dirInfoTo, filter, excludeExts);
        }
#endif
    }

    public static void RemoveDirectoriesLikeRecursive(
        DirectoryInfo dirInfoCurrent,
        string filterLike,
        string filterNotLike) {

#if !UNITY_WEBPLAYER
        foreach (DirectoryInfo dirInfoItem in dirInfoCurrent.GetDirectories()) {
            RemoveDirectoriesLikeRecursive(dirInfoItem, filterLike, filterNotLike);
        }

        if (dirInfoCurrent.FullName.Contains(filterLike)
            && !dirInfoCurrent.FullName.Contains(filterNotLike)) {
            Directory.Delete(dirInfoCurrent.FullName, true);
        }
#endif
    }

    public static bool CheckSignatureFile(string filepath, int signatureSize, string expectedSignature) {

#if !UNITY_WEBPLAYER
        if (String.IsNullOrEmpty(filepath))
            throw new ArgumentException("Must specify a filepath");
        if (String.IsNullOrEmpty(expectedSignature))
            throw new ArgumentException("Must specify a value for the expected file signature");
        using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
            if (fs.Length < signatureSize)
                return false;
            byte[] signature = new byte[signatureSize];
            int bytesRequired = signatureSize;
            int index = 0;
            while (bytesRequired > 0) {
                int bytesRead = fs.Read(signature, index, bytesRequired);
                bytesRequired -= bytesRead;
                index += bytesRead;
            }
            string actualSignature = BitConverter.ToString(signature);
            if (actualSignature == expectedSignature)
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
        using (MemoryStream ms = new MemoryStream(datas)) {
            if (ms.Length < signatureSize)
                return false;
            byte[] signature = new byte[signatureSize];
            int bytesRequired = signatureSize;
            int index = 0;
            while (bytesRequired > 0) {
                int bytesRead = ms.Read(signature, index, bytesRequired);
                bytesRequired -= bytesRead;
                index += bytesRead;
            }
            string actualSignature = BitConverter.ToString(signature);
            if (actualSignature == expectedSignature)
                return true;
            else
                return false;
        }
#else
        return false;
        
#endif
    }
}