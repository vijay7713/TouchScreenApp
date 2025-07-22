using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
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
    public GameObject videoSelectionPanel;
    public GameObject formPanel;
    public GameObject thankYouPanel;
    public GameObject areYouReadyPanel;
    public GameObject feedbackPanel;

    [Header("Video Player Setup")]
    public VideoPlayer videoPlayer;
    public VideoClip[] videoClips;
    public RawImage videoDisplay;
    private CanvasGroup videoCanvasGroup;

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

    [Header("Feedback")]
    public Button[] starButtons;
    private int selectedRating = 0;

    private string formURL = "https://script.google.com/macros/s/AKfycbwatq6_fmcAwtf6BOifqzHvB_eooJWLzuPjCBatkMG0abdwDPN8aJxT_Uy2CVI1AXSq/exec";

    void Start()
    {
        ShowHomePanel();
        videoPlayer.loopPointReached += OnVideoFinished;

        if (videoDisplay != null)
        {
            videoCanvasGroup = videoDisplay.GetComponent<CanvasGroup>();
            if (videoCanvasGroup == null)
            {
                videoCanvasGroup = videoDisplay.gameObject.AddComponent<CanvasGroup>();
            }
            videoCanvasGroup.alpha = 0f;
            videoDisplay.enabled = false;
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        videoPlayer.Stop();
        StartCoroutine(FadeOutVideo());
        ShowVideoSelectionPanel();
    }

    private IEnumerator FadeInVideo()
    {
        if (videoDisplay != null)
        {
            videoDisplay.enabled = true;
            videoCanvasGroup.alpha = 0f;
            float duration = 0.5f;
            float t = 0;
            while (t < duration)
            {
                videoCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t / duration);
                t += Time.deltaTime;
                yield return null;
            }
            videoCanvasGroup.alpha = 1f;
        }
    }

    private IEnumerator FadeOutVideo()
    {
        if (videoDisplay != null)
        {
            float duration = 0.5f;
            float t = 0;
            while (t < duration)
            {
                videoCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t / duration);
                t += Time.deltaTime;
                yield return null;
            }
            videoCanvasGroup.alpha = 0f;
            videoDisplay.enabled = false;
        }
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

    public void AnimatePanelIn(GameObject panel)
    {
        SetAllPanelsInactive();
        panel.SetActive(true);
    }

    public void ShowHomePanel()
    {
        AnimatePanelIn(homePanel);
    }

    public void ShowVideoSelectionPanel()
    {
        AnimatePanelIn(videoSelectionPanel);
    }

    public void ShowFormPanel()
    {
        ClearFormFields();
        AnimatePanelIn(formPanel);
    }

    public void ShowThankYouPanel()
    {
        AnimatePanelIn(thankYouPanel);
    }

    public void ShowAreYouReadyPanel()
    {
        AnimatePanelIn(areYouReadyPanel);
    }

    public void ShowFeedbackPanel()
    {
        selectedRating = 0;
        foreach (Button star in starButtons)
        {
            star.image.color = Color.white;
        }
        AnimatePanelIn(feedbackPanel);
    }

    public void OnExploreButton_Click()
    {
        ShowVideoSelectionPanel();
    }

    public void OnVideoButton_Click(int index)
    {
        if (index >= 0 && index < videoClips.Length)
        {
            videoPlayer.clip = videoClips[index];
            videoPlayer.Play();
            StartCoroutine(FadeInVideo());
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

    public void OnStarClicked(int starNumber)
    {
        selectedRating = starNumber;
        for (int i = 0; i < starButtons.Length; i++)
        {
            starButtons[i].image.color = (i < starNumber) ? Color.yellow : Color.white;
        }
    }

    public void OnSubmitFeedback_Click()
    {
        if (selectedRating > 0)
        {
            StartCoroutine(PostFeedbackData(selectedRating));
            ShowThankYouPanel();
        }
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

    // ✅ Save form locally + send online
    IEnumerator PostFormData(FormData data)
    {
        string jsonData = JsonUtility.ToJson(data);

        // Save locally
        string path = Path.Combine(Application.streamingAssetsPath, "formdata.json");
        File.AppendAllText(path, jsonData + ",\n");

        // Send to Google Sheet
        UnityWebRequest www = new UnityWebRequest(formURL, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();
        Debug.Log("Form posted: " + www.downloadHandler.text);
    }

    // ✅ Save feedback locally + send online
    IEnumerator PostFeedbackData(int rating)
    {
        string jsonData = "{\"starRating\":" + rating + "}";

        // Save locally
        string path = Path.Combine(Application.streamingAssetsPath, "feedback.json");
        File.AppendAllText(path, jsonData + ",\n");

        // Send to Google Sheet
        UnityWebRequest www = new UnityWebRequest(formURL, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();
        Debug.Log("Feedback posted: " + www.downloadHandler.text);
    }
}
