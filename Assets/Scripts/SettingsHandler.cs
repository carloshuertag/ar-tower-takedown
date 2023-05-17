using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsHandler : MonoBehaviour
{
    public Text progressRate;
    public Text rectWidth;
    public Text rectHeight;
    public Text rectDepth;
    public Text squareWidth;
    public Text squareHeight;
    // Start is called before the first frame update
    void Start()
    {
        float progressRateValue = PlayerPrefs.GetFloat("PROGRESS_RATE");
        if(progressRateValue == 0f)
            progressRateValue = 90.0f;
        progressRate.text += displayPref(progressRateValue.ToString());
        int rectWidthValue = PlayerPrefs.GetInt("RECT_WIDTH");
        if(rectWidthValue == 0)
            rectWidthValue = 10;
        rectWidth.text += displayPref(rectWidthValue.ToString());
        int rectHeightValue = PlayerPrefs.GetInt("RECT_HEIGHT");
        if(rectHeightValue == 0)
            rectHeightValue = 10;
        rectHeight.text += displayPref(rectHeightValue.ToString());
        int rectDepthValue = PlayerPrefs.GetInt("RECT_DEPTH");
        if(rectDepthValue == 0)
            rectDepthValue = 5;
        rectDepth.text += displayPref(rectDepthValue.ToString());
        int squareWidthValue = PlayerPrefs.GetInt("SQUARE_WIDTH");
        if(squareWidthValue == 0)
            squareWidthValue = 6;
        squareWidth.text += displayPref(squareWidthValue.ToString());
        int squareHeightValue = PlayerPrefs.GetInt("SQUARE_HEIGHT");
        if(squareHeightValue == 0)
            squareHeightValue = 20;
        squareHeight.text += displayPref(squareHeightValue.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private String displayPref(String pref) {
        return " (" + pref + ")";
    }

    public void saveProgressRate(String value) {
        if(value == "")
            return;
        PlayerPrefs.SetFloat("PROGRESS_RATE", float.Parse(value));
        progressRate.text = "Finishing progress rate" + displayPref(value);
    }

    public void saveRectWidth(String value) {
        if(value == "")
            return;
        PlayerPrefs.SetInt("RECT_WIDTH", int.Parse(value));
        rectWidth.text = "Width" + displayPref(value);
    }

    public void saveRectHeight(String value) {
        if(value == "")
            return;
        PlayerPrefs.SetInt("RECT_HEIGHT", int.Parse(value));
        rectHeight.text = "Height" + displayPref(value);
    }

    public void saveRectDepth(String value) {
        if(value == "")
            return;
        PlayerPrefs.SetInt("RECT_DEPTH", int.Parse(value));
        rectDepth.text = "Depth" + displayPref(value);
    }

    public void saveSquareWidth(String value) {
        if(value == "")
            return;
        PlayerPrefs.SetInt("SQUARE_WIDTH", int.Parse(value));
        squareWidth.text = "Width/Depth" + displayPref(value);
    }

    public void saveSquareHeight(String value) {
        if(value == "")
            return;
        PlayerPrefs.SetInt("SQUARE_HEIGHT", int.Parse(value));
        squareHeight.text = "Height" + displayPref(value);
    }

    public void backToMenu() {
        SceneManager.LoadScene(0);
    }
}
