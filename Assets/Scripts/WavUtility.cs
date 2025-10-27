/*
 * Originally from the Unity Community Wiki
 * http://wiki.unity3d.com/index.php/Saving_AudioClip_as_WAV
 * * This is a public domain utility class for saving AudioClips as WAV files.
 */

using System;
using System.IO;
using UnityEngine;

public class WavUtility
{
    private const int HEADER_SIZE = 44;

    public static byte[] FromAudioClip(AudioClip clip)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            // --- WAV Header ---
            // RIFF chunk
            stream.Write(System.Text.Encoding.UTF8.GetBytes("RIFF"), 0, 4);
            stream.Write(BitConverter.GetBytes(HEADER_SIZE + clip.samples * 2), 0, 4); // File size - 8
            stream.Write(System.Text.Encoding.UTF8.GetBytes("WAVE"), 0, 4);

            // "fmt " sub-chunk (format)
            stream.Write(System.Text.Encoding.UTF8.GetBytes("fmt "), 0, 4);
            stream.Write(BitConverter.GetBytes(16), 0, 4); // Sub-chunk size (16 for PCM)
            stream.Write(BitConverter.GetBytes((ushort)1), 0, 2); // Audio format (1 = PCM)
            stream.Write(BitConverter.GetBytes((ushort)clip.channels), 0, 2);
            stream.Write(BitConverter.GetBytes(clip.frequency), 0, 4);
            stream.Write(BitConverter.GetBytes(clip.frequency * clip.channels * 2), 0, 4); // Byte rate
            stream.Write(BitConverter.GetBytes((ushort)(clip.channels * 2)), 0, 2); // Block align
            stream.Write(BitConverter.GetBytes((ushort)16), 0, 2); // Bits per sample

            // "data" sub-chunk
            stream.Write(System.Text.Encoding.UTF8.GetBytes("data"), 0, 4);
            stream.Write(BitConverter.GetBytes(clip.samples * clip.channels * 2), 0, 4); // Data size

            // --- Audio Data ---
            float[] samples = new float[clip.samples * clip.channels];
            clip.GetData(samples, 0);

            // Convert float samples (-1.0 to 1.0) to 16-bit PCM (short)
            foreach (float sample in samples)
            {
                short pcmSample = (short)(sample * 32767.0f);
                stream.Write(BitConverter.GetBytes(pcmSample), 0, 2);
            }

            return stream.ToArray();
        }
    }
}