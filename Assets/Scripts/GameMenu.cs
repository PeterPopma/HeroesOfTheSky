using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textWind;
    [SerializeField] private TextMeshProUGUI textGraphics;

    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("CanvasExplanation").GetComponent<Canvas>().enabled = false;
        GameObject.Find("CanvasSettings").GetComponent<Canvas>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSettingsClick()
    {
        GameObject.Find("CanvasMainMenu").GetComponent<Canvas>().enabled = false;
        GameObject.Find("CanvasSettings").GetComponent<Canvas>().enabled = true;
    }

    public void OnExplanationClick()
    {
        GameObject.Find("CanvasMainMenu").GetComponent<Canvas>().enabled = false;
        GameObject.Find("CanvasExplanation").GetComponent<Canvas>().enabled = true;
    }

    public void OnToggleGraphicsClick()
    {
        GlobalParams.DetailedGraphics = !GlobalParams.DetailedGraphics;
        if (GlobalParams.DetailedGraphics)
        {
            textGraphics.text = "High";
        }
        else
        {
            textGraphics.text = "Low";
        }
    }

    public void OnToggleWindClick()
    {
        if (GlobalParams.WindStrength==0)
        {
            GlobalParams.WindStrength = 10;
            textWind.text = "On";
        }
        else
        {
            GlobalParams.WindStrength = 0;
            textWind.text = "Off";
        }
    }

    public void OnBackToMenuClick()
    {
        GameObject.Find("CanvasExplanation").GetComponent<Canvas>().enabled = false;
        GameObject.Find("CanvasSettings").GetComponent<Canvas>().enabled = false;
        GameObject.Find("CanvasMainMenu").GetComponent<Canvas>().enabled = true;
    }

    public void OnStartButtonClick()
    {
        //        GlobalParams.GameMode = selectedGameMode;
//        textLoading.enabled = true;
        SceneManager.LoadSceneAsync("Scene1");

    }

    public void OnExitGameClick()
    {
        Application.Quit();
    }
}
