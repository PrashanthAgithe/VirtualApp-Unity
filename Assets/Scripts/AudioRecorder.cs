using UnityEngine;
using System;
using System.IO;
using System.Collections;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
 
public class AudioRecorder : MonoBehaviour
{
    private AudioClip recordedClip;
    private string microphoneDevice;
    private bool isRecording = false;
    private int startSample;
    private float startTime;

    void Start()
    {
        if (Microphone.devices.Length > 0)
        {
            microphoneDevice = Microphone.devices[0];
            Debug.Log("Using Microphone: " + microphoneDevice);
        }
        else
        {
            Debug.LogError("No microphone detected!");
        }
    }

    public void StartRecording()
    {
        if (microphoneDevice == null) return;

        // Start recording with a large buffer (e.g., 300 sec)
        recordedClip = Microphone.Start(microphoneDevice, false, 300, 44100);
        isRecording = true;
        startTime = Time.time;
        MessageManager.Instance.ShowMessage("🎙️Recording started...");
        Debug.Log("🎙️Recording started...");
    }

    public void StopRecording()
    {
        MessageManager.Instance.ShowMessage("⏹️Recording stopped \n Processing...");
        Debug.Log("⏹️StopRecording() triggered");
        if (!isRecording) return;

        int endPosition = Microphone.GetPosition(microphoneDevice);
        Microphone.End(microphoneDevice);
        isRecording = false;

        // Calculate how many samples were recorded
        int samplesRecorded = endPosition;
        Debug.Log("Samples recorded: " + samplesRecorded);

        // Copy only the recorded part into a new trimmed clip
        float[] samples = new float[samplesRecorded * recordedClip.channels];
        recordedClip.GetData(samples, 0);

        AudioClip trimmedClip = AudioClip.Create(
            "TrimmedClip",
            samplesRecorded,
            recordedClip.channels,
            recordedClip.frequency,
            false
        );
        trimmedClip.SetData(samples, 0);

        Debug.Log("Recording stopped. Duration: " + (Time.time - startTime) + "s");

        // Optional: save or send directly to LLM later
        SendToLLM(trimmedClip);
        // SaveWav("RecordedAudio.wav", trimmedClip);
        // Debug.Log("Saved trimmed audio to: " + Application.persistentDataPath);
    }

        // --- 🧠 Send Recorded Audio to Gemini API ---
    private void SendToLLM(AudioClip clip)
    {
        StartCoroutine(SendAudioToGeminiCoroutine(clip));
    }

    private IEnumerator SendAudioToGeminiCoroutine(AudioClip clip)
    {
        Debug.Log("Preparing audio for Gemini API...");

        // 1️⃣ Convert AudioClip to WAV bytes
        byte[] wavBytes = WavUtility.FromAudioClip(clip);

        // 2️⃣ Base64 encode the audio
        string base64Audio = Convert.ToBase64String(wavBytes);

        // 3️⃣ Construct JSON request
        string prompt = "This is an audio of a person speaking in the classroom. Please return an array of 2 or 3 questions in JSON format.";

        JObject requestJson = new JObject
        {
            ["contents"] = new JArray
            {
                new JObject
                {
                    ["parts"] = new JArray
                    {
                        new JObject { ["text"] = prompt },
                        new JObject
                        {
                            ["inline_data"] = new JObject
                            {
                                ["mime_type"] = "audio/wav",
                                ["data"] = base64Audio
                            }
                        }
                    }
                }
            }
        };

        string jsonData = requestJson.ToString();
        Debug.Log("Sending audio to Gemini..." + GeminiConfig.API_KEY);

        // 4️⃣ Setup HTTP request
        string url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key=" + GeminiConfig.API_KEY;
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // 5️⃣ Send request
        yield return request.SendWebRequest();

        // 6️⃣ Handle response
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ Response from Gemini: " + request.downloadHandler.text);

            try
            {
                JObject response = JObject.Parse(request.downloadHandler.text);
                string textOutput = (string)response["candidates"]?[0]?["content"]?["parts"]?[0]?["text"];
                Debug.Log("🎯 Gemini Output:\n" + textOutput);
                MessageManager.Instance.ShowMessage("🤖 Gemini Response:\n" + textOutput);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error parsing Gemini response: " + ex.Message);
            }
        }
        else
        {
            Debug.LogError("❌ Request failed: " + request.error);
            Debug.LogError("Response: " + request.downloadHandler.text);
        }
    }

    //--- Helper: Save AudioClip as WAV ---
    void SaveWav(string filename, AudioClip clip)
    {
        var filepath = Path.Combine(Application.persistentDataPath, filename);
        byte[] wavData = WavUtility.FromAudioClip(clip);
        File.WriteAllBytes(filepath, wavData);
    }
}
