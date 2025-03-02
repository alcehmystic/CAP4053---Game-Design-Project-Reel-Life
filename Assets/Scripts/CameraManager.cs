using UnityEngine;
using Cinemachine;

public class CinemachineCameraManager : MonoBehaviour
{
    private static CinemachineCameraManager instance;
    private CinemachineVirtualCamera virtualCamera;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    private void Start()
    {
        AssignPlayerToCamera();
    }

    public void AssignPlayerToCamera()
    {
        if (Player.Instance != null)
        {
            virtualCamera.Follow = Player.Instance.transform;
       
        }
    }
}