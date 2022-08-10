五虎的UI框架

把UI面板分为以下几个类型

TYPE_ITEM,    // 只是用资源路径

TYPE_BASE,          // 常驻场景的UI，Close不销毁 一级UI
TYPE_POP,           // 弹出式UI，互斥，当前只能有一个弹出界面 二级弹出 在一级之上 阻止移动 无法操作后面UI
TYPE_STORY,         // 故事界面，故事界面出现，所有UI消失，故事界面关闭，其他界面恢复
TYPE_TIP,           // 三级弹出 在二级之上 不互斥 阻止移动 无法操作后面UI
TYPE_MENUPOP,       // TYPE_POP的一个分支 由主菜单MenuBar打开的二级UI 主要用于动态加载特殊屏蔽区域 其他和POPUI完全一致
 TYPE_MESSAGE,       // 消息提示UI 在三级之上 一般是最高层级 不互斥 不阻止移动 可操作后面UI
 TYPE_DEATH,         // 死亡UI 目前只有复活界面 用于添加复活特殊规则

每个层级都有有一个对应的 Dictionary<string, GameObject> 缓存字典，此外还有一个关闭面板的缓存字典，关闭的UI都会放到这个字典中