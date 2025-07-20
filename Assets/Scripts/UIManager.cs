using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class FormData
{
    public string name;
    public string companyName;
    public string designation;
    public string phoneNumber;
    public string email;
}

[System.Serializable]
public class FormDataList
{
    public List<FormData> forms = new List<FormData>();
}

[System.Serializable]
public class FeedbackData
{
    public int starRating;
}

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject homePanel;
    public GameObject videoSelectionPanel;
    public GameObject formPanel;
    public GameObject thankYouPanel;
    public GameObject areYouReadyPanel;
    public GameObject feedbackPanel;

    [Header("Video Player Setup")]
    public VideoPlayer videoPlayer;
    public VideoClip[] videoClips;
    public RawImage videoDisplay;

    [Header("Form Fields")]
    public TMP_InputField nameField;
    public TMP_InputField companyNameField;
    public TMP_InputField designationField;
    public TMP_InputField phoneField;
    public TMP_InputField emailField;

    [Header("Error Texts")]
    public TMP_Text nameError;
    public TMP_Text companyError;
    public TMP_Text designationError;
    public TMP_Text phoneError;
    public TMP_Text emailError;

    [Header("Feedback Stars")]
    public Button[] starButtons; // Assign 5 star buttons
    private int selectedRating = 0;

    void Start()
    {
        ShowHomePanel();
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        videoPlayer.Stop();

        RenderTexture rt = videoPlayer.targetTexture;
        if (rt != null)
        {
            RenderTexture.active = rt;
            GL.Clear(true, true, Color.black);
            RenderTexture.active = null;
        }

        if (videoDisplay != null)
        {
            videoDisplay.enabled = false;
        }

        ShowVideoSelectionPanel();
    }

    private void ClearFormFields()
    {
        nameField.text = "";
        companyNameField.text = "";
        designationField.text = "";
        phoneField.text = "";
        emailField.text = "";

        nameError.text = "";
        companyError.text = "";
        designationError.text = "";
        phoneError.text = "";
        emailError.text = "";
    }

    public void ShowHomePanel()
    {
        SetAllPanelsInactive();
        homePanel.SetActive(true);
        videoPlayer.Stop();
    }

    public void ShowVideoSelectionPanel()
    {
        SetAllPanelsInactive();
        videoSelectionPanel.SetActive(true);
        videoPlayer.Stop();
    }

    public void ShowFormPanel()
    {
        SetAllPanelsInactive();
        ClearFormFields();
        formPanel.SetActive(true);
        videoPlayer.Stop();
    }

    public void ShowThankYouPanel()
    {
        SetAllPanelsInactive();
        thankYouPanel.SetActive(true);
    }

    public void ShowAreYouReadyPanel()
    {
        SetAllPanelsInactive();
        areYouReadyPanel.SetActive(true);
    }

    public void ShowFeedbackPanel()
    {
        SetAllPanelsInactive();
        feedbackPanel.SetActive(true);
    }

    private void SetAllPanelsInactive()
    {
        homePanel.SetActive(false);
        videoSelectionPanel.SetActive(false);
        formPanel.SetActive(false);
        thankYouPanel.SetActive(false);
        areYouReadyPanel.SetActive(false);
        feedbackPanel.SetActive(false);
    }

    public void OnHomeButton_Click()
    {
        ShowHomePanel();
    }

    public void OnExploreButton_Click()
    {
        ShowVideoSelectionPanel();
    }

    public void OnVideoButton_Click(int index)
    {
        if (index >= 0 && index < videoClips.Length)
        {
            if (videoDisplay != null)
            {
                videoDisplay.enabled = true;
            }

            videoPlayer.clip = videoClips[index];
            videoPlayer.Play();
        }
    }

    public void OnGetInTouch_Click()
    {
        ShowAreYouReadyPanel();
    }

    public void OnYesLetsDoItNow_Click()
    {
        ShowFormPanel();
    }

    public void OnMaybeLater_Click()
    {
        ShowFeedbackPanel();
    }

    public void OnSubmitForm_Click()
    {
        if (ValidateForm())
        {
            SaveFormData();
            ShowThankYouPanel();
        }
        else
        {
            Debug.LogWarning("Form validation failed.");
        }
    }

    public void OnThankYouHome_Click()
    {
        ShowHomePanel();
    }

    private bool ValidateForm()
    {
        bool isValid = true;

        string name = nameField.text.Trim();
        string company = companyNameField.text.Trim();
        string designation = designationField.text.Trim();
        string phone = phoneField.text.Trim();
        string email = emailField.text.Trim();

        nameError.text = "";
        companyError.text = "";
        designationError.text = "";
        phoneError.text = "";
        emailError.text = "";

        Color errorColor = Color.red;

        if (string.IsNullOrWhiteSpace(name))
        {
            nameError.text = "Please enter your name.";
            nameError.color = errorColor;
            StartCoroutine(ClearTextAfterDelay(nameError));
            isValid = false;
        }

        if (string.IsNullOrWhiteSpace(company))
        {
            companyError.text = "Please enter your company.";
            companyError.color = errorColor;
            StartCoroutine(ClearTextAfterDelay(companyError));
            isValid = false;
        }

        if (string.IsNullOrWhiteSpace(designation))
        {
            designationError.text = "Please enter your designation.";
            designationError.color = errorColor;
            StartCoroutine(ClearTextAfterDelay(designationError));
            isValid = false;
        }

        if (!Regex.IsMatch(phone, @"^\d{7,15}$"))
        {
            phoneError.text = "Phone must be 7–15 digits.";
            phoneError.color = errorColor;
            StartCoroutine(ClearTextAfterDelay(phoneError));
            isValid = false;
        }

        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            emailError.text = "Invalid email format.";
            emailError.color = errorColor;
            StartCoroutine(ClearTextAfterDelay(emailError));
            isValid = false;
        }

        return isValid;
    }

    private IEnumerator ClearTextAfterDelay(TMP_Text errorField)
    {
        yield return new WaitForSeconds(2f);
        if (errorField != null)
        {
            errorField.text = "";
        }
    }

    private void SaveFormData()
    {
        FormData newData = new FormData
        {
            name = nameField.text.Trim(),
            companyName = companyNameField.text.Trim(),
            designation = designationField.text.Trim(),
            phoneNumber = phoneField.text.Trim(),
            email = emailField.text.Trim()
        };

        string path = Path.Combine(Application.streamingAssetsPath, "formdata.json");
        FormDataList dataList = new FormDataList();

        if (File.Exists(path))
        {
            string existingJson = File.ReadAllText(path);
            dataList = JsonUtility.FromJson<FormDataList>(existingJson);
        }

        dataList.forms.Add(newData);
        string json = JsonUtility.ToJson(dataList, true);

#if UNITY_EDITOR
        File.WriteAllText(path, json);
        Debug.Log("Form data saved to: " + path);
#else
        string fallback = Path.Combine(Application.persistentDataPath, "formdata.json");
        File.WriteAllText(fallback, json);
        Debug.Log("Form data saved to (mobile): " + fallback);
#endif
    }

    // ⭐ Save Star Rating
    public void OnStarClicked(int rating)
    {
        selectedRating = rating;

        // Highlight selected stars
        for (int i = 0; i < starButtons.Length; i++)
        {
            Color color = (i < rating) ? Color.yellow : Color.white;
            starButtons[i].GetComponent<Image>().color = color;
        }

        SaveFeedbackRating(rating);
    }

    private void SaveFeedbackRating(int rating)
    {
        FeedbackData data = new FeedbackData { starRating = rating };
        string path = Path.Combine(Application.streamingAssetsPath, "feedback.json");
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        Debug.Log("⭐ Feedback saved: " + rating + " stars");
    }
}
