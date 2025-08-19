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

    // ✅ Final working Web App URL
    private string formURL = "https://script.google.com/macros/s/AKfycbwwhnC5ulr7G6Hjqb9Fv_M69Aw-H0BSprTr0L8Z6-eKq3Tjf3ThbYkoV5KD75o53koz/exec";

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
            designation = designationField.text,   // optional (can be blank)
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
        errorField.fontSize = 60;
        errorField.fontStyle = FontStyles.Normal;
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

        // ❌ Designation check removed (optional now)

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

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Form POST failed: " + www.error + " | Response: " + www.downloadHandler.text);
        }
        else
        {
            Debug.Log("Form posted successfully. Response: " + www.downloadHandler.text);
        }
    }
}
