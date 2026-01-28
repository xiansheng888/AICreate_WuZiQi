using UnityEngine;
using System.Collections.Generic;

namespace Gomoku.GameLogic
{
    /// <summary>
    /// AI玩家系统，负责AI决策和移动
    /// </summary>
    public class AIPlayer : MonoBehaviour
    {
        [Header("AI配置")]
        [SerializeField] private PieceType aiPieceType = PieceType.White;
        [SerializeField] private AIDifficulty difficulty = AIDifficulty.Medium;
        [SerializeField] private float thinkTime = 1.0f;
        [SerializeField] private bool useMinimax = true;
        
        [Header("组件引用")]
        [SerializeField] private BoardManager boardManager;
        [SerializeField] private GameRules gameRules;
        
        private bool isThinking = false;
        private Vector2Int pendingMove;
        private System.Random random;
        
        /// <summary>
        /// AI难度枚举
        /// </summary>
        public enum AIDifficulty
        {
            Easy,       // 随机移动
            Medium,     // 简单评估
            Hard,       // 深度搜索
            Expert      // 完整搜索
        }
        
        private void Awake()
        {
            random = new System.Random();
            
            // 自动获取组件引用
            if (boardManager == null)
            {
                boardManager = FindObjectOfType<BoardManager>();
            }
            
            if (gameRules == null)
            {
                gameRules = FindObjectOfType<GameRules>();
                if (gameRules == null)
                {
                    // 如果GameRules不存在，创建一个
                    GameObject rulesObj = new GameObject("GameRules");
                    gameRules = rulesObj.AddComponent<GameRules>();
                }
            }
        }
        
        /// <summary>
        /// 设置AI棋子类型
        /// </summary>
        public void SetPieceType(PieceType pieceType)
        {
            aiPieceType = pieceType;
        }
        
        /// <summary>
        /// 设置AI难度
        /// </summary>
        public void SetDifficulty(AIDifficulty newDifficulty)
        {
            difficulty = newDifficulty;
        }
        
        /// <summary>
        /// 开始思考下一步移动
        /// </summary>
        public void StartThinking()
        {
            if (isThinking) return;
            
            isThinking = true;
            Invoke("CalculateMove", thinkTime);
        }
        
        /// <summary>
        /// 计算AI移动
        /// </summary>
        private void CalculateMove()
        {
            if (boardManager == null || gameRules == null)
            {
                Debug.LogError("AI缺少必要的组件引用");
                isThinking = false;
                return;
            }
            
            Vector2Int move;
            
            switch (difficulty)
            {
                case AIDifficulty.Easy:
                    move = GetRandomMove();
                    break;
                    
                case AIDifficulty.Medium:
                    move = GetHeuristicMove();
                    break;
                    
                case AIDifficulty.Hard:
                    move = GetMinimaxMove(2); // 2层深度
                    break;
                    
                case AIDifficulty.Expert:
                    move = GetMinimaxMove(3); // 3层深度
                    break;
                    
                default:
                    move = GetRandomMove();
                    break;
            }
            
            pendingMove = move;
            isThinking = false;
            
            // 触发移动事件
            OnMoveCalculated?.Invoke(move);
        }
        
        /// <summary>
        /// 获取随机移动
        /// </summary>
        private Vector2Int GetRandomMove()
        {
            List<Vector2Int> validMoves = gameRules.GetAllValidMoves(aiPieceType);
            
            if (validMoves.Count == 0)
            {
                return new Vector2Int(boardManager.BoardSize / 2, boardManager.BoardSize / 2);
            }
            
            // 优先选择中心区域
            List<Vector2Int> centerMoves = new List<Vector2Int>();
            int centerStart = boardManager.BoardSize / 2 - 3;
            int centerEnd = boardManager.BoardSize / 2 + 3;
            
            foreach (var move in validMoves)
            {
                if (move.x >= centerStart && move.x <= centerEnd && 
                    move.y >= centerStart && move.y <= centerEnd)
                {
                    centerMoves.Add(move);
                }
            }
            
            if (centerMoves.Count > 0)
            {
                return centerMoves[random.Next(0, centerMoves.Count)];
            }
            
            return validMoves[random.Next(0, validMoves.Count)];
        }
        
        /// <summary>
        /// 获取启发式移动
        /// </summary>
        private Vector2Int GetHeuristicMove()
        {
            List<Vector2Int> validMoves = gameRules.GetAllValidMoves(aiPieceType);
            
            if (validMoves.Count == 0)
            {
                return new Vector2Int(boardManager.BoardSize / 2, boardManager.BoardSize / 2);
            }
            
            Vector2Int bestMove = validMoves[0];
            int bestScore = int.MinValue;
            
            foreach (var move in validMoves)
            {
                int score = EvaluateMove(move, aiPieceType);
                
                // 检查是否立即获胜
                if (gameRules.CheckWinCondition(move.x, move.y, aiPieceType))
                {
                    score += 100000;
                }
                
                // 检查是否需要防守（对手可能获胜）
                PieceType opponent = aiPieceType == PieceType.Black ? PieceType.White : PieceType.Black;
                if (gameRules.CheckWinCondition(move.x, move.y, opponent))
                {
                    score += 50000;
                }
                
                // 中心区域加分
                int centerX = boardManager.BoardSize / 2;
                int centerY = boardManager.BoardSize / 2;
                int distanceFromCenter = Mathf.Abs(move.x - centerX) + Mathf.Abs(move.y - centerY);
                score += (boardManager.BoardSize - distanceFromCenter) * 10;
                
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = move;
                }
            }
            
            return bestMove;
        }
        
        /// <summary>
        /// 获取Minimax算法移动
        /// </summary>
        private Vector2Int GetMinimaxMove(int depth)
        {
            List<Vector2Int> validMoves = gameRules.GetAllValidMoves(aiPieceType);
            
            if (validMoves.Count == 0)
            {
                return new Vector2Int(boardManager.BoardSize / 2, boardManager.BoardSize / 2);
            }
            
            Vector2Int bestMove = validMoves[0];
            int bestValue = int.MinValue;
            
            // 使用Alpha-Beta剪枝优化
            foreach (var move in validMoves)
            {
                // 模拟落子
                // 注意：这里需要BoardManager支持模拟落子和撤销
                // 暂时使用评估函数代替
                
                int moveValue = Minimax(move, depth - 1, int.MinValue, int.MaxValue, false);
                
                if (moveValue > bestValue)
                {
                    bestValue = moveValue;
                    bestMove = move;
                }
            }
            
            return bestMove;
        }
        
        /// <summary>
        /// Minimax算法实现
        /// </summary>
        private int Minimax(Vector2Int move, int depth, int alpha, int beta, bool isMaximizing)
        {
            if (depth == 0 || gameRules.CheckWinCondition(move.x, move.y, aiPieceType))
            {
                return gameRules.EvaluateBoard(aiPieceType);
            }
            
            if (isMaximizing)
            {
                int maxEval = int.MinValue;
                List<Vector2Int> validMoves = gameRules.GetAllValidMoves(aiPieceType);
                
                foreach (var nextMove in validMoves)
                {
                    int eval = Minimax(nextMove, depth - 1, alpha, beta, false);
                    maxEval = Mathf.Max(maxEval, eval);
                    alpha = Mathf.Max(alpha, eval);
                    
                    if (beta <= alpha)
                        break; // Beta剪枝
                }
                
                return maxEval;
            }
            else
            {
                int minEval = int.MaxValue;
                PieceType opponent = aiPieceType == PieceType.Black ? PieceType.White : PieceType.Black;
                List<Vector2Int> validMoves = gameRules.GetAllValidMoves(opponent);
                
                foreach (var nextMove in validMoves)
                {
                    int eval = Minimax(nextMove, depth - 1, alpha, beta, true);
                    minEval = Mathf.Min(minEval, eval);
                    beta = Mathf.Min(beta, eval);
                    
                    if (beta <= alpha)
                        break; // Alpha剪枝
                }
                
                return minEval;
            }
        }
        
        /// <summary>
        /// 评估移动的分数
        /// </summary>
        private int EvaluateMove(Vector2Int move, PieceType playerType)
        {
            if (boardManager == null || gameRules == null) return 0;
            
            int score = 0;
            
            // 进攻性评估：检查是否能形成有利模式
            score += gameRules.EvaluateBoard(playerType);
            
            // 防守性评估：检查对手的威胁
            PieceType opponent = playerType == PieceType.Black ? PieceType.White : PieceType.Black;
            score -= gameRules.EvaluateBoard(opponent) / 2;
            
            // 位置评估：中心区域更好
            int centerX = boardManager.BoardSize / 2;
            int centerY = boardManager.BoardSize / 2;
            int distanceFromCenter = Mathf.Abs(move.x - centerX) + Mathf.Abs(move.y - centerY);
            score += (boardManager.BoardSize - distanceFromCenter) * 5;
            
            // 连接性评估：靠近已有棋子
            for (int dx = -2; dx <= 2; dx++)
            {
                for (int dy = -2; dy <= 2; dy++)
                {
                    int checkX = move.x + dx;
                    int checkY = move.y + dy;
                    
                    if (boardManager.IsValidPosition(checkX, checkY) && 
                        boardManager.GetPieceType(checkX, checkY) == playerType)
                    {
                        int distance = Mathf.Abs(dx) + Mathf.Abs(dy);
                        score += (3 - distance) * 10; // 越近分数越高
                    }
                }
            }
            
            return score;
        }
        
        /// <summary>
        /// 获取待执行的移动
        /// </summary>
        public Vector2Int GetPendingMove()
        {
            return pendingMove;
        }
        
        /// <summary>
        /// 清除待执行的移动
        /// </summary>
        public void ClearPendingMove()
        {
            pendingMove = new Vector2Int(-1, -1);
        }
        
        /// <summary>
        /// 检查AI是否正在思考
        /// </summary>
        public bool IsThinking()
        {
            return isThinking;
        }
        
        /// <summary>
        /// 停止思考
        /// </summary>
        public void StopThinking()
        {
            isThinking = false;
            CancelInvoke("CalculateMove");
        }
        
        /// <summary>
        /// 设置思考时间
        /// </summary>
        public void SetThinkTime(float time)
        {
            thinkTime = Mathf.Clamp(time, 0.1f, 5.0f);
        }
        
        /// <summary>
        /// AI移动计算完成事件
        /// </summary>
        public event System.Action<Vector2Int> OnMoveCalculated;
    }
}