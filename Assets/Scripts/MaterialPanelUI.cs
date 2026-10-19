
using UnityEngine;
using TMPro;

public class MaterialPanelUI : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text titleText;
    public TMP_Text contentText;

    public void Show(MaterialData data)
    {
        titleText.text = data.title;
        contentText.text = data.content;
        panel.SetActive(true);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}
