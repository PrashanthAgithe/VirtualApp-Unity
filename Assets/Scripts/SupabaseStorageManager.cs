using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SupabaseStorageManager : MonoBehaviour
{
    // Singleton instance
    public static SupabaseStorageManager Instance { get; private set; }

    private void Awake()
    {
        // Ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Uploads an AudioClip to a folder named after the username in the Supabase bucket.
    /// </summary>
    /// <param name="username">The name of the user (used as folder name).</param>
    /// <param name="filename">The specific name for this audio file.</param>
    /// <param name="clip">The AudioClip to upload.</param>
    public void UploadAudio(string username, string filename, AudioClip clip)
    {
        StartCoroutine(UploadToSupabaseRoutine(username, filename, clip));
    }

    private IEnumerator UploadToSupabaseRoutine(string username, string filename, AudioClip clip)
    {
        // 1. Convert AudioClip to WAV bytes using your existing WavUtility
        byte[] wavData = WavUtility.FromAudioClip(clip);

        // 2. Construct the file path using username as the folder
        // Format: username/filename.wav
        string filePath = $"{username}/{filename}";
        
        // 3. Construct the full upload URL
        string uploadUrl = $"{Config.Supabase_URL}/storage/v1/object/{Config.Bucket_Name}/{filePath}";

        using (UnityWebRequest request = new UnityWebRequest(uploadUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(wavData);
            request.downloadHandler = new DownloadHandlerBuffer();

            // 4. Set Headers from your Config file
            request.SetRequestHeader("Authorization", "Bearer " + Config.Supabase_AnonKey);
            request.SetRequestHeader("apikey", Config.Supabase_AnonKey);
            request.SetRequestHeader("Content-Type", "audio/wav");

            Debug.Log($"[Supabase] Uploading to folder '{username}' as {filename}...");
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[Supabase] Upload Failed: {request.error}\n{request.downloadHandler.text}");
                MessageManager.Instance?.ShowMessage("❌ Upload Failed!");
            }
            else
            {
                // On success, the dashboard will now show a folder named after the username
                Debug.Log($"[Supabase] Success! File path: {filePath}");
                MessageManager.Instance?.ShowMessage($"🚀 Uploaded to {username}'s folder!");
            }
        }
    }
}