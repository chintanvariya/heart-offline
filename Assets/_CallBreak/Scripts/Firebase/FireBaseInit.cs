using System.Collections;
using System.Collections.Generic;
using Firebase;
using UnityEngine;

public class FireBaseInit : MonoBehaviour
{
    private FirebaseApp app;

    public static FireBaseInit instance;

    public static bool isFirebaseInit;

    public static System.Action<string, string, int> CallFireBaseEvent;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        FireBaseInitialize();
    }

    private void OnEnable()
    {
        CallFireBaseEvent += FirelogEvent;
    }

    private void OnDisable()
    {
        CallFireBaseEvent -= FirelogEvent;
    }


    public void FirelogEvent(string s1, string s2, int Num)
    {
        if (isFirebaseInit)
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent(s1, s2, Num);
        }
    }

    public void FireBaseInitialize()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {

                app = Firebase.FirebaseApp.DefaultInstance;
                Debug.Log("Firebase Successful Init");
                isFirebaseInit = true;

                //FireBaseInitRemote.instance.DataFetch();
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }
}
