using UnityEngine;
using System.Linq;

public class MicrophoneInput : MonoBehaviour {
    private string device;
    private AudioClip clip;
    private const int sampleWindow = 1024;
    private float[] samples = new float[sampleWindow];

    public float volume;
    private Vector3 volumeSpeed;

    void Start() {
        StartMic();
    }

    void Update() {
        volume = getVolume();
        //Vector3 currScale = this.transform.localScale;
        //this.transform.localScale = new Vector3 (currScale.x, currScale.y * volume, currScale.z);
        // this.GameObject.transform.localScale = new Vector3(volume, volume, )
    }

    private void StartMic() {
        if (Microphone.devices.Length > 0) {
            device = Microphone.devices[0];
            clip = Microphone.Start(device, true, 1, AudioSettings.outputSampleRate);
        } else {
            Debug.LogError("no microphone detected!");
        }
    }

    private float getVolume() {
        float vol = 0f;
        if (clip == null) {
            return vol; // 0f
        }
        int micPos = Microphone.GetPosition(device) - sampleWindow;
        if (micPos < 0) {
            return vol; // 0f
        }
        clip.GetData(samples, micPos);
        return samples.Select(Mathf.Abs).Average(); // average absolute amplitude
    }
}



