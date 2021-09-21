using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;

namespace MtC.Mod.ChineseParents.Yuri
{
    /// <summary>
    /// 获取所有相亲选项的方法，原逻辑是儿子只能获取女同学、女儿只能获取男同学，在这里让女儿也能显示女同学
    /// </summary>
    [HarmonyPatch(typeof(Panel_blinddate), "create_blinddates")]
    public static class Panel_blinddate_create_blinddates
    {
        /// <summary>
        /// 暂存同学 ID 和好感度的类
        /// </summary>
        public class ClassmateLoving
        {
            /// <summary>
            /// 同学 ID
            /// </summary>
            public int id;
            /// <summary>
            /// 好感度
            /// </summary>
            public int loving;

            public ClassmateLoving(int id, int loving)
            {
                this.id = id;
                this.loving = loving;
            }
        }

        private static void Postfix(Panel_blinddate __instance)
        {
            // 如果 Mod 未启动则不处理
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

            Main.ModEntry.Logger.Log("在相亲列表中添加同学方法调用完毕");

            // 获取所有同学的列表
            List<ClassmateLoving> classmates = new List<ClassmateLoving>();

            // 如果有女同学，将所有女同学添加到同学列表中
            if (girlmanager.InstanceGirlmanager != null && girlmanager.InstanceGirlmanager.GirlsDictionary != null)
            {
                List<ClassmateLoving> girlClassmates = girlmanager.InstanceGirlmanager.girlsDictionary
                    .Select(girlPair => new ClassmateLoving(girlPair.Key, girlPair.Value))
                    .ToList();

                classmates.AddRange(girlClassmates);
            }

            // 如果有男同学，将所有男同学添加到同学列表中
            if (BoysManager.Instance != null && BoysManager.Instance.BoysDictionary != null)
            {
                List<ClassmateLoving> boyClassmates = BoysManager.Instance.BoysDictionary
                    .Select(boyPair => new ClassmateLoving(boyPair.Key, boyPair.Value.loving))
                    .ToList();

                classmates.AddRange(boyClassmates);
            }

            // 按照好感度从高到低排列
            classmates.Sort((a, b) => b.loving - a.loving);

            Main.ModEntry.Logger.Log("所有同学列表加载完毕");

            // 相亲列表中同学的最大数量，这里要进行限制：不能超过原逻辑生成的选项数量、不能超过同学总量
            int maxClassmateBlinddatesNumber = Mathf.Min(Main.settings.maxClassmateBlinddatesNumber, __instance.Blinddates.Count, classmates.Count);

            // 用同学的相亲选项替换掉原来的选项
            for (int i = 0; i < maxClassmateBlinddatesNumber; i++)
            {
                // 如果当前循环到的同学已经没有好感了，结束循环
                if (classmates[i].loving <= 0)
                {
                    break;
                }

                blinddate blinddate;
                if (classmates[i].id == 3008)
                {
                    // 3008 是闺蜜，原逻辑中她没有相亲数据
                    blinddate = getConfidanteMengMengBlinddateData();
                }
                else
                {
                    // 其他同学使用原逻辑的方法创建相亲选项
                    blinddate = blinddate.create(classmates[i].id);
                }

                // 按照原逻辑设定求婚成功率
                blinddate.base_winrate = Mathf.Min(80, classmates[i].loving);

                // 用这个同学的相亲选项替换到原来的相亲选项
                __instance.Blinddates[i] = blinddate;
            }
        }

        /// <summary>
        /// 获取 闺蜜-萌萌 的相亲数据
        /// </summary>
        /// <returns></returns>
        private static blinddate getConfidanteMengMengBlinddateData()
        {
            blinddate blinddate = new blinddate();

            blinddate.desc = "你的闺蜜，毕业后一直和你保持联系，现在是一名旅行作家，在旅行到你所在的城市时被介绍给了你，世界真是小。";

            // 我觉得萌萌应该就是章涵之，至少在定位上差不多，数据用的就是章涵之的数据，正好章涵之的数据很平均
            blinddate.name = "闺蜜-萌萌";
            blinddate.type = 2;
            blinddate.need_status = 15;
            blinddate.need_money = 50;
            blinddate.IQ_round = 2;
            blinddate.EQ_round = 2;
            blinddate.imagination_round = 2;
            blinddate.memory_round = 2;
            blinddate.stamination_round = 0;
            blinddate.base_winrate = 0;

            return blinddate;
        }
    }
}
