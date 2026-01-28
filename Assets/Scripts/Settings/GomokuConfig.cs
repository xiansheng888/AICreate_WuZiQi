using UnityEngine;

namespace Gomoku.Settings
{
    /// <summary>
    /// 五子棋游戏配置
    /// </summary>
    [CreateAssetMenu(fileName = "GomokuConfig", menuName = "Gomoku/Game Config")]
    public class GomokuConfig : ScriptableObject
    {
        [Header("棋盘设置")]
        [SerializeField] private int boardSize = 15;
        [SerializeField] private float cellSize = 1.0f;
        [SerializeField] private float pieceScale = 0.8f;
        
        [Header("游戏规则")]
        [SerializeField] private int winCondition = 5; // 连子数获胜条件
        [SerializeField] private bool enableAI = true;
        [SerializeField] private float aiMoveDelay = 1.0f;
        
        [Header("视觉效果")]
        [SerializeField] private bool showPieceAnimations = true;
        [SerializeField] private bool showHighlightOnHover = true;
        [SerializeField] private float piecePlaceAnimationDuration = 0.3f;
        
        [Header("颜色设置")]
        [SerializeField] private Color blackPieceColor = Color.black;
        [SerializeField] private Color whitePieceColor = Color.white;
        [SerializeField] private Color boardBackgroundColor = new Color(0.8f, 0.6f, 0.4f, 1f);
        [SerializeField] private Color gridLineColor = Color.black;
        [SerializeField] private Color highlightColor = Color.yellow;
        
        // 公共属性
        public int BoardSize => boardSize;
        public float CellSize => cellSize;
        public float PieceScale => pieceScale;
        public int WinCondition => winCondition;
        public bool EnableAI => enableAI;
        public float AIMoveDelay => aiMoveDelay;
        public bool ShowPieceAnimations => showPieceAnimations;
        public bool ShowHighlightOnHover => showHighlightOnHover;
        public float PiecePlaceAnimationDuration => piecePlaceAnimationDuration;
        public Color BlackPieceColor => blackPieceColor;
        public Color WhitePieceColor => whitePieceColor;
        public Color BoardBackgroundColor => boardBackgroundColor;
        public Color GridLineColor => gridLineColor;
        public Color HighlightColor => highlightColor;
        
        private void OnValidate()
        {
            // 确保值在合理范围内
            boardSize = Mathf.Clamp(boardSize, 9, 19);
            cellSize = Mathf.Clamp(cellSize, 0.5f, 2.0f);
            pieceScale = Mathf.Clamp(pieceScale, 0.3f, 1.0f);
            winCondition = Mathf.Clamp(winCondition, 3, 6);
            aiMoveDelay = Mathf.Clamp(aiMoveDelay, 0.1f, 5.0f);
        }
    }
}