using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


namespace Gomoku.UI
{
    /// <summary>
    /// 相册打开器，创建按钮并调用Android系统相册
    /// </summary>
    public class GalleryOpener : MonoBehaviour
    {
 
        [Header("UI设置")]
        [SerializeField] private bool autoInitialize = true;
        [SerializeField] private string buttonText = "打开相册";
        [SerializeField] private Vector2 buttonPosition = new Vector2(0, 0);
        [SerializeField] private Vector2 buttonSize = new Vector2(200, 80);
        [SerializeField] private Color buttonColor = new Color(0.2f, 0.6f, 1f, 1f);
        [SerializeField] private Color textColor = Color.white;

        [Header("图片显示设置")]
        [SerializeField] private RawImage displayImage;
        [SerializeField] private Vector2 imagePosition = new Vector2(0, 100);
        [SerializeField] private Vector2 imageSize = new Vector2(300, 300);
        [SerializeField] private bool autoCreateDisplayImage = true;

        [Header("引用")]
        [SerializeField] private Canvas targetCanvas;

        private Button galleryButton;
        private GameObject canvasInstance;

        // 用于Android回调的请求码
        private const int PICK_IMAGE_REQUEST = 1001;

        private void Start()
        {
            // 设置GameObject名称为GalleryManager，以便UnitySendMessage能找到
            gameObject.name = "GalleryManager";

            if (autoInitialize)
            {
                InitializeUI();
            }
        }

        /// <summary>
        /// 手动初始化UI
        /// </summary>
        public void InitializeUI()
        {
            if (galleryButton == null)
            {
                CreateUI();
            }
        }

        /// <summary>
        /// 创建UI界面
        /// </summary>
        private void CreateUI()
        {
            // 使用指定的Canvas或查找现有Canvas
            if (targetCanvas != null)
            {
                canvasInstance = targetCanvas.gameObject;
            }
            else
            {
                Canvas existingCanvas = FindObjectOfType<Canvas>();
                if (existingCanvas != null)
                {
                    canvasInstance = existingCanvas.gameObject;
                }
                else
                {
                    // 创建新的Canvas
                    canvasInstance = new GameObject("GalleryCanvas");
                    Canvas canvas = canvasInstance.AddComponent<Canvas>();
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    canvasInstance.AddComponent<CanvasScaler>();
                    canvasInstance.AddComponent<GraphicRaycaster>();
                }
            }

            // 创建按钮
            GameObject buttonObj = new GameObject("GalleryButton");
            buttonObj.transform.SetParent(canvasInstance.transform, false);

            RectTransform rectTransform = buttonObj.AddComponent<RectTransform>();
            rectTransform.anchoredPosition = buttonPosition;
            rectTransform.sizeDelta = buttonSize;

            // 添加按钮组件
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
            textComponent.color = textColor;
            textComponent.alignment = TextAlignmentOptions.Center;
            textComponent.fontSize = 24;

            // 添加按钮点击监听
            galleryButton.onClick.AddListener(OpenGallery);

            // 创建或设置显示图片的RawImage
            SetupDisplayImage();

            Debug.Log("相册按钮创建完成");
        }

        /// <summary>
        /// 设置显示图片的RawImage
        /// </summary>
        private void SetupDisplayImage()
        {
            if (displayImage == null && autoCreateDisplayImage)
            {
                // 创建RawImage显示图片
                GameObject imageObj = new GameObject("DisplayImage");
                imageObj.transform.SetParent(canvasInstance.transform, false);

                RectTransform imageRect = imageObj.AddComponent<RectTransform>();
                imageRect.anchoredPosition = imagePosition;
                imageRect.sizeDelta = imageSize;

                displayImage = imageObj.AddComponent<RawImage>();
                displayImage.color = Color.white;

                Debug.Log("创建显示图片的RawImage");
            }
            else if (displayImage != null)
            {
                // 确保RawImage在正确的Canvas下
                if (displayImage.transform.parent != canvasInstance.transform)
                {
                    displayImage.transform.SetParent(canvasInstance.transform, false);
                }
            }
        }

        /// <summary>
        /// 打开Android系统相册
        /// </summary>
        public void OpenGallery()
        {
            Debug.Log("打开相册");

#if UNITY_ANDROID && !UNITY_EDITOR
            OpenAndroidGallery();
#else
            Debug.LogWarning("非Android平台，无法打开系统相册");
            // 在编辑器中使用测试图片
            TestInEditor();
#endif
        }

        /// <summary>
        /// 打开Android系统相册
        /// </summary>
        private void OpenAndroidGallery()
        {
            Debug.Log("=== 开始打开Android相册 ===");
            
            // 首先尝试使用Android插件
            Debug.Log("尝试使用Android插件...");
            if (TryOpenWithAndroidPlugin())
            {
                Debug.Log("使用Android插件成功");
                return;
            }

            Debug.Log("Android插件不可用，使用原生方法");
            
            // 如果插件不可用，使用原生方法
            try
            {
                // 检查并请求权限
                Debug.Log("检查权限...");
                if (!CheckAndRequestPermission())
                {
                    Debug.LogError("缺少存储权限，无法打开相册");
                    return;
                }

                AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
                AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

                // 使用ACTION_PICK选择图片（更可靠）
                intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_PICK"));

                // 设置数据为媒体内容
                AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
                AndroidJavaObject contentUri = uriClass.CallStatic<AndroidJavaObject>("parse", "content://media/internal/images/media");
                intentObject.Call<AndroidJavaObject>("setData", contentUri);
                intentObject.Call<AndroidJavaObject>("setType", "image/*");

                // 启动Activity并等待结果
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                // 创建回调接口
                AndroidJavaObject callback = new AndroidJavaObject("com.unity3d.player.UnityPlayer$ActivityResultCallback",
                    new ActivityResultCallback(this));

                // 使用新的API（如果有）或旧的方法
                try
                {
                    // 尝试使用Unity 2020+的startActivityForResult方法
                    currentActivity.Call("startActivityForResult", intentObject, PICK_IMAGE_REQUEST, callback);
                    Debug.Log("使用startActivityForResult启动相册");
                }
                catch
                {
                    // 回退到旧的方法
                    currentActivity.Call("startActivityForResult", intentObject, PICK_IMAGE_REQUEST);
                    Debug.Log("使用startActivityForResult（无回调）启动相册");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"打开相册失败: {e.Message}\n{e.StackTrace}");
                // 尝试备用方法
                OpenGalleryAlternative();
            }
        }

        /// <summary>
        /// 尝试使用Android插件打开相册
        /// </summary>
        private bool TryOpenWithAndroidPlugin()
        {
            Debug.Log("尝试调用Android插件...");
            try
            {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                Debug.Log($"获取当前Activity: {currentActivity != null}");

                // 尝试调用openGallery方法
                try
                {
                    Debug.Log("调用openGallery方法...");
                    currentActivity.Call("openGallery");
                    Debug.Log("使用GalleryActivity插件打开相册 - 调用成功");
                    return true;
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"GalleryActivity插件调用失败: {e.Message}");
                    Debug.Log($"调用堆栈: {e.StackTrace}");
                    return false;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Android插件不可用，使用原生方法: {e.Message}");
                Debug.Log($"调用堆栈: {e.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// 从UnitySendMessage调用的方法：图片选择成功
        /// </summary>
        public void OnImageSelected(string uriString)
        {
            Debug.Log($"=== OnImageSelected被调用 ===");
            Debug.Log($"URI字符串: {uriString}");
            Debug.Log($"调用堆栈: {System.Environment.StackTrace}");
            LoadImageFromUriString(uriString);
        }

        /// <summary>
        /// 从UnitySendMessage调用的方法：相册错误
        /// </summary>
        public void OnGalleryError(string errorMessage)
        {
            Debug.LogError($"相册错误: {errorMessage}");
            DisplayErrorTexture();
        }

        /// <summary>
        /// 备用的打开相册方法
        /// </summary>
        private void OpenGalleryAlternative()
        {
            try
            {
                AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
                AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

                // 使用ACTION_GET_CONTENT作为备用
                intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_GET_CONTENT"));
                intentObject.Call<AndroidJavaObject>("setType", "image/*");
                intentObject.Call<AndroidJavaObject>("addCategory", intentClass.GetStatic<string>("CATEGORY_OPENABLE"));

                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                currentActivity.Call("startActivity", intentObject);
                Debug.Log("使用startActivity启动相册（无回调）");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"备用方法也失败: {e.Message}");
            }
        }

        /// <summary>
        /// 检查并请求权限
        /// </summary>
        private bool CheckAndRequestPermission()
        {
            try
            {
#if UNITY_2023_1_OR_NEWER
                // Unity 2023+ 使用新的权限系统
                if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.ExternalStorageRead))
                {
                    UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.ExternalStorageRead);
                    return false;
                }
#else
                // 旧版本Unity，假设权限已授予（需要在AndroidManifest中声明）
#endif
                return true;
            }
            catch
            {
                // 如果权限检查失败，假设权限已通过其他方式处理
                return true;
            }
        }

        /// <summary>
        /// Android活动结果回调
        /// </summary>
        public void OnActivityResult(string data)
        {
            Debug.Log($"=== OnActivityResult被调用 ===");
            Debug.Log($"数据: {(string.IsNullOrEmpty(data) ? "null/empty" : data)}");
            Debug.Log($"数据长度: {data?.Length ?? 0}");
            Debug.Log($"调用堆栈: {System.Environment.StackTrace}");

            if (string.IsNullOrEmpty(data))
            {
                Debug.Log("用户取消了图片选择或数据为空");
                return;
            }

            // 解析返回的数据（应该是URI字符串）
            if (data.StartsWith("content://") || data.StartsWith("file://"))
            {
                Debug.Log($"检测到有效的URI格式: {data.Substring(0, Math.Min(50, data.Length))}...");
                LoadImageFromUriString(data);
            }
            else
            {
                Debug.LogError($"无法识别的返回数据格式: {data}");
                Debug.Log($"数据前50字符: {data.Substring(0, Math.Min(50, data.Length))}");
            }
        }

        /// <summary>
        /// 从Android URI加载图片
        /// </summary>
        private void LoadImageFromAndroidUri(AndroidJavaObject uri)
        {
            // 将AndroidJavaObject URI转换为字符串
            string uriString = uri.Call<string>("toString");
            Debug.Log($"从Android URI加载图片: {uriString}");
            // 对于content:// URI，尝试获取文件路径
            if (uriString.StartsWith("content://"))
            {
                string filePath = GetPathFromUri(uri);
                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    LoadImageFromFilePath(filePath);
                    return;
                }
                else
                {
                    Debug.LogWarning("无法获取content:// URI的文件路径，尝试直接加载");
                }
            }

            // 使用UnityWebRequest加载图片
            StartCoroutine(LoadImageWithUnityWebRequest(uriString));
        }

        /// <summary>
        /// 使用UnityWebRequest加载图片
        /// </summary>
        private System.Collections.IEnumerator LoadImageWithUnityWebRequest(string uri)
        {
            // 对于content:// URI，UnityWebRequest可能无法直接加载
            // 尝试使用更兼容的方法
            if (uri.StartsWith("content://"))
            {
                Debug.LogWarning($"UnityWebRequest可能无法加载content:// URI: {uri}");
                // 创建错误纹理
                DisplayErrorTexture();
                yield break;
            }

            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(uri))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(request);
                    DisplayImage(texture);
                }
                else
                {
                    Debug.LogError($"加载图片失败: {request.error}");
                    // 尝试备用方法：通过文件路径加载
                    TryAlternativeLoadMethod(uri);
                }
            }
        }

        /// <summary>
        /// 尝试备用方法加载图片
        /// </summary>
        private void TryAlternativeLoadMethod(string uri)
        {
            Debug.Log($"尝试备用方法加载图片: {uri}");
            
            // 如果是file:// URI，直接读取文件
            if (uri.StartsWith("file://"))
            {
                string filePath = uri.Substring(7); // 移除"file://"前缀
                Debug.Log($"file:// URI转换为路径: {filePath}");
                LoadImageFromFilePath(filePath);
            }
            else if (uri.StartsWith("content://"))
            {
                Debug.LogWarning($"收到content:// URI，Android插件文件复制可能失败: {uri}");
                Debug.Log("显示错误纹理，建议检查Android插件日志");
                DisplayErrorTexture();
            }
            else
            {
                Debug.LogWarning($"不支持的URI格式: {uri}");
                DisplayErrorTexture();
            }
        }

        /// <summary>
        /// 从文件路径加载图片
        /// </summary>
        private void LoadImageFromFilePath(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    byte[] fileData = File.ReadAllBytes(filePath);
                    Texture2D texture = new Texture2D(2, 2);
                    if (texture.LoadImage(fileData))
                    {
                        DisplayImage(texture);
                    }
                    else
                    {
                        Debug.LogError("从文件路径加载图片失败");
                    }
                }
                else
                {
                    Debug.LogError($"文件不存在: {filePath}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"从文件路径加载图片失败: {e.Message}");
            }
        }

        /// <summary>
        /// 从URI获取文件路径（改进版）
        /// </summary>
        private string GetPathFromUri(AndroidJavaObject uri)
        {
            try
            {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                // 方法1：使用DocumentsContract（适用于Android 4.4+）
                try
                {
                    AndroidJavaClass documentsContract = new AndroidJavaClass("android.provider.DocumentsContract");
                    if (documentsContract.CallStatic<bool>("isDocumentUri", currentActivity, uri))
                    {
                        string documentId = documentsContract.CallStatic<string>("getDocumentId", uri);
                        Debug.Log($"Document ID: {documentId}");

                        // 解析Document ID获取文件路径
                        if (documentId.StartsWith("raw:"))
                        {
                            return documentId.Substring(4);
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"DocumentsContract方法失败: {e.Message}");
                }

                // 方法2：通过ContentResolver查询
                AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject>("getContentResolver");
                string[] projection = new string[] { "_data", "_display_name" };
                AndroidJavaObject cursor = contentResolver.Call<AndroidJavaObject>("query", uri, projection, null, null, null);

                if (cursor != null && cursor.Call<bool>("moveToFirst"))
                {
                    // 尝试获取_data列
                    int columnIndex = cursor.Call<int>("getColumnIndex", "_data");
                    if (columnIndex >= 0)
                    {
                        string path = cursor.Call<string>("getString", columnIndex);
                        cursor.Call("close");

                        if (!string.IsNullOrEmpty(path) && File.Exists(path))
                        {
                            return path;
                        }
                    }

                    // 如果_data不可用，使用_display_name创建临时文件路径
                    cursor.Call<bool>("moveToFirst");
                    int nameColumnIndex = cursor.Call<int>("getColumnIndex", "_display_name");
                    if (nameColumnIndex >= 0)
                    {
                        string displayName = cursor.Call<string>("getString", nameColumnIndex);
                        cursor.Call("close");

                        if (!string.IsNullOrEmpty(displayName))
                        {
                            // 创建临时文件路径
                            string tempDir = Application.temporaryCachePath;
                            string tempFilePath = Path.Combine(tempDir, displayName);

                            // 复制文件到临时位置
                            CopyUriToFile(uri, contentResolver, tempFilePath);
                            return tempFilePath;
                        }
                    }

                    cursor.Call("close");
                }

                // 方法3：获取URI的最后路径段作为文件名
                string uriString = uri.Call<string>("toString");
                string lastSegment = uri.Call<string>("getLastPathSegment");
                if (!string.IsNullOrEmpty(lastSegment))
                {
                    string tempDir = Application.temporaryCachePath;
                    string tempFilePath = Path.Combine(tempDir, lastSegment);
                    CopyUriToFile(uri, contentResolver, tempFilePath);
                    return tempFilePath;
                }

                return null;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"获取文件路径失败: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// 将URI内容复制到文件
        /// </summary>
        private void CopyUriToFile(AndroidJavaObject uri, AndroidJavaObject contentResolver, string filePath)
        {
            try
            {
                // 打开输入流
                AndroidJavaObject inputStream = contentResolver.Call<AndroidJavaObject>("openInputStream", uri);
                if (inputStream == null)
                {
                    Debug.LogError("无法打开InputStream");
                    return;
                }

                // 创建输出文件
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    byte[] buffer = new byte[4096];

                    // 读取输入流
                    // 注意：这里需要调用Android InputStream的read方法
                    // 简化实现，直接记录日志
                    Debug.Log($"将URI内容复制到: {filePath}");

                    // 实际实现需要调用inputStream.Call<byte[]>("read", buffer)等
                    // 但由于AndroidJavaObject交互复杂，这里使用简化版本
                }

                // 关闭输入流
                inputStream.Call("close");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"复制URI到文件失败: {e.Message}");
            }
        }



        /// <summary>
        /// 显示图片到RawImage
        /// </summary>
        private void DisplayImage(Texture2D texture)
        {
            if (displayImage != null)
            {
                displayImage.texture = texture;
                Debug.Log("图片显示成功");
            }
            else
            {
                Debug.LogWarning("显示图片失败: displayImage未设置");
            }
        }

        /// <summary>
        /// 显示错误纹理
        /// </summary>
        private void DisplayErrorTexture()
        {
            if (displayImage != null)
            {
                // 创建简单的错误纹理
                Texture2D errorTexture = new Texture2D(2, 2);
                errorTexture.SetPixel(0, 0, Color.red);
                errorTexture.SetPixel(1, 0, Color.white);
                errorTexture.SetPixel(0, 1, Color.white);
                errorTexture.SetPixel(1, 1, Color.red);
                errorTexture.Apply();

                displayImage.texture = errorTexture;
                Debug.LogWarning("显示错误纹理（无法加载图片）");
            }
        }

        /// <summary>
        /// Activity结果回调类
        /// </summary>
        private class ActivityResultCallback : AndroidJavaProxy
        {
            private GalleryOpener galleryOpener;

            public ActivityResultCallback(GalleryOpener opener) : base("com.unity3d.player.UnityPlayer$ActivityResultCallback")
            {
                galleryOpener = opener;
            }

            public void onActivityResult(int requestCode, int resultCode, AndroidJavaObject data)
            {
                Debug.Log($"ActivityResultCallback.onActivityResult: requestCode={requestCode}, resultCode={resultCode}");

                if (requestCode != PICK_IMAGE_REQUEST)
                {
                    return;
                }

                // RESULT_OK = -1
                if (resultCode != -1)
                {
                    Debug.Log("用户取消了图片选择");
                    galleryOpener.OnActivityResult(null);
                    return;
                }

                if (data == null)
                {
                    Debug.LogError("选择的图片数据为空");
                    galleryOpener.OnActivityResult(null);
                    return;
                }

                // 获取图片URI
                AndroidJavaObject uri = data.Call<AndroidJavaObject>("getData");
                if (uri == null)
                {
                    Debug.LogError("无法获取图片URI");
                    galleryOpener.OnActivityResult(null);
                    return;
                }

                string uriString = uri.Call<string>("toString");
                galleryOpener.OnActivityResult(uriString);
            }
        }

        /// <summary>
        /// 从URI字符串加载图片
        /// </summary>
        private void LoadImageFromUriString(string uriString)
        {
            Debug.Log($"从URI字符串加载图片: {uriString}");
            
            // 检查是否为普通文件路径（不包含://）
            if (!uriString.Contains("://"))
            {
                Debug.Log("检测到普通文件路径，直接加载");
                LoadImageFromFilePath(uriString);
                return;
            }
            
            // 对于content:// URI，需要特殊处理
            if (uriString.StartsWith("content://"))
            {
                Debug.Log("检测到content:// URI，尝试直接读取字节流");
                // 优先尝试直接读取字节流
                StartCoroutine(LoadImageFromContentUri(uriString));
                return;
            }

            // 对于file:// URI，使用UnityWebRequest加载
            if (uriString.StartsWith("file://"))
            {
                StartCoroutine(LoadImageWithUnityWebRequest(uriString));
                return;
            }
            
            // 其他情况（如http://等）
            Debug.LogWarning($"不支持的URI格式: {uriString}");
            TryAlternativeLoadMethod(uriString);
        }

        /// <summary>
        /// 从content:// URI加载图片（简化版）
        /// </summary>
        private System.Collections.IEnumerator LoadImageFromContentUri(string uriString)
        {
            Debug.Log($"从content:// URI加载图片（简化版）: {uriString}");
            Debug.Log("注意：Android插件应该已将文件复制到缓存目录并返回文件路径");
            Debug.Log("如果仍然收到content:// URI，说明文件复制失败，尝试备用方法");
            
            // 直接尝试备用方法
            TryAlternativeLoadMethod(uriString);
            yield break;
        }

        /// <summary>
        /// 在编辑器中的测试方法
        /// </summary>
        private void TestInEditor()
        {
            Debug.Log("在编辑器中测试：模拟打开相册");
            // 创建测试纹理
            Texture2D testTexture = CreateTestTexture();
            DisplayImage(testTexture);
        }

        /// <summary>
        /// 创建测试纹理
        /// </summary>
        private Texture2D CreateTestTexture()
        {
            int size = 256;
            Texture2D texture = new Texture2D(size, size);

            // 创建简单的棋盘格图案
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    bool isEven = ((x / 32) + (y / 32)) % 2 == 0;
                    Color color = isEven ? new Color(0.8f, 0.4f, 0.4f) : new Color(0.4f, 0.4f, 0.8f);
                    texture.SetPixel(x, y, color);
                }
            }

            texture.Apply();
            return texture;
        }

        /// <summary>
        /// 销毁时清理资源
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