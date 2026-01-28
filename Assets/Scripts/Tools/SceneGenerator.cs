using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Gomoku.GameLogic;
using Gomoku.Managers;
using Gomoku.UI;

namespace Gomoku.Tools
{
    /// <summary>
    /// 场景自动生成器 - 在Unity编辑器中运行此脚本来生成完整的五子棋游戏场景
    /// </summary>
    public class SceneGenerator : MonoBehaviour
    {
        [Header("场景生成设置")]
        [SerializeField] private bool generateMainMenu = true;
        [SerializeField] private bool generateGameScene = true;
        [SerializeField] private bool generatePrefabs = true;
        
        [ContextMenu("生成主菜单场景")]
        public void GenerateMainMenuScene()
        {
            Debug.Log("开始生成主菜单场景...");
            
            // 创建Canvas
            GameObject canvas = CreateCanvas();
            
            // 创建标题
            CreateTitle(canvas);
            
            // 创建按钮面板
            CreateMenuButtons(canvas);
            
            // 创建MainMenuManager
            CreateMainMenuManager();
            
            Debug.Log("主菜单场景生成完成！");
        }
        
        [ContextMenu("生成游戏场景")]
        public void GenerateGameScene()
        {
            Debug.Log("开始生成游戏场景...");
            
            // 设置摄像机
            SetupCamera();
            
            // 创建游戏管理器
            CreateGameManager();
            
            // 创建棋盘
            CreateBoardManager();
            
            // 创建输入管理器
            CreateInputManager();
            
            // 创建游戏UI
            CreateGameUI();
            
            // 创建资源管理器
            CreateResourceManager();
            
            Debug.Log("游戏场景生成完成！");
        }
        
        [ContextMenu("生成预制件")]
        public void GeneratePrefabs()
        {
            Debug.Log("开始生成预制件...");
            
            CreatePiecePrefabs();
            CreateMaterials();
            
            Debug.Log("预制件生成完成！");
        }
        
        [ContextMenu("生成完整场景")]
        public void GenerateCompleteGame()
        {
            GeneratePrefabs();
            GenerateGameScene();
            
            Debug.Log("完整游戏场景生成完成！现在可以运行游戏了。");
        }
        
        #region 主菜单场景生成
        private GameObject CreateCanvas()
        {
            GameObject canvas = new GameObject("Canvas");
            Canvas canvasComponent = canvas.AddComponent<Canvas>();
            canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.AddComponent<CanvasScaler>();
            canvas.AddComponent<GraphicRaycaster>();
            
            // 创建EventSystem
            if (FindObjectOfType<EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
            }
            
            return canvas;
        }
        
        private void CreateTitle(GameObject canvas)
        {
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(canvas.transform, false);
            
            TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "五子棋游戏";
            titleText.fontSize = 72;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.color = Color.black;
            
            RectTransform rectTransform = titleText.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.8f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.9f);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = new Vector2(400, 100);
        }
        
        private void CreateMenuButtons(GameObject canvas)
        {
            GameObject buttonPanel = new GameObject("ButtonPanel");
            buttonPanel.transform.SetParent(canvas.transform, false);
            
            RectTransform panelRect = buttonPanel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.4f);
            panelRect.anchorMax = new Vector2(0.5f, 0.6f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(300, 200);
            
            // 开始游戏按钮
            GameObject startButton = CreateButton("StartButton", buttonPanel.transform, new Vector2(0, 50), "开始游戏");
            
            // 退出游戏按钮
            GameObject exitButton = CreateButton("ExitButton", buttonPanel.transform, new Vector2(0, -50), "退出游戏");
        }
        
        private GameObject CreateButton(string name, Transform parent, Vector2 position, string text)
        {
            GameObject buttonObj = new GameObject(name);
            buttonObj.transform.SetParent(parent, false);
            
            RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
            buttonRect.anchoredPosition = position;
            buttonRect.sizeDelta = new Vector2(250, 60);
            
            Button button = buttonObj.AddComponent<Button>();
            
            // 按钮背景
            GameObject bgImage = new GameObject("Background");
            bgImage.transform.SetParent(buttonObj.transform, false);
            RectTransform bgRect = bgImage.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            
            Image bgImageComponent = bgImage.AddComponent<Image>();
            bgImageComponent.color = new Color(0.2f, 0.6f, 1f, 1f);
            button.targetGraphic = bgImageComponent;
            
            // 按钮文本
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform, false);
            
            TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
            textComponent.text = text;
            textComponent.fontSize = 24;
            textComponent.alignment = TextAlignmentOptions.Center;
            textComponent.color = Color.white;
            
            RectTransform textRect = textComponent.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            return buttonObj;
        }
        
        private void CreateMainMenuManager()
        {
            GameObject managerObj = new GameObject("MainMenuManager");
            MainMenuManager manager = managerObj.AddComponent<MainMenuManager>();
            
            // 这里需要在Inspector中手动配置按钮引用
        }
        #endregion
        
        #region 游戏场景生成
        private void SetupCamera()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                GameObject cameraObj = new GameObject("Main Camera");
                mainCamera = cameraObj.AddComponent<Camera>();
                cameraObj.tag = "MainCamera";
            }
            
            // 设置摄像机为俯视角度
            mainCamera.transform.position = new Vector3(7, 10, -7);
            mainCamera.transform.rotation = Quaternion.Euler(45, 0, 0);
            mainCamera.orthographic = true;
            mainCamera.orthographicSize = 8;
            
            // 设置摄像机背景色
            mainCamera.backgroundColor = new Color(0.9f, 0.9f, 0.9f, 1f);
        }
        
        private void CreateGameManager()
        {
            GameObject gameManagerObj = new GameObject("GameManager");
            gameManagerObj.AddComponent<GameManager>();
        }
        
        private void CreateBoardManager()
        {
            GameObject boardManagerObj = new GameObject("BoardManager");
            BoardManager boardManager = boardManagerObj.AddComponent<BoardManager>();
            
            // 创建棋盘父对象
            GameObject boardParent = new GameObject("BoardParent");
            boardParent.transform.SetParent(boardManagerObj.transform);
            
            // 添加Collider到棋盘背景用于点击检测
            boardParent.AddComponent<BoxCollider>();
            boardParent.layer = LayerMask.NameToLayer("Default");
            
            // 这里需要在Inspector中配置预制件引用
        }
        
        private void CreateInputManager()
        {
            GameObject inputManagerObj = new GameObject("InputManager");
            InputManager inputManager = inputManagerObj.AddComponent<InputManager>();
            
            // 配置Layer Mask
            int defaultLayer = LayerMask.GetMask("Default");
            inputManager.SetBoardLayerMask(defaultLayer);
        }
        
        private void CreateGameUI()
        {
            GameObject canvas = CreateCanvas();
            
            // 创建游戏状态面板
            CreateGameStatusPanel(canvas);
            
            // 创建游戏结束面板
            CreateGameOverPanel(canvas);
        }
        
        private void CreateGameStatusPanel(GameObject canvas)
        {
            GameObject statusPanel = new GameObject("GameStatusPanel");
            statusPanel.transform.SetParent(canvas.transform, false);
            
            RectTransform panelRect = statusPanel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0, 0.9f);
            panelRect.anchorMax = new Vector2(0.3f, 1f);
            panelRect.offsetMin = new Vector2(20, -20);
            panelRect.offsetMax = new Vector2(-20, -10);
            
            // 当前玩家文本
            GameObject currentPlayerText = CreateUIText("CurrentPlayerText", statusPanel.transform, new Vector2(0, 10), "当前玩家: 黑棋");
            currentPlayerText.GetComponent<TextMeshProUGUI>().fontSize = 18;
            
            // 游戏状态文本
            GameObject gameStatusText = CreateUIText("GameStatusText", statusPanel.transform, new Vector2(0, -10), "游戏进行中");
            gameStatusText.GetComponent<TextMeshProUGUI>().fontSize = 16;
        }
        
        private void CreateGameOverPanel(GameObject canvas)
        {
            GameObject gameOverPanel = new GameObject("GameOverPanel");
            gameOverPanel.transform.SetParent(canvas.transform, false);
            gameOverPanel.SetActive(false); // 默认隐藏
            
            RectTransform panelRect = gameOverPanel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.3f, 0.3f);
            panelRect.anchorMax = new Vector2(0.7f, 0.7f);
            panelRect.anchoredPosition = Vector2.zero;
            
            // 背景面板
            Image panelBg = gameOverPanel.AddComponent<Image>();
            panelBg.color = new Color(0, 0, 0, 0.8f);
            
            // 获胜者文本
            GameObject winnerText = CreateUIText("WinnerText", gameOverPanel.transform, new Vector2(0, 50), "游戏结束");
            winnerText.GetComponent<TextMeshProUGUI>().fontSize = 36;
            winnerText.GetComponent<TextMeshProUGUI>().color = Color.white;
            
            // 重新开始按钮
            GameObject restartButton = CreateButton("RestartButton", gameOverPanel.transform, new Vector2(-70, -50), "重新开始");
            
            // 返回主菜单按钮
            GameObject mainMenuButton = CreateButton("MainMenuButton", gameOverPanel.transform, new Vector2(70, -50), "主菜单");
        }
        
        private GameObject CreateUIText(string name, Transform parent, Vector2 position, string text)
        {
            GameObject textObj = new GameObject(name);
            textObj.transform.SetParent(parent, false);
            
            RectTransform rectTransform = textObj.AddComponent<RectTransform>();
            rectTransform.anchoredPosition = position;
            rectTransform.sizeDelta = new Vector2(200, 30);
            
            TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
            textComponent.text = text;
            textComponent.alignment = TextAlignmentOptions.Center;
            textComponent.color = Color.black;
            
            return textObj;
        }
        
        private void CreateResourceManager()
        {
            GameObject resourceManagerObj = new GameObject("ResourceManager");
            resourceManagerObj.AddComponent<ResourceManager>();
        }
        #endregion
        
        #region 预制件生成
        private void CreatePiecePrefabs()
        {
            #if UNITY_EDITOR
            // 确保Prefabs文件夹存在
            if (!System.IO.Directory.Exists("Assets/Prefabs"))
            {
                UnityEditor.AssetDatabase.CreateFolder("Assets", "Prefabs");
            }
            
            // 创建黑棋子
            GameObject blackPiece = CreatePiecePrefab("BlackPiece", Color.black);
            UnityEditor.PrefabUtility.SaveAsPrefabAsset(blackPiece, "Assets/Prefabs/BlackPiece.prefab");
            DestroyImmediate(blackPiece);
            
            // 创建白棋子
            GameObject whitePiece = CreatePiecePrefab("WhitePiece", Color.white);
            UnityEditor.PrefabUtility.SaveAsPrefabAsset(whitePiece, "Assets/Prefabs/WhitePiece.prefab");
            DestroyImmediate(whitePiece);
            
            UnityEditor.AssetDatabase.Refresh();
            #else
            Debug.LogWarning("预制件生成功能仅在Unity编辑器中可用");
            #endif
        }
        
        private GameObject CreatePiecePrefab(string name, Color color)
        {
            GameObject piece = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            piece.name = name;
            
            // 设置大小
            piece.transform.localScale = Vector3.one * 0.8f;
            
            // 创建材质
            Material material = new Material(Shader.Find("Standard"));
            material.color = color;
            material.SetFloat("_Metallic", 0.1f);
            material.SetFloat("_Glossiness", 0.8f);
            
            Renderer renderer = piece.GetComponent<Renderer>();
            renderer.material = material;
            
            // 添加Piece脚本
            Piece pieceScript = piece.AddComponent<Piece>();
            
            return piece;
        }
        
        private void CreateMaterials()
        {
            #if UNITY_EDITOR
            // 确保Materials文件夹存在
            if (!System.IO.Directory.Exists("Assets/Materials"))
            {
                UnityEditor.AssetDatabase.CreateFolder("Assets", "Materials");
            }
            
            // 创建棋盘材质
            Material boardMaterial = new Material(Shader.Find("Standard"));
            boardMaterial.color = new Color(0.8f, 0.6f, 0.4f, 1f);
            boardMaterial.SetFloat("_Metallic", 0f);
            boardMaterial.SetFloat("_Glossiness", 0.2f);
            UnityEditor.AssetDatabase.CreateAsset(boardMaterial, "Assets/Materials/BoardMaterial.mat");
            
            // 创建高亮材质
            Material highlightMaterial = new Material(Shader.Find("Standard"));
            highlightMaterial.color = Color.yellow;
            highlightMaterial.SetFloat("_Metallic", 0.2f);
            highlightMaterial.SetFloat("_Glossiness", 0.9f);
            UnityEditor.AssetDatabase.CreateAsset(highlightMaterial, "Assets/Materials/HighlightMaterial.mat");
            
            UnityEditor.AssetDatabase.Refresh();
            #else
            Debug.LogWarning("材质生成功能仅在Unity编辑器中可用");
            #endif
        }
        #endregion
    }
}