# Unity C# 开发指南 - AI Agent 指令

## 语言和风格
- 始终使用简体中文回复
- 直接回答问题，不要客套话
- 代码注释也用中文

## 构建和测试命令

### Unity 编辑器命令
- **打开项目**: 在 Unity Hub 中打开项目文件夹
- **构建项目**: `File > Build Settings > Build` 或 Unity Cloud Build
- **运行场景**: 点击 Unity 编辑器顶部的播放按钮
- **脚本编译**: Unity 自动编译，或 `Assets > Refresh`

### 命令行构建
```bash
# Windows 批处理文件构建
Unity.exe -batchmode -quit -projectPath [项目路径] -buildTarget StandaloneWindows64 -executeMethod BuildScript.Build

# macOS/Linux 构建
Unity -batchmode -quit -projectPath [项目路径] -buildTarget StandaloneOSX -executeMethod BuildScript.Build
```

### 测试框架
- **Unity Test Framework**: 使用 Unity Test Runner 窗口 (`Window > General > Test Runner`)
- **运行所有测试**: 在 Test Runner 中点击 "Run All"
- **运行单个测试**: 在 Test Runner 中双击特定测试类或方法
- **命令行测试**: `Unity.exe -runTests -projectPath [项目路径] -testResults [结果文件路径]`

## 代码规范

### 命名约定
```csharp
// 类名：PascalCase
public class PlayerController : MonoBehaviour
{
    // 私有字段：camelCase，可选下划线前缀
    private float _movementSpeed = 5f;
    private Transform playerTransform;
    
    // 公共属性：PascalCase
    public float Health { get; private set; }
    
    // 公共方法：PascalCase
    public void TakeDamage(float damage)
    {
        Health -= damage;
    }
    
    // 私有方法：camelCase
    private void handleMovement()
    {
        // 实现逻辑
    }
}

// 常量：PascalCase 或全大写
public static class GameConstants
{
    public const float MAX_HEALTH = 100f;
    public const string PLAYER_TAG = "Player";
}

// 接口：PascalCase，可选 I 前缀
public interface IDamageable
{
    void TakeDamage(float damage);
}
```

### 文件组织
```csharp
// 1. 引用命名空间
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2. 命名空间（可选）
namespace MyGame.Player
{
    // 3. 类定义
    public class PlayerController : MonoBehaviour
    {
        // 4. 字段和属性
        [SerializeField] private GameObject playerPrefab;
        [Header("移动设置")]
        [Range(1f, 10f)] private float moveSpeed = 5f;
        
        // 5. Unity 生命周期方法
        private void Awake() { }
        private void Start() { }
        private void Update() { }
        private void FixedUpdate() { }
        
        // 6. 公共方法
        public void Initialize() { }
        
        // 7. 私有方法
        private void HandleInput() { }
    }
}
```

### 格式化规范
- **缩进**: 4 个空格（Unity 默认）
- **大括号**: K&R 风格，左大括号不换行
- **行长度**: 建议 120 字符以内
- **空格**: 操作符前后加空格，函数调用后不加空格

### 特性使用
```csharp
public class ExampleComponent : MonoBehaviour
{
    [Header("基础设置")]           // 分组标题
    [SerializeField] private int maxHealth = 100;
    
    [Space(10)]                  // 增加垂直间距
    [SerializeField] private float speed = 5f;
    
    [Range(0f, 10f)]             // 滑块范围
    [SerializeField] private float volume = 1f;
    
    [Tooltip("移动速度的描述")]    // 工具提示
    [SerializeField] private bool isGrounded;
    
    [ContextMenu("重置设置")]      // 右键菜单
    private void ResetSettings()
    {
        maxHealth = 100;
        speed = 5f;
    }
}
```

## 错误处理和调试

### 异常处理
```csharp
public void LoadLevel(string levelName)
{
    try
    {
        SceneManager.LoadScene(levelName);
    }
    catch (System.Exception e)
    {
        Debug.LogError($"加载关卡失败: {levelName}\n{e}");
        // 降级处理
        LoadMainMenu();
    }
}
```

### 日志规范
```csharp
// 信息日志
Debug.Log("玩家已初始化");

// 警告日志
Debug.LogWarning("未找到音频组件，可能影响音效播放");

// 错误日志
Debug.LogError("关键资源加载失败，游戏无法继续");

// 条件日志
if (debugMode)
{
    Debug.Log($"详细调试信息: {playerPosition}");
}
```

### 空值检查
```csharp
public void Attack(GameObject target)
{
    if (target == null)
    {
        Debug.LogWarning("攻击目标为空");
        return;
    }
    
    var damageable = target.GetComponent<IDamageable>();
    if (damageable == null)
    {
        Debug.LogWarning($"目标 {target.name} 没有伤害接口");
        return;
    }
    
    damageable.TakeDamage(damageAmount);
}
```

## Unity 最佳实践

### 组件设计
```csharp
// 使用 RequireComponent 确保必要组件
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private Animator animator;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }
    
    // 使用 SerializeField 让私有字段在 Inspector 中可见
    [SerializeField] private float moveSpeed = 5f;
    
    // 使用属性封装复杂逻辑
    public bool IsMoving 
    { 
        get { return rb.velocity.magnitude > 0.1f; } 
    }
}
```

### 对象池模式
```csharp
public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int poolSize = 10;
    
    private Queue<GameObject> pool = new Queue<GameObject>();
    
    private void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }
    
    public GameObject GetObject()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        
        return Instantiate(prefab);
    }
    
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
```

### 单例模式
```csharp
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
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
    
    // 游戏状态管理
    public int Score { get; private set; }
    
    public void AddScore(int points)
    {
        Score += points;
        // 触发 UI 更新事件
        OnScoreChanged?.Invoke(Score);
    }
    
    public event System.Action<int> OnScoreChanged;
}
```

## 性能优化指南

### Update 方法优化
```csharp
// 避免在 Update 中进行昂贵操作
private void Update()
{
    // 好：简单的逻辑
    HandleInput();
    UpdateAnimation();
    
    // 避免：查找组件、创建对象等
    // GetComponent<Transform>(); // 不好！
    // Instantiate(prefab);        // 不好！
}

// 将昂贵操作缓存到 Start 或 Awake
private Transform cachedTransform;
private void Start()
{
    cachedTransform = GetComponent<Transform>();
}
```

### Coroutines 使用
```csharp
public void StartDelayedAction()
{
    StartCoroutine(DelayedAction());
}

private IEnumerator DelayedAction()
{
    // 等待 3 秒
    yield return new WaitForSeconds(3f);
    
    // 执行延迟操作
    Debug.Log("延迟操作完成");
    
    // 等待到下一帧
    yield return null;
    
    // 继续操作
}
```

## 工作习惯
- 修改代码前先阅读相关文件，理解项目结构
- 不确定时先问，不要猜测 Unity API 或项目逻辑
- 每次只做最小必要的修改
- 测试新功能时使用 Play Mode
- 提交前确保代码在 Unity 中编译无错误
- 使用版本控制，提交前排除 Library、Temp、obj 等临时文件夹