using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Gomoku.GameLogic;
using Gomoku.Managers;

namespace Gomoku.UI
{
    /// <summary>
    /// 游戏界面管理器，控制游戏流程
    /// </summary>
    public class GameUIManager : MonoBehaviour
    {
        [Header("UI组件")]
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private TextMeshProUGUI currentPlayerText;
        [SerializeField] private TextMeshProUGUI gameStatusText;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TextMeshProUGUI winnerText;
        
        [Header("游戏管理器引用")]
        [SerializeField] private BoardManager boardManager;
        [SerializeField] private InputManager inputManager;
        [SerializeField] private AIPlayer aiPlayer;
        
        private GameState currentGameState = GameState.Playing;
        private PieceType currentPlayer = PieceType.Black; // 黑棋先手
        
        public GameState CurrentGameState => currentGameState;
        public PieceType CurrentPlayer => currentPlayer;
        
        private void Awake()
        {
            Debug.Log("GameUIManager Awake");
        }
        
        private void Start()
        {
            Debug.Log("GameUIManager Start");
            InitializeUI();
            SetupEventListeners();
            StartNewGame();
        }
        
        /// <summary>
        /// 初始化UI
        /// </summary>
        private void InitializeUI()
        {
            Debug.Log("初始化游戏UI");
            
            // 隐藏游戏结束面板
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
            }
            
            // 初始化棋盘管理器引用
            if (boardManager == null)
            {
                boardManager = FindObjectOfType<BoardManager>();
                if (boardManager == null)
                {
                    Debug.LogError("未找到棋盘管理器组件");
                }
                else
                {
                    Debug.Log($"找到棋盘管理器: {boardManager.gameObject.name}");
                }
            }
            
            // 初始化输入管理器引用
            if (inputManager == null)
            {
                inputManager = FindObjectOfType<InputManager>();
                if (inputManager == null)
                {
                    Debug.LogError("未找到输入管理器组件");
                }
                else
                {
                    Debug.Log($"找到输入管理器: {inputManager.gameObject.name}");
                }
            }
            
            // 初始化AI玩家引用
            if (aiPlayer == null)
            {
                aiPlayer = FindObjectOfType<AIPlayer>();
                if (aiPlayer == null)
                {
                    Debug.LogWarning("未找到AI玩家组件");
                }
                else
                {
                    Debug.Log($"找到AI玩家: {aiPlayer.gameObject.name}");
                    // 订阅AI移动完成事件
                    aiPlayer.OnMoveCalculated += HandleAIMoveCalculated;
                }
            }
            
            UpdateCurrentPlayerDisplay();
            UpdateGameStatusDisplay();
        }
        
        /// <summary>
        /// 设置事件监听
        /// </summary>
        private void SetupEventListeners()
        {
            if (restartButton != null)
            {
                restartButton.onClick.AddListener(RestartGame);
            }
            
            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.AddListener(ReturnToMainMenu);
            }
        }
        
        /// <summary>
        /// 开始新游戏
        /// </summary>
        private void StartNewGame()
        {
            Debug.Log("开始新游戏");
            
            // 重置游戏状态
            currentGameState = GameState.Playing;
            currentPlayer = PieceType.Black;
            
            // 重置棋盘
            if (boardManager != null)
            {
                boardManager.ResetBoard();
            }
            
            // 启用输入
            if (inputManager != null)
            {
                inputManager.SetInputEnabled(true);
            }
            
            // 停止AI思考（如果有）
            if (aiPlayer != null)
            {
                aiPlayer.StopThinking();
            }
            
            // 隐藏游戏结束面板
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
            }
            
            UpdateCurrentPlayerDisplay();
            UpdateGameStatusDisplay();
        }
        
        /// <summary>
        /// 处理玩家移动
        /// </summary>
        public void HandlePlayerMove(int x, int y)
        {
            Debug.Log($"GameUIManager.HandlePlayerMove被调用: ({x}, {y})");
            
            if (currentGameState != GameState.Playing)
            {
                Debug.Log($"游戏状态不是Playing: {currentGameState}");
                return;
            }
            
            if (boardManager == null)
            {
                Debug.LogWarning("棋盘管理器未初始化");
                return;
            }
            
            if (!boardManager.IsEmptyPosition(x, y))
            {
                Debug.Log($"位置({x}, {y})已被占用");
                return;
            }
            
            Debug.Log($"准备放置{currentPlayer}棋子在({x}, {y})");
            
            // 放置玩家棋子
            if (boardManager.PlacePiece(x, y, currentPlayer))
            {
                Debug.Log($"成功放置棋子");
                
                // 检查胜利条件
                if (CheckWinCondition(x, y, currentPlayer))
                {
                    Debug.Log($"玩家获胜");
                    // 玩家获胜
                    EndGame(currentPlayer);
                }
                else if (IsBoardFull())
                {
                    Debug.Log($"棋盘已满，平局");
                    // 平局
                    EndGame(GameState.Draw);
                }
                else
                {
                    Debug.Log($"切换到AI回合");
                    // 切换到AI回合
                    SwitchToAITurn();
                }
            }
            else
            {
                Debug.LogWarning($"放置棋子失败");
            }
        }
        
        /// <summary>
        /// 切换到AI回合
        /// </summary>
        private void SwitchToAITurn()
        {
            currentPlayer = PieceType.White;
            UpdateCurrentPlayerDisplay();
            
            // 禁用玩家输入
            if (inputManager != null)
            {
                inputManager.SetInputEnabled(false);
            }
            
            // 使用AI玩家系统执行移动
            if (aiPlayer != null)
            {
                aiPlayer.StartThinking();
            }
            else
            {
                // 如果AI玩家不可用，使用备用方案
                Debug.LogWarning("AI玩家不可用，使用随机AI");
                Invoke("ExecuteAIMove", 1.0f);
            }
        }
        
        /// <summary>
        /// 执行AI移动
        /// </summary>
        private void ExecuteAIMove()
        {
            // 这里应该调用AI系统来获取最佳移动
            // 临时：随机选择一个空位
            Vector2Int aiMove = GetRandomAIMove();
            
            if (boardManager != null && boardManager.IsEmptyPosition(aiMove.x, aiMove.y))
            {
                boardManager.PlacePiece(aiMove.x, aiMove.y, PieceType.White);
                
                // 检查AI胜利条件
                if (CheckWinCondition(aiMove.x, aiMove.y, PieceType.White))
                {
                    // AI获胜
                    EndGame(PieceType.White);
                }
                else if (IsBoardFull())
                {
                    // 平局
                    EndGame(GameState.Draw);
                }
                else
                {
                    // 切换回玩家回合
                    SwitchToPlayerTurn();
                }
            }
        }

        /// <summary>
        /// 处理AI移动计算完成事件
        /// </summary>
        private void HandleAIMoveCalculated(Vector2Int aiMove)
        {
            if (boardManager != null && boardManager.IsEmptyPosition(aiMove.x, aiMove.y))
            {
                boardManager.PlacePiece(aiMove.x, aiMove.y, PieceType.White);
                
                // 检查AI胜利条件
                if (CheckWinCondition(aiMove.x, aiMove.y, PieceType.White))
                {
                    // AI获胜
                    EndGame(PieceType.White);
                }
                else if (IsBoardFull())
                {
                    // 平局
                    EndGame(GameState.Draw);
                }
                else
                {
                    // 切换回玩家回合
                    SwitchToPlayerTurn();
                }
            }
            else
            {
                Debug.LogWarning("AI计算的移动位置无效，使用随机移动");
                // 如果AI移动无效，使用备用方案
                Invoke("ExecuteAIMove", 0.5f);
            }
        }

        /// <summary>
        /// 切换到玩家回合
        /// </summary>
        private void SwitchToPlayerTurn()
        {
            currentPlayer = PieceType.Black;
            UpdateCurrentPlayerDisplay();
            
            // 启用玩家输入
            if (inputManager != null)
            {
                inputManager.SetInputEnabled(true);
            }
        }
        
        /// <summary>
        /// 获取随机AI移动（临时实现）
        /// </summary>
        private Vector2Int GetRandomAIMove()
        {
            if (boardManager == null)
                return new Vector2Int(7, 7);
            
            // 简单随机选择空位
            System.Random random = new System.Random();
            int maxAttempts = 100;
            
            for (int i = 0; i < maxAttempts; i++)
            {
                int x = random.Next(0, boardManager.BoardSize);
                int y = random.Next(0, boardManager.BoardSize);
                
                if (boardManager.IsEmptyPosition(x, y))
                {
                    return new Vector2Int(x, y);
                }
            }
            
            // 如果随机失败，返回中心位置
            return new Vector2Int(boardManager.BoardSize / 2, boardManager.BoardSize / 2);
        }
        
        /// <summary>
        /// 检查胜利条件
        /// </summary>
        private bool CheckWinCondition(int lastX, int lastY, PieceType playerType)
        {
            // 这里应该实现完整的五子连珠检查逻辑
            // 临时简化版本：只检查水平和垂直方向
            
            // 检查水平方向
            int horizontalCount = CountConsecutivePieces(lastX, lastY, playerType, Vector2Int.right);
            if (horizontalCount >= 5) return true;
            
            // 检查垂直方向
            int verticalCount = CountConsecutivePieces(lastX, lastY, playerType, Vector2Int.up);
            if (verticalCount >= 5) return true;
            
            // 检查对角线方向（左上到右下）
            int diagonal1Count = CountConsecutivePieces(lastX, lastY, playerType, new Vector2Int(1, 1));
            if (diagonal1Count >= 5) return true;
            
            // 检查对角线方向（右上到左下）
            int diagonal2Count = CountConsecutivePieces(lastX, lastY, playerType, new Vector2Int(1, -1));
            if (diagonal2Count >= 5) return true;
            
            return false;
        }
        
        /// <summary>
        /// 计算连续棋子数量
        /// </summary>
        private int CountConsecutivePieces(int startX, int startY, PieceType playerType, Vector2Int direction)
        {
            int count = 1;
            
            // 向正方向检查
            for (int i = 1; i < 5; i++)
            {
                int x = startX + direction.x * i;
                int y = startY + direction.y * i;
                
                if (boardManager.GetPieceType(x, y) == playerType)
                    count++;
                else
                    break;
            }
            
            // 向负方向检查
            for (int i = 1; i < 5; i++)
            {
                int x = startX - direction.x * i;
                int y = startY - direction.y * i;
                
                if (boardManager.GetPieceType(x, y) == playerType)
                    count++;
                else
                    break;
            }
            
            return count;
        }
        
        /// <summary>
        /// 检查棋盘是否已满
        /// </summary>
        private bool IsBoardFull()
        {
            if (boardManager == null) return true;
            
            for (int x = 0; x < boardManager.BoardSize; x++)
            {
                for (int y = 0; y < boardManager.BoardSize; y++)
                {
                    if (boardManager.IsEmptyPosition(x, y))
                        return false;
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// 结束游戏
        /// </summary>
        private void EndGame(GameState gameState)
        {
            currentGameState = gameState;
            
            // 禁用输入
            if (inputManager != null)
            {
                inputManager.SetInputEnabled(false);
            }
            
            // 显示游戏结束面板
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
                
                if (winnerText != null)
                {
                    switch (gameState)
                    {
                        case GameState.BlackWin:
                            winnerText.text = "黑棋获胜！";
                            break;
                        case GameState.WhiteWin:
                            winnerText.text = "白棋获胜！";
                            break;
                        case GameState.Draw:
                            winnerText.text = "平局！";
                            break;
                    }
                }
            }
            
            UpdateGameStatusDisplay();
            Debug.Log($"游戏结束: {gameState}");
        }
        
        /// <summary>
        /// 结束游戏（重载方法）
        /// </summary>
        private void EndGame(PieceType winner)
        {
            GameState gameState = winner == PieceType.Black ? GameState.BlackWin : GameState.WhiteWin;
            EndGame(gameState);
        }
        
        /// <summary>
        /// 重新开始游戏
        /// </summary>
        private void RestartGame()
        {
            StartNewGame();
        }
        
        /// <summary>
        /// 返回主菜单
        /// </summary>
        private void ReturnToMainMenu()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
        
        /// <summary>
        /// 更新当前玩家显示
        /// </summary>
        private void UpdateCurrentPlayerDisplay()
        {
            if (currentPlayerText != null)
            {
                string playerName = currentPlayer == PieceType.Black ? "黑棋" : "白棋";
                currentPlayerText.text = $"当前玩家: {playerName}";
            }
        }
        
        /// <summary>
        /// 更新游戏状态显示
        /// </summary>
        private void UpdateGameStatusDisplay()
        {
            if (gameStatusText != null)
            {
                string statusText = currentGameState switch
                {
                    GameState.Playing => "游戏进行中",
                    GameState.BlackWin => "黑棋获胜",
                    GameState.WhiteWin => "白棋获胜",
                    GameState.Draw => "平局",
                    _ => "未知状态"
                };
                gameStatusText.text = statusText;
            }
        }
        
        private void OnDestroy()
        {
            // 清理事件监听
            if (restartButton != null)
            {
                restartButton.onClick.RemoveListener(RestartGame);
            }
            
            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.RemoveListener(ReturnToMainMenu);
            }
            
            // 清理AI玩家事件订阅
            if (aiPlayer != null)
            {
                aiPlayer.OnMoveCalculated -= HandleAIMoveCalculated;
            }
        }
    }
}