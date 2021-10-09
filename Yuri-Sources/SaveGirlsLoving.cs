using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;

namespace MtC.Mod.ChineseParents.Yuri
{
    /// <summary>
    /// 保存游戏时调用的方法。原逻辑中女儿不会保存女生好感，在这里进行补充
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

            Main.ModEntry.Logger.Log("保存游戏，补充女生好感列表");

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
