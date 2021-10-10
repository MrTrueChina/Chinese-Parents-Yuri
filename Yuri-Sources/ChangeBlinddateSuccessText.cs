using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;

namespace MtC.Mod.ChineseParents.Yuri
{
    /// <summary>
    /// 相亲成功后执行的方法，在这里记录下相亲成功的对象
    /// </summary>
    [HarmonyPatch(typeof(Panel_blinddate), "win_process")]
    public static class Panel_blinddate_win_process_RecordBlinddate
    {
        /// <summary>
        /// 选择的相亲对象的 ID
        /// </summary>
        internal static int selectedBlinddateId;

        private static void Prefix(blinddate myBlinddate)
        {
            // 保存相亲对象
            selectedBlinddateId = myBlinddate.id;
        }
    }

    /// <summary>
    /// 读取字符串的方法，在这里修改获取到的求婚成功文本
    /// </summary>
    [HarmonyPatch(typeof(ReadXml), "GetString")]
    public static class ReadXml_GetString
    {
        private static void Prefix(out string __state, string name)
        {
            // 将参数传给后缀
            __state = name;
        }

        private static void Postfix(string __state, ref string __result)
        {
            // 如果 Mod 未启动则不处理
            if (!Main.enabled)
            {
                return;
            }

            // 当前周目是儿子则不处理
            if (record_manager.InstanceManagerRecord.CurrentRecord.playerSex == 1)
            {
                return;
            }

            // 不是相亲成功第一句，不处理
            if (!("Blinddate_Success1".Equals(__state)))
            {
                return;
            }

            // 获取所有女同学的 id
            List<int> girlIdList = new List<int>();
            // 添加闺蜜的 id
            girlIdList.Add(3008);
            // 添加儿子版女同学的 id
            if(girlmanager.InstanceGirlmanager == null && girlmanager.InstanceGirlmanager.GirlsDictionary == null)
            {
                foreach(KeyValuePair<int,int> girlLoving in girlmanager.InstanceGirlmanager.GirlsDictionary)
                {
                    girlIdList.Add(girlLoving.Key);
                }
            }

            // 如果选择的相亲对象是女同学，修改相亲文本
            if (girlIdList.Contains(Panel_blinddate_win_process_RecordBlinddate.selectedBlinddateId))
            {
                // 女儿版相亲成功第一句原文：终于，在某个春风沉醉的夜晚，我接受了对方的求婚。
                __result = "终于，在某个春风沉醉的夜晚，我求婚成功了。";
            }
        }
    }
}
