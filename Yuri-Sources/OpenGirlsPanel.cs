using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;
using MtC.Mod.ChineseParents.LovingTimeController;
using UnityEngine.UI;

namespace MtC.Mod.ChineseParents.Yuri
{
    /// <summary>
    /// 读取玩家数据方法，可能是读档的时候用到的方法，这里有开启按钮功能。在这里控制显隐社交按钮
    /// </summary>
    [HarmonyPatch(typeof(player_data), "read_data")]
    public static class player_data_read_data
    {
        private static void Postfix()
        {
            // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
            if (!Main.enabled)
            {
                return;
            }

            Main.ModEntry.Logger.Log("读取玩家数据方法调用完毕");

            // 如果当前这代是儿子，不进行处理
            if (record_manager.InstanceManagerRecord.IsBoy())
            {
                Main.ModEntry.Logger.Log("这一代是儿子，不进行处理");
                return;
            }

            // 没有获取到女儿版的社交面板则不处理
            if (CreateGirldPanelButton.GirlsPanelButton == null) {
                return;
            }

            // 如果到了约会出现回合数，则显示女生面板按钮，否则禁用女生面板按钮
            if (player_data.Instance.Round_current >= LovingTimeController.Main.settings.lovingOpenRounds)
            {
                CreateGirldPanelButton.GirlsPanelButton.SetActive(true);
            }
            else
            {
                CreateGirldPanelButton.GirlsPanelButton.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 每周开始时开启按钮的方法。在这里控制显隐社交按钮
    /// </summary>
    [HarmonyPatch(typeof(week_player), "system_open")]
    public static class week_player_system_open
    {

        private static void Postfix()
        {
            // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
            if (!Main.enabled)
            {
                return;
            }

            Main.ModEntry.Logger.Log("每周开始时开启按钮的方法调用完毕");

            // 如果当前这代是儿子，不进行处理
            if (record_manager.InstanceManagerRecord.IsBoy())
            {
                Main.ModEntry.Logger.Log("这一代是儿子，不进行处理");
                return;
            }

            // 没有获取到女儿版的社交面板则不处理
            if (CreateGirldPanelButton.GirlsPanelButton == null)
            {
                return;
            }

            // 如果到了约会出现回合数，则显示女生面板按钮，否则禁用女生面板按钮
            if (player_data.Instance.Round_current >= LovingTimeController.Main.settings.lovingOpenRounds)
            {
                CreateGirldPanelButton.GirlsPanelButton.SetActive(true);
            }
            else
            {
                CreateGirldPanelButton.GirlsPanelButton.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 这是进行 UI 文本国际化的组件的方法，这个方法会进行 UI 文本的国际化，也就是把 UI 文本替换成符合游戏选择语言的内容
    /// </summary>
    [HarmonyPatch(typeof(UILabel), "Start")]
    public static class UILabel_Start
    {
        private static void Prefix(UILabel __instance, out string __state)
        {
            // 将 UI 原本的文本传给后缀
            __state = __instance.gameObject.GetComponent<Text>().text;
        }

        private static void Postfix(UILabel __instance, string __state)
        {
            // 如果 Mod 未启动则直接按照游戏原本的逻辑进行调用
            if (!Main.enabled)
            {
                return;
            }

            // UI 本来的文本不是要修改的两个社交按钮的文本，说明这个方法的调用对象不是目标，不作处理
            if(!CreateGirldPanelButton.GIRLS_PANEL_BUTTON_NAME.Equals(__state) && !CreateGirldPanelButton.BOYS_PANEL_BUTTON_NAME.Equals(__state))
            {
                return;
            }

            // 把 UI 文本改回去
            __instance.gameObject.GetComponent<Text>().text = __state;
        }
    }

    /// <summary>
    /// 给女儿版创建打开女生社交面板的按钮的类
    /// </summary>
    public static class CreateGirldPanelButton
    {
        /// <summary>
        /// 打开女生面板的社交按钮显示的文本
        /// </summary>
        public const string GIRLS_PANEL_BUTTON_NAME = "约会";
        /// <summary>
        /// 打开男生面板的社交按钮显示的文本
        /// </summary>
        public const string BOYS_PANEL_BUTTON_NAME = "邀约";

        /// <summary>
        /// 女儿版的打开女生面板按钮
        /// </summary>
        internal static GameObject GirlsPanelButton
        {
            get
            {
                if (girlsPanelButton != null)
                {
                    return girlsPanelButton;
                }

                CreateButton();

                return girlsPanelButton;
            }
        }
        private static GameObject girlsPanelButton;

        /// <summary>
        /// 给女儿版创建打开女生社交面板的按钮
        /// </summary>
        private static void CreateButton()
        {
            Main.ModEntry.Logger.Log("生成新的女生面板按钮");

            // 获取原来的那个社交按钮
            GameObject originLovingButton = GameObject.Find("button_girl");

            // 防止场景中没有社交按钮，比如在开始菜单之类的情况
            if (originLovingButton == null)
            {
                Main.ModEntry.Logger.Log("场景中没有社交按钮，放弃生成");
                return;
            }

            Main.ModEntry.Logger.Log("开始实例化新的社交按钮作为女生面板按钮");

            // 生成新的社交按钮
            girlsPanelButton = GameObject.Instantiate(originLovingButton, originLovingButton.transform.parent);

            // 重命名，和原版的社交按钮区分开
            girlsPanelButton.name = Main.ModEntry.Info.Id + "Yuri Girl Panel";

            // 移动男同学社交按钮到原位置的左侧
            originLovingButton.transform.position = originLovingButton.transform.position + Vector3.left * 1.5f;

            // 修改按钮文本，注意这里修改后还是会被 UILabel 的国际化给覆盖掉，需要配合对 UILabel 的修改才能真正设置文本
            originLovingButton.transform.Find("Text").GetComponent<Text>().text = BOYS_PANEL_BUTTON_NAME;
            GirlsPanelButton.transform.Find("Text").GetComponent<Text>().text = GIRLS_PANEL_BUTTON_NAME;

            // 给社交按钮的点击事件绑定打开面板的方法
            EventTriggerListener.Get(GirlsPanelButton).onClick = delegate (GameObject go)
            {
                // 打开女生面板
                GameObject panel = (Resources.Load("UI/girls/Panel_girls") as GameObject);

                // 绑定发出警告事件
                int alertness = girlmanager.InstanceGirlmanager.alertness;
                open_system.InstanceOpenSystem.alert_talk(alertness, panel);
            };
        }
    }
}
