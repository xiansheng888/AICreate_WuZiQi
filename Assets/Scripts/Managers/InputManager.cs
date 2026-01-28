using Gomoku.GameLogic;
using Gomoku.UI;
using UnityEngine;

namespace Gomoku.Managers
{
    /// <summary>
    /// 输入管理器，处理用户输入和交互
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        [Header("输入配置")]
        [SerializeField] private LayerMask boardLayerMask;
        [SerializeField] private Camera mainCamera;
        
        private BoardManager boardManager;
        private GameLogic.BoardManager gameBoardManager;
        private GameUIManager gameUIManager;
        private bool isInputEnabled = true;
        
        public bool IsInputEnabled 
        { 
            get => isInputEnabled; 
            set => isInputEnabled = value; 
        }
        
        private void Awake()
        {
            Debug.Log("InputManager Awake");
        }
        
        private void Start()
        {
            Debug.Log("InputManager Start");
            InitializeComponents();
        }
        
        private void Update()
        {
            if (!isInputEnabled) return;
            
            HandleMouseInput();
        }
        
        /// <summary>
        /// 初始化组件引用
        /// </summary>
        private void InitializeComponents()
        {
            // 获取摄像机
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
                if (mainCamera == null)
                {
                    mainCamera = FindObjectOfType<Camera>();
                }
            }
            
            // 获取棋盘管理器
            gameBoardManager = FindObjectOfType<GameLogic.BoardManager>();
            if (gameBoardManager == null)
            {
                Debug.LogWarning("初始化时未找到棋盘管理器");
                Debug.LogWarning("请确保场景中有BoardManager对象");
            }
            else
            {
                Debug.Log($"找到棋盘管理器: {gameBoardManager.gameObject.name}");
            }
            
            // 获取游戏UI管理器
            gameUIManager = FindObjectOfType<GameUIManager>();
            if (gameUIManager == null)
            {
                Debug.LogWarning("初始化时未找到游戏UI管理器（可能在Start之后创建）");
                Debug.Log("将在第一次点击时再次尝试查找");
            }
            else
            {
                Debug.Log($"找到游戏UI管理器: {gameUIManager.gameObject.name}");
            }
        }
        
        /// <summary>
        /// 处理鼠标输入
        /// </summary>
        private void HandleMouseInput()
        {
            if (Input.GetMouseButtonDown(0)) // 左键点击
            {
                HandleMouseClick();
            }
            else if (Input.GetMouseButton(0)) // 左键按住
            {
                HandleMouseHover();
            }
        }
        
        /// <summary>
        /// 处理鼠标点击事件
        /// </summary>
        private void HandleMouseClick()
        {
            Vector2Int boardPosition = GetBoardPositionFromMouse();
            
            if (boardPosition.x >= 0 && boardPosition.y >= 0)
            {
                // 触发棋子放置事件
                OnCellClicked(boardPosition.x, boardPosition.y);
            }
        }
        
        /// <summary>
        /// 处理鼠标悬停事件
        /// </summary>
        private void HandleMouseHover()
        {
            Vector2Int boardPosition = GetBoardPositionFromMouse();
            
            if (boardPosition.x >= 0 && boardPosition.y >= 0)
            {
                // 触发悬停事件，可以用于显示预览
                OnCellHovered(boardPosition.x, boardPosition.y);
            }
        }
        
        /// <summary>
        /// 从鼠标位置获取棋盘坐标
        /// </summary>
        private Vector2Int GetBoardPositionFromMouse()
        {
            if (mainCamera == null)
            {
                Debug.LogWarning("主摄像机未设置");
                return new Vector2Int(-1, -1);
            }
            
            if (gameBoardManager == null)
            {
                Debug.LogWarning("棋盘管理器未设置");
                return new Vector2Int(-1, -1);
            }
            
            // 获取鼠标射线
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Debug.Log($"鼠标位置: {Input.mousePosition}, 射线: {ray.origin} -> {ray.direction}");
            
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, boardLayerMask))
            {
                Debug.Log($"射线命中: {hit.point}, 命中对象: {hit.collider?.gameObject?.name}");
                // 将击中点转换为棋盘坐标
                Vector2Int boardPos = gameBoardManager.GetBoardPosition(hit.point);
                Debug.Log($"转换为棋盘坐标: ({boardPos.x}, {boardPos.y})");
                return boardPos;
            }
            else
            {
                Debug.Log($"射线未命中棋盘，层遮罩: {boardLayerMask.value}");
                // 调试：显示所有命中的对象
                RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);
                foreach (var h in hits)
                {
                    Debug.Log($"潜在命中: {h.collider?.gameObject?.name}, 层: {h.collider?.gameObject?.layer}");
                }
            }
            
            return new Vector2Int(-1, -1);
        }
        
        /// <summary>
        /// 格子点击事件处理
        /// </summary>
        private void OnCellClicked(int x, int y)
        {
            Debug.Log($"点击了棋盘位置: ({x}, {y})");
            
            // 检查棋盘管理器是否存在
            if (gameBoardManager == null)
            {
                Debug.LogWarning("棋盘管理器未初始化");
                return;
            }
            
            // 检查位置是否为空
            if (!gameBoardManager.IsEmptyPosition(x, y))
            {
                Debug.Log($"位置({x}, {y})已被占用");
                return;
            }
            
            // 检查游戏UI管理器是否存在，如果不存在则尝试重新查找
            if (gameUIManager == null)
            {
                Debug.LogWarning("游戏UI管理器未初始化，尝试重新查找...");
                gameUIManager = FindObjectOfType<GameUIManager>();
                
                if (gameUIManager == null)
                {
                    Debug.LogError("=== 游戏UI管理器未找到！ ===");
                    Debug.LogError("解决方案：");
                    Debug.LogError("1. 确保场景中有GameUIManager对象");
                    Debug.LogError("2. 运行QuickSceneSetup脚本：");
                    Debug.LogError("   - 在场景中创建空GameObject");
                    Debug.LogError("   - 添加QuickSceneSetup组件");
                    Debug.LogError("   - 勾选'setupGameScene'选项");
                    Debug.LogError("3. 或者手动创建GameUIManager：");
                    Debug.LogError("   - 创建空GameObject命名为'GameUIManager'");
                    Debug.LogError("   - 添加GameUIManager脚本组件");
                    return;
                }
                else
                {
                    Debug.Log($"成功找到游戏UI管理器: {gameUIManager.gameObject.name}");
                }
            }
            
            Debug.Log($"通知游戏UI管理器处理玩家移动");
            gameUIManager.HandlePlayerMove(x, y);
        }
        
        /// <summary>
        /// 格子悬停事件处理
        /// </summary>
        private void OnCellHovered(int x, int y)
        {
            // 可以在这里实现悬停效果
            // 比如显示半透明的棋子预览
        }
        
        /// <summary>
        /// 设置摄像机
        /// </summary>
        public void SetCamera(Camera camera)
        {
            mainCamera = camera;
        }
        
        /// <summary>
        /// 设置棋盘层遮罩
        /// </summary>
        public void SetBoardLayerMask(LayerMask layerMask)
        {
            boardLayerMask = layerMask;
        }
        
        /// <summary>
        /// 启用/禁用输入
        /// </summary>
        public void SetInputEnabled(bool enabled)
        {
            isInputEnabled = enabled;
        }
    }
}