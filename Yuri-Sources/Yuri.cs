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

        private static bool Prefix(open_system __instance)
        {
            // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
            if (!Main.enabled)
            {
                return true;
            }

            Main.ModEntry.Logger.Log("open_system.AddListener 即将调用");

            if (TipsManager.instance != null)
            {
                TipsManager.instance.AddTips("open_system.AddListener 即将调用", 1);
            }

            // 以下代码直接复制粘贴自反编译
            EventTriggerListener.Get(__instance.GirlGameObject).onClick = delegate (GameObject go)
            {
                int alertness;
                GameObject panel;

                ////////----////////----//////// Mod 修改部分 ////////----////////----////////

                // 原来的逻辑，点击后如果是儿子则打开女生面板，否则打开男生面板
                //if (record_manager.InstanceManagerRecord.IsBoy())

                // 修改后的逻辑，儿子版依然是只能打开女生面板，女儿版交替显示男生和女生面板
                if (record_manager.InstanceManagerRecord.IsBoy() || anotherSex)

                ////////----////////----//////// Mod 修改部分 ////////----////////----////////

                // 以下代码直接复制粘贴自反编译
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

                ////////----////////----//////// Mod 修改部分 ////////----////////----////////
                
                // 每次点击更新是否显示相反性别的标志量
                anotherSex = !anotherSex;

                ////////----////////----//////// Mod 修改部分 ////////----////////----////////
            };

            return false;
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
            // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
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
            }

            Main.ModEntry.Logger.Log("新档是女儿，填充儿子独有的朋友给女儿");

            // 反编译出的代码，目测是所有可能登场的朋友的列表，男女根据需要使用一部分
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
        private static bool Prefix(panel_girls __instance, List<GameObject> ___list, GameObject ___girl, XmlList ___girlList, ScrollRect ___scroll, GameObject ___infoPanel, Image[] ___bgButtons, Button ___close)
        {
            // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
            if (!Main.enabled)
            {
                return true;
            }

            Main.ModEntry.Logger.Log("panel_girls.refresh 即将调用");

            // 测试功能，遍历女同学列表
            foreach(KeyValuePair<int,int> pair in girlmanager.InstanceGirlmanager.GirlsDictionary)
            {
                Main.ModEntry.Logger.Log("遍历女同学列表：key = " + pair.Key + ", value = " + pair.Value);
            }

            // 获取编译器生成的私有内部类 <refresh>c__AnonStorey1，这个类在反编译器中一般是默认隐藏的，需要开启编译器的显示自动生成的类功能
            Type type_c__AnonStorey1 = typeof(panel_girls).GetNestedType("<refresh>c__AnonStorey1", BindingFlags.NonPublic);

            // 创建 <refresh>c__AnonStorey1 的实例
            Activator.CreateInstance(type_c__AnonStorey1, new object[] { });

            // 这里应该是清除已有的选项，就是所有的女同学
            for (int i = 0; i < ___list.Count; i++)
            {
                UnityEngine.Object.Destroy(___list[i]);
            }
            ___list.Clear();

            int num = 0;
            using (Dictionary<int, int>.Enumerator enumerator = girlmanager.InstanceGirlmanager.GirlsDictionary.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    // 发现这个对象在代码里几乎就是个数据中转站，而且都是从这个方法里中转的，把那些中转去掉后就剩下这一行创建对象了，不知道是为什么
                    object c__AnonStorey = Activator.CreateInstance(type_c__AnonStorey1, new object[] { });

                    GameObject item = UnityEngine.Object.Instantiate<GameObject>(___girl, ___bgButtons[num].transform);
                    ___list.Add(item);
                    item.name = enumerator.Current.Key.ToString();
                    GameObject gameObject = item.transform.Find("Button_Name").gameObject;
                    Text component = gameObject.transform.Find("Text_Name").GetComponent<Text>();
                    Image component2 = item.transform.Find("head").GetComponent<Image>();
                    Text component3 = item.transform.Find("loving").GetComponent<Text>();
                    SkeletonGraphic talkSpine = item.transform.Find("TalkBubble").GetComponent<SkeletonGraphic>();
                    SkeletonGraphic heartSpine = item.transform.Find("Heart").GetComponent<SkeletonGraphic>();
                    if (enumerator.Current.Value >= 30)
                    {
                        heartSpine.gameObject.SetActive(true);
                        item.transform.Find("NormalHeart").gameObject.SetActive(false);
                    }
                    XmlData xmlData = ___girlList.Get(enumerator.Current.Key);
                    component.text = xmlData.GetStringLanguage("name");
                    component2.sprite = (Resources.Load("UI/girls/" + xmlData.GetString("head"), typeof(Sprite)) as Sprite);
                    component2.SetNativeSize();
                    component3.text = enumerator.Current.Value.ToString();
                    XmlData itemData = ___girlList.Get(int.Parse(item.name));
                    ScrollRectListener gameObject2 = ScrollRectListener.GetGameObject(___bgButtons[num].gameObject);
                    gameObject2.SetScrollRect(___scroll);
                    EventTriggerListener.Get(___bgButtons[num].gameObject).onClick = delegate (GameObject go)
                    {
                        MonoBehaviour.print(" ScrollRectListener.Get(bgButtons[index].gameObject).onClick");
                        if (!__instance.is_talk(int.Parse(item.name)))
                        {
                            chat_manager.InstanceChatManager.start_chat(itemData.GetInt("hello_bad"), 0, 0, null, null, null, string.Empty, false, false);
                        }
                        else if (player_data.Instance.Potentiality >= (float)typeof(panel_girls).GetField("need_potential", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null))
                        {
                            player_data.Instance.Potentiality -= (float)typeof(panel_girls).GetField("need_potential", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);

                            __instance.chatPanel(int.Parse(item.name));
                            MessageCenter.sendMessage("refresh_ui_data", null);
                        }
                        else
                        {
                            TipsManager.instance.AddTips(ReadXml.GetString("PotentialityNotEnough"), 1);
                        }
                    };
                    EventTriggerListener.Get(___bgButtons[num].gameObject).onEnter = delegate (GameObject go)
                    {
                        talkSpine.gameObject.SetActive(true);
                        lzhspine.change_anim_ui(talkSpine, talkSpine.SkeletonDataAsset, "animation", true, null);
                        int num2 = 0;
                        if (enumerator.Current.Value >= 30)
                        {
                            num2 = 1;
                        }
                        else if (enumerator.Current.Value >= 50)
                        {
                            num2 = 2;
                        }
                        else if (enumerator.Current.Value >= 80)
                        {
                            num2 = 3;
                        }
                        if (num2 > 0)
                        {
                            heartSpine.timeScale = (float)num2;
                            lzhspine.change_anim_ui2(heartSpine, heartSpine.SkeletonDataAsset, "play", true);
                        }
                        go.transform.Find("Image_Light").GetComponent<Image>().DOFade(0.3f, 0.5f);
                    };
                    EventTriggerListener.Get(___bgButtons[num].gameObject).onExit = delegate (GameObject go)
                    {
                        talkSpine.gameObject.SetActive(false);
                        if (enumerator.Current.Value >= 30)
                        {
                            heartSpine.timeScale = 1f;
                            lzhspine.change_anim_ui2(heartSpine, heartSpine.SkeletonDataAsset, "idle", true);
                        }
                        go.transform.Find("Image_Light").GetComponent<Image>().DOFade(0f, 0.5f);
                    };
                    ScrollRectListener gameObject3 = ScrollRectListener.GetGameObject(gameObject);
                    gameObject3.SetScrollRect(___scroll);
                    EventTriggerListener.Get(gameObject).onClick = delegate (GameObject go)
                    {


                        ___close.gameObject.SetActive(false);
                        GameObject info = UnityEngine.Object.Instantiate<GameObject>(___infoPanel, __instance.transform);
                        info.GetComponent<Button>().onClick.AddListener(delegate ()
                        {


                            ___close.gameObject.SetActive(true);
                            UnityEngine.Object.DestroyObject(info);
                        });
                        info.transform.Find("head").GetComponent<Image>().sprite = (Resources.Load("UI/girls/" + itemData.GetString("image_info"), typeof(Sprite)) as Sprite);
                        info.transform.Find("name").GetComponent<Text>().text = itemData.GetStringLanguage("name");
                        info.transform.Find("desc").GetComponent<Text>().text = itemData.GetStringLanguage("desc");
                    };
                    num++;
                }
            }
            int[] array = new int[]
            {
                27,
                28,
                28,
                29,
                29
            };
            for (int j = 4; j <= 8; j++)
            {
                if (___list[j] == null)
                {
                    return false;
                }
                if (player_data.Instance.Round_current >= array[j - 4])
                {
                    ___list[j].gameObject.SetActive(true);
                    if (j == 8)
                    {
                        ___bgButtons[8].gameObject.SetActive(true);
                        ___scroll.vertical = true;
                    }
                }
                else
                {
                    ___bgButtons[j].raycastTarget = false;
                    ___list[j].gameObject.SetActive(false);
                }
            }


            return false;
        }
    }
}
