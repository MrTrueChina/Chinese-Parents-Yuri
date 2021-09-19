using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityModManagerNet;
using System.Reflection;
using UnityEngine;
using MtC.Mod.ChineseParents.ForceGirl;
using UnityEngine.UI;
using Spine.Unity;
using DG.Tweening;

namespace MtC.Mod.ChineseParents.Yuri
{
    public static class Main
    {
        /// <summary>
        /// Mod 对象
        /// </summary>
        public static UnityModManager.ModEntry ModEntry { get; set; }

        /// <summary>
        /// 这个 Mod 是否启动
        /// </summary>
        public static bool enabled;

        /// <summary>
        /// 相亲页面最多出现多少个同学
        /// </summary>
        public static int maxClassmateBlinddatesNumber = 2;

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            // 保存 Mod 对象
            ModEntry = modEntry;
            ModEntry.OnToggle = OnToggle;

            // 加载 Harmony
            var harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll();

            modEntry.Logger.Log("百合 Mod 加载完成");

            // 返回加载成功
            return true;
        }

        /// <summary>
        /// Mod Manager 对 Mod 进行控制的时候会调用这个方法
        /// </summary>
        /// <param name="modEntry"></param>
        /// <param name="value">这个 Mod 是否激活</param>
        /// <returns></returns>
        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            // 将 Mod Manager 切换的状态保存下来
            enabled = value;

            // 返回 true 表示这个 Mod 切换到 Mod Manager 切换的状态，返回 false 表示 Mod 依然保持原来的状态
            return true;
        }
    }

    ////////--------////////--------//////// 让女儿版可以打开儿子版的社交窗口的方法 ////////--------////////--------////////

    /// <summary>
    /// 打开社交窗口的方法，确切地说是给社交按钮绑定打开社交窗口方法的方法
    /// </summary>
    [HarmonyPatch(typeof(open_system), "AddListener")]
    public static class open_system_AddListener
    {
        /// <summary>
        /// 是否打开玩家相反性别的社交面板
        /// </summary>
        public static bool anotherSex = false;

        private static void Postfix(open_system __instance)
        {
            // 如果 Mod 未启动则不作处理
            if (!Main.enabled)
            {
                return;
            }

            Main.ModEntry.Logger.Log("open_system.AddListener 调用完毕");

            // 如果当前这代是儿子，不进行处理
            if (record_manager.InstanceManagerRecord.IsBoy())
            {
                Main.ModEntry.Logger.Log("这一代是儿子，不进行处理");
                return;
            }

            // 给社交按钮的点击事件绑定打开面板的方法
            EventTriggerListener.Get(__instance.GirlGameObject).onClick = delegate (GameObject go)
            {
                int alertness;
                GameObject panel;

                // 如果是打开相反性别的社交面板，则打开女生面板，否则打开男生面板
                if (anotherSex)
                {
                    alertness = girlmanager.InstanceGirlmanager.alertness;
                    panel = (Resources.Load("UI/girls/Panel_girls") as GameObject);
                }
                else
                {
                    alertness = BoysManager.Instance.alertness;
                    panel = (Resources.Load("UI/Boys/Panel_Boys") as GameObject);
                }
                __instance.alert_talk(alertness, panel);

                // 每次点击更新是否显示相反性别的标志量
                anotherSex = !anotherSex;
            };
        }
    }

    ////////--------////////--------//////// 同步儿子版的开局数据给女儿版（可能不是必须的）、让女儿版可以保存女同学好感度（可能不是必须的） ////////--------////////--------////////

    /// <summary>
    /// 开新周目时生成数据的方法，里面有制作组说过的二周目性别和一周目相反的代码
    /// </summary>
    [HarmonyPatch(typeof(end_panel_info), "reset_record")]
    public static class end_panel_info_reset_record
    {
        private static void Postfix()
        {
            // 如果 Mod 未启动则不作处理
            if (!Main.enabled)
            {
                return;
            }

            Main.ModEntry.Logger.Log("生成新周目玩家数据方法即将调用");

            // 周目是儿子则不处理
            if (record_manager.InstanceManagerRecord.CurrentRecord.playerSex == 2)
            {
                Main.ModEntry.Logger.Log("新周目是儿子，不进行处理");
                return;
            }

            Main.ModEntry.Logger.Log("新周目是女儿，填充儿子独有的朋友给女儿");

            // 反编译出的代码，目测是所有可能登场的朋友的列表，男女根据需要使用一部分
            List<int> idlist = ReadXml.GetIDList("friend", null, null);

            // 反编译出的循环，原逻辑是根据性别填充
            foreach (int num2 in idlist)
            {
                XmlData data = ReadXml.GetData("friend", num2);
                int @int = data.GetInt("sex");

                // 原本的逻辑，0 推测是一个特殊角色，其他的是按照性别填入的
                //if (@int == 0 || @int == record_manager.InstanceManagerRecord.CurrentRecord.playerSex)

                // 在原本的条件上，如果是儿子的朋友，而且在列表里没有的话，就加进去。因为这个逻辑的女儿部分在原方法就执行过，这里只要补上儿子独有的就行
                if ((@int == 0 || @int == 1) && !record_manager.InstanceManagerRecord.CurrentRecord.FriendlistDictionary.ContainsKey(num2))
                {
                    record_manager.InstanceManagerRecord.CurrentRecord.FriendlistDictionary.Add(num2, data.GetInt("init"));
                }
            }
        }
    }

    /// <summary>
    /// 在开新档时生成新的玩家数据的方法
    /// </summary>
    [HarmonyPatch(typeof(record_manager), "create_new")]
    public static class record_manager_create_new
    {
        private static void Postfix(ref record __result)
        {
            // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
            if (!Main.enabled)
            {
                return;
            }

            Main.ModEntry.Logger.Log("开新档时生成玩家数据方法即将调用");

            // 新档是儿子则不处理
            if (__result.playerSex == 1)
            {
                Main.ModEntry.Logger.Log("新档是儿子，不作处理");
                return;
            }

            Main.ModEntry.Logger.Log("填充儿子独有的朋友给女儿");

            // 反编译出的代码，目测是所有可能登场的朋友的列表，男女根据需要使用一部分，但这个朋友列表并不是社交的男女同学列表
            List<int> idlist = ReadXml.GetIDList("friend", null, null);

            // 反编译出的循环，原逻辑是根据性别填充
            foreach (int num in idlist)
            {
                XmlData data = ReadXml.GetData("friend", num);
                int @int = data.GetInt("sex");

                // 原本的逻辑，0 推测是一个特殊角色，其他的是按照性别填入的
                //if (@int == 0 || @int == __result.playerSex)

                // 在原本的条件上，如果是儿子的朋友，而且在列表里没有的话，就加进去。因为这个逻辑的女儿部分在原方法就执行过，这里只要补上儿子独有的就行
                if ((@int == 0 || @int == 1) && !__result.FriendlistDictionary.ContainsKey(num))
                {
                    __result.FriendlistDictionary.Add(num, data.GetInt("init"));
                }
            }

            Main.ModEntry.Logger.Log("给女儿设置女同学列表");
            // 反编译出的代码，本来是儿子才会执行的，填充女同学列表
            record_manager.InstanceManagerRecord.CurrentRecord.GirlsDictionary = new Dictionary<int, int>();
            foreach (int key in ReadXml.GetIDList("girl", null, null))
            {
                record_manager.InstanceManagerRecord.CurrentRecord.GirlsDictionary.Add(key, 0);
            }
        }
    }

    ////////--------////////--------//////// 让女儿可以看到所有的女生 ////////--------////////--------////////

    /// <summary>
    /// 女生面板刷新的方法
    /// </summary>
    [HarmonyPatch(typeof(panel_girls), "refresh")]
    public static class panel_girls_refresh
    {
        private static void Postfix(ScrollRect ___scroll, Image[] ___bgButtons, List<GameObject> ___list)
        {
            // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
            if (!Main.enabled)
            {
                return;
            }

            Main.ModEntry.Logger.Log("panel_girls.refresh 调用完毕");

            // 如果当前这代是儿子，不进行处理
            if (record_manager.InstanceManagerRecord.IsBoy())
            {
                Main.ModEntry.Logger.Log("这一代是儿子，不进行处理");
                return;
            }

            // 激活所有按钮背景的接收射线检测功能
            ___bgButtons.ToList().ForEach(button => button.raycastTarget = true);

            // 激活所有按钮
            ___list.ForEach(button => button.SetActive(true));

            // 激活第 9 个按钮，这个按钮跟别的不一样，它背景都是隐藏的
            ___bgButtons[8].gameObject.SetActive(true);

            // 启动滚动条的垂直滚动功能
            ___scroll.vertical = true;
        }

        ///// <summary>
        ///// 来自反编译的代码，这是刷新女生面板的方法，这个方法放在这里留作参考，不会进行调用
        ///// </summary>
        ///// <param name="__instance">panel_girls 对象</param>
        ///// <param name="___list">所有得女生按钮列表</param>
        ///// <param name="___girl">女生按钮的预制</param>
        ///// <param name="___girlList">所有女生的数据</param>
        ///// <param name="___scroll">社交面板的滚动条组件</param>
        ///// <param name="___infoPanel"></param>
        ///// <param name="___bgButtons">背景列表，所有按钮是背景+角色画像组合的</param>
        ///// <param name="___close"></param>
        //private static bool Prefix(panel_girls __instance, List<GameObject> ___list, GameObject ___girl, XmlList ___girlList, ScrollRect ___scroll, GameObject ___infoPanel, Image[] ___bgButtons, Button ___close)
        //{
        //    // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
        //    if (!Main.enabled)
        //    {
        //        return true;
        //    }

        //    Main.ModEntry.Logger.Log("panel_girls.refresh 即将调用");

        //    // 测试代码输出女生列表
        //    foreach (KeyValuePair<int, int> pair in girlmanager.InstanceGirlmanager.GirlsDictionary)
        //    {
        //        Main.ModEntry.Logger.Log("遍历女生列表：key = " + pair.Key + ", value = " + pair.Value);
        //    }

        //    // 输出女生数据
        //    foreach (KeyValuePair<int, XmlData> girl in ___girlList.value)
        //    {
        //        int id = girl.Key;
        //        string name = girl.Value.GetStringLanguage("name");
        //        string image = "UI/girls/" + girl.Value.GetString("head");

        //        Main.ModEntry.Logger.Log("遍历女生数据，id = " + id + ", 名称 = " + name + ", 图片 = " + image);
        //    }

        //    // 以下是原始代码

        //    // 获取编译器生成的私有内部类 <refresh>c__AnonStorey1，这个类在反编译器中一般是默认隐藏的，需要开启编译器的显示自动生成的类功能
        //    Type type_c__AnonStorey1 = typeof(panel_girls).GetNestedType("<refresh>c__AnonStorey1", BindingFlags.NonPublic);

        //    // 这里应该是清除已有的选项，就是所有的女同学
        //    for (int i = 0; i < ___list.Count; i++)
        //    {
        //        UnityEngine.Object.Destroy(___list[i]);
        //    }
        //    ___list.Clear();

        //    // 这个循环的功能是给面板里添加女同学
        //    int num = 0;
        //    using (Dictionary<int, int>.Enumerator enumerator = girlmanager.InstanceGirlmanager.GirlsDictionary.GetEnumerator())
        //    {
        //        while (enumerator.MoveNext())
        //        {
        //            // 发现这个对象在代码里几乎就是个数据中转站，而且都是从这个方法里中转的，把那些中转去掉后就剩下这一行创建对象了，不知道是为什么
        //            object c__AnonStorey = Activator.CreateInstance(type_c__AnonStorey1, new object[] { });

        //            // 这里是实例化女生选项，之后有初始化，就是图片和文本的显示
        //            GameObject item = UnityEngine.Object.Instantiate<GameObject>(___girl, ___bgButtons[num].transform);
        //            ___list.Add(item);
        //            item.name = enumerator.Current.Key.ToString();
        //            GameObject gameObject = item.transform.Find("Button_Name").gameObject;
        //            Text component = gameObject.transform.Find("Text_Name").GetComponent<Text>();
        //            Image component2 = item.transform.Find("head").GetComponent<Image>();
        //            Text component3 = item.transform.Find("loving").GetComponent<Text>();
        //            SkeletonGraphic talkSpine = item.transform.Find("TalkBubble").GetComponent<SkeletonGraphic>();
        //            SkeletonGraphic heartSpine = item.transform.Find("Heart").GetComponent<SkeletonGraphic>();
        //            if (enumerator.Current.Value >= 30)
        //            {
        //                heartSpine.gameObject.SetActive(true);
        //                item.transform.Find("NormalHeart").gameObject.SetActive(false);
        //            }
        //            XmlData xmlData = ___girlList.Get(enumerator.Current.Key);
        //            // 女生名字
        //            component.text = xmlData.GetStringLanguage("name");
        //            // 设置图片
        //            component2.sprite = (Resources.Load("UI/girls/" + xmlData.GetString("head"), typeof(Sprite)) as Sprite);
        //            component2.SetNativeSize();
        //            // 好感度
        //            component3.text = enumerator.Current.Value.ToString();
        //            XmlData itemData = ___girlList.Get(int.Parse(item.name));

        //            // 测试输出
        //            Main.ModEntry.Logger.Log("原代码循环内，id = " + enumerator.Current.Key + ", 名称 = " + xmlData.GetStringLanguage("name") + ", 图片 = " + xmlData.GetString("head"));

        //            // 这里应该是和滚动条有关的
        //            ScrollRectListener gameObject2 = ScrollRectListener.GetGameObject(___bgButtons[num].gameObject);
        //            gameObject2.SetScrollRect(___scroll);

        //            // 绑定点击效果
        //            EventTriggerListener.Get(___bgButtons[num].gameObject).onClick = delegate (GameObject go)
        //            {
        //                MonoBehaviour.print(" ScrollRectListener.Get(bgButtons[index].gameObject).onClick");
        //                if (!__instance.is_talk(int.Parse(item.name)))
        //                {
        //                    chat_manager.InstanceChatManager.start_chat(itemData.GetInt("hello_bad"), 0, 0, null, null, null, string.Empty, false, false);
        //                }
        //                else if (player_data.Instance.Potentiality >= (int)typeof(panel_girls).GetField("need_potential", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null))
        //                {
        //                    player_data.Instance.Potentiality -= (int)typeof(panel_girls).GetField("need_potential", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);

        //                    __instance.chatPanel(int.Parse(item.name));
        //                    MessageCenter.sendMessage("refresh_ui_data", null);
        //                }
        //                else
        //                {
        //                    TipsManager.instance.AddTips(ReadXml.GetString("PotentialityNotEnough"), 1);
        //                }
        //            };

        //            // 绑定鼠标进入效果，猜测是鼠标悬浮时的高光效果
        //            EventTriggerListener.Get(___bgButtons[num].gameObject).onEnter = delegate (GameObject go)
        //            {
        //                talkSpine.gameObject.SetActive(true);
        //                lzhspine.change_anim_ui(talkSpine, talkSpine.SkeletonDataAsset, "animation", true, null);
        //                int num2 = 0;
        //                if (enumerator.Current.Value >= 30)
        //                {
        //                    num2 = 1;
        //                }
        //                else if (enumerator.Current.Value >= 50)
        //                {
        //                    num2 = 2;
        //                }
        //                else if (enumerator.Current.Value >= 80)
        //                {
        //                    num2 = 3;
        //                }
        //                if (num2 > 0)
        //                {
        //                    heartSpine.timeScale = (float)num2;
        //                    lzhspine.change_anim_ui2(heartSpine, heartSpine.SkeletonDataAsset, "play", true);
        //                }
        //                go.transform.Find("Image_Light").GetComponent<Image>().DOFade(0.3f, 0.5f);
        //            };

        //            // 绑定鼠标离开效果，猜测是鼠标离开后取消高光效果
        //            EventTriggerListener.Get(___bgButtons[num].gameObject).onExit = delegate (GameObject go)
        //            {
        //                talkSpine.gameObject.SetActive(false);
        //                if (enumerator.Current.Value >= 30)
        //                {
        //                    heartSpine.timeScale = 1f;
        //                    lzhspine.change_anim_ui2(heartSpine, heartSpine.SkeletonDataAsset, "idle", true);
        //                }
        //                go.transform.Find("Image_Light").GetComponent<Image>().DOFade(0f, 0.5f);
        //            };

        //            ScrollRectListener gameObject3 = ScrollRectListener.GetGameObject(gameObject);
        //            gameObject3.SetScrollRect(___scroll);
        //            EventTriggerListener.Get(gameObject).onClick = delegate (GameObject go)
        //            {


        //                ___close.gameObject.SetActive(false);
        //                GameObject info = UnityEngine.Object.Instantiate<GameObject>(___infoPanel, __instance.transform);
        //                info.GetComponent<Button>().onClick.AddListener(delegate ()
        //                {


        //                    ___close.gameObject.SetActive(true);
        //                    UnityEngine.Object.DestroyObject(info);
        //                });
        //                info.transform.Find("head").GetComponent<Image>().sprite = (Resources.Load("UI/girls/" + itemData.GetString("image_info"), typeof(Sprite)) as Sprite);
        //                info.transform.Find("name").GetComponent<Text>().text = itemData.GetStringLanguage("name");
        //                info.transform.Find("desc").GetComponent<Text>().text = itemData.GetStringLanguage("desc");
        //            };
        //            num++;
        //        }
        //    }

        //    // 这里是根据回合数隐藏同学的功能，包括第 9 个同学出现时开启滚动条的功能
        //    int[] array = new int[]
        //    {
        //        27,
        //        28,
        //        28,
        //        29,
        //        29
        //    };
        //    for (int j = 4; j <= 8; j++)
        //    {
        //        if (___list[j] == null)
        //        {
        //            return false;
        //        }
        //        if (player_data.Instance.Round_current >= array[j - 4])
        //        {
        //            ___list[j].gameObject.SetActive(true);
        //            if (j == 8)
        //            {
        //                // 索引 8，这是第 9 个按钮，这个按钮出现后就要启动滚动范围，之前这个按钮是不在显示范围里的
        //                ___bgButtons[8].gameObject.SetActive(true);
        //                ___scroll.vertical = true;
        //            }
        //        }
        //        else
        //        {
        //            // 这里是隐藏还没到显示时间的按钮，可以看出来除了禁用外还关闭了接收射线的功能
        //            ___bgButtons[j].raycastTarget = false;
        //            ___list[j].gameObject.SetActive(false);
        //        }
        //    }

        //    return false;
        //}
    }

    ////////--------////////--------//////// 让女儿版和女生的社交文本像儿子版一样正常显示 ////////--------////////--------////////

    /// <summary>
    /// 获取多语种字符串的方法，几乎所有显示的文本都来自这个方法
    /// </summary>
    [HarmonyPatch(typeof(XmlData), "GetStringLanguage", new Type[] { typeof(string), typeof(bool) })]
    public static class XmlData_GetStringLanguage_name_sex
    {
        /// <summary>
        /// 在前后缀之间传递参数的类
        /// </summary>
        public class GetStringLanguageParams
        {
            public string name;
            public bool sex;

            public GetStringLanguageParams(string name, bool sex = false)
            {
                this.name = name;
                this.sex = sex;
            }
        }

        private static void Prefix(out GetStringLanguageParams __state, string name, ref bool sex)
        {
            // 向后缀传递参数，虽然后缀也可以像前缀一样用同名参数获取方法参数，但这个方法内部修改了参数，所以需要在前缀备份一份直接发给后缀
            __state = new GetStringLanguageParams(name, sex);
        }

        private static void Postfix(XmlData __instance, GetStringLanguageParams __state, ref string __result)
        {
            // 如果 Mod 未启动则不作处理
            if (!Main.enabled)
            {
                return;
            }

            // 这一代是儿子则不处理
            if (record_manager.InstanceManagerRecord.IsBoy())
            {
                Main.ModEntry.Logger.Log("这一代是儿子，不作处理");
                return;
            }

            // 原本调用的时候就是无视性别的，不作处理
            if (!__state.sex)
            {
                return;
            }

            // 如果没获取到文本，使用无视性别的方式再获取一次
            if ("0".Equals(__result))
            {
                __result = __instance.GetStringLanguage(__state.name, false);
                return;
            }

            // 如果是获取对话人物名称，而且返回的是错误的名称，使用无视性别的方式再获取一次
            List<string> wrongNames = new List<string>() { "女孩1", "女孩2", "女孩3", "女孩4", "女孩5", "女孩6", "女孩7", "女孩8", "女孩9" };
            if ("player".Equals(__state.name) && wrongNames.Contains(__result))
            {
                __result = __instance.GetStringLanguage(__state.name, false);
                return;
            }
        }
    }

    ////////--------////////--------//////// 让女儿版可以保存女同学好感度 ////////--------////////--------////////

    /// <summary>
    /// 保存游戏时调用的方法
    /// </summary>
    [HarmonyPatch(typeof(record_manager), "saverecord")]
    public static class record_manager_saverecord
    {
        private static void Postfix(record_manager __instance)
        {
            // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
            if (!Main.enabled)
            {
                return;
            }

            Main.ModEntry.Logger.Log("保存游戏方法调用结束");

            // 这一代是儿子则不处理
            if (record_manager.InstanceManagerRecord.IsBoy())
            {
                Main.ModEntry.Logger.Log("这一代是儿子，不作处理");
                return;
            }

            Main.ModEntry.Logger.Log("保存游戏，补充女生列表");

            // 清空女生列表，从女生数据里重新存入
            __instance.CurrentRecord.GirlsDictionary.Clear();
            foreach (KeyValuePair<int, int> keyValuePair7 in girlmanager.InstanceGirlmanager.GirlsDictionary)
            {
                Main.ModEntry.Logger.Log("遍历女生列表，id = " + keyValuePair7.Key + ", 好感 = " + keyValuePair7.Value);
                __instance.CurrentRecord.GirlsDictionary.Add(keyValuePair7.Key, keyValuePair7.Value);
            }
        }
    }

    ////////--------////////--------//////// 同步男女社交面板的警告值 ////////--------////////--------////////

    /// <summary>
    /// 女生面板增加恋爱警告值的方法，这个方法会随机增加一定量的恋爱警告值，恋爱警告会根据警告值出现
    /// </summary>
    [HarmonyPatch(typeof(panel_girls), "add_alert")]
    public static class panel_girls_add_alert
    {
        private static void Postfix()
        {
            // 如果 Mod 没有启动，不进行处理
            if (!Main.enabled)
            {
                return;
            }

            // 这一代是儿子则不处理
            if (record_manager.InstanceManagerRecord.IsBoy())
            {
                Main.ModEntry.Logger.Log("这一代是儿子，不作处理");
                return;
            }

            Main.ModEntry.Logger.Log("女生面板增加恋爱警告值方法调用完毕");

            // 将女生面板的警告值同步到男生面板
            BoysManager.Instance.alertness = girlmanager.InstanceGirlmanager.alertness;
        }
    }

    /// <summary>
    /// 男生面板增加恋爱警告值的方法，这个方法和女生面板一样随机增加警告值。另外一提，这个方法在原代码中还负责继续向后调用
    /// </summary>
    [HarmonyPatch(typeof(BoysManager), "AddAlert")]
    public static class BoysManager_AddAlert
    {
        private static void Postfix(BoysManager __instance)
        {
            // 如果 Mod 没有启动，不进行处理
            if (!Main.enabled)
            {
                return;
            }

            // 这一代是儿子则不处理
            if (record_manager.InstanceManagerRecord.IsBoy())
            {
                Main.ModEntry.Logger.Log("这一代是儿子，不作处理");
                return;
            }

            Main.ModEntry.Logger.Log("男生面板增加恋爱警告值方法调用完毕");

            // 将男生面板的警告值同步到女生面板
            girlmanager.InstanceGirlmanager.alertness = BoysManager.Instance.alertness;
        }
    }

    ////////--------////////--------//////// 让女同学和闺蜜可以出现在相亲列表里 ////////--------////////--------////////

    /// <summary>
    /// 获取所有相亲选项的方法
    /// </summary>
    [HarmonyPatch(typeof(Panel_blinddate), "create_blinddates")]
    public static class Panel_blinddate_create_blinddates
    {
        /// <summary>
        /// 暂存同学 ID 和好感度的类
        /// </summary>
        public class ClassmateLoving
        {
            /// <summary>
            /// 同学 ID
            /// </summary>
            public int id;
            /// <summary>
            /// 好感度
            /// </summary>
            public int loving;

            public ClassmateLoving(int id,int loving)
            {
                this.id = id;
                this.loving = loving;
            }
        }

        private static void Postfix(Panel_blinddate __instance)
        {
            // 如果 Mod 未启动则不处理
            if (!Main.enabled)
            {
                return;
            }

            // 这一代是儿子则不处理
            if (record_manager.InstanceManagerRecord.IsBoy())
            {
                Main.ModEntry.Logger.Log("这一代是儿子，不作处理");
                return;
            }

            Main.ModEntry.Logger.Log("在相亲列表中添加同学方法调用完毕");

            // 获取所有同学的列表
            List<ClassmateLoving> classmates = new List<ClassmateLoving>();

            // 如果有女同学，将所有女同学添加到同学列表中
            if(girlmanager.InstanceGirlmanager != null && girlmanager.InstanceGirlmanager.GirlsDictionary != null)
            {
                List<ClassmateLoving> girlClassmates = girlmanager.InstanceGirlmanager.girlsDictionary
                    .Select(girlPair => new ClassmateLoving(girlPair.Key,girlPair.Value))
                    .ToList();

                classmates.AddRange(girlClassmates);
            }

            // 如果有男同学，将所有男同学添加到同学列表中
            if (BoysManager.Instance != null && BoysManager.Instance.BoysDictionary != null)
            {
                List<ClassmateLoving> boyClassmates = BoysManager.Instance.BoysDictionary
                    .Select(boyPair => new ClassmateLoving(boyPair.Key, boyPair.Value.loving))
                    .ToList();

                classmates.AddRange(boyClassmates);
            }

            // 按照好感度从高到低排列
            classmates.Sort((a, b) => b.loving - a.loving);

            Main.ModEntry.Logger.Log("所有同学列表加载完毕");

            // 相亲列表中同学的最大数量，这里要进行限制：不能超过原逻辑生成的选项数量、不能超过同学总量
            int maxClassmateBlinddatesNumber = Mathf.Min(Main.maxClassmateBlinddatesNumber, __instance.Blinddates.Count, classmates.Count);

            // 用同学的相亲选项替换掉原来的选项
            for (int i = 0; i < maxClassmateBlinddatesNumber; i++)
            {
                // 如果当前循环到的同学已经没有好感了，结束循环
                if(classmates[i].loving <= 0)
                {
                    break;
                }

                blinddate blinddate;
                if (classmates[i].id == 3008)
                {
                    // 3008 是闺蜜，原逻辑中她没有相亲数据
                    blinddate = getConfidanteMengMengBlinddateData();
                }
                else
                {
                    // 其他同学使用原逻辑的方法创建相亲选项
                    blinddate = blinddate.create(classmates[i].id);
                }

                // 按照原逻辑设定求婚成功率
                blinddate.base_winrate = Mathf.Min(80, classmates[i].loving);

                // 用这个同学的相亲选项替换到原来的相亲选项
                __instance.Blinddates[i] = blinddate;
            }
        }

        /// <summary>
        /// 获取 闺蜜-萌萌 的相亲数据
        /// </summary>
        /// <returns></returns>
        private static blinddate getConfidanteMengMengBlinddateData()
        {
            blinddate blinddate = new blinddate();

            blinddate.desc = "你的闺蜜，毕业后一直和你保持联系，现在是一名旅行作家，在旅行到你所在的城市时被介绍给了你，世界真是小。";

            // 我觉得萌萌应该就是章涵之，至少在定位上差不多，数据用的就是章涵之的数据，正好章涵之的数据很平均
            blinddate.name = "闺蜜-萌萌";
            blinddate.type = 2;
            blinddate.need_status = 15;
            blinddate.need_money = 50;
            blinddate.IQ_round = 2;
            blinddate.EQ_round = 2;
            blinddate.imagination_round = 2;
            blinddate.memory_round = 2;
            blinddate.stamination_round = 0;
            blinddate.base_winrate = 0;

            return blinddate;
        }
    }

    ////////--------////////--------//////// 测试代码 ////////--------////////--------////////

    /// <summary>
    /// 游戏界面的那些按钮的隐藏方法，用于显示社交按钮辅助测试
    /// </summary>
    [HarmonyPatch(typeof(open_system), "closeall")]
    public static class open_system_closeall
    {
        private static bool Prefix()
        {
            // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
            if (!Main.enabled)
            {
                return true;
            }

            Main.ModEntry.Logger.Log("open_system.closeall 即将调用");

            return false;
        }
    }
}
