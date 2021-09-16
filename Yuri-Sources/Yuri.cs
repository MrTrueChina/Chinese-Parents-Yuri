using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityModManagerNet;
using System.Reflection;
using UnityEngine;

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

    ///// <summary>
    ///// 功能不确定，怀疑和按钮有关
    ///// </summary>
    //[HarmonyPatch(typeof(open_system), "OnEnable")]
    //public static class open_system_OnEnable
    //{
    //    private static bool Prefix(open_system __instance)
    //    {
    //        // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
    //        if (!Main.enabled)
    //        {
    //            return true;
    //        }

    //        Main.ModEntry.Logger.Log("open_system.OnEnable 即将调用");

    //        open_system.InstanceOpenSystem = __instance;
    //        __instance.FacerewardGameObject = __instance.transform.Find("face_reward").gameObject;
    //        __instance.skillbutton = __instance.transform.Find("panel_mainplay").transform.Find("Toggle_learn").gameObject;
    //        __instance.taskbutton = __instance.transform.Find("panel_mainplay").transform.Find("Toggle_task").gameObject;
    //        __instance.ParentdreamGameObject = __instance.transform.Find("panel_parentdream").gameObject;
    //        __instance.PressureGameObject = __instance.transform.Find("panel_pressure").gameObject;
    //        __instance.MoneyGameObject = __instance.transform.Find("panel_money").gameObject;
    //        __instance.ShopGameObject = __instance.transform.Find("button_shop").gameObject;
    //        __instance.GirlGameObject = __instance.transform.Find("button_girl").gameObject;
    //        __instance.closeall();
    //        typeof(open_system).GetMethod("AddListener", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(__instance, new object[0]);

    //        //// 测试一下如果调用两次给社交按钮绑定回调的功能会不会出现两个社交按钮。测试结果：并不会出现两个按钮
    //        //typeof(open_system).GetMethod("AddListener", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(__instance, new object[0]);

    //        //// 尝试复制一个社交按钮，复制出来了，但是覆盖了回去，疑似这个按钮上有什么动画效果
    //        //GameObject newGirlButton = GameObject.Instantiate(__instance.GirlGameObject, __instance.GirlGameObject.transform.parent);
    //        //newGirlButton.transform.localScale *= 3;
    //        //newGirlButton.SetActive(true);
    //        //Main.ModEntry.Logger.Log("button_girl.position = " + __instance.GirlGameObject.transform.position);
    //        //Main.ModEntry.Logger.Log("newGirlButton.position = " + newGirlButton.transform.position);

    //        __instance.GirlGameObject.gameObject.GetComponents<MonoBehaviour>().ToList<MonoBehaviour>().ForEach(component =>
    //        {
    //            Main.ModEntry.Logger.Log("社交按钮的组件：" + component.GetType().FullName);
    //        });

    //        //// 测试一下如果销毁按钮会怎么样。测试结果：成功销毁按钮
    //        //GameObject.Destroy(__instance.GirlGameObject);

    //        return false;
    //    }

    //    private static void Postfix()
    //    {
    //        // 如果 Mod 未启动则直接返回
    //        if (!Main.enabled)
    //        {
    //            return;
    //        }

    //        Main.ModEntry.Logger.Log("open_system.OnEnable 调用完毕");

    //        if(TipsManager.instance != null)
    //        {
    //            TipsManager.instance.AddTips("open_system.OnEnable 调用完毕", 1);
    //        }
    //        else
    //        {
    //            Main.ModEntry.Logger.Log("TipsManager.instance = null");
    //        }
    //    }
    //}

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

                // 修改后的逻辑，单数次点击是按照玩家性别显示，双数次点击是相反的面板
                if (record_manager.InstanceManagerRecord.IsBoy() ^ anotherSex)

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

    ///// <summary>
    ///// 与男生社交后发出恋爱次数警告的方法
    ///// </summary>
    //[HarmonyPatch(typeof(open_system), "AlertTalkInPanel")]
    //public static class open_system_AlertTalkInPanel
    //{
    //    private static bool Prefix(open_system __instance, int alertness, begintalkoverAction completeAction)
    //    {
    //        // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
    //        if (!Main.enabled)
    //        {
    //            return true;
    //        }

    //        Main.ModEntry.Logger.Log("open_system.AlertTalkInPanel(" + alertness + ", " + completeAction.Method.Name + ") 即将调用");

    //        if (TipsManager.instance != null)
    //        {
    //            TipsManager.instance.AddTips("open_system.AlertTalkInPanel(" + alertness + ", " + completeAction.Method.Name + ") 即将调用", 1);
    //        }

    //        return true;
    //    }
    //}

    ///// <summary>
    ///// 打开男生面板时发出恋爱次数警告的方法
    ///// </summary>
    //[HarmonyPatch(typeof(open_system), "alert_talk")]
    //public static class open_system_alert_talk
    //{
    //    private static bool Prefix(open_system __instance, int alertness, GameObject panel)
    //    {
    //        // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
    //        if (!Main.enabled)
    //        {
    //            return true;
    //        }

    //        Main.ModEntry.Logger.Log("open_system.alert_talk(" + alertness + ", " + panel.name + ") 即将调用");

    //        if (TipsManager.instance != null)
    //        {
    //            TipsManager.instance.AddTips("open_system.alert_talk(" + alertness + ", " + panel.name + ") 即将调用", 1);
    //        }

    //        return true;
    //    }
    //}

    //[HarmonyPatch(typeof(open_system), "closeall")]
    //public static class open_system_closeall
    //{
    //    private static bool Prefix(open_system __instance)
    //    {
    //        // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
    //        if (!Main.enabled)
    //        {
    //            return true;
    //        }

    //        Main.ModEntry.Logger.Log("open_system.closeall 即将调用");

    //        if (TipsManager.instance != null)
    //        {
    //            TipsManager.instance.AddTips("open_system.closeall 即将调用", 1);
    //        }

    //        return true;
    //    }
    //}

    //[HarmonyPatch(typeof(open_system), "opensystem")]
    //public static class open_system_opensystem
    //{
    //    private static bool Prefix(open_system __instance, GameObject buttonGameObject)
    //    {
    //        // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
    //        if (!Main.enabled)
    //        {
    //            return true;
    //        }

    //        Main.ModEntry.Logger.Log("open_system.opensystem(" + buttonGameObject.name + ") 即将调用");

    //        if (TipsManager.instance != null)
    //        {
    //            TipsManager.instance.AddTips("open_system.opensystem(" + buttonGameObject.name + ") 即将调用", 1);
    //        }

    //        return true;
    //    }
    //}

    //[HarmonyPatch(typeof(XmlData), "GetString", new Type[] { typeof(string),typeof(bool) })]
    //public static class XmlData_GetString_string_bool
    //{
    //    private static bool Prefix(ref string __result, string name, bool sex)
    //    {
    //        // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
    //        if (!Main.enabled)
    //        {
    //            return true;
    //        }

    //        Main.ModEntry.Logger.Log("XmlData.GetString(" + name + ", " + sex + ") 即将调用");

    //        if (TipsManager.instance != null)
    //        {
    //            TipsManager.instance.AddTips("XmlData.GetString(" + name + ", " + sex + ") 即将调用", 1);
    //        }

    //        return true;
    //    }
    //}

    //[HarmonyPatch(typeof(XmlData), "GetString", new Type[] { typeof(string) })]
    //public static class XmlData_GetString_string
    //{
    //    private static bool Prefix(ref string __result, string name)
    //    {
    //        // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
    //        if (!Main.enabled)
    //        {
    //            return true;
    //        }

    //        if ()

    //            Main.ModEntry.Logger.Log("XmlData.GetString(" + name + ") 即将调用");

    //        if (TipsManager.instance != null)
    //        {
    //            TipsManager.instance.AddTips("XmlData.GetString(" + name + ") 即将调用", 1);
    //        }

    //        return true;
    //    }
    //}

    //[HarmonyPatch(typeof(ReadXml), "GetData")]
    //public static class XmlData_GetString_string
    //{
    //    private static bool Prefix(ref XmlData __result, string fileName, int id)
    //    {
    //        // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
    //        if (!Main.enabled)
    //        {
    //            return true;
    //        }

    //        if (!("chat".Equals(fileName)) || id != 8100101)
    //        {
    //            return true;
    //        }

    //        Main.ModEntry.Logger.Log("ReadXml.GetString(" + fileName + ", " + id + ") 即将调用");

    //        if (TipsManager.instance != null)
    //        {
    //            TipsManager.instance.AddTips("ReadXml.GetString(" + fileName + ", " + id + ") 即将调用", 1);
    //        }

    //        return true;
    //    }

    //    private static void Postfix(ref XmlData __result, string fileName, int id)
    //    {
    //        // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
    //        if (!Main.enabled)
    //        {
    //            return;
    //        }

    //        if (!("chat".Equals(fileName)) || id != 8100101)
    //        {
    //            return;
    //        }

    //        Main.ModEntry.Logger.Log("ReadXml.GetString(" + fileName + ", " + id + ") 调用完成，result = " + __result);
    //        //Main.ModEntry.Logger.Log("__result.GetStringLanguage(\"name\"); = " + __result.GetStringLanguage("name"));

    //        foreach(KeyValuePair<string,string> pair in __result.value)
    //        {
    //            Main.ModEntry.Logger.Log("遍历 __result.value.value：key = " + pair.Key + ", value = " + pair.Value);
    //        }

    //        if (TipsManager.instance != null)
    //        {
    //            TipsManager.instance.AddTips("ReadXml.GetString(" + fileName + ", " + id + ") 调用完成，result = " + __result, 1);

    //            //TipsManager.instance.AddTips("__result.GetStringLanguage(\"name\"); = " + __result.GetStringLanguage("name"), 1);
    //        }

    //        //Main.ModEntry.Logger.Log("data.GetInt(\"type\") = " + data.GetInt("type"));
    //        //Main.ModEntry.Logger.Log("data.GetString(\"loving_effect\") = " + data.GetString("loving_effect"));
    //        //Main.ModEntry.Logger.Log("data.GetStringLanguage(\"player\", true) = " + data.GetStringLanguage("player", true));
    //        //Main.ModEntry.Logger.Log("data.GetStringLanguage(\"text\", true) = " + data.GetStringLanguage("text", true));
    //        //Main.ModEntry.Logger.Log("data.GetInt(\"add_task\", true) = " + data.GetInt("add_task", true));
    //        //Main.ModEntry.Logger.Log("data.GetInt(\"effect\") = " + data.GetInt("effect"));
    //        //Main.ModEntry.Logger.Log("data.GetInt(\"next_id\", true) = " + data.GetInt("next_id", true));

    //        if (__result.value.ContainsKey("text_girl"))
    //        {
    //            __result.value["text_girl"] = "text_girl";
    //        }

    //        //if (__result.value.ContainsKey("add_task_girl"))
    //        //{
    //        //    __result.value["add_task_girl"] = "add_task_girl";
    //        //}
    //    }
    //}

    //[HarmonyPatch(typeof(chat_manager), "start_chat")]
    //public static class chat_manager_start_chat
    //{
    //    private static bool Prefix(int id)
    //    {
    //        // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
    //        if (!Main.enabled)
    //        {
    //            return true;
    //        }

    //        if (id != 8100101) {
    //            return true;
    //        }

    //        Main.ModEntry.Logger.Log("chat_manager.start_chat(" + id + ") 即将调用");



    //        XmlData data = ReadXml.GetData("chat", id);

    //        Main.ModEntry.Logger.Log("data.GetInt(\"type\") = " + data.GetInt("type")); 
    //        Main.ModEntry.Logger.Log("data.GetString(\"loving_effect\") = " + data.GetString("loving_effect"));
    //        Main.ModEntry.Logger.Log("data.GetStringLanguage(\"player\", true) = " + data.GetStringLanguage("player", true));
    //        Main.ModEntry.Logger.Log("data.GetStringLanguage(\"text\", true) = " + data.GetStringLanguage("text", true));
    //        Main.ModEntry.Logger.Log("data.GetInt(\"add_task\", true) = " + data.GetInt("add_task", true));
    //        Main.ModEntry.Logger.Log("data.GetInt(\"effect\") = " + data.GetInt("effect"));
    //        Main.ModEntry.Logger.Log("data.GetInt(\"next_id\", true) = " + data.GetInt("next_id", true));


    //        if (TipsManager.instance != null)
    //        {
    //            TipsManager.instance.AddTips("chat_manager.start_chat(" + id + ") 即将调用", 1);
    //        }

    //        return true;
    //    }
    //}

    //[HarmonyPatch(typeof(XmlData), "GetStringLanguage",new Type[] { typeof(string)})]
    //public static class XmlData_GetStringLanguage_string
    //{
    //    private static bool Prefix(ref string __result, string name)
    //    {
    //        // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
    //        if (!Main.enabled)
    //        {
    //            return true;
    //        }

    //        if (!("name".Equals(name)))
    //        {
    //            return true;
    //        }

    //        Main.ModEntry.Logger.Log("XmlData.GetString(" + name + ") 即将调用");

    //        if (TipsManager.instance != null)
    //        {
    //            TipsManager.instance.AddTips("XmlData.GetString(" + name + ") 即将调用", 1);
    //        }

    //        return true;
    //    }
    //    private static void Postfix(ref string __result, string name)
    //    {
    //        // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
    //        if (!Main.enabled)
    //        {
    //            return;
    //        }

    //        if (!("name".Equals(name)))
    //        {
    //            return;
    //        }

    //        Main.ModEntry.Logger.Log("XmlData.GetString(" + name + ") 调用完成，result = " + __result);

    //        if (TipsManager.instance != null)
    //        {
    //            TipsManager.instance.AddTips("XmlData.GetString(" + name + ") 调用完成，result = " + __result, 1);
    //        }
    //    }
    //}
}
