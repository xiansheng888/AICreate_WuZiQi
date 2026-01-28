using UnityEngine;

namespace Gomoku.GameLogic
{
    /// <summary>
    /// 棋子类型枚举
    /// </summary>
    public enum PieceType
    {
        None = 0,
        Black = 1,
        White = 2
    }
    
    /// <summary>
    /// 游戏状态枚举
    /// </summary>
    public enum GameState
    {
        Playing = 0,
        BlackWin = 1,
        WhiteWin = 2,
        Draw = 3
    }
    
    /// <summary>
    /// 棋子数据结构
    /// </summary>
    [System.Serializable]
    public struct PieceData
    {
        public PieceType type;
        public Vector2Int position;
        
        public PieceData(PieceType pieceType, Vector2Int pos)
        {
            type = pieceType;
            position = pos;
        }
    }
}