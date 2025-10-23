using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Database;
using UnityEngine;

public class RealtimeDatabaseManager : IDataService {
    private DatabaseReference root;

    public RealtimeDatabaseManager() {
        if (FirebaseManager.IsReady) {
            root = FirebaseDatabase.DefaultInstance.RootReference;
        } else {
            FirebaseManager.OnFirebaseReady += () => {
                root = FirebaseDatabase.DefaultInstance.RootReference;
            };
            FirebaseManager.OnFirebaseError += (err) => {
                Debug.LogError($"[RealtimeDatabaseManager] Firebase init error: {err}");
            };
        }
    }

    public async Task AddAsync<T>(string path, T data) {
        try {
            await root.Child(path).SetRawJsonValueAsync(JsonUtility.ToJson(data));
        } catch (Exception ex) {
            Debug.LogError($"[RealtimeDatabaseManager] AddAsync failed: {ex.Message}");
        }
    }

    public async Task<T> GetAsync<T>(string path) {
        try {
            var snapshot = await root.Child(path).GetValueAsync();
            if (snapshot.Exists) {
                return JsonUtility.FromJson<T>(snapshot.GetRawJsonValue());
            }
            Debug.LogWarning($"[RealtimeDatabaseManager] Node not found at {path}");
            return default;
        } catch (Exception ex) {
            Debug.LogError($"[RealtimeDatabaseManager] GetAsync failed: {ex.Message}");
            return default;
        }
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(string path, QueryOptions options = null) {
        try {
            var query = root.Child(path);

            if (options != null) {
                if (!string.IsNullOrEmpty(options.OrderBy)) {
                    query = query.OrderByChild(options.OrderBy);
                }
                if (options.Limit.HasValue) {
                    query = query.LimitToFirst(options.Limit.Value);
                }
            }

            var snapshot = await query.GetValueAsync();
            List<T> results = new();
            foreach (var child in snapshot.Children) {
                results.Add(JsonUtility.FromJson<T>(child.GetRawJsonValue()));
            }
            return results;
        } catch (Exception ex) {
            Debug.LogError($"[RealtimeDatabaseManager] QueryAsync failed: {ex.Message}");
            return new List<T>();
        }
    }

    public async Task UpdateAsync<T>(string path, T data) {
        try {
            await root.Child(path).SetRawJsonValueAsync(JsonUtility.ToJson(data));
        } catch (Exception ex) {
            Debug.LogError($"[RealtimeDatabaseManager] UpdateAsync failed: {ex.Message}");
        }
    }

    public async Task DeleteAsync(string path) {
        try {
            await root.Child(path).RemoveValueAsync();
        } catch (Exception ex) {
            Debug.LogError($"[RealtimeDatabaseManager] DeleteAsync failed: {ex.Message}");
        }
    }
}