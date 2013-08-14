using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class FileSystemUtil {
#if !UNITY_WEBPLAYER
	
	
	public static void CreateDirectoryIfNeededAndAllowed(string path) {
		if (!Directory.Exists(path)) {
			if(DirectoryAllowed(path)) {
				LogUtil.Log("CreateDirectoryIfNeededAndAllowed:" + path);
				Directory.CreateDirectory(path);
			}
        }
	}
	
	public static bool DirectoryAllowed(string path) {
		bool allowCreate = true;
		
		if(path.IndexOf(Application.persistentDataPath) == -1
			&& !Application.isEditor) {
			allowCreate = false;
		}
		return allowCreate;
	}
	    
    public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs, bool versioned) {
		
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
			if(file.Extension != ".meta"
		        && file.Extension != ".DS_Store") {
				
				string temppath = Path.Combine(destDirName, file.Name);
				
				
				if(!CheckFileExists(temppath) || Application.isEditor) {
					
					LogUtil.Log("copying ship file: " + file.FullName);
					LogUtil.Log("copying ship file to cache: " + temppath);
					
					file.CopyTo(temppath, true);
					////SystemHelper.SetNoBackupFlag(temppath);
		        }
			}
        }

        if (copySubDirs)  {
			
            foreach (DirectoryInfo subdir in dirs) {
                string temppath = Path.Combine(destDirName, subdir.Name);
                LogUtil.Log("Copying Directory: " + temppath);
                DirectoryCopy(subdir.FullName, temppath, copySubDirs, versioned);
            }
        }
    }
	
	public static void EnsureDirectory(string filePath) {
		EnsureDirectory(filePath, true);
	}
	
	public static void EnsureDirectory(string filePath, bool filterFileName) {
		
		//LogUtil.Log("filePath:" + filePath);
		
		string directory = filePath;
		if(filePath.IndexOf('.') > -1 && filterFileName) {
			directory = filePath.Replace(Path.GetFileName(filePath), "");
		}
		//LogUtil.Log("directory:" + directory);
		CreateDirectoryIfNeededAndAllowed(directory);
	}

    public static bool CheckFileExists(string path) {
		
		bool exists = File.Exists(path);
		
#if UNITY_ANDROID		
		if(!exists && path.Contains(Application.streamingAssetsPath)) {
			// android stores streamingassets in a compressed file, 
			// must use WWW to check if you can access it
			
			if(!path.Contains("file://")){
				path = "file://" + path;
			}
			
			WWW file = new WWW(path);
			
			float currentTime = Time.time;
			float endTime = currentTime + 6f; // only allow some seconds for file check
			while(!file.isDone && currentTime < endTime) {
				currentTime = Time.time;
			};
			
			int length = file.bytes.Length;
			
			LogUtil.Log("CheckFileExists: Android: path:" + path);
			LogUtil.Log("CheckFileExists: Android: file.bytes.length:" + length);
			
			if(file.bytes.Length > 0) {
				exists = true;
			}
		}
#else
#endif	
		
		return exists;
    }
	
    public static void CopyFile(string dataFilePath, string persistenceFilePath) {
		CopyFile(dataFilePath, persistenceFilePath, false);
	}

    public static void CopyFile(string dataFilePath, string persistenceFilePath, bool force) {
		
		EnsureDirectory(dataFilePath);
		EnsureDirectory(persistenceFilePath);
        //LogUtil.Log("dataFilePath: " + dataFilePath);
        //LogUtil.Log("persistenceFilePath: " + persistenceFilePath);
        if (CheckFileExists(dataFilePath) && (!CheckFileExists(persistenceFilePath) || force)) {
			
#if UNITY_ANDROID		
			if(dataFilePath.Contains(Application.streamingAssetsPath)) {
				// android stores streamingassets in a compressed file, 
				// must use WWW to copy contents if you can access it
				
				if(!dataFilePath.Contains("file://")){
					dataFilePath = "file://" + dataFilePath;
				}
				
				WWW file = new WWW(dataFilePath);
				
				float currentTime = Time.time;
				float endTime = currentTime + 6f; // only allow some seconds for file check
				while(!file.isDone && currentTime < endTime) {
					currentTime = Time.time;
				};
				
				int length = file.bytes.Length;
				
				LogUtil.Log("CopyFile: Android: dataFilePath:" + dataFilePath);
				LogUtil.Log("CopyFile: Android: persistenceFilePath:" + persistenceFilePath);
				LogUtil.Log("CopyFile: Android: file.bytes.length:" + length);
				
				if(file.bytes.Length > 0) {		
					// Save file contents to new location					
					FileSystemUtil.WriteAllBytes(persistenceFilePath, file.bytes);					
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
    }
    public static void MoveFile(string dataFilePath, string persistenceFilePath) {
		MoveFile(dataFilePath, persistenceFilePath, false);
	}

    public static void MoveFile(string dataFilePath, string persistenceFilePath, bool force) {
		
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
				
				if(!dataFilePath.Contains("file://")){
					dataFilePath = "file://" + dataFilePath;
				}
				
				WWW file = new WWW(dataFilePath);
				
				float currentTime = Time.time;
				float endTime = currentTime + 6f; // only allow some seconds for file check
				while(!file.isDone && currentTime < endTime) {
					currentTime = Time.time;
				};
				
				int length = file.bytes.Length;
				
				LogUtil.Log("CopyFile: Android: dataFilePath:" + dataFilePath);
				LogUtil.Log("CopyFile: Android: persistenceFilePath:" + persistenceFilePath);
				LogUtil.Log("CopyFile: Android: file.bytes.length:" + length);
				
				if(file.bytes.Length > 0) {		
					// Save file contents to new location					
					FileSystemUtil.WriteAllBytes(persistenceFilePath, file.bytes);					
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
    }

    public static byte[] ReadAllBytes(string fileName) {
        return File.ReadAllBytes(fileName);
    }

    public static void WriteAllBytes(string fileName, byte[] buffer) {
		EnsureDirectory(fileName);
        File.WriteAllBytes(fileName, buffer);
		////SystemHelper.SetNoBackupFlag(fileName);
    }

    public static byte[] ReadStream(string fileName) {
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
    }

    public static void WriteStream(string fileName, byte[] data) {
		EnsureDirectory(fileName);
        StreamWriter sw = new StreamWriter(fileName, false, Encoding.ASCII);
        sw.Write(data);
        sw.Flush();
        sw.Close();
		////SystemHelper.SetNoBackupFlag(fileName);
    }

    public static string ReadString(string fileName) {
        string contents = "";
        if (CheckFileExists(fileName)) {
            StreamReader sr = new StreamReader(fileName, true);
            contents = sr.ReadToEnd();
            sr.Close();
        }
        return contents;
    }

    public static void WriteString(string fileName, string data) {
        WriteString(fileName, data, false);
    }

    public static void WriteString(string fileName, string data, bool append) {
		EnsureDirectory(fileName);
        StreamWriter sw = new StreamWriter(fileName, append);
        sw.Write(data);
        sw.Flush();
        sw.Close();		
		////SystemHelper.SetNoBackupFlag(fileName);
    }
	
	public static void RemoveFile(string file) {
		if(CheckFileExists(file)) {
			File.Delete(file);
		}
	}

    public static void RemoveFilesLikeRecursive(DirectoryInfo dirInfo, string fileKey) {
        foreach (FileInfo fileInfo in dirInfo.GetFiles()) {
            if (fileInfo.FullName.Contains(fileKey)) {
                File.Delete(fileInfo.FullName);
            }
        }

        foreach (DirectoryInfo dirInfoItem in dirInfo.GetDirectories()) {
            RemoveFilesLikeRecursive(dirInfoItem, fileKey);
        }
    }

    public static void CopyFilesLikeRecursive(
        DirectoryInfo dirInfoCurrent,
        DirectoryInfo dirInfoFrom,
        DirectoryInfo dirInfoTo,
        string filter,
        List<string> excludeExts) {
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
    }

    public static void RemoveDirectoriesLikeRecursive(
        DirectoryInfo dirInfoCurrent,
        string filterLike,
        string filterNotLike) {
        foreach (DirectoryInfo dirInfoItem in dirInfoCurrent.GetDirectories()) {
            RemoveDirectoriesLikeRecursive(dirInfoItem, filterLike, filterNotLike);
        }

        if (dirInfoCurrent.FullName.Contains(filterLike)
            && !dirInfoCurrent.FullName.Contains(filterNotLike)) {
            Directory.Delete(dirInfoCurrent.FullName, true);
        }
    }

#endif
}