using System;
using Firebase;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseManager : MonoBehaviour {
    public static FirebaseApp App { get; private set; }
    public static bool IsReady { get; private set; } = false;

    public static event Action OnFirebaseReady;
    public static event Action<string> OnFirebaseError;

    private void Awake() {
        DontDestroyOnLoad(gameObject);
        InitializeFirebase();
    }

    private void InitializeFirebase() {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;

            if (dependencyStatus == DependencyStatus.Available) {
                try {
                    App = FirebaseApp.DefaultInstance;
                    IsReady = true;
                    Debug.Log("[FirebaseManager] Firebase initialized successfully.");
                    OnFirebaseReady?.Invoke();
                }
                catch (Exception ex) {
                    Debug.LogError($"[FirebaseManager] Exception during Firebase init: {ex.Message}");
                    OnFirebaseError?.Invoke(ex.Message);
                }
            } else {
                string errorMsg = $"Could not resolve all Firebase dependencies: {dependencyStatus}";
                Debug.LogError($"[FirebaseManager] {errorMsg}");
                OnFirebaseError?.Invoke(errorMsg);
            }
        });
    }
}