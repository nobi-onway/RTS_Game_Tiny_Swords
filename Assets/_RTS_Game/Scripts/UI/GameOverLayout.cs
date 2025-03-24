using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameOverLayout : MonoBehaviour
{
    [SerializeField] private Button m_restartBtn, m_quitBtn;
    [SerializeField] private TextMeshProUGUI m_gameOverText;

    public UnityAction OnRestartClicked = delegate { };
    public UnityAction OnQuitClicked = delegate { };

    private void OnEnable()
    {
        m_restartBtn.onClick.AddListener(RestartGame);
        m_quitBtn.onClick.AddListener(QuitGame);
    }

    private void OnDisable()
    {
        m_restartBtn.onClick.RemoveListener(RestartGame);
        m_quitBtn.onClick.RemoveListener(QuitGame);
    }

    public void ShowGameOver(bool isVictory)
    {
        m_gameOverText.text = isVictory ? "Victory" : "Defeat";
        gameObject.SetActive(true);
    }

    private void RestartGame()
    {
        AudioManager.Instance.PlayButtonClick();
        OnRestartClicked?.Invoke();
    }

    private void QuitGame()
    {
        AudioManager.Instance.PlayButtonClick();
        OnQuitClicked?.Invoke();
    }
}