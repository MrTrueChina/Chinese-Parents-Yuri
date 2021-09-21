using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;

namespace MtC.Mod.ChineseParents.Yuri
{
    /// <summary>
    /// 打开社交窗口的方法，确切地说是给社交按钮绑定打开社交窗口方法的方法，在这里让女儿版能够打开儿子版的社交面板
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

            Main.ModEntry.Logger.Log("给社交按钮绑定点击事件方法调用完毕");

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
}
