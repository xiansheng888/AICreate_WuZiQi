# 相册功能调试指南

## 问题描述
在真机测试时，选择完图片回到场景后没有看到选择的图片。

## 可能的原因
1. **Android回调未正确触发**：Unity的`OnActivityResult`可能没有被调用
2. **权限问题**：缺少读取存储的权限
3. **URI加载问题**：`content://` URI无法直接通过UnityWebRequest加载
4. **图片路径转换失败**：无法从URI获取实际文件路径

## 解决方案

### 方案1：使用简化的GalleryOpenerSimple
1. 删除当前的GalleryOpener组件
2. 添加`GalleryOpenerSimple`组件到GalleryManager GameObject
3. 这个版本使用最简化的方法，至少确保能打开相册

### 方案2：使用Android插件（推荐）
1. 确保以下文件存在：
   - `Assets/Plugins/Android/GalleryActivity.java`
   - `Assets/Plugins/Android/AndroidManifest.xml`
2. 重新构建APK并安装
3. GalleryOpener会自动尝试使用插件

### 方案3：手动调试
1. 在Android设备上安装应用
2. 使用ADB查看日志：
   ```bash
   adb logcat -s Unity
   ```
3. 查看以下关键日志：
   - "打开相册" - 按钮点击
   - "成功启动相册" - 相册打开成功
   - "OnImageSelected" - 图片选择回调
   - 任何错误信息

## 测试步骤

### 步骤1：基础测试
1. 打开`GalleryScene.unity`
2. 确保场景中有GalleryManager GameObject
3. 添加`GalleryOpener`或`GalleryOpenerSimple`组件
4. 运行场景，点击"打开相册"按钮
5. 检查控制台输出

### 步骤2：Android真机测试
1. 构建APK并安装到Android设备
2. 运行应用，点击"打开相册"
3. 选择一张图片
4. 观察：
   - 是否返回应用
   - 控制台是否有回调日志
   - RawImage是否显示图片

### 步骤3：权限检查
1. 确保AndroidManifest.xml包含：
   ```xml
   <uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />
   ```
2. 对于Android 6.0+，可能需要运行时请求权限
3. 在应用设置中检查权限是否已授予

## 调试代码

### 关键调试点
1. **打开相册**：`OpenAndroidGallery()`方法
2. **回调接收**：`OnImageSelected()`方法
3. **图片加载**：`LoadImageFromUriString()`方法

### 添加更多日志
在`GalleryOpener.cs`中添加更多Debug.Log：
```csharp
Debug.Log($"URI类型: {uriString}");
Debug.Log($"文件是否存在: {File.Exists(filePath)}");
Debug.Log($"Texture尺寸: {texture.width}x{texture.height}");
```

## 备用方案

### 如果回调仍然不工作
1. 使用`GalleryOpenerSimple.TestLoadImage()`方法手动测试图片加载
2. 将图片复制到设备，然后调用：
   ```csharp
   galleryOpener.TestLoadImage("/sdcard/Download/test.jpg");
   ```
3. 这可以验证图片显示功能是否正常

### 使用第三方插件
如果上述方案都不工作，考虑使用成熟的第三方插件：
- **Unity Native Gallery**：专门处理相册功能
- **Android Native Plugin**：提供完整的Android原生功能集成

## 文件清单
- `Assets/Scripts/UI/GalleryOpener.cs` - 主脚本（已更新）
- `Assets/Scripts/UI/GalleryOpenerSimple.cs` - 简化版本
- `Assets/Plugins/Android/GalleryActivity.java` - Android插件
- `Assets/Plugins/Android/AndroidManifest.xml` - Android配置
- `Assets/Scenes/GalleryScene.unity` - 测试场景
- `Assets/GallerySetupGuide.md` - 使用指南

## 联系支持
如果问题仍然存在，请提供：
1. Android设备型号和系统版本
2. Unity版本
3. 完整的控制台日志
4. 问题发生的具体步骤