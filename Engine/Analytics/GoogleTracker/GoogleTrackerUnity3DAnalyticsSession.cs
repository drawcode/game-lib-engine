#define UNITY3D
using UnityEngine;


public class GoogleTrackerUnity3DAnalyticsSession
    : GoogleTrackerAnalyticsSession, GoogleTrackerIAnalyticsSession
{
    private const string StorageKeyUniqueId = "GoogleAnalytics.UniqueID";
    private const string StorageKeyFirstVisitTime = "GoogleAnalytics.FirstVisitTime";
    private const string StorageKeyPreviousVisitTime = "GoogleAnalytics.PrevVisitTime";
    private const string StorageKeySessionCount = "GoogleAnalytics.SessionCount";
	
    protected override string GetUniqueVisitorId() 
	{
        if (!SystemPrefUtil.HasLocalSetting(StorageKeyUniqueId))
        {
            SystemPrefUtil.SetLocalSettingString(StorageKeyUniqueId, base.GetUniqueVisitorId());
        }
        return SystemPrefUtil.GetLocalSettingString(StorageKeyUniqueId);
	}
	 
	protected override int GetFirstVisitTime()
    {
        if (!SystemPrefUtil.HasLocalSetting(StorageKeyFirstVisitTime))
        {
            SystemPrefUtil.SetLocalSettingInt(StorageKeyFirstVisitTime, base.GetFirstVisitTime());
        }
        return SystemPrefUtil.GetLocalSettingInt(StorageKeyFirstVisitTime);
	}
		
    protected override int GetPreviousVisitTime()
    {
        if (!SystemPrefUtil.HasLocalSetting(StorageKeyPreviousVisitTime))
        {
            SystemPrefUtil.SetLocalSettingInt(StorageKeyPreviousVisitTime, base.GetPreviousVisitTime());
        }

        var previousVisitTime = SystemPrefUtil.GetLocalSettingInt(StorageKeyPreviousVisitTime);
        SystemPrefUtil.SetLocalSettingInt(StorageKeyPreviousVisitTime, GetCurrentVisitTime());
        return previousVisitTime;
	}
	
    protected override int GetSessionCount()
    {
        if (!SystemPrefUtil.HasLocalSetting(StorageKeySessionCount))
        {
            SystemPrefUtil.SetLocalSettingInt(StorageKeySessionCount, base.GetSessionCount());
        }
        var sessionCount = SystemPrefUtil.GetLocalSettingInt(StorageKeySessionCount);
        SystemPrefUtil.SetLocalSettingInt(StorageKeySessionCount, sessionCount++);
        return sessionCount;
	}
}
