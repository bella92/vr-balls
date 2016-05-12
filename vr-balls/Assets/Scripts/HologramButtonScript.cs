using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class HologramButtonScript : MonoBehaviour//, ICardboardGazeResponder
{
    public HologramButtonType hologramButtonType;

    private Cardboard cardboard;

    void Start()
    {
        cardboard = FindObjectOfType<Cardboard>();
    }

    //void OnMouseDown()
    //{
    //    ExecuteCommand();
    //}

    private void Play()
    {
        SceneManager.LoadScene("Level1");
    }

    private void SwitchMode()
    {
        Debug.Log("SwitchMode");
        if (Cardboard.SDK != null)
        {
            Debug.Log("SDK");

            cardboard.VRModeEnabled = !cardboard.VRModeEnabled;
        }
    }

    private void Quit()
    {
        Application.Quit();
    }

    //public void OnGazeEnter()
    //{
    //    //throw new NotImplementedException();
    //}

    //public void OnGazeExit()
    //{
    //    //throw new NotImplementedException();
    //}

    //public void OnGazeTrigger()
    //{
    //    Debug.Log("OnGazeTrigger");
    //    ExecuteCommand();
    //}

    public void ExecuteCommand()
    {
        GetComponent<MeshRenderer>().material.color = Color.red;

        switch (hologramButtonType)
        {
            case HologramButtonType.Play:
                Play();
                break;
            case HologramButtonType.SwitchMode:
                SwitchMode();
                break;
            case HologramButtonType.Quit:
                Quit();
                break;
            default:
                throw new ArgumentNullException("HologramButtonType is null");
        }
    }
}
