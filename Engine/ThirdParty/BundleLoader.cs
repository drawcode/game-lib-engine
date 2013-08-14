using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

public enum BundleDownloadState {
    InProgress,
    Failure,
    Success
}

public class BundleDownloadHandle {
    public BundleDownloadState state = BundleDownloadState.InProgress;
    public Uri uri;
    public AssetBundle asset;
    public string error = null;
    public bool fromCache = false;

    public BundleDownloadHandle(string url) {
        uri = new Uri(url);
    }
}

public class BundleLoader : MonoBehaviour {
    private string cachePath;

    //BundleLoader instance;

    public static BundleLoader Instance {
        get {
            var g = GameObject.Find("/_BundleLoader");
            if (g == null)
                g = new GameObject("_BundleLoader");
            g.hideFlags = HideFlags.HideAndDontSave;
            var c = g.GetComponent<BundleLoader>();
            if (c == null)
                c = g.AddComponent<BundleLoader>();
            return c;
        }
    }

    public BundleDownloadHandle Download(string url) {
        var handle = new BundleDownloadHandle(url);
        StartCoroutine(_Download(handle));
        return handle;
    }

    private bool HandleDownload(BundleDownloadHandle handle, WWW www) {
        if (www.error == null) {
            handle.asset = www.assetBundle;
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

    private IEnumerator _Download(BundleDownloadHandle handle) {
        if (cachePath == null)
            cachePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "bundlecache");
        var uri = handle.uri;
        var dir = Path.Combine(Path.Combine(cachePath, uri.Host), Path.GetDirectoryName(uri.AbsolutePath).Substring(1));
        var file = Path.GetFileName(uri.AbsolutePath);
        var path = Path.Combine(dir, file);
        if (File.Exists(path)) {
            var www = new WWW("file://" + path);
            yield return www;
            if (HandleDownload(handle, www)) {
                handle.state = BundleDownloadState.Success;
                handle.fromCache = true;
            }
        }
        if (handle.state != BundleDownloadState.Success) {
            var www = new WWW(uri.ToString());
            yield return www;
            if (HandleDownload(handle, www)) {
                Directory.CreateDirectory(dir);
                File.WriteAllBytes(path, www.bytes);
                handle.error = null;
                handle.state = BundleDownloadState.Success;
            }
            else {
                handle.state = BundleDownloadState.Failure;
            }
        }
    }
}