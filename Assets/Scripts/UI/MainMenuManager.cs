using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace Gomoku.UI
{
    /// <summary>
    /// 主菜单管理器
    /// </summary>
    public class MainMenuManager : MonoBehaviour
    {
        [Header("UI 组件")]
        [SerializeField] private Button startGameButton;
        [SerializeField] private Button exitGameButton;
        [SerializeField] private TextMeshProUGUI titleText;
        
        private void Start()
        {
            InitializeUI();
            SetupButtonListeners();
        }
        
        private void InitializeUI()
        {
            // 设置标题文本
            if (titleText != null)
            {
                titleText.text = "五子棋游戏";
            }
        }
        
        private void SetupButtonListeners()
        {
            if (startGameButton != null)
            {
                startGameButton.onClick.AddListener(StartGame);
            }
            
            if (exitGameButton != null)
            {
                exitGameButton.onClick.AddListener(ExitGame);
            }
        }
        
        private void StartGame()
        {
            Debug.Log("开始游戏");
            SceneManager.LoadScene("GameScene");
        }
        
        private void ExitGame()
        {
            Debug.Log("退出游戏");
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        
        private void OnDestroy()
        {
            // 清理事件监听
            if (startGameButton != null)
            {
                startGameButton.onClick.RemoveListener(StartGame);
            }
            
            if (exitGameButton != null)
            {
                exitGameButton.onClick.RemoveListener(ExitGame);
            }
        }
    }
}