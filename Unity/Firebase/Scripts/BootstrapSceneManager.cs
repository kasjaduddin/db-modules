using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapSceneManager : MonoBehaviour {
    [SerializeField] private string firstSceneName;

    private void Start() {
        FirebaseManager.OnFirebaseReady += HandleFirebaseReady;
        FirebaseManager.OnFirebaseError += HandleFirebaseError;
    }

    private void HandleFirebaseReady() {
        Debug.Log($"[BootstrapSceneManager] Firebase ready, loading {firstSceneName}...");
        SceneManager.LoadScene(firstSceneName, LoadSceneMode.Single);
    }

    private void HandleFirebaseError(string error) {
        Debug.LogError($"[BootstrapSceneManager] Firebase init failed: {error}");
    }
}