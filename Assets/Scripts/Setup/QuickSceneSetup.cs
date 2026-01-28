using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

namespace Gomoku.Setup
{
    /// <summary>
    /// 简单的初始化脚本，在Unity编辑器中运行此脚本来设置场景
    /// </summary>
    public class QuickSceneSetup : MonoBehaviour
    {
        [Header("快速设置")]
        [SerializeField] private bool setupMainMenu = false;
        [SerializeField] private bool setupGameScene = false;
        
        private void Start()
        {
            if (setupMainMenu)
            {
                SetupMainMenuQuick();
            }
            else if (setupGameScene)
            {
                SetupGameSceneQuick();
            }
            
            // 删除这个脚本组件，只用于设置
            Destroy(this);
        }
        
        private void SetupMainMenuQuick()
        {
            Debug.Log("快速设置主菜单场景...");
            
            // 删除现有的默认对象
            ClearDefaultObjects();
            
            // 创建基础UI
            CreateBasicMainMenu();
            
            Debug.Log("主菜单设置完成！请手动配置MainMenuManager脚本中的按钮引用。");
        }
        
        private void SetupGameSceneQuick()
        {
            Debug.Log("快速设置游戏场景...");
            
            // 删除现有的默认对象
            ClearDefaultObjects();
            
            // 设置摄像机
            SetupGameCamera();
            
            // 创建基础游戏对象
            CreateBasicGameObjects();
            
            // 创建基础UI
            CreateBasicGameUI();
            
            Debug.Log("游戏场景设置完成！请手动配置脚本中的预制件和组件引用。");
        }
        
        private void ClearDefaultObjects()
        {
            // 删除默认的立方体、球体等
            GameObject[] defaultObjects = GameObject.FindGameObjectsWithTag("Untagged");
            foreach (GameObject obj in defaultObjects)
            {
                if (obj.name.Contains("Cube") || obj.name.Contains("Sphere") || obj.name.Contains("Cylinder"))
                {
                    DestroyImmediate(obj);
                }
            }
        }
        
        private void SetupGameCamera()
        {
            Camera.main.transform.position = new Vector3(7, 10, -7);
            Camera.main.transform.rotation = Quaternion.Euler(45, 0, 0);
            Camera.main.orthographic = true;
            Camera.main.orthographicSize = 8;
            Camera.main.backgroundColor = new Color(0.9f, 0.9f, 0.9f, 1f);
        }
        
        private void CreateBasicMainMenu()
        {
            // 创建Canvas
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
            
            // 创建主菜单管理器
            GameObject menuManager = new GameObject("MainMenuManager");
            menuManager.AddComponent<Gomoku.UI.MainMenuManager>();
            
            // 创建标题
            CreateTitleUI(canvas);
            
            // 创建按钮
            CreateMenuButtons(canvas);
        }
        
        private void CreateTitleUI(GameObject canvas)
        {
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(canvas.transform, false);
            
            TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "五子棋游戏";
            titleText.fontSize = 72;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.color = Color.black;
            
            RectTransform rectTransform = titleText.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.7f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.9f);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = new Vector2(400, 100);
        }
        
        private void CreateMenuButtons(GameObject canvas)
        {
            // 开始游戏按钮
            GameObject startButton = CreateSimpleButton("StartButton", canvas, new Vector2(0, 0), "开始游戏");
            
            // 退出游戏按钮
            GameObject exitButton = CreateSimpleButton("ExitButton", canvas, new Vector2(0, -80), "退出游戏");
        }
        
        private void CreateBasicGameObjects()
        {
            // 创建GameManager
            GameObject gameManager = new GameObject("GameManager");
            gameManager.AddComponent<Gomoku.Managers.GameManager>();
            
            // 创建BoardManager
            GameObject boardManager = new GameObject("BoardManager");
            boardManager.AddComponent<Gomoku.GameLogic.BoardManager>();
            
            // 创建InputManager
            GameObject inputManager = new GameObject("InputManager");
            inputManager.AddComponent<Gomoku.Managers.InputManager>();
            
            // 创建ResourceManager
            GameObject resourceManager = new GameObject("ResourceManager");
            resourceManager.AddComponent<Gomoku.GameLogic.ResourceManager>();
        }
        
        private void CreateBasicGameUI()
        {
            // 创建Canvas
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
            
            // 创建游戏UI管理器
            GameObject gameUIManager = new GameObject("GameUIManager");
            gameUIManager.AddComponent<Gomoku.UI.GameUIManager>();
            
            // 创建状态文本
            GameObject statusTextObj = new GameObject("GameStatusText");
            statusTextObj.transform.SetParent(canvas.transform, false);
            
            TextMeshProUGUI statusText = statusTextObj.AddComponent<TextMeshProUGUI>();
            statusText.text = "游戏进行中";
            statusText.fontSize = 20;
            statusText.color = Color.black;
            
            RectTransform rectTransform = statusText.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 0.9f);
            rectTransform.anchorMax = new Vector2(0.3f, 1f);
            rectTransform.offsetMin = new Vector2(20, -40);
            rectTransform.offsetMax = new Vector2(-20, -10);
        }
        
        private GameObject CreateSimpleButton(string name, GameObject parent, Vector2 position, string text)
        {
            GameObject buttonObj = new GameObject(name);
            buttonObj.transform.SetParent(parent.transform, false);
            
            RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
            buttonRect.anchoredPosition = position;
            buttonRect.sizeDelta = new Vector2(200, 50);
            
            Button button = buttonObj.AddComponent<Button>();
            
            // 背景图像
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
            
            // 文本
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform, false);
            
            TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
            textComponent.text = text;
            textComponent.fontSize = 18;
            textComponent.alignment = TextAlignmentOptions.Center;
            textComponent.color = Color.white;
            
            RectTransform textRect = textComponent.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            return buttonObj;
        }
    }
}