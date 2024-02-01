using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Engine.Game.Data;
using Engine.Utility;

namespace Engine.Game.App.BaseApp
{
    public class BaseGameProfileTeamAttributes
    {
        public static string ATT_TEAMS = "teams";
    }

    public class BaseGameProfileTeams
    {
        private static volatile BaseGameProfileTeam current;
        private static volatile BaseGameProfileTeams instance;
        private static object syncRoot = new Object();
        public static string DEFAULT_USERNAME = "Player";

        public static BaseGameProfileTeam BaseCurrent
        {
            get
            {
                if (current == null)
                {
                    lock (syncRoot)
                    {
                        if (current == null)
                            current = new BaseGameProfileTeam();
                    }
                }

                return current;
            }
            set
            {
                current = value;
            }
        }

        public static BaseGameProfileTeams BaseInstance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new BaseGameProfileTeams();
                    }
                }

                return instance;
            }
        }

        // TODO: Common profile actions, lookup, count, etc
    }

    public class BaseGameProfileTeam : DataObject
    {
        // BE CAREFUL adding properties as they will cause a need for a profile conversion
        // Best way to add items to the profile is the GetAttribute and SetAttribute class as 
        // that stores as a generic DataAttribute class.  Booleans, strings, objects, serialized json objects etc
        // all work well and cause no need to convert profile on updates. 

        public BaseGameProfileTeam()
        {
            //Reset();
        }

        public override void Reset()
        {
            base.Reset();
            //username = ProfileConfigs.defaultPlayerName;
        }

        // customizations       

        public virtual void SetValue(string code, object value)
        {
            DataAttribute att = new DataAttribute();
            att.val = value;
            att.code = code;
            att.name = "";
            att.type = "bool";
            att.otype = "team";
            SetAttribute(att);
        }

        public virtual bool GetValue(string code)
        {
            bool currentValue = false;
            object objectValue = GetAttribute(code).val;
            if (objectValue != null)
            {
                currentValue = Convert.ToBoolean(objectValue);
            }

            return currentValue;
        }

        public virtual List<DataAttribute> GetList()
        {
            return GetAttributesList("team");
        }
    }
}