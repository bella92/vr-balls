using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class MenuHologramScript : MonoBehaviour
{
    public MenuHologramType menuHologramType;

    void OnMouseDown()
    {
        switch (menuHologramType)
        {
            case MenuHologramType.Play:
                Play();
                break;
            case MenuHologramType.SwitchMode:
                SwitchMode();
                break;
            case MenuHologramType.Quit:
                Quit();
                break;
            default:
                throw new ArgumentNullException("MenuHologramType is null");
        }
    }

    private void Play()
    {
        SceneManager.LoadScene("Level1");
    }

    private void SwitchMode()
    {
        Cardboard.SDK.VRModeEnabled = !Cardboard.SDK.VRModeEnabled;
    }

    private void Quit()
    {
        Application.Quit();
    }
}
