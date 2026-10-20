
using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    public Slider volumeSlider;

    private void OnEnable()
    {
        if (volumeSlider == null)
            return;

        volumeSlider.minValue = 0f;
        volumeSlider.maxValue = 1f;

        volumeSlider.SetValueWithoutNotify(
            AudioManager.MasterVolume
        );

        volumeSlider.onValueChanged.RemoveListener(SetVolume);
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    private void OnDisable()
    {
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveListener(SetVolume);
        }
    }

    private void SetVolume(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMasterVolume(value);
        }
    }
}
