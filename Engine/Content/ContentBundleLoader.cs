using System;
using System.IO;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum ContentBundleDownloadState {
    InProgress,
    Failure,
    Success
}

public class ContentBundleDownloadHandle {

    public ContentBundleDownloadState state = ContentBundleDownloadState.InProgress;
    public Uri uri;
    public AssetBundle asset;
    public string error = null;
    public bool fromCache = false;

    public ContentBundleDownloadHandle(string url) {

        uri = new Uri(url);
    }
}

public class ContentBundleLoader : GameObjectBehavior {

    string cachePath;
    //ContentBundleLoader instance;

    public static ContentBundleLoader Instance {

        get {

            var g = GameObject.Find("/_content_bundle_loader");

            if (g == null) {

                g = new GameObject("_content_bundle_loader");
            }

            g.hideFlags = HideFlags.HideAndDontSave;

            var c = g.GetComponent<ContentBundleLoader>();

            if (c == null) {

                c = g.AddComponent<ContentBundleLoader>();
            }

            return c;
        }
    }

    public ContentBundleDownloadHandle Download(string url) {

        var handle = new ContentBundleDownloadHandle(url);

        StartCoroutine(_Download(handle));

        return handle;
    }

    bool HandleDownload(ContentBundleDownloadHandle handle, UnityWebRequest www) {
        
        if (www.error == null) {

            handle.asset = DownloadHandlerAssetBundle.GetContent(www);//www.downloadHandle.assetBundle;

            if (www.error != null) {

                handle.error = www.error;

                return false;
            }
            else {

                if (handle.asset == null) {

                    handle.error = "Failed to load asset bundle";

                    return false;
                }
                else {
                    return true;
                }
            }
        }
        else {

            handle.error = www.error;

            return false;
        }
    }

    IEnumerator _Download(ContentBundleDownloadHandle handle) {

        if (cachePath == null) {

            cachePath = PathUtil.Combine(Application.persistentDataPath, "bundlecache");
        }

        //PathUtil.Combine(ContentPaths.appCachePlatformPath, "packs/[app]-pack-[app]-[app]-1/" + assetName + ".unity3d");

        var uri = handle.uri;

        var dir = PathUtil.Combine(
            PathUtil.Combine(cachePath, uri.Host),
            Path.GetDirectoryName(uri.AbsolutePath).Substring(1));

        var file = Path.GetFileName(uri.AbsolutePath);
        var path = PathUtil.Combine(dir, file);

        if (File.Exists(path)) {

            var www = UnityWebRequest.Get("file://" + path);

            yield return www.SendWebRequest();

            if (HandleDownload(handle, www)) {

                handle.state = ContentBundleDownloadState.Success;
                handle.fromCache = true;
            }
        }
        if (handle.state != ContentBundleDownloadState.Success) {

            var www = UnityWebRequest.Get(uri.ToString());

            yield return www.SendWebRequest();

            if (HandleDownload(handle, www)) {

                Directory.CreateDirectory(dir);

                FileSystemUtil.WriteAllBytes(path, www.downloadHandler.data);//.bytes);

                handle.error = null;
                handle.state = ContentBundleDownloadState.Success;
            }
            else {

                handle.state = ContentBundleDownloadState.Failure;
            }
        }
    }

    /*
	 
	 IEnumerator _Download (ContentBundleDownloadHandle handle)
	{
		if (cachePath == null)
			cachePath = PathUtil.Combine (Application.persistentDataPath, "bundlecache");
		//PathUtil.Combine(ContentPaths.appCachePlatformPath, "packs/pack-1/" + assetName + ".unity3d");
		var uri = handle.uri;
		var dir = PathUtil.Combine (PathUtil.Combine (cachePath, uri.Host), Path.GetDirectoryName (uri.AbsolutePath).Substring (1));
		var file = Path.GetFileName (uri.AbsolutePath);
		var path = PathUtil.Combine (dir, file);
		if (File.Exists (path)) {
			var www = new WWW ("file://" + path);
			yield return www;
			if (HandleDownload (handle, www)) {
				handle.state = ContentBundleDownloadState.Success;
				handle.fromCache = true;
			}
		}
		if (handle.state != ContentBundleDownloadState.Success) {
			var www = new WWW (uri.ToString ());
			yield return www;
			if (HandleDownload (handle, www)) {
				Directory.CreateDirectory (dir);
				File.WriteAllBytes (path, www.bytes);
				handle.error = null;
				handle.state = ContentBundleDownloadState.Success;
			} else {
				handle.state = ContentBundleDownloadState.Failure;
			}
		}
	} 
	 * */
}