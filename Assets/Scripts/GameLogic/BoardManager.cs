using UnityEngine;
using System.Collections.Generic;

namespace Gomoku.GameLogic
{
    /// <summary>
    /// 棋盘管理器，负责棋盘的状态管理和显示
    /// </summary>
    public class BoardManager : MonoBehaviour
    {
        [Header("棋盘配置")]
        [SerializeField] private int boardSize = 15;
        [SerializeField] private float cellSize = 1.0f;
        [SerializeField] private GameObject blackPiecePrefab;
        [SerializeField] private GameObject whitePiecePrefab;
        [SerializeField] private Transform boardParent;
        
        [Header("视觉配置")]
        [SerializeField] private Material boardMaterial;
        [SerializeField] private Color gridColor = Color.black;
        
        private PieceType[,] board;
        private Dictionary<Vector2Int, Piece> placedPieces = new Dictionary<Vector2Int, Piece>();
        private LineRenderer gridLineRenderer;
        
        public int BoardSize => boardSize;
        public float CellSize => cellSize;
        
        private void Awake()
        {
            InitializeBoard();
            CreateBoardVisual();
        }
        
        /// <summary>
        /// 初始化棋盘数据
        /// </summary>
        private void InitializeBoard()
        {
            board = new PieceType[boardSize, boardSize];
            
            // 初始化所有位置为空
            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    board[x, y] = PieceType.None;
                }
            }
            
            Debug.Log($"棋盘初始化完成，大小: {boardSize}x{boardSize}");
        }
        
        /// <summary>
        /// 创建棋盘视觉效果
        /// </summary>
        private void CreateBoardVisual()
        {
            CreateBoardBackground();
            CreateGridLines();
        }
        
        /// <summary>
        /// 创建棋盘背景
        /// </summary>
        private void CreateBoardBackground()
        {
            // 创建棋盘背景平面
            GameObject boardBg = GameObject.CreatePrimitive(PrimitiveType.Plane);
            boardBg.name = "BoardBackground";
            boardBg.transform.SetParent(boardParent);
            
            // 设置位置和大小
            float boardWidth = boardSize * cellSize;
            boardBg.transform.position = new Vector3(boardWidth / 2f - cellSize / 2f, 0, boardWidth / 2f - cellSize / 2f);
            boardBg.transform.localScale = new Vector3(boardWidth / 10f, 1f, boardWidth / 10f);
            
            // 设置材质
            if (boardMaterial != null)
            {
                Renderer renderer = boardBg.GetComponent<Renderer>();
                renderer.material = boardMaterial;
            }
            else
            {
                // 默认木质颜色材质
                Renderer renderer = boardBg.GetComponent<Renderer>();
                Material woodMaterial = new Material(Shader.Find("Standard"));
                woodMaterial.color = new Color(0.8f, 0.6f, 0.4f, 1f);
                renderer.material = woodMaterial;
            }
        }
        
        /// <summary>
        /// 创建网格线
        /// </summary>
        private void CreateGridLines()
        {
            // 创建网格线渲染器的父对象
            GameObject gridLinesObj = new GameObject("GridLines");
            gridLinesObj.transform.SetParent(boardParent);
            
            float boardSizeWorld = boardSize * cellSize;
            
            // 创建水平线
            for (int i = 0; i < boardSize; i++)
            {
                CreateSingleLine(
                    gridLinesObj.transform, 
                    new Vector3(0, 0.01f, i * cellSize), 
                    new Vector3((boardSize - 1) * cellSize, 0.01f, i * cellSize),
                    $"HLine_{i}"
                );
            }
            
            // 创建垂直线
            for (int i = 0; i < boardSize; i++)
            {
                CreateSingleLine(
                    gridLinesObj.transform,
                    new Vector3(i * cellSize, 0.01f, 0),
                    new Vector3(i * cellSize, 0.01f, (boardSize - 1) * cellSize),
                    $"VLine_{i}"
                );
            }
        }
        
        /// <summary>
        /// 创建单条线
        /// </summary>
        private void CreateSingleLine(Transform parent, Vector3 start, Vector3 end, string name)
        {
            GameObject lineObj = new GameObject(name);
            lineObj.transform.SetParent(parent);
            
            LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = gridColor;
            lineRenderer.startWidth = 0.02f;
            lineRenderer.endWidth = 0.02f;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);
        }
        
        /// <summary>
        /// 放置棋子
        /// </summary>
        public bool PlacePiece(int x, int y, PieceType pieceType)
        {
            Debug.Log($"尝试放置棋子: {pieceType} 在位置 ({x}, {y})");
            
            // 检查位置有效性
            if (!IsValidPosition(x, y))
            {
                Debug.LogError($"位置无效: ({x}, {y})，棋盘大小: {boardSize}x{boardSize}");
                return false;
            }
            
            if (board[x, y] != PieceType.None)
            {
                Debug.LogWarning($"位置({x}, {y})已被{board[x, y]}占用");
                return false;
            }
            
            // 检查预制件
            GameObject piecePrefab = pieceType == PieceType.Black ? blackPiecePrefab : whitePiecePrefab;
            if (piecePrefab == null)
            {
                Debug.LogError($"{pieceType}棋子预制件未设置！请在BoardManager组件中设置预制件引用");
                return false;
            }
            
            // 检查父对象
            if (boardParent == null)
            {
                Debug.LogWarning("棋盘父对象未设置，棋子将创建在根层级");
            }
            
            // 更新数据
            board[x, y] = pieceType;
            Vector2Int position = new Vector2Int(x, y);
            
            try
            {
                // 创建棋子对象
                Vector3 worldPos = GetWorldPosition(x, y);
                Debug.Log($"在世界位置创建棋子: {worldPos}");
                GameObject pieceObj = Instantiate(piecePrefab, worldPos, Quaternion.identity, boardParent);
                
                if (pieceObj == null)
                {
                    Debug.LogError("实例化棋子对象失败");
                    board[x, y] = PieceType.None; // 回滚
                    return false;
                }
                
                // 设置棋子组件
                Piece pieceComponent = pieceObj.GetComponent<Piece>();
                if (pieceComponent == null)
                {
                    Debug.LogWarning("棋子预制件缺少Piece组件，尝试添加...");
                    pieceComponent = pieceObj.AddComponent<Piece>();
                }
                
                if (pieceComponent != null)
                {
                    pieceComponent.Initialize(pieceType, position);
                    pieceComponent.PlayPlaceAnimation();
                }
                else
                {
                    Debug.LogError("无法创建或获取Piece组件");
                }
                
                // 记录已放置的棋子
                placedPieces[position] = pieceComponent;
                
                Debug.Log($"成功放置棋子: {pieceType} 在位置 ({x}, {y})");
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"放置棋子时发生异常: {ex.Message}\n{ex.StackTrace}");
                board[x, y] = PieceType.None; // 回滚
                return false;
            }
        }
        
        /// <summary>
        /// 获取棋盘位置的世界坐标
        /// </summary>
        public Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(x * cellSize, 0.1f, y * cellSize);
        }
        
        /// <summary>
        /// 将世界坐标转换为棋盘坐标
        /// </summary>
        public Vector2Int GetBoardPosition(Vector3 worldPosition)
        {
            int x = Mathf.RoundToInt(worldPosition.x / cellSize);
            int y = Mathf.RoundToInt(worldPosition.z / cellSize);
            
            return new Vector2Int(x, y);
        }
        
        /// <summary>
        /// 检查位置是否有效
        /// </summary>
        public bool IsValidPosition(int x, int y)
        {
            return x >= 0 && x < boardSize && y >= 0 && y < boardSize;
        }
        
        /// <summary>
        /// 获取指定位置的棋子类型
        /// </summary>
        public PieceType GetPieceType(int x, int y)
        {
            if (!IsValidPosition(x, y))
                return PieceType.None;
                
            return board[x, y];
        }
        
        /// <summary>
        /// 检查位置是否为空
        /// </summary>
        public bool IsEmptyPosition(int x, int y)
        {
            return IsValidPosition(x, y) && board[x, y] == PieceType.None;
        }
        
        /// <summary>
        /// 重置棋盘
        /// </summary>
        public void ResetBoard()
        {
            // 清除所有棋子
            foreach (var piece in placedPieces.Values)
            {
                if (piece != null && piece.gameObject != null)
                {
                    Destroy(piece.gameObject);
                }
            }
            placedPieces.Clear();
            
            // 重置数据
            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    board[x, y] = PieceType.None;
                }
            }
            
            Debug.Log("棋盘已重置");
        }
        
        /// <summary>
        /// 移除指定位置的棋子（用于撤销）
        /// </summary>
        public bool RemovePiece(int x, int y)
        {
            if (!IsValidPosition(x, y) || board[x, y] == PieceType.None)
            {
                return false;
            }
            
            Vector2Int position = new Vector2Int(x, y);
            
            // 移除棋子对象
            if (placedPieces.ContainsKey(position))
            {
                Piece piece = placedPieces[position];
                if (piece != null && piece.gameObject != null)
                {
                    Destroy(piece.gameObject);
                }
                placedPieces.Remove(position);
            }
            
            // 更新数据
            board[x, y] = PieceType.None;
            
            Debug.Log($"移除棋子: 位置 ({x}, {y})");
            return true;
        }
        
        /// <summary>
        /// 模拟落子（不创建可视化对象，用于AI计算）
        /// </summary>
        public bool SimulatePlacePiece(int x, int y, PieceType pieceType)
        {
            if (!IsValidPosition(x, y) || board[x, y] != PieceType.None)
            {
                return false;
            }
            
            // 只更新数据，不创建可视化对象
            board[x, y] = pieceType;
            return true;
        }
        
        /// <summary>
        /// 撤销模拟落子（用于AI计算）
        /// </summary>
        public bool UndoSimulatedPlacePiece(int x, int y)
        {
            if (!IsValidPosition(x, y) || board[x, y] == PieceType.None)
            {
                return false;
            }
            
            // 恢复数据
            board[x, y] = PieceType.None;
            return true;
        }
        
        /// <summary>
        /// 获取棋盘状态的深拷贝（用于AI搜索）
        /// </summary>
        public PieceType[,] GetBoardStateCopy()
        {
            PieceType[,] copy = new PieceType[boardSize, boardSize];
            
            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    copy[x, y] = board[x, y];
                }
            }
            
            return copy;
        }
        
        /// <summary>
        /// 从副本恢复棋盘状态（用于AI搜索）
        /// </summary>
        public void RestoreBoardState(PieceType[,] state)
        {
            if (state.GetLength(0) != boardSize || state.GetLength(1) != boardSize)
            {
                Debug.LogError("棋盘状态副本尺寸不匹配");
                return;
            }
            
            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    board[x, y] = state[x, y];
                }
            }
            
            // 需要同步更新可视化棋子
            // 这是一个简化版本，实际使用时可能需要更复杂的同步
            Debug.Log("棋盘状态已恢复");
        }
        
        /// <summary>
        /// 获取最后放置的棋子位置
        /// </summary>
        public Vector2Int GetLastMove()
        {
            // 简单实现：返回第一个找到的棋子位置
            foreach (var kvp in placedPieces)
            {
                if (kvp.Value != null)
                {
                    return kvp.Key;
                }
            }
            
            return new Vector2Int(-1, -1);
        }
        
        /// <summary>
        /// 获取所有已放置棋子的位置
        /// </summary>
        public List<Vector2Int> GetAllPiecePositions()
        {
            List<Vector2Int> positions = new List<Vector2Int>();
            
            foreach (var position in placedPieces.Keys)
            {
                positions.Add(position);
            }
            
            return positions;
        }
        
        /// <summary>
        /// 获取指定玩家的所有棋子位置
        /// </summary>
        public List<Vector2Int> GetPiecePositionsByType(PieceType pieceType)
        {
            List<Vector2Int> positions = new List<Vector2Int>();
            
            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    if (board[x, y] == pieceType)
                    {
                        positions.Add(new Vector2Int(x, y));
                    }
                }
            }
            
            return positions;
        }
    }
}