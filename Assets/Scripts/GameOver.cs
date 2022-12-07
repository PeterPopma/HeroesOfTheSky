using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    private TextMeshProUGUI textReason;
    private TextMeshProUGUI textHealthRed;
    private TextMeshProUGUI textHealthBlue;
    private TextMeshProUGUI textWonRed;
    private TextMeshProUGUI textWonBlue;
    private TextMeshProUGUI textDraw;
    private TextMeshProUGUI textDistanceRed;
    private TextMeshProUGUI textDistanceBlue;
    private Image imageFlagBlue;
    private Image imageFlagRed;

    private void OnContinue(InputValue value)
    {
        SceneManager.LoadSceneAsync("GameMenu");
    }

    private void Awake()
    {
        textReason = transform.Find("TextReason").GetComponent<TextMeshProUGUI>();
        textHealthRed = transform.Find("TextHealthRed").GetComponent<TextMeshProUGUI>();
        textHealthBlue = transform.Find("TextHealthBlue").GetComponent<TextMeshProUGUI>();
        textWonRed = transform.Find("TextWonRed").GetComponent<TextMeshProUGUI>();
        textWonBlue = transform.Find("TextWonBlue").GetComponent<TextMeshProUGUI>();
        textDraw = transform.Find("TextDraw").GetComponent<TextMeshProUGUI>();
        imageFlagRed = transform.Find("FlagRed").GetComponent<Image>();
        imageFlagBlue = transform.Find("FlagBlue").GetComponent<Image>();
        textDistanceRed = transform.Find("TextDistanceRed").GetComponent<TextMeshProUGUI>();
        textDistanceBlue = transform.Find("TextDistanceBlue").GetComponent<TextMeshProUGUI>();
    }

    // Start is called before the first frame update
    void Start()
    {
        textReason.text = GameStats.GameOverReason;
        if (GameStats.PlayerWon==0)
        {
            textWonRed.enabled = true;
            textWonBlue.enabled = false;
            textDraw.enabled = false;
            imageFlagRed.enabled = true;
            imageFlagBlue.enabled = false;
        }
        else if(GameStats.PlayerWon == 1)
        {
            textWonRed.enabled = false;
            textWonBlue.enabled = true;
            textDraw.enabled = false;
            imageFlagRed.enabled = false;
            imageFlagBlue.enabled = true;
        }
        else
        {
            textWonRed.enabled = false;
            textWonBlue.enabled = false;
            textDraw.enabled = true;
            imageFlagRed.enabled = false;
            imageFlagBlue.enabled = false;
        }
        textHealthRed.text = GameStats.HealthRed + " %";
        textHealthBlue.text = GameStats.HealthBlue + " %";
        textDistanceRed.text = GameStats.DistanceRed.ToString("0.0");
        textDistanceBlue.text = GameStats.DistanceBlue.ToString("0.0");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
