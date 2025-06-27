using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    [Header("Validation Settings")]
    public InputValidationRule emailValidation;
    public InputValidationRule passwordValidation;
    public ActionButton submitButton;

    private string email;
    private string password;
    private FirebaseLoginService loginService;

    private void Start()
    {
        loginService = new FirebaseLoginService();

        // Susbscribe to events from the form
        if (emailValidation.inputField != null)
            emailValidation.inputField.onEndEdit.AddListener(OnEmailChanged);
        if (passwordValidation.inputField != null)
            passwordValidation.inputField.onEndEdit.AddListener(OnPasswordChanged);
        if (submitButton.button != null)
            submitButton.button.onClick.AddListener(OnSubmitForm);

        // Cache input default sprite image
        if (emailValidation.inputBackground != null)
            emailValidation.defaultSprite = emailValidation.inputBackground.sprite;
        if (passwordValidation.inputBackground != null)
            passwordValidation.defaultSprite = passwordValidation.inputBackground.sprite;

        // Hide the Error text container
        if (emailValidation.errorText != null)
        {
            emailValidation.errorText.text = "";
            emailValidation.errorText.gameObject.SetActive(false);
        }
        if (passwordValidation.errorText != null)
        {
            passwordValidation.errorText.text = "";
            passwordValidation.errorText.gameObject.SetActive(false);
        }
    }

    public void OnEmailChanged(string input)
    {
        if (IsValidEmail(input))
        {
            SetError(emailValidation, null);
            email = input;
        }
        else
        {
            SetError(emailValidation, emailValidation.errorMessage);
        }
    }

    public void OnPasswordChanged(string input)
    {
        if (IsValidPassword(input))
        {
            SetError(passwordValidation, null);
            password = input;
        }
        else
        {
            SetError(passwordValidation, passwordValidation.errorMessage);
        }
    }

    public void OnSubmitForm()
    {
        if (!FirebaseInitializator.IsFirebaseReady)
        {
            submitButton.buttonText.text = "Inicializando App...";
            return;
        }

        submitButton.button.interactable = false;
        submitButton.buttonText.text = "Cargando...";

        StartCoroutine(HandleLogin(email, password));
    }

    private bool IsValidEmail(string email)
    {
        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern);
    }

    private bool IsValidPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        if (password.Length < 8)
            return false;

        if (!Regex.IsMatch(password, "[A-Z]"))
            return false;

        if (!Regex.IsMatch(password, "[a-z]"))
            return false;

        if (!Regex.IsMatch(password, @"[0-9!@#$%^&*]"))
            return false;

        return true;
    }

    private void SetError(InputValidationRule rule, string message)
    {
        // If there is not message (OR Is Null) desappear the
        // TextMeshPro UI GameObject else make appear the 
        // error message
        if (rule.errorText != null)
        {
            bool hasError = !string.IsNullOrEmpty(message);
            rule.errorText.text = hasError ? message : "";
            rule.errorText.gameObject.SetActive(hasError);
        }

        // Change the Image source of the input form
        if (rule.inputBackground != null)
        {
            rule.inputBackground.sprite = string.IsNullOrEmpty(message) ? rule.defaultSprite : rule.errorSprite;
        }
    }

    private IEnumerator HandleLogin(string email, string password)
    {
        var loginTask = loginService.AuthenticateUserAsync(email, password);

        while (!loginTask.IsCompleted)
            yield return null;


        submitButton.buttonText.text = "Iniciar Sesion";
        if (loginTask.Result.success)
        {
            ToastManager.Instance.ShowToast(loginTask.Result.message, 1f);

            yield return new WaitForSeconds(1f);

            LoadingScreenController.targetScene = "Bohr RA";
            SceneManager.LoadScene("LoadingScene");
        }
        else
        {
            ToastManager.Instance.ShowToast(loginTask.Result.message);
            submitButton.button.interactable = true;
        }
    }
}
