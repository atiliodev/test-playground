using UnityEngine;
using System.Collections;

public class UISystem : MonoBehaviour
{
    public GameObject options;
    bool onPause;
    public static float gameVolume = 1;
    bool wait;

    private MeshRenderer meshRenderer;
    private LODGroup lodGroup;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        lodGroup = GetComponent<LODGroup>();
        AudioListener.volume = gameVolume;
        onPause = false;
        wait = false;
    }

    void Update()
    {
        options.SetActive(onPause);

        if (!onPause)
        {
            AudioListener.volume = gameVolume;
        }

        if (!wait && Input.GetKeyUp(KeyCode.Escape))
        {
            if (onPause)
            {
                ActiveResumeGame();
            }
            else
            {
                ActivePauseGame();
            }

            wait = true;
            StartCoroutine(WaitProcess());
        }
    }

    public void ActivePauseGame()
    {
        Time.timeScale = 0;
        AudioListener.volume = 0;
        onPause = true;
    }

    public void ActiveResumeGame()
    {
        Time.timeScale = 1;
        onPause = false;
        AudioListener.volume = gameVolume;
    }

    IEnumerator WaitProcess()
    {
        yield return new WaitForSecondsRealtime(0.5f); // Usar Realtime para funcionar durante Time.timeScale = 0
        wait = false;
    }

    public void SetVolume(float volume)
    {
        gameVolume = volume;
        if (!onPause)
        {
            AudioListener.volume = gameVolume;
        }
    }

    public void SetQuality(int qualityLevel)
    {
        QualitySettings.SetQualityLevel(qualityLevel);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void EnableDetails(bool turn)
    {
        if (lodGroup != null)
        {
            lodGroup.enabled = !turn;
        }
    }

    public void EnableShadows(bool turn)
    {
        if (meshRenderer != null)
        {
            meshRenderer.shadowCastingMode = turn
                ? UnityEngine.Rendering.ShadowCastingMode.Off
                : UnityEngine.Rendering.ShadowCastingMode.On;
        }
    }
}
