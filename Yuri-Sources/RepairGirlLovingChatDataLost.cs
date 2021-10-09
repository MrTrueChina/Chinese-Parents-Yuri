using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using MtC.Mod.ChineseParents.ChatControlLib;

namespace MtC.Mod.ChineseParents.Yuri
{
    /// <summary>
    /// 修复女儿版进行儿子版独有对话时部分数据丢失的问题
    /// </summary>
    public static class RepairGirlLovingChatDataLost
    {
        /// <summary>
        /// 错误的女生名称 ID 到正确的女生名称 ID 的映射
        /// </summary>
        private static readonly Dictionary<int, int> wrongGirlNameIdToCorrectGirlNameId = new Dictionary<int, int>()
        {
            // 刘偏偏
            { 1025403, 1022634 },
            { 1025410, 1022634 },
            // 席梦来
            { 1025404, 1022635 },
            { 1025411, 1022635 },
            // 秦屿路
            { 1025407, 1022631 },
            { 1025414, 1022631 },
            // 章涵之
            { 1025415, 1022632 },
            { 1025408, 1022632 },
            // 王胜男
            { 1025405, 1022636 },
            { 1025412, 1022636 },
            // 李若放
            { 1025406, 1022637 },
            { 1025413, 1022637 },
            // 汤金娜
            { 1025409, 1022640 },
            { 1025416, 1022640 },
            // 苏芳允
            { 1029676, 1029675 },
            // 牧唯
            { 1030730, 1030822 },
        };

        /// <summary>
        /// 修复女儿版进行儿子版独有对话时部分数据丢失的问题
        /// </summary>
        public static void AddRepair()
        {
            ChatControl.BeforeModifyChat(RepairGirlChatDataLost);
        }

        /// <summary>
        /// 取消修复女儿版进行儿子版独有对话时部分数据丢失的问题
        /// </summary>
        public static void RemoveRepair()
        {
            ChatControl.RemoveBeforeModifyChat(RepairGirlChatDataLost);
        }

        /// <summary>
        /// 修复女儿版读取儿子版独有的对话时部分数据丢失的问题
        /// </summary>
        /// <param name="originChatData"></param>
        /// <param name="modifiedChatData"></param>
        /// <returns></returns>
        public static ChatData RepairGirlChatDataLost(ChatData originChatData, ChatData modifiedChatData)
        {
            // 当前周目是儿子则不处理
            if (record_manager.InstanceManagerRecord.CurrentRecord.playerSex == 1)
            {
                return modifiedChatData;
            }

            ChatData newChatData = modifiedChatData.Copy();

            // 对于没有女儿版文本的，用儿子版文本补充
            if (newChatData.text_girl == null || newChatData.text_girl.Equals("") || newChatData.text_girl.Equals("0"))
            {
                newChatData.text_girl = newChatData.text;
                newChatData.text_girl_id = newChatData.text_id;
            }

            // 对于没有女儿版说话人图片的，用儿子版图片补充，儿子图片则改为女儿图片
            if (newChatData.image_girl == null || newChatData.image_girl.Equals("") || newChatData.image_girl.Equals("0"))
            {
                newChatData.image_girl = newChatData.image.Replace("_son", "_daughter");
            }

            // 对于没有女儿版说话人名称的，用儿子版名称补充
            if (newChatData.player_girl == null || newChatData.player_girl.Equals("") || newChatData.player_girl.Equals("0"))
            {
                newChatData.player_girl = newChatData.player;
            }

            // 对于没有女儿版说话人名称 ID 的，用儿子版名称 ID 补充，这里有一个空 ID，也就是无文本 ID
            if (newChatData.player_girl_id == 0 || newChatData.player_girl_id == ChatData.NO_TEXT_ID)
            {
                newChatData.player_girl_id = newChatData.player_id;
            }

            // 如果女儿版说话人物名称 ID 是错误的 ID，转为正确的 ID
            if (wrongGirlNameIdToCorrectGirlNameId.ContainsKey(newChatData.player_girl_id))
            {
                newChatData.player_girl_id = wrongGirlNameIdToCorrectGirlNameId[newChatData.player_girl_id];
            }

            return newChatData;
        }
    }
}
