## 简介

本工具通过修改的游戏文件 Assembly-CSharp.dll 获取数据，然后在 libTAS 执行 Lua 脚本显示数据到画面上，尽量减少对游戏的影响避免 desync，不过会稍微降低游戏运行速度。

支持 Hollow Knight 1028/1221/1432/1432_Mod Linux 版本
需要最低 libtas_1.4.1_amd64_hk_v9 版本

## Assembly-CSharp.dll 修改的内容

* GameManager 中新增字段 `private static readonly long TasInfoMark = 1234567890123456789` 字段用于辅助内存查找时定位
* GameManager 中新增字段 `public static string TasInfo` 用于 libTAS lua 脚本读取然后绘制到画面上
* CameraController 新增 OnPreRender 和 OnPostRender 方法分别调用 TasInfo 中的同名方法，用于完成镜头居中以及缩放，以及各种数据的获取
* PlayMakerUnity2DProxy.Start() 方法中调用 TasInfo 中的同名方法，用于处理新创建的 Object

## 功能

* 小骑士相关信息，包括位置，速度，状态等
* LiveSplit 相同逻辑的游戏时间
* 敌人的数据与碰撞箱
* 镜头跟随、缩放和禁止震动
* 自定义附加显示的数据，需要对 HK 代码有一定了解
* 通过编辑 `HollowKnightTasInfo.config` 文件，实时开关各项功能

## 使用说明

1. 复制 `hollow_knight_Data/Manager/Assembly-CSharp.dll` 文件到 `游戏目录/hollow_knight_Data/Manager` 目录中进行覆盖。`original/Assembly-CSharp.dll` 是未经修改的原版文件
2. libTAS 菜单 `Video -> OSD -> Lua` 勾选
3. libTAS 菜单 `Tools -> Lua -> Execute lua script` 打开 `HollowKnightTasInfo.lua` 即可在读取存档后显示辅助信息
4. 编辑`游戏目录/HollowKnightTasInfo.config`文件，可以实时开关各项功能以及定制需要获取的数据

## 致谢

* Shout out ot Kilaye for HK 特供版 [libTAS](https://github.com/clementgallet/libTAS/tree/hollowknight) 以及增加了画线的 lua api
* 参考 [LiveSplit.HollowKnight](https://github.com/ShootMe/LiveSplit.HollowKnight) 的游戏时间计算方式
* 参考 [HollowKnight.Modding](https://github.com/HollowKnight-Modding/HollowKnight.Modding) MonoMod 的使用
* 参考 [HollowKnight.HitboxDraw](https://github.com/seresharp/HollowKnight.HitboxDraw) 绘制 hitbox
* 参考 [DebugMod](https://github.com/seresharp/DebugMod) 获取 HP 和镜头跟随缩放
* 借助 [HKWorldEdit2](https://github.com/nesrak1/HKWorldEdit2) 直接在 Unity 浏览场景
* 借助 [HollowKnightFSMView](https://github.com/nesrak1/HollowKnightFSMView) 查看 PlayMaker FSM
* cuber_kk、inoki、Zippy Rhys 的测试
