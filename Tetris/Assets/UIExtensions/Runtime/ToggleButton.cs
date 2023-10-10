using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ToggleButton : MonoBehaviour
{
    Button _onButton;
    Button _offButton;
    [SerializeField]
    bool _isOn;

    public bool IsOn { get { return _isOn; } }

    void Awake()
    {
        _onButton = transform.Find("OnButton").GetComponent<Button>();
        _offButton = transform.Find("OffButton").GetComponent<Button>();
        _onButton.onClick.AddListener(OnClickButton);
        _offButton.onClick.AddListener(OnClickButton);
        ApplyToggleToUI();
    }

    void OnValidate()
    {
        _onButton = transform.Find("OnButton").GetComponent<Button>();
        _offButton = transform.Find("OffButton").GetComponent<Button>();
        ApplyToggleToUI();
    }

    public void SetToggle(bool isOn)
    {
        _isOn = isOn;
        ApplyToggleToUI();
    }

    void OnClickButton()
    {
        _isOn = !_isOn;
        ApplyToggleToUI();
    }

    void ApplyToggleToUI()
    {
        _onButton.gameObject.SetActive(_isOn);
        _offButton.gameObject.SetActive(!_isOn);
    }
}
