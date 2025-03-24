using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeScreen : MonoBehaviour
{
    [SerializeField] private AudioSettingsSO m_backgroundAudioSettings;
    [SerializeField] private Button m_playBtn, m_exitBtn;

    private void OnEnable()
    {
        m_playBtn.onClick.AddListener(OnPlayBtnClicked);
        m_exitBtn.onClick.AddListener(OnExitBtnClicked);
    }

    private void OnDisable()
    {
        m_playBtn.onClick.RemoveListener(OnPlayBtnClicked);
        m_exitBtn.onClick.RemoveListener(OnExitBtnClicked);
    }

    private void Start()
    {
        AudioManager.Instance.PlayerMusic(m_backgroundAudioSettings);
    }

    private void OnPlayBtnClicked()
    {
        AudioManager.Instance.PlayButtonClick();
        SceneManager.LoadScene("GameSCene");
    }

    private void OnExitBtnClicked()
    {
        AudioManager.Instance.PlayButtonClick();
        Application.Quit();
    }
}