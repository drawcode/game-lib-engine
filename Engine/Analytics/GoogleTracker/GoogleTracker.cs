#define UNITY3D
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Net;
using System.Text;

using UnityEngine;

public class GoogleTracker
{
    private const string TrackingAccountConfigurationKey = "GoogleAnalyticsGoogleTracker.TrackingAccount";
    private const string TrackingDomainConfigurationKey = "GoogleAnalyticsGoogleTracker.TrackingDomain";

    const string BeaconUrl = "http://www.google-analytics.com/__utm.gif";
    const string BeaconUrlSsl = "https://ssl.google-analytics.com/__utm.gif";
    const string AnalyticsVersion = "4.3"; // Analytics version - AnalyticsVersion

    private readonly GoogleTrackerUtmeGenerator _UtmeGenerator;

    public string TrackingAccount { get; set; } // utmac
    public string TrackingDomain { get; set; }
    public GoogleTrackerIAnalyticsSession GoogleTrackerAnalyticsSession { get; set; }

    public string Hostname { get; set; }
    public string Language { get; set; }
    public string UserAgent { get; set; }
    public string CharacterSet { get; set; }

    internal GoogleTrackerCustomVariable[] GoogleTrackerCustomVariables { get; set; }

    public bool ThrowOnErrors { get; set; }

    public CookieContainer CookieContainer { get; set; }

    public bool UseSsl { get; set; }
	
	public GoogleTracker(string trackingAccount, string trackingDomain)
		: this(trackingAccount, trackingDomain, new GoogleTrackerUnity3DAnalyticsSession())
    {
			
    }

    public GoogleTracker(string trackingAccount, string trackingDomain, GoogleTrackerIAnalyticsSession googleTrackerAnalyticsSession)
    {
        TrackingAccount = trackingAccount;
        TrackingDomain = trackingDomain;
        GoogleTrackerAnalyticsSession = googleTrackerAnalyticsSession;
        string hostname = "";
#if NETWORK_USE_UNITY
        hostname = Network.player.ipAddress;
#endif
        string osversionstring = SystemInfo.operatingSystem;
        string osplatform = Application.platform.ToString();
        string osversion = SystemInfo.deviceModel;

		Hostname = hostname;
        Language = "en";
        UserAgent = string.Format("GoogleTracker/1.0 ({0}; {1}; {2})", osplatform, osversion, osversionstring);
        CookieContainer = new CookieContainer();

        ThrowOnErrors = false;

        InitializeCharset();

        GoogleTrackerCustomVariables = new GoogleTrackerCustomVariable[5];

        _UtmeGenerator = new GoogleTrackerUtmeGenerator(this);
    }

    private void InitializeCharset()
    {
        CharacterSet = "UTF-8";
    }

    private string GenerateUtmn() {
        var random = new System.Random((int)DateTime.UtcNow.Ticks);
        return random.Next(100000000, 999999999).ToString(CultureInfo.InvariantCulture);
    }

    public void SetGoogleTrackerCustomVariable(int position, string name, string value) {
        if (position < 1 || position > 5)
            throw new ArgumentOutOfRangeException(string.Format("position {0} - {1}", position, "Must be between 1 and 5"));

        GoogleTrackerCustomVariables[position - 1] = new GoogleTrackerCustomVariable(name, value);
    }
	
	// UNITY 3D	
	
	public void TrackPageView(string pageTitle, string pageUrl) {
		
		if(Context.Current.hasNetworkAccess) {
			Dictionary<string, string> parameters = new Dictionary<string, string>();
	        parameters.Add("AnalyticsVersion", AnalyticsVersion);
	        parameters.Add("utmn", GenerateUtmn());
	        parameters.Add("utmhn", Hostname);
	        parameters.Add("utmcs", CharacterSet);
	        parameters.Add("utmul", Language);
	        parameters.Add("utmdt", pageTitle);
	        parameters.Add("utmhid", GoogleTrackerAnalyticsSession.GenerateSessionId());
	        parameters.Add("utmp", pageUrl);
	        parameters.Add("utmac", TrackingAccount);
	        parameters.Add("utmcc", GoogleTrackerAnalyticsSession.GenerateCookieValue());
	
	        var utme = _UtmeGenerator.Generate();
	        if (!string.IsNullOrEmpty(utme))
	            parameters.Add("utme", utme);
			
			StringBuilder data = new StringBuilder();
	        foreach (var parameter in parameters) {
	            data.Append(string.Format("{0}={1}&", parameter.Key, Uri.EscapeDataString(parameter.Value)));
	        }
			
			string url = UseSsl ? BeaconUrlSsl : BeaconUrl;
			url += "?" + data;
			
			//LogUtil.Log("TrackPageView: url: " + url);
			
			Engine.Networking.WebRequests.Instance.RequestGet(url, null, HandleTrackPageViewCallback);
		}
	}
	
	void HandleTrackPageViewCallback(Engine.Networking.WebRequests.ResponseObject response) {
		//string responseText = response.www.text;
		//LogUtil.Log("HandleTrackPageViewCallback responseText:" + responseText);		
	}
		
	public void TrackEvent(string category, string action, string label, int val) {
		
		if(Context.Current.hasNetworkAccess) {
			Dictionary<string, string> parameters = new Dictionary<string, string>();
	        parameters.Add("AnalyticsVersion", AnalyticsVersion);
	        parameters.Add("utmn", GenerateUtmn());
	        parameters.Add("utmhn", Hostname);
	        parameters.Add("utmni", "1");
	        parameters.Add("utmt", "event");
	
	        var utme = _UtmeGenerator.Generate();
	        parameters.Add("utme", string.Format("5({0}*{1}*{2})({3})", category, action, label ?? "", val) + utme);
	
	        parameters.Add("utmcs", CharacterSet);
	        parameters.Add("utmul", Language);
	        parameters.Add("utmhid", GoogleTrackerAnalyticsSession.GenerateSessionId());
	        parameters.Add("utmac", TrackingAccount);
	        parameters.Add("utmcc", GoogleTrackerAnalyticsSession.GenerateCookieValue());
						
			StringBuilder data = new StringBuilder();
	        foreach (var parameter in parameters) {
	            data.Append(string.Format("{0}={1}&", parameter.Key, Uri.EscapeDataString(parameter.Value)));
	        }
			
			string url = UseSsl ? BeaconUrlSsl : BeaconUrl;
			url += "?" + data;
			
			//LogUtil.Log("TrackEvent: url: " + url);
			
			Engine.Networking.WebRequests.Instance.RequestGet(url, null, HandleTrackEventCallback);
		}
	}
	
	void HandleTrackEventCallback(Engine.Networking.WebRequests.ResponseObject response) {
		//string responseText = response.www.text;
		//LogUtil.Log("HandleTrackEventCallback responseText:" + responseText);		
	}
		
	public void TrackTransaction(
		string orderId, string storeName, string total, string tax, 
		string shipping, string city, string region, string country){
		
		if(Context.Current.hasNetworkAccess) {
	        Dictionary<string, string> parameters = new Dictionary<string, string>();
	        parameters.Add("AnalyticsVersion", AnalyticsVersion);
	        parameters.Add("utmn", GenerateUtmn());
	        parameters.Add("utmhn", Hostname);
	        parameters.Add("utmt", "event");
	        parameters.Add("utmcs", CharacterSet);
	        parameters.Add("utmul", Language);
	        parameters.Add("utmhid", GoogleTrackerAnalyticsSession.GenerateSessionId());
	        parameters.Add("utmac", TrackingAccount);
	        parameters.Add("utmcc", GoogleTrackerAnalyticsSession.GenerateCookieValue());
	
	        parameters.Add("utmtid", orderId);
	        parameters.Add("utmtst", storeName);
	        parameters.Add("utmtto", total);
	        parameters.Add("utmttx", tax);
	        parameters.Add("utmtsp", shipping);
	        parameters.Add("utmtci", city);
	        parameters.Add("utmtrg", region);
	        parameters.Add("utmtco", country);
			
			StringBuilder data = new StringBuilder();
	        foreach (var parameter in parameters) {
	            data.Append(string.Format("{0}={1}&", parameter.Key, Uri.EscapeDataString(parameter.Value)));
	        }
			
			string url = UseSsl ? BeaconUrlSsl : BeaconUrl;
			url += "?" + data;
			
			//LogUtil.Log("TrackTransaction: url: " + url);
			
			Engine.Networking.WebRequests.Instance.RequestGet(url, null, HandleTrackTransactionCallback);
		}
	}
	
	void HandleTrackTransactionCallback(Engine.Networking.WebRequests.ResponseObject response) {
		//string responseText = response.www.text;
		//LogUtil.Log("HandleTrackTransactionCallback responseText:" + responseText);		
	}
}
