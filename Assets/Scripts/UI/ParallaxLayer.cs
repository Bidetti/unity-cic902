using UnityEngine;

[ExecuteInEditMode]
public class ParallaxLayer : MonoBehaviour
{
    public float parallaxFactor;
    public bool repeatLayer = false;
    public float layerWidth = 0f;
    public bool enableVerticalParallax = false;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.localPosition;

        if (repeatLayer && layerWidth <= 0f)
        {
            Debug.LogWarning($"Layer '{gameObject.name}' tem repetição ativada, mas 'layerWidth' não foi configurado.");
        }
    }

    public void Move(float deltaX, float deltaY = 0f)
    {
        Vector3 newPos = transform.localPosition;

        newPos.x -= deltaX * parallaxFactor;

        if (enableVerticalParallax)
        {
            newPos.y -= deltaY * parallaxFactor;
        }

        if (repeatLayer && layerWidth > 0f)
        {
            float offset = (newPos.x - startPosition.x) % layerWidth;
            newPos.x = startPosition.x + offset;
        }

        transform.localPosition = newPos;
    }
}
