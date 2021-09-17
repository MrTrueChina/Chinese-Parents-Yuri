using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityModManagerNet;
using System.Reflection;
using UnityEngine;
using MtC.Mod.ChineseParents.ForceGirl;

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
        private static bool Prefix()
        {
            // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
            if (!Main.enabled)
            {
                return true;
            }

            Main.ModEntry.Logger.Log("panel_girls.refresh 即将调用");

            foreach(KeyValuePair<int,int> pair in girlmanager.InstanceGirlmanager.GirlsDictionary)
            {
                Main.ModEntry.Logger.Log("遍历女同学列表：key = " + pair.Key + ", value = " + pair.Value);
            }





            return false;
        }
    }
}
