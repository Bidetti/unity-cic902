using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpUI : MonoBehaviour
{
    public static LevelUpUI Instance { get; private set; }

    public TextMeshProUGUI levelUpText;
    public TextMeshProUGUI levelText;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        HideLevelUp();
    }

    public void ShowLevelUp(int level)
    {
        levelUpText.text = "LEVEL UP!";
        levelText.text = "Level " + level;
        levelUpText.gameObject.SetActive(true);
        levelText.gameObject.SetActive(true);
        Invoke("HideLevelUp", 3f);
    }

    void HideLevelUp()
    {
        levelUpText.gameObject.SetActive(false);
        levelText.gameObject.SetActive(false);
    }
}