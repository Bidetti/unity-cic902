using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameManager3D : MonoBehaviour {
    public static GameManager3D Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject endScreenPanel;
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("Áudio")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioClip sceneBgmClip;   // Único clip para toda a cena

    [Header("Configuração")]
    [Tooltip("Tempo em segundos antes de ir ao Menu após morte/vitória")]
    [SerializeField] private float delayBeforeMenu = 3f;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
            return;
        }
    }

    private void Start() {
        // Toca o único BGM em loop durante toda a cena
        if (sceneBgmClip != null) {
            bgmSource.clip = sceneBgmClip;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    public void NotifyPlayerDeath() {
        ShowEndScreen("Game Over");
        StartCoroutine(LoadMainMenuAfterDelay());
    }

    public void NotifyBossDefeat() {
        ShowEndScreen("Victory!");
        StartCoroutine(LoadMainMenuAfterDelay());
    }

    private void ShowEndScreen(string text) {
        messageText.text = text;
        endScreenPanel.SetActive(true);
    }

    private IEnumerator LoadMainMenuAfterDelay() {
        // aguarda tempo configurado, sem depender de outro áudio
        yield return new WaitForSecondsRealtime(delayBeforeMenu);

        // garante que o tempo esteja normalizado
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main");
    }
}
