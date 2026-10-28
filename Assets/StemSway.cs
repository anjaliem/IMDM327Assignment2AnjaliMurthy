using UnityEngine;
using System.Collections;
using System.Linq;

public class StemSway : MonoBehaviour {

    public GameObject first;
    public GameObject second;
    public GameObject third;
    public GameObject fourth;
    public GameObject fifth;

    private bool firstb;
    private bool secondb;
    private bool thirdb;
    private bool fourthb;
    private bool fifthb;

    private bool forwards;
/*
    private string device;
    private AudioClip clip;
    private const int sampleWindow = 1024;
    private float[] samples = new float[sampleWindow];

    public float volume;
    private const float threshold = 0.01f;
    */

    void Start() {
        firstb = false;
        secondb = false;
        thirdb = true;
        fourthb = false;
        fifthb = false;
        forwards = true;
        //StartMic();
    }

    void Update() {
        //volume = getVolume() * 500000f;
        //while (volume > threshold) {
            DoSomething();
        //}
    }        

    void DoSomething() {
        if (thirdb && forwards) {
            third.SetActive(!third.activeSelf);
            thirdb = !thirdb;
            fourth.SetActive(!fourth.activeSelf);
            fourthb = !fourthb;
        }
        else if (fourthb && forwards) {
            fourth.SetActive(!fourth.activeSelf);
            fourthb = !fourthb;
            fifth.SetActive(!fifth.activeSelf);
            fifthb = !fifthb;
        }
        else if (fifthb) {
            fifth.SetActive(!fifth.activeSelf);
            fifthb = !fifthb;
            fourth.SetActive(!fourth.activeSelf);
            fourthb = !fourthb;
            forwards = !forwards;
        }
        else if (fourthb && !forwards) {
            fourth.SetActive(!fourth.activeSelf);
            fourthb = !fourthb;
            third.SetActive(!third.activeSelf);
            thirdb = !thirdb;
        }
        else if (thirdb && !forwards) {
            third.SetActive(!third.activeSelf);
            thirdb = !thirdb;
            second.SetActive(!second.activeSelf);
            secondb = !secondb;
        }
        else if (secondb && !forwards) {
            second.SetActive(!second.activeSelf);
            secondb = !secondb;
            first.SetActive(!first.activeSelf);
            firstb = !firstb;
        }
        else if (firstb) {
            first.SetActive(!first.activeSelf);
            firstb = !firstb;
            second.SetActive(!second.activeSelf);
            secondb = !secondb;
            forwards = !forwards;
        }
        else if (secondb && forwards) {
            second.SetActive(!second.activeSelf);
            secondb = !secondb;
            third.SetActive(!third.activeSelf);
            thirdb = !thirdb;
        }
    }
/*
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
    */
}