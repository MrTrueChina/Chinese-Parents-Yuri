using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;

namespace MtC.Mod.ChineseParents.Yuri
{
    /// <summary>
    /// 原逻辑中判断一个事件是否符合触发条件的方法。在这里进行修改让女儿也能看到儿子独有的事件
    /// </summary>
    [HarmonyPatch(typeof(comedy_manager), "is_comedy_condition")]
    public static class comedy_manager_is_comedy_condition
    {
        private static void Postfix(comedy_manager __instance, ref bool __result, comedy myComedy)
        {
            // 如果 Mod 未启动则不处理
            if (!Main.enabled)
            {
                return;
            }

            // 这一代是儿子则不处理
            if (record_manager.InstanceManagerRecord.IsBoy())
            {
                return;
            }

            // 这个事件不是儿子独有，或者已经通过检测了则不处理
            if(myComedy.sex != 1 || __result)
            {
                return;
            }

            // 即使经过前面所有的过滤这个方法还是调用量过大，这个功能可能没法打 Log

            // 如果判断结果是不触发，并且这个事件的性别需求是儿子
            if(!__result && myComedy.sex == 1)
            {
                // 复制一个事件出来，防止对原数据的修改
                comedy noSexComedy = CopyComedy(myComedy);

                // 设为无视性别
                noSexComedy.sex = 0;

                // 用无性别的事件再试一次
                __result = __instance.is_comedy_condition(noSexComedy);
            }
        }

        /// <summary>
        /// 复制一个仅对话事件的数据
        /// </summary>
        /// <param name="originComedy"></param>
        /// <returns></returns>
        private static comedy CopyComedy(comedy originComedy)
        {
            comedy newComedy = new comedy();

            newComedy.id = originComedy.id;
            newComedy.generation = originComedy.generation;
            newComedy.generationMax = originComedy.generationMax;
            newComedy.round_min = originComedy.round_min;
            newComedy.round_max = originComedy.round_max;
            newComedy.uncontain_id = originComedy.uncontain_id;
            newComedy.contain_id = originComedy.contain_id;
            newComedy.contain_skill = originComedy.contain_skill;
            newComedy.contain_status = originComedy.contain_status;
            newComedy.love1001 = originComedy.love1001;
            newComedy.love1002 = originComedy.love1002;
            newComedy.love1003 = originComedy.love1003;
            newComedy.love1004 = originComedy.love1004;
            newComedy.love1005 = originComedy.love1005;
            newComedy.love1006 = originComedy.love1006;
            newComedy.love1007 = originComedy.love1007;
            newComedy.love1008 = originComedy.love1008;
            newComedy.love1009 = originComedy.love1009;
            newComedy.shadow = originComedy.shadow;
            newComedy.chat_id = originComedy.chat_id;
            newComedy.isWegameDlc = originComedy.isWegameDlc;
            newComedy.sex = originComedy.sex;

            return newComedy;
        }

        // 原代码逻辑，留在这里作为参考，不真正使用
        //private static bool Prefix(comedy_manager __instance, ref bool __result, comedy myComedy)
        //{
        //    // 如果 Mod 未启动则不处理
        //    if (!Main.enabled)
        //    {
        //        return true;
        //    }

        //    //// 这一代是儿子则不处理
        //    //if (record_manager.InstanceManagerRecord.IsBoy())
        //    //{
        //    //    Main.ModEntry.Logger.Log("这一代是儿子，不作处理");
        //    //    return;
        //    //}

        //    Main.ModEntry.Logger.Log("comedy_manager_Start.is_comedy_condition 方法调用完毕");

        //    __result =
        //        // 不是试用版，或者是 2000000 号事件，2000000 号事件是面子挑战开启事件
        //        (!DEF.isApproval || myComedy.id != 2000000)
        //        // 并且
        //        && ((record_manager.InstanceManagerRecord.CurrentRecord.RecordcomedyInts.Count <= 0 && action_manager.Instance.ComedyList.Count <= 0) || (!record_manager.InstanceManagerRecord.CurrentRecord.RecordcomedyInts.Contains(myComedy.id) && !action_manager.Instance.ComedyList.Contains(myComedy.id)))
        //        // 并且，(当前回合数在事件发生回合数内)，有些剧情必须达到一定回合数才能触发，例如开启面子挑战事件，绝大多数事件的发生时间是全流程
        //        && (player_data.Instance.Round_current >= myComedy.round_min && player_data.Instance.Round_current <= myComedy.round_max)
        //        // 并且，((没有已触发事件的条件 或 已触发事件条件指定的事件在已触发记录里) 并且 (玩家的代数在事件代数范围内))。这个是判断触发条件的，分别是已触发事件限制和代数限制，但不知道为什么这两个条件被括在了一起
        //        && ((myComedy.contain_id == 0 || record_manager.InstanceManagerRecord.CurrentRecord.RecordcomedyInts.Contains(myComedy.contain_id)) && player_data.Instance.Generations >= myComedy.generation && player_data.Instance.Generations <= myComedy.generationMax)
        //        // 并且，(没有未触发事件的条件 或 未触发事件条件指定的事件不在已触发事件记录里)。这是防止重复触发的，可能也是互斥事件，但游戏过程中没遇到这个功能，也可能是遇到了但忘了
        //        && (myComedy.uncontain_id == 0 || !record_manager.InstanceManagerRecord.CurrentRecord.RecordcomedyInts.Contains(myComedy.uncontain_id))
        //        // 并且，(如果事件对技能没有要求 或 玩家已经学会事件需要的技能)。这是对学会技能后的剧情的判断
        //        && (myComedy.contain_skill == 0 || skill_manager.InstanceManager.LearnedDictionary.ContainsKey(myComedy.contain_skill))
        //        // 并且，(如果事件对性格没有要求 或 玩家具有事件需要的性格)。这是需要性格的剧情的判断，在对事件数据遍历后并没有找到这个需求不为 0 的事件，不确定是遍历失误还是真的没有
        //        && (myComedy.contain_status == 0 || player_data.Instance.status.Contains(myComedy.contain_status))
        //        // 并且，(女孩1好感需求是0，即事件对刘偏偏的好感没有要求 或 刘偏偏的好感达到了事件要求)。这是对刘偏偏好感系列事件的判断
        //        && (myComedy.love1001 == 0 || girlmanager.InstanceGirlmanager.GirlsDictionary[1001] >= myComedy.love1001)
        //        // 并且，同上，席梦来的好感系列事件
        //        && (myComedy.love1002 == 0 || girlmanager.InstanceGirlmanager.GirlsDictionary[1002] >= myComedy.love1002)
        //        // 并且，同上，秦屿路的好感系列事件
        //        && (myComedy.love1003 == 0 || girlmanager.InstanceGirlmanager.GirlsDictionary[1003] >= myComedy.love1003)
        //        // 并且，同上，章涵之的好感系列事件
        //        && (myComedy.love1004 == 0 || girlmanager.InstanceGirlmanager.GirlsDictionary[1004] >= myComedy.love1004)
        //        // 并且，同上，王胜男的好感系列事件
        //        && (myComedy.love1005 == 0 || girlmanager.InstanceGirlmanager.GirlsDictionary[1005] >= myComedy.love1005)
        //        // 并且，同上，李若放的好感系列事件
        //        && (myComedy.love1006 == 0 || girlmanager.InstanceGirlmanager.GirlsDictionary[1006] >= myComedy.love1006)
        //        // 并且，同上，汤金娜的好感系列事件
        //        && (myComedy.love1007 == 0 || girlmanager.InstanceGirlmanager.GirlsDictionary[1007] >= myComedy.love1007)
        //        // 并且，同上，苏芳允的好感系列事件
        //        && (myComedy.love1008 == 0 || girlmanager.InstanceGirlmanager.GirlsDictionary[1008] >= myComedy.love1008)
        //        // 并且，同上，牧唯的好感系列事件，这里有一个对牧唯好感的空判断，考虑到牧唯在女生面板里的默认隐藏设置，可能这是早期的逻辑，牧唯可能有一段时间是转学后才会添加进女同学列表的
        //        && (myComedy.love1009 == 0 || (girlmanager.InstanceGirlmanager.GirlsDictionary.ContainsKey(1009) && girlmanager.InstanceGirlmanager.GirlsDictionary[1009] >= myComedy.love1009))
        //        // 并且，(事件的心理阴影要求是 0 或 玩家心理阴影达到了事件所需的心理阴影)。这是对心理阴影事件的判断
        //        && (myComedy.shadow == 0 || player_data.Instance.Shadow >= myComedy.shadow)
        //        // 并且，((没有 WeGameDLC 要求 或 有 WeGameDLC) 并且 (事件没有性别要求 或 人物性别符合事件性别要求))。其实是 WeGame 平台的 DLC 条件和性别条件，也是不知道为什么括到了一起
        //        && ((myComedy.isWegameDlc == 0 || (wegame.Instance && wegame.Instance.wegameDlc)) && (myComedy.sex == 0 || myComedy.sex == record_manager.InstanceManagerRecord.CurrentRecord.playerSex));

        //    return false;
        //}
    }
}
