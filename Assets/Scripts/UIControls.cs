using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIControls : MonoBehaviour
{
    [SerializeField] private GameManager manager;
    [SerializeField] private MazeController mazeScript;
    [SerializeField] private GameObject controlsContainer;
    [SerializeField] private CanvasGroup pauseScreen;
    [SerializeField] private Sprite ellipse;
    [SerializeField] private Image resumeIcon;  // for a workaround
    [SerializeField] private Image pauseIcon;  // for a workaround
    [SerializeField] private Color lightModeColor;
    [SerializeField] private Color darkModeColor;
    [SerializeField] private Material mazeMat;
    [SerializeField] private Material backgroundMat;
    [SerializeField] private float transitionTime;
    private bool soundOn = false;
    private bool darkModeOn = false;
    private bool timerOn = false;

    void Start()
    {
        DefaultUIControls();
    }

    public void DefaultUIControls()
    {
        ChangeMode();
    }

    public void PauseGame()
    {
        mazeScript.enabled = false;
        Time.timeScale = 0;
        pauseScreen.gameObject.SetActive(true);
        StartCoroutine(FadeCanvasGroup(pauseScreen, 1f, transitionTime)); // Fade in
    }
    public void ResumeGame()
    {
        mazeScript.enabled = true;
        Time.timeScale = 1;
        HidePauseMenu();
    }
    public void RestartGame()
    {
        ResumeGame();
        manager.RespawnBallOutOfMaze();
    }
    public void SwitchGameTheme()
    {
        darkModeOn = !darkModeOn;
        Debug.Log("Switched !!");
        ChangeMode();
    }
    public void ControlGameSound()
    {
        // Enable Sound 
        // Disable Sound 

        soundOn = !soundOn;
    }
    public void ControlGameTimer()
    {
        // Enable Timer 
        // Disable Timer 

        timerOn = !timerOn;
    }
    public void LoadGameScene(int index)
    {
        SceneManager.LoadScene(index);
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    private Color ChangeWithHSV(Color color, float offsetValue)
    {
        float h, s, v;
        Color.RGBToHSV(color, out h, out s, out v);

        v = Mathf.Clamp01(v + offsetValue);

        color = Color.HSVToRGB(h, s, v);

        return color;
    }
    private void ChangeUIMode(Color color)
    {
        Image[] images = controlsContainer.GetComponentsInChildren<Image>(true);

        foreach (Image image in images)
        {
            if (image.sprite == ellipse) image.color = color;
        }
        resumeIcon.color = color;
        pauseIcon.color = color;
    }
    private void ChangeMode()
    {
        Color targetMazeColor;
        Color targetBackgroundColor;
        Color targetUIColor;
        
        if (!darkModeOn)
        {
            targetMazeColor = lightModeColor;
            targetBackgroundColor = ChangeWithHSV(lightModeColor, -0.2f);
            targetUIColor = ChangeWithHSV(lightModeColor, -0.1f);
        }
        else
        {
            targetMazeColor = ChangeWithHSV(darkModeColor, +0.2f);
            targetBackgroundColor = ChangeWithHSV(darkModeColor, +0.1f);
            targetUIColor = ChangeWithHSV(darkModeColor, +0.15f);
        }
        
        // Start separate transitions
        StartCoroutine(TransitionMazeColor(targetMazeColor));
        StartCoroutine(TransitionBackgroundColor(targetBackgroundColor));
        StartCoroutine(TransitionUIColor(targetUIColor));
    }

    private IEnumerator TransitionMazeColor(Color target)
    {
        Color start = mazeMat.color;
        float elapsed = 0f;
        
        while (elapsed < transitionTime)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / transitionTime;
            mazeMat.color = Color.Lerp(start, target, t);
            yield return null;
        }
        
        mazeMat.color = target;
    }

    private IEnumerator TransitionBackgroundColor(Color target)
    {
        Color start = backgroundMat.color;
        float elapsed = 0f;
        
        while (elapsed < transitionTime)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / transitionTime;
            backgroundMat.color = Color.Lerp(start, target, t);
            yield return null;
        }
        
        backgroundMat.color = target;
    }

    private IEnumerator TransitionUIColor(Color target)
    {
        Color start = resumeIcon.color;
        float elapsed = 0f;
        
        while (elapsed < transitionTime)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / transitionTime;
            ChangeUIMode(Color.Lerp(start, target, t));
            yield return null;
        }
        
        ChangeUIMode(target);
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; // Use unscaled if fading during pause
            float t = elapsed / duration;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }
        
        canvasGroup.alpha = targetAlpha;
    }

    public void HidePauseMenu()
    {
        StartCoroutine(FadeOutAndDisable());
    }

    private IEnumerator FadeOutAndDisable()
    {
        yield return StartCoroutine(FadeCanvasGroup(pauseScreen, 0f, transitionTime));
        pauseScreen.gameObject.SetActive(false); // Runs after fade completes
    }
}
