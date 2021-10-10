using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MtC.Mod.ChineseParents.EventControlLib;
using MtC.Mod.ChineseParents.LovingTimeController;

namespace MtC.Mod.ChineseParents.Yuri
{
    class AddTransferEvent
    {
        /// <summary>
        /// 所有添加的事件
        /// </summary>
        private static List<EventControl.ChatEventControlParam> addCharEventParams = new List<EventControl.ChatEventControlParam>();

        /// <summary>
        /// 向事件控制前置 Mod 添加修改转学事件的事件
        /// </summary>
        public static void ChangeTransferEvents()
        {
            // 每个女同学的 原有转学回合、设置转学回合、转学事件 ID
            List<int[]> transferRoundParams = new List<int[]>();
            // 王胜男
            transferRoundParams.Add(new int[] { LovingTimeController.Main.VANILLA_WANG_SHENG_NAN_TRANSFER_ROUNDS, LovingTimeController.Main.settings.wangShegnNanTransferRounds, LovingTimeController.Main.WANG_SHENG_NAN_TRANSFER_DATA_ID });
            // 李若放
            transferRoundParams.Add(new int[] { LovingTimeController.Main.VANILLA_LI_RUO_FANG_TRANSFER_ROUNDS, LovingTimeController.Main.settings.liRuoFangTransferRounds, LovingTimeController.Main.LI_RUO_FANG_TRANSFER_DATA_ID });
            // 汤金娜
            transferRoundParams.Add(new int[] { LovingTimeController.Main.VANILLA_TANG_JIN_NA_TRANSFER_ROUNDS, LovingTimeController.Main.settings.tangJinNaTransferRounds, LovingTimeController.Main.TANG_JIN_NA_TRANSFER_DATA_ID });
            // 苏芳允
            transferRoundParams.Add(new int[] { LovingTimeController.Main.VANILLA_SU_FANG_YUN_TRANSFER_ROUNDS, LovingTimeController.Main.settings.suFangYunTransferRounds, LovingTimeController.Main.SU_FANG_YUN_TRANSFER_DATA_ID });
            // 牧唯
            transferRoundParams.Add(new int[] { LovingTimeController.Main.VANILLA_MU_WEI_TRANSFER_ROUNDS, LovingTimeController.Main.settings.muWeiTransferRounds, LovingTimeController.Main.MU_WEI_TRANSFER_DATA_ID });

            // 遍历所有女同学，修改转学事件发出回合数
            transferRoundParams.ForEach(param =>
            {
                // 如果设置的转学回合数和原本的转学回合数相同，则这个事件不会在改变恋爱时间 Mod 中被注册，而游戏原逻辑女儿版不会发出这个转学事件，需要让事件管理器添加这个事件
                if (param[1] == param[0])
                {
                    // 添加转学事件，出现条件是：是出现的回合 并且 这一周目是女儿
                    EventControl.ChatEventControlParam addParam = EventControl.AddChatEvent(
                        param[2],
                        (id) => player_data.Instance.round_current == param[1] && record_manager.InstanceManagerRecord.CurrentRecord.playerSex == 2,
                        (id) => { });
                    // 记录添加的事件
                    addCharEventParams.Add(addParam);
                }
            });
        }

        /// <summary>
        /// 从事件控制前置 Mod 移除修改转学事件的事件
        /// </summary>
        public static void UnchangeTransferEvents()
        {
            // 移除所有添加的事件
            addCharEventParams.ForEach(param =>
            {
                EventControl.RemoveAddChatEvent(param);
            });
        }
    }
}
