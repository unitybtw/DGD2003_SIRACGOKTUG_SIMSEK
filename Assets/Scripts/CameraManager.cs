using UnityEngine;
using Unity.Cinemachine; 

public class CameraManager : MonoBehaviour
{
    [Header("Kameraları Buraya Sürükle")]
    public CinemachineCamera ghostCam;
    public CinemachineCamera cctvCam;
    public CinemachineCamera studentCam;

    void Start()
    {
        // Oyun başladığında otomatik olarak Ghost kamerasına geç
        SwitchCamera(ghostCam);
    }

    void Update()
    {
        // 7, 8 ve 9 tuşlarıyla geçiş yap
        if (Input.GetKeyDown(KeyCode.Alpha7)) SwitchCamera(ghostCam);
        if (Input.GetKeyDown(KeyCode.Alpha8)) SwitchCamera(cctvCam);
        if (Input.GetKeyDown(KeyCode.Alpha9)) SwitchCamera(studentCam);
    }

    private void SwitchCamera(CinemachineCamera targetCam)
    {
        // Önce hepsini kapat
        ghostCam.Priority = 0;
        cctvCam.Priority = 0;
        studentCam.Priority = 0;

        // Sadece hedefleneni aç
        targetCam.Priority = 10;
    }
}