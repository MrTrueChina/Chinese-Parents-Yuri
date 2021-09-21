using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;

namespace MtC.Mod.ChineseParents.Yuri
{
    /// <summary>
    /// 女生面板增加恋爱警告值的方法，这个方法会随机增加一定量的恋爱警告值，恋爱警告会根据警告值出现。男女同学警告值不通用，在这里进行同步
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
    /// 男生面板增加恋爱警告值的方法，这个方法和女生面板一样随机增加警告值。另外一提，这个方法在原代码中还负责继续向后调用。男女同学警告值不通用，在这里进行同步
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
}
