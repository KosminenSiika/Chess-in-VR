using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Chessbot5000Settings : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI whiteMinutes;
    [SerializeField] private TextMeshProUGUI whiteHours;
    [SerializeField] private TextMeshProUGUI whiteHText;
    [SerializeField] public Slider whiteMinuteSlider;
    [SerializeField] public Slider whiteHourSlider;

    [SerializeField] private TextMeshProUGUI blackMinutes;
    [SerializeField] private TextMeshProUGUI blackHours;
    [SerializeField] private TextMeshProUGUI blackHText;
    [SerializeField] public Slider blackMinuteSlider;
    [SerializeField] public Slider blackHourSlider;

    [SerializeField] private Button reverseButton;

    [SerializeField] public Toggle playerWhiteToggle;

    [SerializeField] private TextMeshProUGUI difficultyValue;
    [SerializeField] public Slider difficultySlider;

    // Called in editor by OnValueChanged in whiteMinuteSlider and whiteHourSlider
    public void UpdateWhiteTime()
    {
        whiteMinutes.text = whiteMinuteSlider.value.ToString();
        whiteHours.text = whiteHourSlider.value.ToString();

        whiteHours.enabled = whiteHourSlider.value == 0 ? false : true;
        whiteHText.enabled = whiteHourSlider.value == 0 ? false : true;
    }

    // Called in editor by OnValueChanged in blackMinuteSlider and blackHourSlider
    public void UpdateBlackTime()
    {
        blackMinutes.text = blackMinuteSlider.value.ToString();
        blackHours.text = blackHourSlider.value.ToString();

        blackHours.enabled = blackHourSlider.value == 0 ? false : true;
        blackHText.enabled = blackHourSlider.value == 0 ? false : true;
    }

    // Called in editor by OnClick in reverseButton
    public void ReverseTimes()
    {
        float temp = whiteMinuteSlider.value;
        whiteMinuteSlider.value = blackMinuteSlider.value;
        blackMinuteSlider.value = temp;

        temp = whiteHourSlider.value;
        whiteHourSlider.value = blackHourSlider.value;
        blackHourSlider.value = temp;
    }

    // Called in editor by OnValueChanged in difficultySlider
    public void UpdateDifficultyValue()
    {
        difficultyValue.text = difficultySlider.value.ToString();
    }
}
