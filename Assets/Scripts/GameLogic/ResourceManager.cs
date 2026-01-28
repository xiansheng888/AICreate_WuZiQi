using UnityEngine;

namespace Gomoku.GameLogic
{
    /// <summary>
    /// 游戏资源管理器，负责统一管理预制件和材质资源
    /// </summary>
    public class ResourceManager : MonoBehaviour
    {
        [Header("棋子预制件")]
        [SerializeField] private GameObject blackPiecePrefab;
        [SerializeField] private GameObject whitePiecePrefab;
        
        [Header("材质资源")]
        [SerializeField] private Material blackPieceMaterial;
        [SerializeField] private Material whitePieceMaterial;
        [SerializeField] private Material boardMaterial;
        [SerializeField] private Material highlightMaterial;
        
        [Header("其他配置")]
        [SerializeField] private float pieceScale = 0.8f;
        [SerializeField] private float boardSize = 15f;
        [SerializeField] private float cellSize = 1.0f;
        
        // 单例访问
        public static ResourceManager Instance { get; private set; }
        
        // 公共属性访问
        public GameObject BlackPiecePrefab => blackPiecePrefab;
        public GameObject WhitePiecePrefab => whitePiecePrefab;
        public Material BlackPieceMaterial => blackPieceMaterial;
        public Material WhitePieceMaterial => whitePieceMaterial;
        public Material BoardMaterial => boardMaterial;
        public Material HighlightMaterial => highlightMaterial;
        public float PieceScale => pieceScale;
        public float BoardSize => boardSize;
        public float CellSize => cellSize;
        
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
            // 如果没有设置预制件，创建默认的
            EnsureResourcesExist();
            
            // 清理场景中可能意外创建的棋子对象
            CleanupStrayPieces();
        }
        
        /// <summary>
        /// 确保资源存在，如果不存在则创建默认的
        /// </summary>
        private void EnsureResourcesExist()
        {
            // 创建黑色棋子预制件
            if (blackPiecePrefab == null)
            {
                blackPiecePrefab = CreateDefaultPiecePrefab("BlackPiece", Color.black);
            }
            
            // 创建白色棋子预制件
            if (whitePiecePrefab == null)
            {
                whitePiecePrefab = CreateDefaultPiecePrefab("WhitePiece", Color.white);
            }
            
            // 创建棋盘材质
            if (boardMaterial == null)
            {
                boardMaterial = CreateDefaultBoardMaterial();
            }
            
            // 创建棋子材质
            if (blackPieceMaterial == null)
            {
                blackPieceMaterial = CreateDefaultPieceMaterial(Color.black);
            }
            
            if (whitePieceMaterial == null)
            {
                whitePieceMaterial = CreateDefaultPieceMaterial(Color.white);
            }
            
            // 创建高亮材质
            if (highlightMaterial == null)
            {
                highlightMaterial = CreateDefaultHighlightMaterial();
            }
            
            Debug.Log("资源检查和创建完成");
        }
        
        /// <summary>
        /// 创建默认棋子预制件
        /// </summary>
        private GameObject CreateDefaultPiecePrefab(string name, Color color)
        {
            // 创建基础球体
            GameObject piece = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            piece.name = name;
            
            // 设置大小
            piece.transform.localScale = Vector3.one * pieceScale;
            
            // 设置材质
            Renderer renderer = piece.GetComponent<Renderer>();
            Material material = CreateDefaultPieceMaterial(color);
            renderer.material = material;
            
            // 添加Piece脚本
            Piece pieceScript = piece.AddComponent<Piece>();
            
            // 重要：隐藏棋子，避免在场景中显示
            piece.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
            
            // 设置为预制件（在实际Unity编辑器中会自动处理）
            Debug.Log($"创建默认{name}预制件（仅用于运行时参考）");
            return piece;
        }
        
        /// <summary>
        /// 创建默认棋子材质
        /// </summary>
        private Material CreateDefaultPieceMaterial(Color color)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = color;
            mat.SetFloat("_Metallic", 0.1f);
            mat.SetFloat("_Glossiness", 0.8f);
            return mat;
        }
        
        /// <summary>
        /// 创建默认棋盘材质
        /// </summary>
        private Material CreateDefaultBoardMaterial()
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = new Color(0.8f, 0.6f, 0.4f, 1f); // 木色
            mat.SetFloat("_Metallic", 0f);
            mat.SetFloat("_Glossiness", 0.2f);
            return mat;
        }
        
        /// <summary>
        /// 创建默认高亮材质
        /// </summary>
        private Material CreateDefaultHighlightMaterial()
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = Color.yellow;
            mat.SetFloat("_Metallic", 0.2f);
            mat.SetFloat("_Glossiness", 0.9f);
            return mat;
        }
        
        /// <summary>
        /// 清理场景中可能意外创建的棋子对象
        /// </summary>
        private void CleanupStrayPieces()
        {
            // 查找名为"BlackPiece"或"WhitePiece"的棋子对象
            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                if (obj.name == "BlackPiece" || obj.name == "WhitePiece")
                {
                    // 检查是否有Piece组件，确保我们删除的是棋子而不是预制件引用
                    Piece pieceComponent = obj.GetComponent<Piece>();
                    if (pieceComponent != null)
                    {
                        Debug.Log($"清理意外创建的棋子: {obj.name}");
                        Destroy(obj);
                    }
                }
            }
        }
    }
}