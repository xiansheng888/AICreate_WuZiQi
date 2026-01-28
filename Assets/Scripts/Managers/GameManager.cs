using UnityEngine;

namespace Gomoku.Managers
{
    /// <summary>
    /// 游戏主管理器，负责整个游戏的生命周期管理
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        [Header("游戏配置")]
        [SerializeField] private GameObject blackPiecePrefab;
        [SerializeField] private GameObject whitePiecePrefab;
        [SerializeField] private Transform boardParent;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            InitializeGame();
        }
        
        private void InitializeGame()
        {
            Debug.Log("五子棋游戏初始化完成");
        }
    }
}