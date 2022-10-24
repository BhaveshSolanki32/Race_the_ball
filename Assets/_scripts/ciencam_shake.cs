using Cinemachine;
using UnityEngine;

public class ciencam_shake : MonoBehaviour
{
    CinemachineBasicMultiChannelPerlin CVcam_noise;
    public float shake_time;
    public float shake_intensity;
    public bool shakeon = false;
    float shaketime_count;

    private void Start()
    {
        shaketime_count = shake_time;
        CVcam_noise = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }
    private void Update()
    {
        if (shakeon)
        {
            shaketime_count -= Time.deltaTime;
            CVcam_noise.m_AmplitudeGain = Mathf.Lerp(shake_intensity,0,1-(shaketime_count/shake_time));
            
            if (shaketime_count <= 0)
            {
                CVcam_noise.m_AmplitudeGain = 0f;
                shakeon = false;
                shaketime_count = shake_time;
            }
        }
    }
}
