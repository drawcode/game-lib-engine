using System;
using System.Collections.Generic;
using System.IO;
using Engine.Game.App.BaseApp;
using UnityEngine;

namespace Engine.Game.App
{
    public class AppContentListItems : BaseAppContentListItems<AppContentListItem>
    {
        private static volatile AppContentListItem current;
        private static volatile AppContentListItems instance;
        private static object syncRoot = new System.Object();
        public static string DATA_KEY = "app-content-list-item-data";

        public static AppContentListItem Current
        {
            get
            {
                if (current == null)
                {
                    lock (syncRoot)
                    {
                        if (current == null)
                            current = new AppContentListItem();
                    }
                }

                return current;
            }
            set
            {
                current = value;
            }
        }

        public static AppContentListItems Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new AppContentListItems(true);
                    }
                }

                return instance;
            }
            set
            {
                instance = value;
            }
        }

        public AppContentListItems()
        {
            Reset();
        }

        public AppContentListItems(bool loadData)
        {
            Reset();
            path = "data/" + DATA_KEY + ".json";
            pathKey = DATA_KEY;
            LoadData();
        }

        public void SaveData(string fileFullPath, string data)
        {
#if !UNITY_WEBPLAYER
            FileSystemUtil.WriteString(fileFullPath, data);
#endif
        }

        public void ChangeState(string code, string packCode)
        {
            if (Current.code != code)
            {
                foreach (AppContentListItem action in GetListByCodeAndPackCode(code, packCode))
                {
                    Current = action;
                    break;
                }
            }
        }
        /*
        public List<AppContentListItem> GetListByPack(string packCode) {
            List<AppContentListItem> filteredList = new List<AppContentListItem>();
            foreach(AppContentListItem obj in GetAll()) {
                if(packCode.ToLower() == obj.pack_code.ToLower()) {
                    filteredList.Add(obj);
                }
            }

            return filteredList;
        }
        */

        public List<AppContentListItem> GetListByCodeAndPackCode(
            string actionCode, string packCode)
        {
            List<AppContentListItem> filteredList = new List<AppContentListItem>();
            foreach (AppContentListItem obj in GetListByPack(packCode))
            {
                if (actionCode.ToLower() == obj.code.ToLower())
                {
                    filteredList.Add(obj);
                }
            }

            return filteredList;
        }

        public List<AppContentListItem> GetListByPackCodeAndPathVersioned(
            string packCode, string pathVersioned)
        {
            List<AppContentListItem> filteredList = new List<AppContentListItem>();
            foreach (AppContentListItem obj in GetListByPack(packCode))
            {

                if (pathVersioned.ToLower() == obj.data.filePathVersioned.ToLower()
                    || (pathVersioned.ToLower().Contains(obj.data.filePathNonVersioned)
                    && pathVersioned.ToLower().Contains(obj.data.directoryFull)))
                {
                    filteredList.Add(obj);
                }
            }

            return filteredList;
        }
    }

    public enum AppContentListItemDataEnum
    {
        SYSTEM,
        FULL,
        APP,
        PACK
    }

    public enum AppContentListItemFormatEnum
    {
        NON_VERSIONED,
        VERSIONED,
        VERSIONED_SYNC
    }

    public class AppContentListItemPath
    {
        public string pathCurrentAppBase;
        public string pathNonVersionedSystem;
        public string pathNonVersionedFull;
        public string pathNonVersionedApp;
        public string pathVersionedSystem;
        public string pathVersionedFull;
        public string pathVersionedApp;
        public string pathVersionedSystemSync;
        public string pathVersionedFullSync;
        public string pathVersionedAppSync;

        public AppContentListItemPath(string filePathNonVersioned, string filePathVersioned)
        {
            Fill(filePathNonVersioned, filePathVersioned);
        }

        public void Fill(string filePathNonVersioned, string filePathVersioned)
        {
            pathCurrentAppBase = ContentsConfig.contentRootFolder + "/";
            pathCurrentAppBase += ContentsConfig.contentAppFolder + "/";

            pathNonVersionedApp = filePathNonVersioned.Replace(pathCurrentAppBase, "");
            pathNonVersionedSystem
                = PathUtil.Combine(ContentPaths.appCachePath, pathNonVersionedApp);
            pathNonVersionedFull = filePathNonVersioned;

            pathVersionedApp = filePathVersioned.Replace(pathCurrentAppBase, "");
            pathVersionedSystem
                = PathUtil.Combine(ContentPaths.appCachePath, pathVersionedApp);
            pathVersionedFull = filePathVersioned;

            pathVersionedSystemSync = Contents.GetFullPathVersionedSync(pathVersionedSystem);
            pathVersionedFullSync = Contents.GetFullPathVersionedSync(pathVersionedFull);
            pathVersionedAppSync = Contents.GetFullPathVersionedSync(pathVersionedApp);
        }
    }

    public class AppContentListItemData
    {
        public string hash = "";
        public string directoryPath = "";
        public string directoryFull = "";
        public string directoryName = "";
        public string directoryPathVersioned = "";
        public string filePathVersioned = "";
        public string filePathNonVersioned = "";
        public string filePath = "";
        public string fileName = "";
        public string fileExtension = "";
        public string fileNoExtension = "";
        public string version = "1.0";
        public int versionIncrement = 1;
        public string packCode = "";
        public string platform = "shared";
        public bool versioned = false;

        public AppContentListItemData()
        {

        }

        public AppContentListItemPath GetFilePaths()
        {
            AppContentListItemPath paths
                = new AppContentListItemPath(
                    filePathNonVersioned, filePathVersioned);
            return paths;
        }
    }

    public class AppContentListItem : BaseAppContentListItem
    {

        public AppContentListItemData data = null;

        public AppContentListItem()
        {
            Reset();
        }

        public override void Reset()
        {
            base.Reset();
            data = new AppContentListItemData();
        }

        public AppContentListItemData GetContentListItemData()
        {
            return data;
        }
    }
}