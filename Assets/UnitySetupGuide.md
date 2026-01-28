# 五子棋游戏 - Unity项目设置指南

## 🎯 快速开始

### 第一步：打开项目
1. 打开Unity Hub
2. 点击"Open"按钮
3. 选择项目文件夹：`C:\Users\Administrator\Desktop\demo`
4. 等待Unity加载完成

### 第二步：设置主菜单场景
1. 双击打开 `Assets/Scenes/MainMenu.unity`
2. 在场景中创建一个空的GameObject，命名为"Setup"
3. 添加 `QuickSceneSetup` 脚本
4. 在Inspector中勾选 `SetupMainMenu`
5. 点击Play按钮运行一次，Unity会自动创建主菜单UI
6. 停止Play，删除Setup GameObject

### 第三步：设置游戏场景
1. 双击打开 `Assets/Scenes/GameScene.unity`
2. 在场景中创建一个空的GameObject，命名为"Setup"
3. 添加 `QuickSceneSetup` 脚本
4. 在Inspector中勾选 `SetupGameScene`
5. 点击Play按钮运行一次，Unity会自动创建游戏场景基础结构
6. 停止Play，删除Setup GameObject

### 第四步：创建预制件
1. 在Project窗口右键 `Assets > Create > Prefab`，命名为"BlackPiece"
2. 在场景中创建一个Sphere (`GameObject > 3D Object > Sphere`)
3. 设置Scale为(0.8, 0.8, 0.8)
4. 创建新材质，颜色设为黑色，应用到Sphere
5. 添加 `Piece` 脚本到Sphere
6. 拖拽Sphere到BlackPiece预制件上
7. 重复步骤创建"WhitePiece"预制件（颜色为白色）

### 第五步：配置脚本引用
在GameScene中：
1. 选择"ResourceManager"对象
2. 拖拽BlackPiece和WhitePiece预制件到对应的字段
3. 选择"BoardManager"对象
4. 拖拽预制件到对应字段
5. 选择"InputManager"对象
6. 设置Main Camera引用

在MainMenu中：
1. 选择"MainMenuManager"对象
2. 拖拽StartButton和ExitButton到对应字段

## 🎮 测试游戏

### 添加场景到构建设置
1. `File > Build Settings`
2. 点击"Add Open Scenes"添加当前场景
3. 添加MainMenu和GameScene两个场景
4. 将MainMenu设置为第一个（索引0）

### 运行测试
1. 点击Play按钮
2. 在主菜单点击"开始游戏"
3. 在游戏中点击棋盘格子下棋

## 🛠️ 脚本功能说明

### 核心脚本
- `GameManager.cs` - 游戏主管理器
- `BoardManager.cs` - 棋盘管理和显示
- `InputManager.cs` - 用户输入处理
- `GameUIManager.cs` - 游戏界面控制
- `MainMenuManager.cs` - 主菜单控制

### 工具脚本
- `QuickSceneSetup.cs` - 快速场景初始化
- `SceneGenerator.cs` - 完整场景生成器
- `ResourceManager.cs` - 资源管理

### 数据脚本
- `GameData.cs` - 游戏数据结构
- `GomokuConfig.cs` - 游戏配置

## 🎨 美术资源

### 材质
- 黑白棋子材质
- 棋盘木质背景材质
- 高亮效果材质

### UI
- 简洁的按钮设计
- 文本显示
- 游戏状态面板

## 🔧 常见问题解决

### 问题1：UI不显示
- 检查Canvas的Render Mode设置为Screen Space Overlay
- 确认EventSystem存在
- 检查UI元素的锚点和位置

### 问题2：点击无响应
- 检查InputManager的Layer Mask设置
- 确认Main Camera设置正确
- 检查棋盘是否有Collider

### 问题3：预制件引用丢失
- 重新拖拽预制件到脚本字段
- 确认预制件文件存在
- 检查脚本编译无错误

## 🚀 进阶功能

### AI系统
- 当前使用随机AI
- 可以升级为评分算法
- 支持难度调节

### 音效系统
- 添加落子音效
- 背景音乐
- UI交互音效

### 视觉效果
- 棋子放置动画
- 获胜高亮效果
- 粒子特效

## 📱 平台适配

### 移动端
- 添加触摸输入支持
- 调整UI布局
- 优化性能

### Web端
- WebGL构建设置
- 浏览器兼容性
- 加载优化

---

## 🔧 第二阶段更新：核心游戏逻辑和AI系统

第二阶段开发已经完成！以下是新增的功能和组件：

### 🆕 新增脚本

1. **GameRules.cs** - 游戏规则管理器
   - 完整的五子连珠胜负判定算法
   - 支持禁手规则（可配置）
   - 棋盘评估和分数计算
   - 合法移动验证

2. **AIPlayer.cs** - AI玩家系统
   - 4种难度级别：简单、中等、困难、专家
   - 启发式搜索和评估
   - Minimax算法支持（带Alpha-Beta剪枝）
   - 可配置的思考时间

3. **BoardManager增强功能**
   - 撤销/重做支持
   - 棋盘状态模拟和恢复
   - 棋子位置查询和过滤
   - 棋盘状态深拷贝

### 🎮 更新使用指南

#### 1. 添加新组件到游戏场景
在GameScene中：
1. 选择"BoardManager"对象
2. 添加 `GameRules` 组件
3. 创建新的GameObject，命名为"AIPlayer"
4. 添加 `AIPlayer` 组件到AIPlayer对象

#### 2. 配置AI系统
1. 选择"AIPlayer"对象
2. 在Inspector中设置：
   - AI Piece Type: White (白棋)
   - Difficulty: Medium (中等难度)
   - Think Time: 1.0 (思考时间)
   - Board Manager: 拖拽BoardManager对象
   - Game Rules: 拖拽BoardManager上的GameRules组件

#### 3. 更新GameUIManager
1. 选择"GameUIManager"对象
2. 在Inspector中添加新引用：
   - Game Rules: 拖拽BoardManager上的GameRules组件
   - AI Player: 拖拽AIPlayer对象

#### 4. 测试新功能
1. 运行游戏场景
2. 测试胜负判定：连续放置5个同色棋子应该触发胜利
3. 测试AI对手：AI应该会合理地下棋
4. 测试撤销功能（如果实现）

### 🚀 技术特性

#### 游戏规则系统
- **胜负判定**：检查四个方向（水平、垂直、两个对角线）的五子连珠
- **棋盘评估**：基于棋子模式计算分数（活四、冲四、活三等）
- **合法移动验证**：检查位置是否有效，是否为空，是否违反禁手规则

#### AI系统
- **简单难度**：随机移动，优先选择中心区域
- **中等难度**：启发式评估，考虑进攻和防守
- **困难难度**：2层Minimax搜索
- **专家难度**：3层Minimax搜索，带Alpha-Beta剪枝

#### 性能优化
- 对象池管理棋子
- 棋盘状态缓存
- 评估函数优化

### 🐛 已知问题和解决方案

#### 问题1：AI思考时间过长
- **解决方案**：降低AI难度或减少思考时间
- **配置**：在AIPlayer组件中设置Difficulty为Easy或Medium，ThinkTime为0.5

#### 问题2：撤销功能不工作
- **解决方案**：确保使用BoardManager的RemovePiece方法而不是直接修改数据
- **注意**：模拟落子使用SimulatePlacePiece，实际落子使用PlacePiece

#### 问题3：禁手规则误判
- **解决方案**：暂时禁用禁手规则
- **配置**：在GameRules组件中取消勾选Enable Forbidden Moves

### 📈 下一步开发建议

#### 短期优化
1. 添加音效系统
2. 实现悔棋功能
3. 添加游戏历史记录
4. 优化UI动画效果

#### 长期功能
1. 网络多人对战
2. 棋谱保存和加载
3. 高级AI训练
4. 3D视觉效果增强

---

现在你可以按照这个指南在Unity中设置和运行完整的五子棋游戏了！所有核心代码都已经编写完成，包括完整的游戏规则和智能AI系统。