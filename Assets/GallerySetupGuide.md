# 相册功能设置指南

## 功能概述
- 创建一个UI按钮，点击后调用Android系统相册
- 自动在场景中创建Canvas和按钮
- 支持Android平台，编辑器模式下有测试提示

## 使用步骤

### 1. 打开场景
- 在Unity编辑器中打开 `Assets/Scenes/GalleryScene.unity`

### 2. 添加相册打开器脚本
- 在Hierarchy面板中创建一个空GameObject，命名为 "GalleryManager"
- 将 `Assets/Scripts/UI/GalleryOpener.cs` 脚本拖拽到该GameObject上

### 3. 配置UI（可选）
- 在Inspector面板中可以调整按钮位置、大小和文本
- 默认设置：按钮位于屏幕中央，尺寸200x80，文本"打开相册"
- 图片显示设置：可调整图片位置、大小，或指定现有RawImage组件

### 4. 运行测试
- 点击Play按钮进入运行模式
- 点击屏幕上的"打开相册"按钮
- **Android真机**：会打开系统相册，选择的图片将显示在上方区域
- **编辑器模式**：自动生成测试图片并显示在上方区域

## Android权限配置

### 1. 修改AndroidManifest.xml
在 `Assets/Plugins/Android/AndroidManifest.xml` 中添加以下权限：

```xml
<!-- 读取外部存储权限 -->
<uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />
<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />

<!-- Android 11及以上需要额外权限 -->
<uses-permission android:name="android.permission.MANAGE_EXTERNAL_STORAGE" />
```

### 2. 如果不存在AndroidManifest.xml
- 在Unity编辑器中打开 `File > Build Settings`
- 切换到Android平台
- 点击 `Player Settings`
- 在 `Other Settings` 中找到 `Write Permission` 并设置为 `External (SDCard)`

## 图片显示功能

### 1. 自动创建显示区域
- 脚本会自动创建一个RawImage组件用于显示选择的图片
- 图片显示区域默认位于按钮上方（Y轴偏移100像素）
- 默认尺寸300x300像素，白色背景

### 2. 自定义显示设置
在GalleryOpener组件的Inspector中可调整：
- **Display Image**：指定已有的RawImage组件（如不指定则自动创建）
- **Image Position**：图片显示位置
- **Image Size**：图片显示尺寸
- **Auto Create Display Image**：是否自动创建RawImage

### 3. 使用现有RawImage
如需使用场景中已有的RawImage组件：
1. 在Hierarchy中创建或选择RawImage对象
2. 将RawImage拖拽到GalleryOpener组件的"Display Image"字段
3. 设置"Auto Create Display Image"为false

## 扩展功能建议

### 1. 获取选择的图片
当前版本已实现图片获取和显示功能：
- 使用 `StartActivityForResult` 获取选择结果
- 在Unity中实现 `onActivityResult` 回调处理
- 自动将选择的图片转换为Texture2D并显示在RawImage上

### 2. 适配更多平台
当前脚本仅支持Android，可扩展支持：
- iOS：使用 `UIImagePickerController`
- PC：使用 `System.Windows.Forms.OpenFileDialog`

## 问题排查

### 1. 按钮不显示
- 检查是否有多个Canvas，脚本可能创建了新的Canvas
- 确认Canvas的Render Mode为 `Screen Space - Overlay`

### 2. Android上无法打开相册
- 检查AndroidManifest.xml权限配置
- 确认API Level >= 23时需要动态请求权限
- 查看Logcat日志获取详细错误信息

### 3. 编辑器模式下无反应
- 正常现象，脚本只在Android平台执行原生调用
- 控制台会显示"非Android平台，无法打开系统相册"提示
- 编辑器模式下会自动生成测试图片并显示

### 4. 图片无法显示
- 检查AndroidManifest.xml是否包含读取存储权限
- 确认API Level >= 29 (Android 10) 时需要适配作用域存储
- 对于某些设备，可能需要动态请求权限
- 检查Logcat日志获取详细的错误信息

### 5. RawImage不显示或位置不对
- 确认Canvas的Render Mode为 `Screen Space - Overlay`
- 检查RawImage的RectTransform设置是否正确
- 尝试调整Image Position和Image Size参数

### 6. 真机回调不工作（常见问题）
如果选择图片后没有显示，可能是以下原因：
1. **Android回调未触发**：使用提供的Android插件方案
2. **权限问题**：确保AndroidManifest.xml包含正确的权限
3. **URI加载失败**：content:// URI需要特殊处理

**解决方案**：
- 查看`Assets/GalleryDebugGuide.md`获取详细调试步骤
- 尝试使用`GalleryOpenerSimple`简化版本
- 使用ADB查看完整的Unity日志

## 快速测试
如果遇到问题，按以下步骤测试：
1. 使用`GalleryOpenerSimple`组件
2. 在编辑器模式下测试图片显示
3. 构建APK并检查Android日志
4. 参考`GalleryDebugGuide.md`进行调试

## 脚本位置
`Assets/Scripts/UI/GalleryOpener.cs`

## 场景位置
`Assets/Scenes/GalleryScene.unity`