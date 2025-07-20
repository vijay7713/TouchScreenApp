using UnityEngine;
using TMPro;

public class TouchKeyboardOpener : MonoBehaviour
{
    private TMP_InputField inputField;

    void Awake()
    {
        inputField = GetComponent<TMP_InputField>();

        // Subscribe to OnSelect event
        if (inputField != null)
            inputField.onSelect.AddListener(OpenKeyboard);
    }

    private void OpenKeyboard(string text)
    {
#if !UNITY_EDITOR
        TouchScreenKeyboard.Open(inputField.text, TouchScreenKeyboardType.Default, false, false, false, false);
#endif
    }

    void OnDestroy()
    {
        if (inputField != null)
            inputField.onSelect.RemoveListener(OpenKeyboard);
    }
}
