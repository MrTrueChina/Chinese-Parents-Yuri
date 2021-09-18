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

    /// <summary>
    /// 游戏界面的那些按钮的隐藏方法，这个方法会在游戏开始后将所有按钮隐藏掉之后在别的地方按需开启
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

            //// 如果当前这代是儿子，不进行处理
            //if (record_manager.InstanceManagerRecord.IsBoy())
            //{
            //    Main.ModEntry.Logger.Log("这一代是儿子，不进行处理");
            //    return;
            //}

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

    [HarmonyPatch(typeof(XmlData), "GetInt", new Type[] { typeof(string) })]
    public static class XmlData_GetInt
    {
        private static void Postfix(int __result, string name)
        {
            // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
            if (!Main.enabled)
            {
                return;
            }

            // 不是目标
            if (!("hello_bad".Equals(name)))
            {
                return;
            }

            Main.ModEntry.Logger.Log("XmlData.GetInt(\"hello_bad\") 调用完毕，结果 = " + __result);
        }
    }

    [HarmonyPatch(typeof(ReadXml), "GetData")]
    public static class ReadXml_GetData
    {
        private static void Postfix(XmlData __result, string fileName, int id)
        {
            // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
            if (!Main.enabled)
            {
                return;
            }

            // 不是目标
            if (!("chat".Equals(fileName)) || (id != 8200801))
            {
                return;
            }

            Main.ModEntry.Logger.Log("ReadXml.GetData(\"chat\", 8200801) 调用完毕，结果 = " + __result);

            foreach (KeyValuePair<string, string> pair in __result.value)
            {
                Main.ModEntry.Logger.Log("遍历结果：key = " + pair.Key + ", value = " + pair.Value);
            }
        }
    }

    [HarmonyPatch(typeof(chat_manager), "start_chat")]
    public static class chat_manager_start_chat
    {
        private static bool Prefix(chat_manager __instance, ref GameObject ___PanelGameObject, ref bool ___changedBGM, ref List<string> ___chatPlayerList, ref List<string>[] ___chatPlayerLists, ref List<string>[] ___chatDaughterLists, ref int ___effectId, ref int ___nextId, ref int ___curId, int id, int next, int next2, begintalkoverAction completeAction, string newtext, Transform parentTransform, string param, bool moreText, bool isAnniversary)
        {
            // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
            if (!Main.enabled)
            {
                return true;
            }

            PrintChatData(id);

            // 不是目标
            if (id != 8200801 && id != 8200901)
            {
                return true;
            }

            Main.ModEntry.Logger.Log("发现目标，进行 ID = " + id + " 的对话");

            // 疑似是试玩版的面子对决特殊处理，15001 是第一次出现面子对决事件，201000001 是第一次出现面子挑战事件
            if (DEF.isApproval && (id == 15001 || id == 201000001))
            {
                begintalkoverAction begintalkoverAction = completeAction;
                if (begintalkoverAction != null)
                {
                    begintalkoverAction();
                }
                return false;
            }

            // 疑似是对开发者的特殊处理，开发者应该可以在另一个位置看对话
            Transform transform;
            if (DEF.IsDevelopment)
            {
                transform = UIManager.instance.Canvas_transform;
                ___PanelGameObject = (Resources.Load("UIpanel/Panel_chat") as GameObject);
            }
            else
            {
                transform = __instance.transform;
                if (__instance.transform == null)
                {
                    transform = UIManager.instance.Canvas_transform;
                    Debug.Log("transform" + __instance.transform);
                }
            }

            // aGameObject 目前看来是对话的根物体，之后的大量操作都是从这个 aGameObject 向下搜索进行
            GameObject aGameObject = UnityEngine.Object.Instantiate<GameObject>(___PanelGameObject, transform.position, Quaternion.identity, transform);
            // 禁用了 "next" 物体，这个物体从名字看应该就是进入下一段对话的功能的物体，但是考虑到对话是按空格或左键都可以向后，这个物体可能并不是按钮
            aGameObject.transform.Find("next").gameObject.SetActive(false);
            
            // 获取对话数据
            XmlData data = ReadXml.GetData("chat", id);

            // 与周年纪念日有关，但不确定是什么事件，isAnniversary 翻译过来是“是周年纪念日”，不同于常见的对话 id 和 type 相同，没有 id 7000000 的对话
            if (data.GetInt("type") == 7000000 && !isAnniversary)
            {
                // XXX：此处不知道对不对
                MethodInfo anniversaryTipMethod = typeof(chat_manager).GetMethod("AnniversaryTip", BindingFlags.NonPublic | BindingFlags.Instance);
                completeAction = (begintalkoverAction)Delegate.CreateDelegate(typeof(chat_manager), anniversaryTipMethod);
            }

            // 周年剧情的特殊处理，7001003 是周年剧情
            if (id == 7001003)
            {
                ___changedBGM = true;
                audio_manager.InstanceAudioManager.PlayBGM("brithday", false);
            }

            // 获取人物图片信息
            string @string = data.GetString("image", true);

            // 疑似是恋爱对话好结果的处理，代码是创建了一个好结果的物体，然后延迟删除
            if (data.GetString("loving_effect") == "1")
            {
                GameObject obj = UnityEngine.Object.Instantiate<GameObject>(__instance.LovinggoodGameObject, aGameObject.gameObject.transform.position, Quaternion.identity, aGameObject.gameObject.transform);
                UnityEngine.Object.Destroy(obj, 1f);
            }

            // 疑似是恋爱对话坏结果的处理，代码是创建了一个坏结果的物体，然后延迟删除
            if (data.GetString("loving_effect") == "2")
            {
                GameObject obj2 = UnityEngine.Object.Instantiate<GameObject>(__instance.LovingbadGameObject, aGameObject.gameObject.transform.position, Quaternion.identity, aGameObject.gameObject.transform);
                UnityEngine.Object.Destroy(obj2, 1f);
            }

            // 如果没人物图片，禁用掉人物图片物体
            if (@string == "0")
            {
                aGameObject.transform.Find("Image").gameObject.transform.GetComponent<Image>().sprite = null;
                aGameObject.transform.Find("Image").gameObject.SetActive(false);
            }

            // 开始创建人物图片
            Sprite sprite = new Sprite();
            Vector2 anchoredPosition = default(Vector2);

            // 获取玩家名称
            string text = data.GetStringLanguage("player", true);
            Main.ModEntry.Logger.Log("第一次获取名称文本 = " + text);

            if (@string.Contains("son") || @string.Contains("daughter"))
            {
                // 无从解释，怀疑是跟玩家图片有关的
                Main.ModEntry.Logger.Log("疑似和玩家图片有关的 if 调用，对话 id = " + id);
                TipsManager.instance.AddTips("疑似和玩家图片有关的 if 调用，对话 id = " + id, 1);

                for (int i = 0; i < player_data.GrowSort.Length - 1; i++)
                {
                    if (player_data.Instance.Round_current >= player_data.GrowRounds[player_data.GrowSort[i]] && player_data.Instance.Round_current < player_data.GrowRounds[player_data.GrowSort[i + 1]])
                    {
                        string text2 = (!@string.Contains("son")) ? "daughter" : "son";
                        int index = ___chatPlayerList.IndexOf(@string.Replace(text2, "player"));
                        string str = (!@string.Contains("son")) ? ___chatDaughterLists[i][index].Replace("player", text2) : ___chatPlayerLists[i][index].Replace("player", text2);
                        sprite = (Resources.Load("UI/Main_ui/chat/" + str, typeof(Sprite)) as Sprite);
                        break;
                    }
                }
                if (DEF.IsDevelopment)
                {
                    string str2 = (!@string.Contains("son")) ? @string.Insert(@string.LastIndexOf("daughter") + 8, "16") : @string.Insert(@string.LastIndexOf("son") + 3, "16");
                    sprite = (Resources.Load("UI/Main_ui/chat/" + str2, typeof(Sprite)) as Sprite);
                }
                anchoredPosition = new Vector2(663f, -540f);
            }
            else
            {
                string text3 = "UI/Main_ui/chat/" + @string;
                if (@string.Contains("mom"))
                {
                    Main.ModEntry.Logger.Log("疑似和母亲有关的 if 调用，对话 id = " + id);
                    TipsManager.instance.AddTips("疑似和母亲有关的 if 调用，对话 id = " + id, 1);

                    if (!record_manager.InstanceManagerRecord.IsFather())
                    {
                        text3 += "_girl";
                    }
                    else
                    {
                        text3 = text3 + "_" + record_manager.InstanceManagerRecord.CurrentRecord.mother_id;
                    }
                }
                else if (@string.Contains("father"))
                {
                    Main.ModEntry.Logger.Log("疑似和父亲有关的 if 调用，对话 id = " + id);
                    TipsManager.instance.AddTips("疑似和父亲有关的 if 调用，对话 id = " + id, 1);

                    if (record_manager.InstanceManagerRecord.CurrentRecord.father_name != string.Empty && player_data.Instance.Generations != 1)
                    {
                        text3 += "_boy";
                    }
                    else
                    {
                        text3 = text3 + "_" + record_manager.InstanceManagerRecord.CurrentRecord.father_id;
                    }
                }
                else if (@string.Contains("boy") && BoysManager.Instance.curInviteBoyId != 0)
                {
                    Main.ModEntry.Logger.Log("疑似和男生有关的 if 调用，对话 id = " + id);
                    TipsManager.instance.AddTips("疑似和男生有关的 if 调用，对话 id = " + id, 1);

                    text3 = text3 + "_" + BoysManager.Instance.curInviteBoyId;
                    text = BoysManager.Instance.AllBoys[BoysManager.Instance.curInviteBoyId].name;
                }

                sprite = (Resources.Load(text3, typeof(Sprite)) as Sprite);
                anchoredPosition = new Vector2(-663f, -540f);
            }

            // 这里开始应该是显示部分
            Image component = aGameObject.transform.Find("Image").GetComponent<Image>();
            RectTransform component2 = aGameObject.transform.Find("Image").GetComponent<RectTransform>();
            // 这里是左侧的人物图片的初始化
            component.sprite = sprite;
            component2.anchoredPosition = anchoredPosition;
            component2.DOScale(new Vector3(1.15f, 1.15f, 1f), 0f);
            // 人物名称文本
            aGameObject.transform.Find("name/Text").gameObject.GetComponent<Text>().text = text;
            // 清空对话框里的文本
            aGameObject.transform.Find("Text").GetComponent<Text>().text = string.Empty;
            // 对话框的那个弹一下的动画效果
            if (data.GetString("shake") == "1" && !moreText)
            {
                aGameObject.transform.Find("inner_bg").DOPunchScale(new Vector3(0.1f, 0.1f, 0f), 0.2f, 1, 0.5f);
            }
            // 第一次领零用钱但由母亲发的对话，20001 是第一次零用钱事件，这个事件在儿子版是父亲发的，女儿版在这里增加了一个母亲发的替换
            if (id == 20001 && !record_manager.InstanceManagerRecord.IsFather())
            {
                string str3 = "chat_mom_girl";
                sprite = (Resources.Load("UI/Main_ui/chat/" + str3, typeof(Sprite)) as Sprite);
                component.sprite = sprite;
                aGameObject.transform.Find("name/Text").gameObject.GetComponent<Text>().text = ReadXml.GetString("ChatMother");
            }
            component.SetNativeSize();

            // 这里应该是对话背景图
            if (data.GetString("graph") == "0")
            {
                aGameObject.transform.Find("bg").gameObject.SetActive(false);
            }
            else
            {
                aGameObject.transform.Find("bg").gameObject.transform.GetComponent<Image>().sprite = (Resources.Load("graph/" + data.GetString("graph"), typeof(Sprite)) as Sprite);
            }

            // 疑似是对话框里的文本
            string text4;
            if (newtext == null)
            {
                // 没有通过 newtext 参数传入对话内容
                if (param != string.Empty)
                {
                    // 通过 param 参数传入内容，对文本进行格式化替换
                    text4 = string.Format(data.GetStringLanguage("text", true), param);
                }
                else
                {
                    // 没有通过 param 参数传入对话内容，直接获取文本
                    text4 = data.GetStringLanguage("text", true);
                }
            }
            else
            {
                // 这里是 newtext 参数传入的对话框内容
                text4 = newtext;
            }
            string nextText = string.Empty;
            if (text4.Contains("@@"))
            {
                int num = text4.IndexOf("@@");
                nextText = text4.Substring(num + 2, text4.Length - num - 2);
                text4 = text4.Substring(0, num);
            }
            Tweener aTweener = aGameObject.transform.Find("Text").gameObject.GetComponent<Text>().DOText(text4, (float)text4.Length * 0.1f, true, ScrambleMode.None, null).SetEase(Ease.Linear);
            bool doneclik = false;
            if (!DEF.IsDevelopment)
            {
                SpaceController.SpaceAction action = delegate ()
                {
                    if (!doneclik)
                    {
                        aTweener.timeScale = (float)typeof(chat_manager).GetField("speedUp", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
                        doneclik = true;
                    }
                };
                SpaceController.Instance.SetAction(action);
            }

            aTweener.OnUpdate(delegate
            {
                if (Input.GetMouseButtonDown(0) && !doneclik)
                {
                    aTweener.timeScale = (float)typeof(chat_manager).GetField("speedUp", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
                    doneclik = true;
                }
            });

            int @int = data.GetInt("add_task", true);
            if (@int != 0)
            {
                task_manager.InstancManager.add_task(@int, false);
                MessageCenter.sendMessage("refresh_ui_data", null);
            }

            if (!parentTransform)
            {
                if (nextText == string.Empty)
                {
                    ___effectId = data.GetInt("effect");
                    ___nextId = data.GetInt("next_id", true);
                }
                else
                {
                    ___curId = id;
                }
                aTweener.OnComplete(delegate
                {
                    typeof(chat_manager).GetField("isOnComplete", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, true);
                    aGameObject.transform.Find("next").gameObject.SetActive(true);
                    typeof(chat_manager).GetField("aGameObject", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, aGameObject);
                    typeof(chat_manager).GetField("next", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, next);
                    typeof(chat_manager).GetField("next2", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, next2);
                    typeof(chat_manager).GetField("nextText", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, nextText);
                    typeof(chat_manager).GetField("isAnniversary", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, isAnniversary);
                    typeof(chat_manager).GetField("completeAction", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, completeAction);
                    aGameObject.transform.GetComponent<Button>().onClick.AddListener(delegate ()
                    {
                        typeof(chat_manager).GetMethod("OnChatComplete", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(__instance, new object[0]);
                    });
                });
            }
            else
            {
                aTweener.OnComplete(delegate
                {
                });
                aGameObject.transform.parent = parentTransform;
                aGameObject.transform.SetAsFirstSibling();
            }

            // 阻断对原方法的调用
            return false;
        }

        /// <summary>
        /// 输出对话数据的方法，用于辅助测试
        /// </summary>
        /// <param name="id"></param>
        private static void PrintChatData(int id)
        {
            Main.ModEntry.Logger.Log("输出对话数据：id = " + id);

            if(!ReadXml.HaveData("chat", id))
            {
                Main.ModEntry.Logger.Log("没有 id " + id + " 的对话数据");
                return;
            }

            XmlData data = ReadXml.GetData("chat", id);

            foreach(KeyValuePair<string,string> pair in data.value)
            {
                Main.ModEntry.Logger.Log("输出对话数据：id = " + id + ", key = " + pair.Key + ", value = " + pair.Value);
            }
        }
    }

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

        private static bool Prefix(out GetStringLanguageParams __state, string name, ref bool sex)
        {
            // 如果 Mod 未启动则不作处理
            if (!Main.enabled)
            {
                // 虽然原则上因为 Mod 未启动不处理的话后缀也不会处理，但为了防止后缀特殊需求还是传递参数
                __state = new GetStringLanguageParams(name, sex);
                return true;
            }

            // 这一代是儿子则不处理
            if (record_manager.InstanceManagerRecord.IsBoy())
            {
                Main.ModEntry.Logger.Log("这一代是儿子，不作处理");

                // 虽然原则上因为这一代是儿子不处理的话后缀也不会处理，但为了防止后缀特殊需求还是传递参数
                __state = new GetStringLanguageParams(name, sex);
                return true;
            }

            // 获取对话的人物名称，而且使用性别区分
            if (("player".Equals(name) && sex))
            {
                Main.ModEntry.Logger.Log("发现获取对话人物名称");

                // 修改为不区分性别
                sex = false;
            }

            // 向后缀传递参数，这是因为 XmlData.GetStringLanguageParams 方法内部修改了参数，必须靠这种方式把原始参数保存下来
            __state = new GetStringLanguageParams(name, sex);
            return true;
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

            // 如果获取到了文本则不处理
            if (!"0".Equals(__result))
            {
                return;
            }

            // 使用无视性别的方式再获取一次
            __result = __instance.GetStringLanguage(__state.name, false);
        }

        //private static void Postfix(XmlData __instance, ref string __result, string name, bool sex)
        //{
        //    // 如果 Mod 未启动则不作处理
        //    if (!Main.enabled)
        //    {
        //        return;
        //    }

        //    //// 这一代是儿子则不处理
        //    //if (record_manager.InstanceManagerRecord.IsBoy())
        //    //{
        //    //    Main.ModEntry.Logger.Log("这一代是儿子，不作处理");
        //    //    retun;
        //    //}

        //    //// 不是目标，不作处理
        //    //if (!("player".Equals(name) && sex))
        //    //{
        //    //    return;
        //    //}

        //    // 精确定位中二少女
        //    if (!"女孩8".Equals(__result))
        //    {
        //        return;
        //    }

        //    Main.ModEntry.Logger.Log("XmlData.GetStringLanguage(" + name + ", " + sex + ") 调用完毕，result = " + __result);

        //    //// 替换为不使用性别的读取方式
        //    //__result = __instance.GetStringLanguage("player_girl", false);
        //}
    }

    //[HarmonyPatch(typeof(girlmanager), "Start")]
    //public static class girlmanager_init
    //{
    //    private static bool Prefix(girlmanager __instance)
    //    {
    //        // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
    //        if (!Main.enabled)
    //        {
    //            return true;
    //        }

    //        Main.ModEntry.Logger.Log("girlmanager.Start 即将调用");

    //        //// 这一代是儿子则不处理
    //        //if (record_manager.InstanceManagerRecord.IsBoy())
    //        //{
    //        //    Main.ModEntry.Logger.Log("这一代是儿子，不作处理");
    //        //    return true;
    //        //}

    //        girlmanager.InstanceGirlmanager = __instance;
    //        if (record_manager.InstanceManagerRecord.CurrentRecord.GirlsDictionary.Count == 0)
    //        {
    //            Main.ModEntry.Logger.Log("girlmanager 进入 init 分支");
    //            __instance.init();
    //        }
    //        else
    //        {
    //            Main.ModEntry.Logger.Log("girlmanager 没有进入 init 分支");
    //            if (!record_manager.InstanceManagerRecord.CurrentRecord.GirlsDictionary.ContainsKey(1008))
    //            {
    //                if (DEF.isApproval && record_manager.InstanceManagerRecord.CurrentRecord.round_current > 25)
    //                {
    //                    record_manager.InstanceManagerRecord.CurrentRecord.GirlsDictionary.Add(1008, 100);
    //                }
    //                else
    //                {
    //                    record_manager.InstanceManagerRecord.CurrentRecord.GirlsDictionary.Add(1008, 0);
    //                }
    //            }
    //            __instance.GirlsDictionary = new Dictionary<int, int>();
    //            foreach (KeyValuePair<int, int> keyValuePair in record_manager.InstanceManagerRecord.CurrentRecord.GirlsDictionary)
    //            {
    //                if (DEF.isApproval && record_manager.InstanceManagerRecord.CurrentRecord.round_current > 25)
    //                {
    //                    girlmanager.InstanceGirlmanager.GirlsDictionary.Add(keyValuePair.Key, 100);
    //                }
    //                else
    //                {
    //                    girlmanager.InstanceGirlmanager.GirlsDictionary.Add(keyValuePair.Key, keyValuePair.Value);
    //                }
    //            }
    //        }

    //        return false;
    //    }
    //}

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
}
