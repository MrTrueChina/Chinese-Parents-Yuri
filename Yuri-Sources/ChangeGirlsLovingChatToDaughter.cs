using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using MtC.Mod.ChineseParents.ChatControlLib;

namespace MtC.Mod.ChineseParents.Yuri
{
    /// <summary>
    /// 修改女同学社交对话中的针对于儿子性别的对话内容
    /// </summary>
    class ChangeGirlsLovingChatToDaughter
    {
        /// <summary>
        /// 修改女同学社交对话中的针对于儿子性别的对话内容
        /// </summary>
        public static void AddChange()
        {
            // 刘偏偏
            ChatControl.ModifyChat(810010303, ChangeLiuPianPianLoving30);
            ChatControl.ModifyChat(810010804, ChangeLiuPianPianLoving80);
            
            // 章涵之
            ChatControl.ModifyChat(810040710, ChangeZhangHanZhiLoving70);
            ChatControl.ModifyChat(810040806, ChangeZhangHanZhiLoving80);
            ChatControl.ModifyChat(810040904, ChangeZhangHanZhiLoving90_1);
            ChatControl.ModifyChat(810040905, ChangeZhangHanZhiLoving90_2);

            // 汤金娜
            ChatControl.ModifyChat(810070801, ChangeTangJinNaLoving80);

            // 苏芳允
            ChatControl.ModifyChat(810080310, ChangeSuFangYunLoving30);
            ChatControl.ModifyChat(810080502, ChangeSuFangYunLoving50_1);
            ChatControl.ModifyChat(810080504, ChangeSuFangYunLoving50_2);
            ChatControl.ModifyChat(8300801, ChangeSuFangYunGood1);
            ChatControl.ModifyChat(8300802, ChangeSuFangYunGood2);
            ChatControl.ModifyChat(8300803, ChangeSuFangYunGood3);
            ChatControl.ModifyChat(8500803, ChangeSuFangYunBad);

            // 牧唯
            ChatControl.ModifyChat(810090218, ChangeMuWeiLoving20_1);
            ChatControl.ModifyChat(810090222, ChangeMuWeiLoving20_2);
            ChatControl.ModifyChat(810090301, ChangeMuWeiLoving30_1);
            ChatControl.ModifyChat(810090309, ChangeMuWeiLoving30_2);
            ChatControl.ModifyChat(810090803, ChangeMuWeiLoving80_1);
            ChatControl.ModifyChat(810090804, ChangeMuWeiLoving80_2);
            ChatControl.ModifyChat(810090805, ChangeMuWeiLoving80_3);
            ChatControl.ModifyChat(810090806, ChangeMuWeiLoving80_4);
            ChatControl.ModifyChat(810090908, ChangeMuWeiLoving90);
            ChatControl.ModifyChat(8500903, ChangeMuWeiBad);
        }

        /// <summary>
        /// 取消修改女同学社交对话中的针对于儿子性别的对话内容
        /// </summary>
        public static void RemoveChange()
        {
            // 刘偏偏
            ChatControl.RemoveModifyChat(810010303, ChangeLiuPianPianLoving30);
            ChatControl.RemoveModifyChat(810010804, ChangeLiuPianPianLoving80);

            // 章涵之
            ChatControl.RemoveModifyChat(810040710, ChangeZhangHanZhiLoving70);
            ChatControl.RemoveModifyChat(810040806, ChangeZhangHanZhiLoving80);
            ChatControl.RemoveModifyChat(810040904, ChangeZhangHanZhiLoving90_1);
            ChatControl.RemoveModifyChat(810040905, ChangeZhangHanZhiLoving90_2);

            // 汤金娜
            ChatControl.RemoveModifyChat(810070801, ChangeTangJinNaLoving80);

            // 苏芳允
            ChatControl.RemoveModifyChat(810080310, ChangeSuFangYunLoving30);
            ChatControl.RemoveModifyChat(810080502, ChangeSuFangYunLoving50_1);
            ChatControl.RemoveModifyChat(810080504, ChangeSuFangYunLoving50_2);
            ChatControl.RemoveModifyChat(8300801, ChangeSuFangYunGood1);
            ChatControl.RemoveModifyChat(8300802, ChangeSuFangYunGood2);
            ChatControl.RemoveModifyChat(8300803, ChangeSuFangYunGood3);
            ChatControl.RemoveModifyChat(8500803, ChangeSuFangYunBad);

            // 牧唯
            ChatControl.RemoveModifyChat(810090218, ChangeMuWeiLoving20_1);
            ChatControl.RemoveModifyChat(810090222, ChangeMuWeiLoving20_2);
            ChatControl.RemoveModifyChat(810090301, ChangeMuWeiLoving30_1);
            ChatControl.RemoveModifyChat(810090309, ChangeMuWeiLoving30_2);
            ChatControl.RemoveModifyChat(810090803, ChangeMuWeiLoving80_1);
            ChatControl.RemoveModifyChat(810090804, ChangeMuWeiLoving80_2);
            ChatControl.RemoveModifyChat(810090805, ChangeMuWeiLoving80_3);
            ChatControl.RemoveModifyChat(810090806, ChangeMuWeiLoving80_4);
            ChatControl.RemoveModifyChat(810090908, ChangeMuWeiLoving90);
            ChatControl.RemoveModifyChat(8500903, ChangeMuWeiBad);
        }

        /// <summary>
        /// 修改刘偏偏 30 好感度对话
        /// </summary>
        /// <param name="originChatData"></param>
        /// <param name="modifiedChatData"></param>
        /// <returns></returns>
        public static ChatData ChangeLiuPianPianLoving30(ChatData originChatData, ChatData modifiedChatData)
        {
            // 当前周目是儿子则不处理
            if (record_manager.InstanceManagerRecord.CurrentRecord.playerSex == 1)
            {
                return modifiedChatData;
            }

            ChatData newChatData = modifiedChatData.Copy();

            // 原文是：谁给你打电话！怎么是个男生！
            newChatData.text = "谁给你打电话！";
            newChatData.text_girl = "谁给你打电话！";

            return newChatData;
        }

        /// <summary>
        /// 修改刘偏偏 80 好感度对话
        /// </summary>
        /// <param name="originChatData"></param>
        /// <param name="modifiedChatData"></param>
        /// <returns></returns>
        public static ChatData ChangeLiuPianPianLoving80(ChatData originChatData, ChatData modifiedChatData)
        {
            // 当前周目是儿子则不处理
            if (record_manager.InstanceManagerRecord.CurrentRecord.playerSex == 1)
            {
                return modifiedChatData;
            }

            ChatData newChatData = modifiedChatData.Copy();

            // 原文是：不要总是粘着我的样子，做点自己的事情吧，你看隔壁班学习好的男生，都有自己的理想
            newChatData.text = "不要总是粘着我的样子，做点自己的事情吧，你看隔壁班学习好的学生，都有自己的理想";
            newChatData.text_girl = "不要总是粘着我的样子，做点自己的事情吧，你看隔壁班学习好的学生，都有自己的理想";

            return newChatData;
        }

        /// <summary>
        /// 修改章涵之 70 好感度对话
        /// </summary>
        /// <param name="originChatData"></param>
        /// <param name="modifiedChatData"></param>
        /// <returns></returns>
        public static ChatData ChangeZhangHanZhiLoving70(ChatData originChatData, ChatData modifiedChatData)
        {
            // 当前周目是儿子则不处理
            if (record_manager.InstanceManagerRecord.CurrentRecord.playerSex == 1)
            {
                return modifiedChatData;
            }

            ChatData newChatData = modifiedChatData.Copy();

            // 原文是：额……这是在吐槽我吗？我这么玉树临风！这么一表人才……
            newChatData.text = "额……这是在吐槽我吗？我这么天生丽质！这么倾国倾城……";
            newChatData.text_girl = "额……这是在吐槽我吗？我这么天生丽质！这么倾国倾城……";

            return newChatData;
        }

        /// <summary>
        /// 修改章涵之 80 好感度对话
        /// </summary>
        /// <param name="originChatData"></param>
        /// <param name="modifiedChatData"></param>
        /// <returns></returns>
        public static ChatData ChangeZhangHanZhiLoving80(ChatData originChatData, ChatData modifiedChatData)
        {
            // 当前周目是儿子则不处理
            if (record_manager.InstanceManagerRecord.CurrentRecord.playerSex == 1)
            {
                return modifiedChatData;
            }

            ChatData newChatData = modifiedChatData.Copy();

            // 原文是：就是自信吧，特殊的帅哥才有的，骄傲的气场
            newChatData.text = "就是自信吧，特殊的帅哥美女才有的，骄傲的气场";
            newChatData.text_girl = "就是自信吧，特殊的帅哥美女才有的，骄傲的气场";

            return newChatData;
        }

        /// <summary>
        /// 修改章涵之 90 好感度对话，第一部分
        /// </summary>
        /// <param name="originChatData"></param>
        /// <param name="modifiedChatData"></param>
        /// <returns></returns>
        public static ChatData ChangeZhangHanZhiLoving90_1(ChatData originChatData, ChatData modifiedChatData)
        {
            // 当前周目是儿子则不处理
            if (record_manager.InstanceManagerRecord.CurrentRecord.playerSex == 1)
            {
                return modifiedChatData;
            }

            ChatData newChatData = modifiedChatData.Copy();

            // 原文是：是啊，所以你在说我不是渣男是吗？
            newChatData.text = "是啊，所以你在绕着圈说我不是渣女是吗？";
            newChatData.text_girl = "是啊，所以你在绕着圈说我不是渣女是吗？";

            return newChatData;
        }

        /// <summary>
        /// 修改章涵之 90 好感度对话，第二部分
        /// </summary>
        /// <param name="originChatData"></param>
        /// <param name="modifiedChatData"></param>
        /// <returns></returns>
        public static ChatData ChangeZhangHanZhiLoving90_2(ChatData originChatData, ChatData modifiedChatData)
        {
            // 当前周目是儿子则不处理
            if (record_manager.InstanceManagerRecord.CurrentRecord.playerSex == 1)
            {
                return modifiedChatData;
            }

            ChatData newChatData = modifiedChatData.Copy();

            // 原文是：对，做渣男也是要有资本的……
            newChatData.text = "对，做渣女也是要有资本的……";
            newChatData.text_girl = "对，做渣女也是要有资本的……";

            return newChatData;
        }

        /// <summary>
        /// 修改汤金娜 80 好感度对话
        /// </summary>
        /// <param name="originChatData"></param>
        /// <param name="modifiedChatData"></param>
        /// <returns></returns>
        public static ChatData ChangeTangJinNaLoving80(ChatData originChatData, ChatData modifiedChatData)
        {
            // 当前周目是儿子则不处理
            if (record_manager.InstanceManagerRecord.CurrentRecord.playerSex == 1)
            {
                return modifiedChatData;
            }

            ChatData newChatData = modifiedChatData.Copy();

            // 原文是：看来，穷小子的生活也可以过得有滋有味啊
            newChatData.text = "看来，穷孩子的生活也可以过得有滋有味啊";
            newChatData.text_girl = "看来，穷孩子的生活也可以过得有滋有味啊";

            return newChatData;
        }

        /// <summary>
        /// 修改苏芳允 30 好感度对话
        /// </summary>
        /// <param name="originChatData"></param>
        /// <param name="modifiedChatData"></param>
        /// <returns></returns>
        public static ChatData ChangeSuFangYunLoving30(ChatData originChatData, ChatData modifiedChatData)
        {
            // 当前周目是儿子则不处理
            if (record_manager.InstanceManagerRecord.CurrentRecord.playerSex == 1)
            {
                return modifiedChatData;
            }

            ChatData newChatData = modifiedChatData.Copy();

            // 原文是：哈哈哈，你也是，真是一场酣畅淋漓的战斗！光明之子哟！
            newChatData.text = "哈哈哈，你也是，真是一场酣畅淋漓的战斗！光明之女哟！";
            newChatData.text_girl = "哈哈哈，你也是，真是一场酣畅淋漓的战斗！光明之女哟！";

            return newChatData;
        }

        /// <summary>
        /// 修改苏芳允 50 好感度对话，第一部分
        /// </summary>
        /// <param name="originChatData"></param>
        /// <param name="modifiedChatData"></param>
        /// <returns></returns>
        public static ChatData ChangeSuFangYunLoving50_1(ChatData originChatData, ChatData modifiedChatData)
        {
            // 当前周目是儿子则不处理
            if (record_manager.InstanceManagerRecord.CurrentRecord.playerSex == 1)
            {
                return modifiedChatData;
            }

            ChatData newChatData = modifiedChatData.Copy();

            // 原文是：感谢你的信任，巴尔德之子，但很遗憾，我并没有看过你说的那部动画。
            newChatData.text = "感谢你的信任，巴尔德之女，但很遗憾，我并没有看过你说的那部动画。";
            newChatData.text_girl = "感谢你的信任，巴尔德之女，但很遗憾，我并没有看过你说的那部动画。";

            return newChatData;
        }

        /// <summary>
        /// 修改苏芳允 50 好感度对话，第二部分
        /// </summary>
        /// <param name="originChatData"></param>
        /// <param name="modifiedChatData"></param>
        /// <returns></returns>
        public static ChatData ChangeSuFangYunLoving50_2(ChatData originChatData, ChatData modifiedChatData)
        {
            // 当前周目是儿子则不处理
            if (record_manager.InstanceManagerRecord.CurrentRecord.playerSex == 1)
            {
                return modifiedChatData;
            }

            ChatData newChatData = modifiedChatData.Copy();

            // 原文是：喔喔！很懂嘛！巴尔德之子！对于尚未觉醒的光明之子来说能察觉到正义的力量！
            newChatData.text = "喔喔！很懂嘛！巴尔德之女！对于尚未觉醒的光明之女来说能察觉到正义的力量！";
            newChatData.text_girl = "喔喔！很懂嘛！巴尔德之女！对于尚未觉醒的光明之女来说能察觉到正义的力量！";

            return newChatData;
        }

        /// <summary>
        /// 修改苏芳允好感度上升对话 1
        /// </summary>
        /// <param name="originChatData"></param>
        /// <param name="modifiedChatData"></param>
        /// <returns></returns>
        public static ChatData ChangeSuFangYunGood1(ChatData originChatData, ChatData modifiedChatData)
        {
            // 当前周目是儿子则不处理
            if (record_manager.InstanceManagerRecord.CurrentRecord.playerSex == 1)
            {
                return modifiedChatData;
            }

            ChatData newChatData = modifiedChatData.Copy();

            // 原文是：哈哈哈，嗯唔！很懂的嘛！少年！
            newChatData.text = "哈哈哈，嗯唔！很懂的嘛！少女！";
            newChatData.text_girl = "哈哈哈，嗯唔！很懂的嘛！少女！";

            return newChatData;
        }

        /// <summary>
        /// 修改苏芳允好感度上升对话 2
        /// </summary>
        /// <param name="originChatData"></param>
        /// <param name="modifiedChatData"></param>
        /// <returns></returns>
        public static ChatData ChangeSuFangYunGood2(ChatData originChatData, ChatData modifiedChatData)
        {
            // 当前周目是儿子则不处理
            if (record_manager.InstanceManagerRecord.CurrentRecord.playerSex == 1)
            {
                return modifiedChatData;
            }

            ChatData newChatData = modifiedChatData.Copy();

            // 原文是：哈哈~少年，能让我这么开心的，你还是第一个！
            newChatData.text = "哈哈~少女，能让我这么开心的，你还是第一个！";
            newChatData.text_girl = "哈哈~少女，能让我这么开心的，你还是第一个！";

            return newChatData;
        }

        /// <summary>
        /// 修改苏芳允好感度上升对话 3
        /// </summary>
        /// <param name="originChatData"></param>
        /// <param name="modifiedChatData"></param>
        /// <returns></returns>
        public static ChatData ChangeSuFangYunGood3(ChatData originChatData, ChatData modifiedChatData)
        {
            // 当前周目是儿子则不处理
            if (record_manager.InstanceManagerRecord.CurrentRecord.playerSex == 1)
            {
                return modifiedChatData;
            }

            ChatData newChatData = modifiedChatData.Copy();

            // 原文是：谢谢，不愧是是被选中的少年哟！
            newChatData.text = "谢谢，不愧是是被选中的少女哟！";
            newChatData.text_girl = "谢谢，不愧是是被选中的少女哟！";

            return newChatData;
        }

        /// <summary>
        /// 修改苏芳允好感度下降对话
        /// </summary>
        /// <param name="originChatData"></param>
        /// <param name="modifiedChatData"></param>
        /// <returns></returns>
        public static ChatData ChangeSuFangYunBad(ChatData originChatData, ChatData modifiedChatData)
        {
            // 当前周目是儿子则不处理
            if (record_manager.InstanceManagerRecord.CurrentRecord.playerSex == 1)
            {
                return modifiedChatData;
            }

            ChatData newChatData = modifiedChatData.Copy();

            // 原文是：虽然不想直说，但是少年你的无聊之力可以让独眼巨人沉睡……
            newChatData.text = "虽然不想直说，但是少女你的无聊之力可以让独眼巨人沉睡……";
            newChatData.text_girl = "虽然不想直说，但是少女你的无聊之力可以让独眼巨人沉睡……";

            return newChatData;
        }

        /// <summary>
        /// 修改牧唯 20 好感度对话，第一部分
        /// </summary>
        /// <param name="originChatData"></param>
        /// <param name="modifiedChatData"></param>
        /// <returns></returns>
        public static ChatData ChangeMuWeiLoving20_1(ChatData originChatData, ChatData modifiedChatData)
        {
            // 当前周目是儿子则不处理
            if (record_manager.InstanceManagerRecord.CurrentRecord.playerSex == 1)
            {
                return modifiedChatData;
            }

            ChatData newChatData = modifiedChatData.Copy();

            // 原文是：你！你小子威胁我？！呃……好好好，我是牧唯！我还想继续学业所以转学过来，毕竟不想被公司发现我擅自行动，会有麻烦的。这样行了吧！
            newChatData.text = "你！你个小丫头威胁我？！呃……好好好，我是牧唯！我还想继续学业所以转学过来，毕竟不想被公司发现我擅自行动，会有麻烦的。这样行了吧！";
            newChatData.text_girl = "你！你个小丫头威胁我？！呃……好好好，我是牧唯！我还想继续学业所以转学过来，毕竟不想被公司发现我擅自行动，会有麻烦的。这样行了吧！";

            return newChatData;
        }

        /// <summary>
        /// 修改牧唯 20 好感度对话，第二部分
        /// </summary>
        /// <param name="originChatData"></param>
        /// <param name="modifiedChatData"></param>
        /// <returns></returns>
        public static ChatData ChangeMuWeiLoving20_2(ChatData originChatData, ChatData modifiedChatData)
        {
            // 当前周目是儿子则不处理
            if (record_manager.InstanceManagerRecord.CurrentRecord.playerSex == 1)
            {
                return modifiedChatData;
            }

            ChatData newChatData = modifiedChatData.Copy();

            // 原文是：哈哈哈，我说是就是！公司有事，先走喽，下次见啦，小老弟~
            newChatData.text = "哈哈哈，我说是就是！公司有事，先走喽，下次见啦，小老妹~";
            newChatData.text_girl = "哈哈哈，我说是就是！公司有事，先走喽，下次见啦，小老妹~";

            return newChatData;
        }

        /// <summary>
        /// 修改牧唯 30 好感度对话，第一部分
        /// </summary>
        /// <param name="originChatData"></param>
        /// <param name="modifiedChatData"></param>
        /// <returns></returns>
        public static ChatData ChangeMuWeiLoving30_1(ChatData originChatData, ChatData modifiedChatData)
        {
            // 当前周目是儿子则不处理
            if (record_manager.InstanceManagerRecord.CurrentRecord.playerSex == 1)
            {
                return modifiedChatData;
            }

            ChatData newChatData = modifiedChatData.Copy();

            // 原文是：这不是小老弟吗~好巧啊，你家也是这个方向么？一起走？
            newChatData.text = "这不是小老妹吗~好巧啊，你家也是这个方向么？一起走？";
            newChatData.text_girl = "这不是小老妹吗~好巧啊，你家也是这个方向么？一起走？";

            return newChatData;
        }

        /// <summary>
        /// 修改牧唯 30 好感度对话，第二部分
        /// </summary>
        /// <param name="originChatData"></param>
        /// <param name="modifiedChatData"></param>
        /// <returns></returns>
        public static ChatData ChangeMuWeiLoving30_2(ChatData originChatData, ChatData modifiedChatData)
        {
            // 当前周目是儿子则不处理
            if (record_manager.InstanceManagerRecord.CurrentRecord.playerSex == 1)
            {
                return modifiedChatData;
            }

            ChatData newChatData = modifiedChatData.Copy();

            // 原文是：嘿，你小子竟然还质疑姐姐，那是时候教育教育你了~
            newChatData.text = "嘿，你这小丫头竟然还质疑姐姐，那是时候教育教育你了~";
            newChatData.text_girl = "嘿，你这小丫头竟然还质疑姐姐，那是时候教育教育你了~";

            return newChatData;
        }

        /// <summary>
        /// 修改牧唯 80 好感度对话，第一部分
        /// </summary>
        /// <param name="originChatData"></param>
        /// <param name="modifiedChatData"></param>
        /// <returns></returns>
        public static ChatData ChangeMuWeiLoving80_1(ChatData originChatData, ChatData modifiedChatData)
        {
            // 当前周目是儿子则不处理
            if (record_manager.InstanceManagerRecord.CurrentRecord.playerSex == 1)
            {
                return modifiedChatData;
            }

            ChatData newChatData = modifiedChatData.Copy();

            // 原文是：哥哥，你是姐姐的男朋友么？
            newChatData.text = "姐姐，你是姐姐的朋友么？";
            newChatData.text_girl = "姐姐，你是姐姐的朋友么？";

            return newChatData;
        }

        /// <summary>
        /// 修改牧唯 80 好感度对话，第二部分
        /// </summary>
        /// <param name="originChatData"></param>
        /// <param name="modifiedChatData"></param>
        /// <returns></returns>
        public static ChatData ChangeMuWeiLoving80_2(ChatData originChatData, ChatData modifiedChatData)
        {
            // 当前周目是儿子则不处理
            if (record_manager.InstanceManagerRecord.CurrentRecord.playerSex == 1)
            {
                return modifiedChatData;
            }

            ChatData newChatData = modifiedChatData.Copy();

            // 原文是：噗，不是不是……
            newChatData.text = "对啊~";
            newChatData.text_girl = "对啊~";

            return newChatData;
        }

        /// <summary>
        /// 修改牧唯 80 好感度对话，第三部分
        /// </summary>
        /// <param name="originChatData"></param>
        /// <param name="modifiedChatData"></param>
        /// <returns></returns>
        public static ChatData ChangeMuWeiLoving80_3(ChatData originChatData, ChatData modifiedChatData)
        {
            // 当前周目是儿子则不处理
            if (record_manager.InstanceManagerRecord.CurrentRecord.playerSex == 1)
            {
                return modifiedChatData;
            }

            ChatData newChatData = modifiedChatData.Copy();

            // 原文是：哼，就他？就这样还想泡我姐？
            newChatData.text = "我就说没有男生敢追我姐，来的都是女的。";
            newChatData.text_girl = "我就说没有男生敢追我姐，来的都是女的。";

            return newChatData;
        }

        /// <summary>
        /// 修改牧唯 80 好感度对话，第四部分
        /// </summary>
        /// <param name="originChatData"></param>
        /// <param name="modifiedChatData"></param>
        /// <returns></returns>
        public static ChatData ChangeMuWeiLoving80_4(ChatData originChatData, ChatData modifiedChatData)
        {
            // 当前周目是儿子则不处理
            if (record_manager.InstanceManagerRecord.CurrentRecord.playerSex == 1)
            {
                return modifiedChatData;
            }

            ChatData newChatData = modifiedChatData.Copy();

            // 原文是：你们俩又欠揍了是不是！怎么跟客人说话呢！快去洗手，准备吃饭了！
            newChatData.text = "你们俩又欠揍了是不是！怎么当着客人面什么都乱说呢！快去洗手，准备吃饭了！";
            newChatData.text_girl = "你们俩又欠揍了是不是！怎么当着客人面什么都乱说呢！快去洗手，准备吃饭了！";

            return newChatData;
        }

        /// <summary>
        /// 修改牧唯 90 好感度对话
        /// </summary>
        /// <param name="originChatData"></param>
        /// <param name="modifiedChatData"></param>
        /// <returns></returns>
        public static ChatData ChangeMuWeiLoving90(ChatData originChatData, ChatData modifiedChatData)
        {
            // 当前周目是儿子则不处理
            if (record_manager.InstanceManagerRecord.CurrentRecord.playerSex == 1)
            {
                return modifiedChatData;
            }

            ChatData newChatData = modifiedChatData.Copy();

            // 原文是：光谢我多没有诚意啊，这样吧，要不以后你就喊我哥吧，哈哈哈。
            newChatData.text = "光谢我多没有诚意啊，这样吧，要不以后你就喊我姐吧，哈哈哈。";
            newChatData.text_girl = "光谢我多没有诚意啊，这样吧，要不以后你就喊我姐吧，哈哈哈。";

            return newChatData;
        }

        /// <summary>
        /// 修改牧唯降低好感度对话
        /// </summary>
        /// <param name="originChatData"></param>
        /// <param name="modifiedChatData"></param>
        /// <returns></returns>
        public static ChatData ChangeMuWeiBad(ChatData originChatData, ChatData modifiedChatData)
        {
            // 当前周目是儿子则不处理
            if (record_manager.InstanceManagerRecord.CurrentRecord.playerSex == 1)
            {
                return modifiedChatData;
            }

            ChatData newChatData = modifiedChatData.Copy();

            // 原文是：虽然不想直说，但是小老弟你这次真的有点无聊
            newChatData.text = "虽然不想直说，但是小老妹你这次真的有点无聊";
            newChatData.text_girl = "虽然不想直说，但是小老妹你这次真的有点无聊";

            return newChatData;
        }
    }

    // FIXME：测试代码，让女儿和女生互动后好感度立刻拉满，便于测试所有的好感度对话
    /// <summary>
    /// 让女儿和女生互动后好感度立刻拉满，便于测试所有的好感度对话
    /// </summary>
    [HarmonyPatch(typeof(panel_girls), "effect")]
    public static class panel_girls_effect
    {
        private static void Postfix(int id, int girl_id)
        {
            // 如果 Mod 未启动则不作处理
            if (!Main.enabled)
            {
                return;
            }
            // 当前周目是儿子则不处理
            if (record_manager.InstanceManagerRecord.CurrentRecord.playerSex == 1)
            {
                return;
            }

            // 修改原逻辑，这里强行设置好感度到爆表
            if (girlmanager.InstanceGirlmanager.GirlsDictionary.ContainsKey(girl_id))
            {
                Dictionary<int, int> girlsDictionary;
                (girlsDictionary = girlmanager.InstanceGirlmanager.GirlsDictionary)[girl_id] = 150;
            }
        }
    }
    // FIXME：测试代码，让女儿可以无视条件和女同学社交
    /// <summary>
    /// 让女儿可以无视条件和女同学社交，便于测试所有的好感度对话
    /// </summary>
    [HarmonyPatch(typeof(panel_girls), "is_talk")]
    public static class panel_girls_is_talk
    {
        private static void Postfix(ref bool __result)
        {
            // 如果 Mod 未启动则不作处理
            if (!Main.enabled)
            {
                return;
            }
            // 当前周目是儿子则不处理
            if (record_manager.InstanceManagerRecord.CurrentRecord.playerSex == 1)
            {
                return;
            }

            __result = true;
        }
    }
}
