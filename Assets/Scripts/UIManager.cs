using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using System.Collections;
using UnityEngine.Networking;

[System.Serializable]
public class FormData
{
    public string name;
    public string companyName;
    public string designation;
    public string phoneNumber;
    public string email;
}

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject homePanel;
    public GameObject formPanel;
    public GameObject thankYouPanel;

    [Header("Form Fields")]
    public TMP_InputField nameField;
    public TMP_InputField companyNameField;
    public TMP_InputField designationField;
    public TMP_InputField phoneField;
    public TMP_InputField emailField;

    [Header("Error Messages")]
    public TMP_Text nameError;
    public TMP_Text companyNameError;
    public TMP_Text designationError;
    public TMP_Text phoneError;
    public TMP_Text emailError;

    private string formURL = "https://script.google.com/macros/s/AKfycbx-MJ9MvU0ZkFlP2Y_LNxbgUgx9Gg98gj_E9KlgQUZWfZYCDt79kDC8DkBQI0uW2DcGjA/exec";

    void Start()
    {
        ShowHomePanel();
    }

    private void SetAllPanelsInactive()
    {
        homePanel.SetActive(false);
        formPanel.SetActive(false);
        thankYouPanel.SetActive(false);
    }

    public void ShowHomePanel()
    {
        SetAllPanelsInactive();
        homePanel.SetActive(true);
    }

    public void ShowFormPanel()
    {
        ClearFormFields();
        SetAllPanelsInactive();
        formPanel.SetActive(true);
    }

    public void ShowThankYouPanel()
    {
        SetAllPanelsInactive();
        thankYouPanel.SetActive(true);
    }

    public void OnExploreButton_Click()
    {
        ShowFormPanel();
    }

    public void OnSubmitForm_Click()
    {
        if (!ValidateForm()) return;

        FormData data = new FormData
        {
            name = nameField.text,
            companyName = companyNameField.text,
            designation = designationField.text,
            phoneNumber = phoneField.text,
            email = emailField.text
        };

        StartCoroutine(PostFormData(data));
        ShowThankYouPanel();
    }

    public void OnThankYouHome_Click()
    {
        ShowHomePanel();
    }

    private void ClearFormFields()
    {
        nameField.text = "";
        companyNameField.text = "";
        designationField.text = "";
        phoneField.text = "";
        emailField.text = "";
        ClearAllErrors();
    }

    private void ClearAllErrors()
    {
        nameError.text = "";
        companyNameError.text = "";
        designationError.text = "";
        phoneError.text = "";
        emailError.text = "";
    }

    private void ShowTemporaryError(TMP_Text errorField, string message)
    {
        errorField.text = message;
        errorField.color = Color.red;
        errorField.fontSize = 60; // original size as requested
        errorField.fontStyle = FontStyles.Normal; // not bold
        StartCoroutine(ClearAfterDelay(errorField, 2f));
    }

    IEnumerator ClearAfterDelay(TMP_Text errorField, float delay)
    {
        yield return new WaitForSeconds(delay);
        errorField.text = "";
    }

    private bool ValidateForm()
    {
        bool isValid = true;
        ClearAllErrors();

        if (string.IsNullOrWhiteSpace(nameField.text))
        {
            ShowTemporaryError(nameError, "Please enter your name");
            isValid = false;
        }
        if (string.IsNullOrWhiteSpace(companyNameField.text))
        {
            ShowTemporaryError(companyNameError, "Enter your company name");
            isValid = false;
        }
        if (string.IsNullOrWhiteSpace(designationField.text))
        {
            ShowTemporaryError(designationError, "Enter your designation");
            isValid = false;
        }
        if (!Regex.IsMatch(phoneField.text, "^\\d{10}$"))
        {
            ShowTemporaryError(phoneError, "Enter a valid 10-digit phone");
            isValid = false;
        }
        if (!Regex.IsMatch(emailField.text, "^[^@]+@[^@]+\\.[^@]+$"))
        {
            ShowTemporaryError(emailError, "Enter a valid email");
            isValid = false;
        }

        return isValid;
    }

    IEnumerator PostFormData(FormData data)
    {
        string jsonData = JsonUtility.ToJson(data);
        UnityWebRequest www = new UnityWebRequest(formURL, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();
        Debug.Log("Form posted: " + www.downloadHandler.text);
    }
}
