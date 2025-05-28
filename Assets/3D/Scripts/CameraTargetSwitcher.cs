using UnityEngine;
using Unity.Cinemachine;

public class CameraTargetSwitcher : MonoBehaviour
{
    private CinemachineFreeLook freeLook;
    public Transform playerTarget;
    public Transform bossTarget;
    private bool lockedOn;

    void Awake()
    {
        freeLook = GetComponent<CinemachineFreeLook>();
    }

    void Start()
    {
        freeLook.Follow = playerTarget;
        freeLook.LookAt  = playerTarget;
        FindObjectOfType<InputReader>().TargetEvent += ToggleLock;
    }

    void ToggleLock()
    {
        lockedOn = !lockedOn;
        var t = lockedOn ? bossTarget : playerTarget;
        freeLook.Follow = t;
        freeLook.LookAt  = t;
    }
}
