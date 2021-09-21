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
    /// <summary>
    /// 这个 Mod 的设置
    /// </summary>
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        /// <summary>
        /// 相亲页面最多替换多少个同学
        /// </summary>
        [Draw("相亲页面最多替换多少个同学 - How Many Classmates Can Be Replaced On The Blinddate Panel At Most")]
        public int maxClassmateBlinddatesNumber = 2;

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        public void OnChange()
        {
        }
    }

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
        /// 这个 Mod 的设置
        /// </summary>
        public static Settings settings;

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            // 读取设置
            settings = Settings.Load<Settings>(modEntry);

            // 保存 Mod 对象
            ModEntry = modEntry;
            ModEntry.OnToggle = OnToggle;
            ModEntry.OnGUI = OnGUI;
            ModEntry.OnSaveGUI = OnSaveGUI;

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

            // 如果是激活 Mod 则修改转校事件，否则取消修改
            if (enabled)
            {
                AddTransferEvent.ChangeTransferEvents();
            }
            else
            {
                AddTransferEvent.UnchangeTransferEvents();
            }

            // 返回 true 表示这个 Mod 切换到 Mod Manager 切换的状态，返回 false 表示 Mod 依然保持原来的状态
            return true;
        }

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Draw(modEntry);
        }

        static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            // 保存设置
            settings.Save(modEntry);

            // 取消对转校事件的修改，并用新的配置再次修改（这里是假设恋爱时间控制 Mod 发生了修改）
            AddTransferEvent.UnchangeTransferEvents();
            AddTransferEvent.ChangeTransferEvents();
        }
    }
}
