using UnityEngine;
using System.Collections;

namespace Gomoku.GameLogic
{
    /// <summary>
    /// 棋子组件，控制单个棋子的行为和显示
    /// </summary>
    public class Piece : MonoBehaviour
    {
        [Header("棋子配置")]
        [SerializeField] private PieceType pieceType;
        [SerializeField] private Vector2Int boardPosition;
        
        private Renderer pieceRenderer;
        private Material originalMaterial;
        
        public PieceType Type => pieceType;
        public Vector2Int BoardPosition => boardPosition;
        
        private void Awake()
        {
            pieceRenderer = GetComponent<Renderer>();
            if (pieceRenderer != null)
            {
                originalMaterial = pieceRenderer.material;
            }
        }
        
        /// <summary>
        /// 初始化棋子
        /// </summary>
        public void Initialize(PieceType type, Vector2Int position)
        {
            pieceType = type;
            boardPosition = position;
            
            UpdateAppearance();
        }
        
        /// <summary>
        /// 更新棋子外观
        /// </summary>
        private void UpdateAppearance()
        {
            if (pieceRenderer == null) return;
            
            // 根据棋子类型设置颜色
            Color pieceColor = pieceType == PieceType.Black ? Color.black : Color.white;
            
            // 创建新材质实例
            Material newMaterial = new Material(originalMaterial);
            newMaterial.color = pieceColor;
            
            pieceRenderer.material = newMaterial;
        }
        
        /// <summary>
        /// 设置棋子高亮状态
        /// </summary>
        public void SetHighlight(bool isHighlighted)
        {
            if (pieceRenderer == null) return;
            
            if (isHighlighted)
            {
                // 高亮效果（可以添加发光或边框）
                Color highlightColor = Color.yellow;
                Material highlightMaterial = new Material(originalMaterial);
                highlightMaterial.color = Color.Lerp(pieceRenderer.material.color, highlightColor, 0.3f);
                pieceRenderer.material = highlightMaterial;
            }
            else
            {
                UpdateAppearance();
            }
        }
        
        /// <summary>
        /// 播放放置动画
        /// </summary>
        public void PlayPlaceAnimation()
        {
            StartCoroutine(PlayPlaceAnimationCoroutine());
        }
        
        /// <summary>
        /// 放置动画协程
        /// </summary>
        private IEnumerator PlayPlaceAnimationCoroutine()
        {
            // 简单的缩放动画
            Vector3 originalScale = transform.localScale;
            transform.localScale = Vector3.zero;
            
            float duration = 0.3f;
            float elapsedTime = 0f;
            
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / duration;
                
                // 使用简单的缓动效果
                float easedProgress = 1f - Mathf.Pow(1f - progress, 3f); // easeOutCubic
                transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, easedProgress);
                
                yield return null;
            }
            
            transform.localScale = originalScale;
        }
    }
}