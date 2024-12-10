using System.Collections;
using UnityEngine;

public class ObjectiveMessage : MonoBehaviour
{
    public CanvasGroup objectivePanel; // Panel UI untuk pesan awal
    public float fadeDuration = 1f;   // Durasi animasi fade in/out

    private bool hasStarted = false; // Untuk mencegah aksi berulang

    void Start()
    {
        // Panel terlihat di awal
        objectivePanel.alpha = 1f;
        objectivePanel.blocksRaycasts = true;
        objectivePanel.interactable = true;

        // Hentikan gameplay sementara
        Time.timeScale = 0f; // Pause game
    }

    void Update()
    {
        // Tunggu player menekan tombol untuk memulai game
        if (Input.anyKey && !hasStarted)
        {
            hasStarted = true;
            StartCoroutine(FadeOutPanel());
        }
    }

    IEnumerator FadeOutPanel()
    {
        float startAlpha = objectivePanel.alpha;

        for (float t = 0; t < fadeDuration; t += Time.unscaledDeltaTime)
        {
            objectivePanel.alpha = Mathf.Lerp(startAlpha, 0, t / fadeDuration);
            yield return null;
        }

        // Panel dihilangkan sepenuhnya
        objectivePanel.alpha = 0f;
        objectivePanel.blocksRaycasts = false;
        objectivePanel.interactable = false;

        // Lanjutkan gameplay
        Time.timeScale = 1f; // Resume game
    }
}
