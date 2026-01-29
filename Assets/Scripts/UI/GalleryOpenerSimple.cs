using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using System.IO;
using System.Collections;

namespace Gomoku.UI
{
    /// <summary>
    /// 简化的相册打开器 - 解决真机回调问题
    /// </summary>
    public class GalleryOpenerSimple : MonoBehaviour
    {
        [Header("UI设置")]
        [SerializeField] private string buttonText = "打开相册";
        [SerializeField] private Vector2 buttonPosition = new Vector2(0, 0);
        [SerializeField] private Vector2 buttonSize = new Vector2(200, 80);
        [SerializeField] private Color buttonColor = new Color(0.2f, 0.6f, 1f, 1f);
        
        [Header("图片显示设置")]
        [SerializeField] private RawImage displayImage;
        [SerializeField] private Vector2 imagePosition = new Vector2(0, 100);
        [SerializeField] private Vector2 imageSize = new Vector2(300, 300);
        
        private Button galleryButton;
        private GameObject canvasInstance;
        
        private void Start()
        {
            CreateUI();
        }
        
        /// <summary>
        /// 创建UI界面
        /// </summary>
        private void CreateUI()
        {
            // 查找或创建Canvas
            Canvas existingCanvas = FindObjectOfType<Canvas>();
            if (existingCanvas != null)
            {
                canvasInstance = existingCanvas.gameObject;
            }
            else
            {
                canvasInstance = new GameObject("GalleryCanvas");
                Canvas canvas = canvasInstance.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasInstance.AddComponent<CanvasScaler>();
                canvasInstance.AddComponent<GraphicRaycaster>();
            }
            
            // 创建按钮
            CreateButton();
            
            // 创建显示区域
            CreateDisplayArea();
            
            Debug.Log("相册界面创建完成");
        }
        
        /// <summary>
        /// 创建按钮
        /// </summary>
        private void CreateButton()
        {
            GameObject buttonObj = new GameObject("GalleryButton");
            buttonObj.transform.SetParent(canvasInstance.transform, false);
            
            RectTransform rectTransform = buttonObj.AddComponent<RectTransform>();
            rectTransform.anchoredPosition = buttonPosition;
            rectTransform.sizeDelta = buttonSize;
            
            galleryButton = buttonObj.AddComponent<Button>();
            Image buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = buttonColor;
            
            // 添加文本
            GameObject textObj = new GameObject("ButtonText");
            textObj.transform.SetParent(buttonObj.transform, false);
            
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchoredPosition = Vector2.zero;
            textRect.sizeDelta = buttonSize;
            
            TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
            textComponent.text = buttonText;
            textComponent.color = Color.white;
            textComponent.alignment = TextAlignmentOptions.Center;
            textComponent.fontSize = 24;
            
            // 添加点击事件
            galleryButton.onClick.AddListener(OpenGallery);
        }
        
        /// <summary>
        /// 创建显示区域
        /// </summary>
        private void CreateDisplayArea()
        {
            if (displayImage == null)
            {
                GameObject imageObj = new GameObject("DisplayImage");
                imageObj.transform.SetParent(canvasInstance.transform, false);
                
                RectTransform imageRect = imageObj.AddComponent<RectTransform>();
                imageRect.anchoredPosition = imagePosition;
                imageRect.sizeDelta = imageSize;
                
                displayImage = imageObj.AddComponent<RawImage>();
                displayImage.color = Color.white;
                
                // 显示初始提示
                DisplayPlaceholderTexture();
            }
        }
        
        /// <summary>
        /// 打开相册
        /// </summary>
        public void OpenGallery()
        {
            Debug.Log("打开相册");
            
            #if UNITY_ANDROID && !UNITY_EDITOR
            OpenAndroidGallerySimple();
            #else
            TestInEditor();
            #endif
        }
        
        /// <summary>
        /// 简化的Android相册打开方法
        /// </summary>
        private void OpenAndroidGallerySimple()
        {
            try
            {
                // 使用最简单的方法打开相册
                AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
                AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
                
                // ACTION_PICK方法
                intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_PICK"));
                intentObject.Call<AndroidJavaObject>("setType", "image/*");
                
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                
                // 直接启动，不等待回调（简化）
                currentActivity.Call("startActivity", intentObject);
                
                Debug.Log("已打开系统相册，请手动记录选择的图片路径");
                Debug.Log("提示：选择图片后，图片路径会显示在系统日志中");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"打开相册失败: {e.Message}");
                DisplayErrorTexture("打开相册失败");
            }
        }
        
        /// <summary>
        /// 测试方法：加载测试图片
        /// </summary>
        public void TestLoadImage(string imagePath)
        {
            if (File.Exists(imagePath))
            {
                StartCoroutine(LoadImageCoroutine(imagePath));
            }
            else
            {
                Debug.LogError($"图片文件不存在: {imagePath}");
                DisplayErrorTexture("图片不存在");
            }
        }
        
        /// <summary>
        /// 协程加载图片
        /// </summary>
        private IEnumerator LoadImageCoroutine(string imagePath)
        {
            // 将文件路径转换为file:// URL
            string url = "file://" + imagePath;
            
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
            {
                yield return request.SendWebRequest();
                
                if (request.result == UnityWebRequest.Result.Success)
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(request);
                    displayImage.texture = texture;
                    Debug.Log($"图片加载成功: {imagePath}");
                }
                else
                {
                    Debug.LogError($"加载图片失败: {request.error}");
                    DisplayErrorTexture("加载失败");
                }
            }
        }
        
        /// <summary>
        /// 显示占位符纹理
        /// </summary>
        private void DisplayPlaceholderTexture()
        {
            if (displayImage != null)
            {
                // 创建简单的占位符
                Texture2D texture = new Texture2D(2, 2);
                Color[] colors = new Color[4] {
                    new Color(0.9f, 0.9f, 0.9f),
                    new Color(0.8f, 0.8f, 0.8f),
                    new Color(0.8f, 0.8f, 0.8f),
                    new Color(0.9f, 0.9f, 0.9f)
                };
                texture.SetPixels(colors);
                texture.Apply();
                
                displayImage.texture = texture;
            }
        }
        
        /// <summary>
        /// 显示错误纹理
        /// </summary>
        private void DisplayErrorTexture(string message = "错误")
        {
            if (displayImage != null)
            {
                Texture2D texture = new Texture2D(2, 2);
                texture.SetPixel(0, 0, Color.red);
                texture.SetPixel(1, 0, Color.white);
                texture.SetPixel(0, 1, Color.white);
                texture.SetPixel(1, 1, Color.red);
                texture.Apply();
                
                displayImage.texture = texture;
                Debug.LogWarning($"显示错误纹理: {message}");
            }
        }
        
        /// <summary>
        /// 编辑器测试
        /// </summary>
        private void TestInEditor()
        {
            Debug.Log("编辑器测试：生成测试图片");
            
            // 创建测试纹理
            Texture2D testTexture = new Texture2D(256, 256);
            for (int y = 0; y < 256; y++)
            {
                for (int x = 0; x < 256; x++)
                {
                    bool isEven = ((x / 32) + (y / 32)) % 2 == 0;
                    Color color = isEven ? new Color(0.2f, 0.5f, 0.8f) : new Color(0.8f, 0.5f, 0.2f);
                    testTexture.SetPixel(x, y, color);
                }
            }
            testTexture.Apply();
            
            if (displayImage != null)
            {
                displayImage.texture = testTexture;
                Debug.Log("测试图片显示成功");
            }
        }
        
        /// <summary>
        /// 清理资源
        /// </summary>
        private void OnDestroy()
        {
            if (galleryButton != null)
            {
                galleryButton.onClick.RemoveListener(OpenGallery);
            }
        }
    }
}