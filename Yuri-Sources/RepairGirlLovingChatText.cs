using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using MtC.Mod.ChineseParents.ChatControlLib;

namespace MtC.Mod.ChineseParents.Yuri
{
    /// <summary>
    /// 修复女儿版进行儿子版独有对话时获取不到文本的问题
    /// </summary>
    public static class RepairGirlLovingChatText
    {
        /// <summary>
        /// 修复女儿版进行儿子版独有对话时获取不到文本的问题
        /// </summary>
        public static void AddRepair()
        {
            ChatControl.BeforeModifyChat(RepairGirlTextNone);
        }

        /// <summary>
        /// 取消修复女儿版进行儿子版独有对话时获取不到文本的问题
        /// </summary>
        public static void RemoveRepair()
        {
            ChatControl.RemoveBeforeModifyChat(RepairGirlTextNone);
        }

        /// <summary>
        /// 修复女儿版读取儿子版独有的对话时读取不到对话文本的问题
        /// </summary>
        /// <param name="originChatData"></param>
        /// <param name="modifiedChatData"></param>
        /// <returns></returns>
        public static ChatControl.ChatData RepairGirlTextNone(ChatControl.ChatData originChatData, ChatControl.ChatData modifiedChatData)
        {
            ChatControl.ChatData newChatData = modifiedChatData.Copy();

            if(newChatData.text_girl == null || newChatData.text_girl.Equals("") || newChatData.text_girl.Equals("0"))
            {
                newChatData.text_girl = newChatData.text;
                newChatData.text_girl_id = newChatData.text_id;
            }

            return newChatData;
        }
    }
}
