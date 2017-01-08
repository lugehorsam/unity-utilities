﻿using System;
using System.Linq;
using UnityEngine;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections;

public class DataSource<TData> : DataSource
    where TData : struct {

    public ReadOnlyCollection<TData> Data {
        get {
            return new ReadOnlyCollection<TData> (shouldOverrideData ? overrideData : data);
        }
    }

    List<TData> data = new List<TData>();

    string CacheKey {
        get {
            return cacheKey;
        }
    }

    [SerializeField]
    private string cacheKey;

    [SerializeField]
    string baseUrl;

    public event Action<TData []> OnDataPublish {
        add {
            onDataPublish += value;
            onDataPublish (data.ToArray());
        }
        remove {
            onDataPublish -= value;
        }
    }

    event Action<TData[]> onDataPublish = (data) => { };

    [SerializeField] protected NetworkConfig networkConfig;
    [SerializeField] private bool shouldOverrideData;
    [SerializeField] private List<TData> overrideData;

    protected IEnumerator FetchData (string endPoint, Dictionary<string, string> postData)
    {

        DataRequest<TData> request = CreateDataRequest (BuildEndpoint(endPoint), postData);
        yield return request;
        HandleDataFetched (request.Data);

        Set (request.Data);
    }

    WebDataRequest<TData> CreateDataRequest (string endpoint, Dictionary<string, string> postData)
    {
        WWWForm form = new WWWForm ();
        if (postData != null) {
            foreach (KeyValuePair<string, string> postDatum in postData) {
                form.AddField (postDatum.Key, postDatum.Value);
            }
        }
        WebDataRequest<TData> request = new WebDataRequest<TData> (endpoint, form);
        return request;
    }

    protected virtual void HandleDataFetched (TData [] data) { }

    public void WriteToCache ()
    {
        if (cacheKey == null || !EnableCache) {
            return;
        }
        string stringToCache = JsonUtility.ToJson (new JsonArray<TData> (data.ToArray ()));

        PlayerPrefs.SetString (cacheKey, stringToCache);
    }

    public bool TryReadFromCache ()
    {
        if (cacheKey == null || !EnableCache) {
            return false;
        }

        if (PlayerPrefs.HasKey (cacheKey)) {
            Set(JsonUtility.FromJson<JsonArray<TData>> (PlayerPrefs.GetString (cacheKey)).Data);

            return true;
        }
        return false;
    }

    public void Set (TData[] data)
    {

        if (this.data.SequenceEqual (data)) {
            return;
        }
        this.data = data.ToList();

        onDataPublish (data);
    }

    public void Push (TData datum)
    {
        List<TData> listCopy = new List<TData> (data);
        listCopy.Add (datum);
        Set (listCopy.ToArray());
    }

    public override string ToString ()
    {
        return string.Format ("[DataSource] {0} {1}", cacheKey, data.ToFormattedString ());
    }

    string BuildEndpoint (string endPoint)
    {
        return string.Format ("{0}/{1}/{2}", networkConfig.UrlBase, baseUrl, endPoint);
    }
}

public class DataSource : ScriptableObject
{
    public static bool EnableCache {
        get {
            return enableCache;
        }
        set {
            enableCache = value;
        }
    }

    static bool enableCache = true;
}
