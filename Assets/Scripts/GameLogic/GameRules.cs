using UnityEngine;
using System.Collections.Generic;

namespace Gomoku.GameLogic
{
    /// <summary>
    /// 游戏规则管理器，负责胜负判定和游戏规则执行
    /// </summary>
    public class GameRules : MonoBehaviour
    {
        [Header("规则配置")]
        [SerializeField] private int winCondition = 5; // 连子数获胜条件
        [SerializeField] private bool enableForbiddenMoves = false; // 是否启用禁手规则
        
        private BoardManager boardManager;
        
        private void Awake()
        {
            boardManager = GetComponent<BoardManager>();
            if (boardManager == null)
            {
                boardManager = FindObjectOfType<BoardManager>();
            }
        }
        
        /// <summary>
        /// 检查落子是否有效
        /// </summary>
        public bool IsValidMove(int x, int y, PieceType pieceType)
        {
            if (boardManager == null) return false;
            
            // 检查位置是否有效
            if (!boardManager.IsValidPosition(x, y)) return false;
            
            // 检查位置是否为空
            if (!boardManager.IsEmptyPosition(x, y)) return false;
            
            // 如果启用禁手规则，检查是否为禁手
            if (enableForbiddenMoves && pieceType == PieceType.Black)
            {
                if (IsForbiddenMove(x, y, pieceType))
                {
                    return false;
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// 检查是否为禁手（黑棋专用）
        /// </summary>
        private bool IsForbiddenMove(int x, int y, PieceType pieceType)
        {
            if (pieceType != PieceType.Black) return false;
            if (boardManager == null) return false;
            
            // 暂时禁用禁手检查，后续可以完善
            // 禁手规则包括：长连禁手、双活三禁手、四四禁手、三三禁手等
            return false;
            
            /*
            // 完整的禁手检查实现示例：
            // 1. 保存当前棋盘状态
            PieceType[,] savedState = boardManager.GetBoardStateCopy();
            
            // 2. 模拟落子
            bool placed = boardManager.SimulatePlacePiece(x, y, pieceType);
            if (!placed) return false;
            
            // 3. 检查禁手规则
            bool isForbidden = CheckLongLine(x, y, pieceType) || 
                              CheckDoubleThree(x, y, pieceType) || 
                              CheckDoubleFour(x, y, pieceType);
            
            // 4. 恢复棋盘状态
            boardManager.RestoreBoardState(savedState);
            
            return isForbidden;
            */
        }
        
        /// <summary>
        /// 检查是否有长连
        /// </summary>
        private bool CheckLongLine(int x, int y, PieceType pieceType)
        {
            // 检查四个方向
            Vector2Int[] directions = {
                Vector2Int.right,      // 水平
                Vector2Int.up,         // 垂直
                new Vector2Int(1, 1),  // 左上到右下
                new Vector2Int(1, -1)  // 右上到左下
            };
            
            foreach (var direction in directions)
            {
                int count = CountConsecutivePieces(x, y, pieceType, direction, true);
                if (count > winCondition)
                {
                    return true;
                }
            }
            
            return false;
        }
        
        /// <summary>
        /// 检查胜利条件
        /// </summary>
        public bool CheckWinCondition(int lastX, int lastY, PieceType playerType)
        {
            if (boardManager == null) return false;
            
            // 检查四个方向是否有连子达到胜利条件
            Vector2Int[] directions = {
                Vector2Int.right,      // 水平
                Vector2Int.up,         // 垂直
                new Vector2Int(1, 1),  // 左上到右下
                new Vector2Int(1, -1)  // 右上到左下
            };
            
            foreach (var direction in directions)
            {
                int count = CountConsecutivePieces(lastX, lastY, playerType, direction, false);
                if (count >= winCondition)
                {
                    return true;
                }
            }
            
            return false;
        }
        
        /// <summary>
        /// 计算连续棋子数量
        /// </summary>
        private int CountConsecutivePieces(int startX, int startY, PieceType playerType, Vector2Int direction, bool includeSelf = true)
        {
            int count = includeSelf ? 1 : 0;
            
            if (includeSelf)
            {
                // 向正方向检查
                for (int i = 1; i <= winCondition; i++)
                {
                    int x = startX + direction.x * i;
                    int y = startY + direction.y * i;
                    
                    if (boardManager.GetPieceType(x, y) == playerType)
                        count++;
                    else
                        break;
                }
                
                // 向负方向检查
                for (int i = 1; i <= winCondition; i++)
                {
                    int x = startX - direction.x * i;
                    int y = startY - direction.y * i;
                    
                    if (boardManager.GetPieceType(x, y) == playerType)
                        count++;
                    else
                        break;
                }
            }
            else
            {
                // 不包含起始点，只检查连子
                // 向正方向检查
                for (int i = 1; i <= winCondition; i++)
                {
                    int x = startX + direction.x * i;
                    int y = startY + direction.y * i;
                    
                    if (boardManager.GetPieceType(x, y) == playerType)
                        count++;
                    else
                        break;
                }
                
                // 向负方向检查
                for (int i = 1; i <= winCondition; i++)
                {
                    int x = startX - direction.x * i;
                    int y = startY - direction.y * i;
                    
                    if (boardManager.GetPieceType(x, y) == playerType)
                        count++;
                    else
                        break;
                }
            }
            
            return count;
        }
        
        /// <summary>
        /// 检查棋盘是否已满（平局）
        /// </summary>
        public bool IsBoardFull()
        {
            if (boardManager == null) return false;
            
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
        /// 获取所有合法移动位置
        /// </summary>
        public List<Vector2Int> GetAllValidMoves(PieceType playerType)
        {
            List<Vector2Int> validMoves = new List<Vector2Int>();
            
            if (boardManager == null) return validMoves;
            
            for (int x = 0; x < boardManager.BoardSize; x++)
            {
                for (int y = 0; y < boardManager.BoardSize; y++)
                {
                    if (IsValidMove(x, y, playerType))
                    {
                        validMoves.Add(new Vector2Int(x, y));
                    }
                }
            }
            
            return validMoves;
        }
        
        /// <summary>
        /// 评估棋盘分数（用于AI）
        /// </summary>
        public int EvaluateBoard(PieceType playerType)
        {
            if (boardManager == null) return 0;
            
            int score = 0;
            
            // 评估每个位置的分数
            for (int x = 0; x < boardManager.BoardSize; x++)
            {
                for (int y = 0; y < boardManager.BoardSize; y++)
                {
                    PieceType piece = boardManager.GetPieceType(x, y);
                    if (piece != PieceType.None)
                    {
                        // 计算该位置的影响力
                        int positionScore = EvaluatePosition(x, y, piece);
                        if (piece == playerType)
                            score += positionScore;
                        else
                            score -= positionScore;
                    }
                }
            }
            
            return score;
        }
        
        /// <summary>
        /// 评估单个位置的分数
        /// </summary>
        private int EvaluatePosition(int x, int y, PieceType pieceType)
        {
            int score = 0;
            
            // 四个方向的评估
            Vector2Int[] directions = {
                Vector2Int.right,      // 水平
                Vector2Int.up,         // 垂直
                new Vector2Int(1, 1),  // 左上到右下
                new Vector2Int(1, -1)  // 右上到左下
            };
            
            foreach (var direction in directions)
            {
                int patternScore = EvaluatePattern(x, y, pieceType, direction);
                score += patternScore;
            }
            
            return score;
        }
        
        /// <summary>
        /// 评估棋子模式分数
        /// </summary>
        private int EvaluatePattern(int x, int y, PieceType pieceType, Vector2Int direction)
        {
            // 模式分数表
            // 连五: 10000分
            // 活四: 5000分
            // 冲四: 1000分
            // 活三: 500分
            // 眠三: 100分
            // 活二: 50分
            // 眠二: 10分
            // 活一: 1分
            
            // 检查连续棋子
            int consecutiveCount = 0;
            int emptyCount = 0;
            
            // 向正方向检查
            for (int i = 0; i < 5; i++)
            {
                int checkX = x + direction.x * i;
                int checkY = y + direction.y * i;
                
                if (!boardManager.IsValidPosition(checkX, checkY))
                    break;
                    
                PieceType piece = boardManager.GetPieceType(checkX, checkY);
                if (piece == pieceType)
                {
                    consecutiveCount++;
                }
                else if (piece == PieceType.None)
                {
                    emptyCount++;
                    break;
                }
                else
                {
                    break;
                }
            }
            
            // 向负方向检查
            for (int i = 1; i < 5; i++)
            {
                int checkX = x - direction.x * i;
                int checkY = y - direction.y * i;
                
                if (!boardManager.IsValidPosition(checkX, checkY))
                    break;
                    
                PieceType piece = boardManager.GetPieceType(checkX, checkY);
                if (piece == pieceType)
                {
                    consecutiveCount++;
                }
                else if (piece == PieceType.None)
                {
                    emptyCount++;
                    break;
                }
                else
                {
                    break;
                }
            }
            
            // 根据连子数和空位计算分数
            if (consecutiveCount >= 5)
                return 10000;
            else if (consecutiveCount == 4)
                return emptyCount > 0 ? 5000 : 1000; // 活四:5000, 冲四:1000
            else if (consecutiveCount == 3)
                return emptyCount > 0 ? 500 : 100;   // 活三:500, 眠三:100
            else if (consecutiveCount == 2)
                return emptyCount > 0 ? 50 : 10;     // 活二:50, 眠二:10
            else if (consecutiveCount == 1)
                return 1;                            // 活一:1
            
            return 0;
        }
        
        /// <summary>
        /// 获取最佳移动建议（用于AI）
        /// </summary>
        public Vector2Int GetBestMove(PieceType playerType, int depth = 2)
        {
            if (boardManager == null)
            {
                Debug.LogError("BoardManager is null in GameRules.GetBestMove");
                return new Vector2Int(7, 7); // 默认返回中心位置
            }
            
            List<Vector2Int> validMoves = GetAllValidMoves(playerType);
            if (validMoves.Count == 0)
                return new Vector2Int(-1, -1);
            
            // 简单的启发式搜索：选择最高分数的位置
            Vector2Int bestMove = validMoves[0];
            int bestScore = int.MinValue;
            
            foreach (var move in validMoves)
            {
                int score = EvaluateMoveScore(move, playerType);
                
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = move;
                }
            }
            
            return bestMove;
        }
        
        /// <summary>
        /// 评估移动的分数（不实际落子）
        /// </summary>
        private int EvaluateMoveScore(Vector2Int move, PieceType playerType)
        {
            if (boardManager == null) return 0;
            
            int score = 0;
            PieceType opponent = playerType == PieceType.Black ? PieceType.White : PieceType.Black;
            
            // 1. 检查是否立即获胜
            if (WouldWin(move.x, move.y, playerType))
            {
                score += 100000;
            }
            
            // 2. 检查是否需要防守（对手可能获胜）
            if (WouldWin(move.x, move.y, opponent))
            {
                score += 50000;
            }
            
            // 3. 位置分数：中心区域更好
            int centerX = boardManager.BoardSize / 2;
            int centerY = boardManager.BoardSize / 2;
            int distanceFromCenter = Mathf.Abs(move.x - centerX) + Mathf.Abs(move.y - centerY);
            score += (boardManager.BoardSize - distanceFromCenter) * 10;
            
            // 4. 连接性分数：靠近已有棋子
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
                        score += (3 - distance) * 20; // 越近分数越高
                    }
                }
            }
            
            // 5. 模式分数：检查四个方向可能形成的模式
            Vector2Int[] directions = {
                Vector2Int.right,      // 水平
                Vector2Int.up,         // 垂直
                new Vector2Int(1, 1),  // 左上到右下
                new Vector2Int(1, -1)  // 右上到左下
            };
            
            foreach (var direction in directions)
            {
                score += EvaluatePatternScore(move.x, move.y, playerType, direction);
            }
            
            return score;
        }
        
        /// <summary>
        /// 检查如果在此位置落子是否会获胜
        /// </summary>
        private bool WouldWin(int x, int y, PieceType playerType)
        {
            if (boardManager == null) return false;
            
            // 临时模拟：保存当前状态，模拟落子，检查胜负，恢复状态
            PieceType originalPiece = boardManager.GetPieceType(x, y);
            if (originalPiece != PieceType.None) return false;
            
            // 模拟落子
            boardManager.SimulatePlacePiece(x, y, playerType);
            
            // 检查是否获胜
            bool wouldWin = CheckWinCondition(x, y, playerType);
            
            // 撤销模拟
            boardManager.UndoSimulatedPlacePiece(x, y);
            
            return wouldWin;
        }
        
        /// <summary>
        /// 评估模式的分数
        /// </summary>
        private int EvaluatePatternScore(int x, int y, PieceType playerType, Vector2Int direction)
        {
            // 分析在这个方向上的棋子模式
            // 这是一个简化的实现
            
            int playerCount = 0;
            int emptyCount = 0;
            int opponentCount = 0;
            
            // 检查5个位置
            for (int i = -4; i <= 4; i++)
            {
                int checkX = x + direction.x * i;
                int checkY = y + direction.y * i;
                
                if (!boardManager.IsValidPosition(checkX, checkY))
                    continue;
                    
                PieceType piece = boardManager.GetPieceType(checkX, checkY);
                
                if (i == 0) // 当前位置
                {
                    // 假设我们会落子在这里
                    playerCount++;
                }
                else if (piece == playerType)
                {
                    playerCount++;
                }
                else if (piece == PieceType.None)
                {
                    emptyCount++;
                }
                else
                {
                    opponentCount++;
                }
            }
            
            // 根据模式计算分数
            if (playerCount >= 5) return 10000;
            if (playerCount == 4 && emptyCount > 0) return 5000;
            if (playerCount == 3 && emptyCount > 0) return 500;
            if (playerCount == 2 && emptyCount > 0) return 50;
            
            return 0;
        }
        
        /// <summary>
        /// 设置胜利条件
        /// </summary>
        public void SetWinCondition(int condition)
        {
            winCondition = Mathf.Clamp(condition, 3, 10);
        }
        
        /// <summary>
        /// 设置是否启用禁手规则
        /// </summary>
        public void SetForbiddenMovesEnabled(bool enabled)
        {
            enableForbiddenMoves = enabled;
        }
    }
}