# 五子棋游戏场景搭建指南

## 场景配置步骤

### 1. 主菜单场景 (MainMenu.unity)
1. **创建场景文件**
   - 在Unity中右键Assets/Scenes文件夹
   - 选择Create > Scene，命名为"MainMenu"

2. **设置场景内容**
   ```
   Canvas
   ├── Title (TextMeshPro) - "五子棋游戏"
   ├── Panel (背景面板)
   │   ├── StartButton
   │   └── ExitButton
   └── EventSystem
   ```

3. **添加MainMenuManager组件**
   - 创建空GameObject，命名为"MainMenuManager"
   - 添加MainMenuManager脚本
   - 在Inspector中拖拽对应的UI组件

### 2. 游戏场景 (GameScene.unity)
1. **创建场景文件**
   - 创建新场景，命名为"GameScene"

2. **设置摄像机**
   - 设置Main Camera位置：(7, 10, -7)
   - 设置Rotation：(45, 0, 0) - 俯视角度
   - 设置Projection为Orthographic，Size为8-10

3. **创建游戏对象**
   ```
   GameManager (GameObject)
   ├── BoardManager (GameObject)
   │   ├── GridLines
   │   └── BoardBackground
   ├── InputManager (GameObject)
   └── ResourceManager (GameObject)
   ```

4. **设置UI Canvas**
   ```
   Canvas
   ├── GameStatusPanel
   │   ├── CurrentPlayerText
   │   └── GameStatusText
   ├── GameOverPanel (默认隐藏)
   │   ├── WinnerText
   │   ├── RestartButton
   │   └── MainMenuButton
   └── EventSystem
   ```

5. **添加组件**
   - BoardManager：添加BoardManager脚本
   - InputManager：添加InputManager脚本
   - Canvas：添加GameUIManager脚本
   - 某个GameObject：添加ResourceManager脚本

## 预制件创建

### 1. 棋子预制件
1. **黑棋子**
   - 创建Sphere，命名为"BlackPiece"
   - Scale设置为(0.8, 0.8, 0.8)
   - 创建材质，颜色设置为黑色
   - 添加Piece脚本组件
   - 拖拽到Prefabs文件夹创建预制件

2. **白棋子**
   - 复制黑棋子，命名为"WhitePiece"
   - 修改材质颜色为白色
   - 创建预制件

### 2. 材质设置
1. **棋盘材质**
   - 创建新材质，命名为"BoardMaterial"
   - 颜色设置为木色(0.8, 0.6, 0.4)
   - 设置Smoothness为0.2

2. **网格线材质**
   - 创建材质，使用Sprites/Default着色器
   - 颜色设置为黑色

## 脚本配置

### BoardManager配置
- Board Size: 15
- Cell Size: 1.0
- Black Piece Prefab: 拖拽黑棋子预制件
- White Piece Prefab: 拖拽白棋子预制件
- Board Material: 拖拽棋盘材质

### InputManager配置
- Main Camera: 拖拽场景中的主摄像机
- Board Layer Mask: 设置为棋盘所在的层

### ResourceManager配置
- 如果预制件和材质已创建，拖拽对应组件
- 否则脚本会自动创建默认资源

## 测试步骤

1. **构建测试**
   - File > Build Settings
   - 添加两个场景到构建列表
   - 点击Build测试

2. **功能测试**
   - 启动主菜单场景
   - 测试开始游戏按钮
   - 在游戏场景测试棋子放置
   - 测试重新开始和返回菜单

## 常见问题

1. **UI不显示**
   - 检查Canvas的Render Mode
   - 确认EventSystem存在
   - 检查UI元素的锚点设置

2. **点击无响应**
   - 检查摄像机设置
   - 确认InputManager的Layer Mask设置
   - 检查棋盘是否有Collider组件

3. **棋子不显示**
   - 检查预制件引用
   - 确认材质设置
   - 检查Scale设置

## 优化建议

1. **性能优化**
   - 使用对象池管理棋子
   - 避免频繁创建/销毁对象
   - 优化UI重绘

2. **视觉效果**
   - 添加棋子放置音效
   - 增加棋盘纹理
   - 添加粒子效果

3. **用户体验**
   - 添加鼠标悬停预览
   - 实现悔棋功能
   - 添加游戏历史记录