using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuStuff : MonoBehaviour
{
    public string nextSceneName; // Nama scene berikutnya untuk dimuat

    public void B_LoadScene()
    {
        // Muat scene Loading terlebih dahulu
        SceneManager.LoadScene("LoadingScene"); // Pastikan nama scene ini sesuai
    }

    public void B_QuitGame()
    {
        Application.Quit();
    }
}
