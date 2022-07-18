using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class pause_menu : MonoBehaviour
{
    bool pausebool = false;
    public Slider slider_sense;
    public GameObject Sphere;
    public GameObject tap_canon;

    void Start()
    {
        PlayerPrefs.SetInt("Totalcoins", 0);
        if (!PlayerPrefs.HasKey("sense"))
        { PlayerPrefs.SetFloat("sense", slider_sense.value); }
        slider_sense.value = PlayerPrefs.GetFloat("sense");
    }
    public void pause()
    {
        if (pausebool) {
            tap_canon.SetActive(true);
            Time.timeScale = 1f; }
        else {
            tap_canon.SetActive(false);
            Time.timeScale = 0f; }
        pausebool = !pausebool;
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }
    public void quit() => Application.Quit();
    public void sense_slid() => PlayerPrefs.SetFloat("sense", slider_sense.value);

}
