using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Engine.Game.Data
{
    public class NPCData : DataObject
    {
        public string uuid;
        public string packName;

        public NPCData()
        {
            Reset();
        }

        public void ChangeData(string name)
        {
            Reset();

            packName = name;

            //fileName = "npc-data-" + System.Uri.EscapeUriString(packName) + ".json";
            //fileFullPath = PathUtil.Combine(filePath, fileName);
        }

        public override void Reset()
        {
            base.Reset();

            packName = "default";
            uuid = "";//Puid.New();

            //filePath = PathUtil.AppPersistencePath;
        }
    }
}