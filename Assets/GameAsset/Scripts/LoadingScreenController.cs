using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneController : MonoBehaviour
{
    public string nextSceneName;  // Nama scene yang akan dimuat setelah loading
    public Text objectiveText;    // UI Text untuk menampilkan pesan
    public CanvasGroup objectivePanel;  // CanvasGroup untuk kontrol visibilitas panel

    private bool isWaitingForKeyPress = true;

    void Start()
    {
        // Menampilkan pesan tujuan game saat scene loading dimulai
        objectivePanel.alpha = 1f; // Menampilkan panel
        objectiveText.text = "Welcome to the game!\nYour objective is to find the hidden keys and escape the haunted maze.\nPress any key to start.";
    }

    void Update()
    {
        // Mengecek jika pemain menekan tombol apa saja
        if (isWaitingForKeyPress && Input.anyKeyDown)
        {
            StartCoroutine(LoadNextScene());
        }
    }

    // Coroutine untuk menunggu beberapa detik sebelum memuat scene berikutnya
    private IEnumerator LoadNextScene()
    {
        isWaitingForKeyPress = false;

        // Fade out panel (optional)
        float fadeDuration = 1f;
        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            objectivePanel.alpha = 1 - Mathf.Clamp01(time / fadeDuration); // Fade out
            yield return null;
        }

        objectivePanel.alpha = 0f; // Pastikan panel hilang setelah fade out selesai
        SceneManager.LoadScene(nextSceneName);
    }
}
