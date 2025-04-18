using UnityEngine;
using TMPro;

public class damageText : MonoBehaviour
{
    public TextMeshProUGUI damage;
    private bool isCritical = false;
    private bool isPlayerDamage = false;
    public int sortingOrder = 10;

    void Start()
    {
        ConfigureVisibility();
        Destroy(gameObject, 1.0f);
        StartCoroutine(AnimateText());
    }

    void ConfigureVisibility()
    {
        Canvas canvas = GetComponentInChildren<Canvas>();
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingOrder = sortingOrder;

            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            if (canvasRect != null)
            {
                canvasRect.sizeDelta = new Vector2(2f, 0.7f);
                canvasRect.localPosition = Vector3.zero;
            }
        }
        else
        {
            TextMeshPro textMeshPro = GetComponentInChildren<TextMeshPro>();
            if (textMeshPro != null)
            {
                textMeshPro.sortingOrder = sortingOrder;
            }
        }

        if (damage != null)
        {
            RectTransform textRect = damage.GetComponent<RectTransform>();
            if (textRect != null)
            {
                textRect.anchorMin = new Vector2(0.5f, 0.5f);
                textRect.anchorMax = new Vector2(0.5f, 0.5f);
                textRect.pivot = new Vector2(0.5f, 0.5f);
                textRect.anchoredPosition = Vector2.zero;
                textRect.sizeDelta = new Vector2(2f, 0.7f);
            }

            damage.alignment = TextAlignmentOptions.Center;
            damage.textWrappingMode = TextWrappingModes.NoWrap;

            damage.overflowMode = TextOverflowModes.Overflow;

            if (damage.canvas != null)
            {
                damage.canvas.sortingOrder = sortingOrder;
            }
        }
    }

    public void SetDamage(float damageAmount, bool critical, bool playerDamage)
    {
        isCritical = critical;
        isPlayerDamage = playerDamage;

        if (damage != null)
        {
            damage.text = damageAmount.ToString();
            damage.alignment = TextAlignmentOptions.Center;

            if (isPlayerDamage)
            {
                if (critical)
                {
                    damage.color = new Color(0.8f, 0f, 1f); // Roxo para dano crítico do inimigo  
                }
                else
                {
                    damage.color = new Color(1f, 0.2f, 0.2f); // Vermelho para dano normal do inimigo  
                }
            }
            else
            {
                if (critical)
                {
                    damage.color = new Color(0f, 0.5f, 1f); // Azul para dano crítico do jogador  
                }
                else
                {
                    damage.color = new Color(0f, 0.8f, 0.2f); // Verde para dano normal do jogador  
                }
            }

            if (isCritical)
            {
                damage.fontSize *= 1.2f;
            }

            ConfigureVisibility();
        }
        else
        {
            Debug.LogError("Componente TextMeshProUGUI não encontrado no prefab damageText!");
        }
    }

    System.Collections.IEnumerator AnimateText()
    {
        float duration = 1.0f;
        float elapsed = 0;

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(0, 0.5f, 0);

        if (isCritical && damage != null)
        {
            damage.transform.localScale *= 1.2f;
        }

        Color startColor = damage.color;
        Color endColor;

        if (isPlayerDamage)
        {
            if (isCritical)
            {
                endColor = new Color(0.9f, 0.5f, 1f, 0f); // Gradiente do roxo para branco com alpha 0  
            }
            else
            {
                endColor = new Color(1f, 0.7f, 0.7f, 0f); // Gradiente do vermelho para branco com alpha 0  
            }
        }
        else
        {
            if (isCritical)
            {
                endColor = new Color(0.5f, 0.7f, 1f, 0f); // Gradiente do azul para branco com alpha 0  
            }
            else
            {
                endColor = new Color(0.7f, 1f, 0.8f, 0f); // Gradiente do verde para branco com alpha 0  
            }
        }

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);

            if (damage != null)
            {
                damage.color = Color.Lerp(startColor, endColor, elapsed / duration);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
