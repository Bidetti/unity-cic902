using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    public Camera mainCamera;
    private ParallaxLayer[] parallaxLayers;
    private Vector3 previousCameraPosition;

    void Start()
    {
        parallaxLayers = GetComponentsInChildren<ParallaxLayer>();
        previousCameraPosition = mainCamera.transform.position;
    }

    void Update()
    {
        Vector3 currentCameraPosition = mainCamera.transform.position;
        float deltaX = currentCameraPosition.x - previousCameraPosition.x;
        float deltaY = currentCameraPosition.y - previousCameraPosition.y;

        if (deltaX != 0 || deltaY != 0)
        {
            foreach (ParallaxLayer layer in parallaxLayers)
            {
                layer.Move(deltaX, deltaY);
            }
        }

        previousCameraPosition = currentCameraPosition;
    }
}
