using UnityEngine;
using UnityEngine.UI;

public class LevelUpUI : MonoBehaviour
{
    public Text levelUpText;
    public Text levelText;

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