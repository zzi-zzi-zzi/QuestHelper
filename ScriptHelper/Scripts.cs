using System;
using Buddy.BladeAndSoul.Game;
using System.Linq;
using Buddy.BladeAndSoul.Game.Objects;
using Buddy.BotCommon;
using log4net;
using Buddy.Coroutines;
using QuestHub.Context;

namespace QuestHub.ScriptHelper
{
    public static class Scripts
    {
        private static readonly ILog Log = LogManager.GetLogger("[QuestHubPlugin]");

        public static bool HasItem(int id)
        {
            return GameManager.LocalPlayer.InBagItems.Any(i => i.ItemId == id);
        }

        public static bool IsEquipped(int id)
        {
            return GameManager.LocalPlayer.EquippedItems.Any(i => i.ItemId == id);
        }
        
        public static bool IsClass(string target)
        {
            var localClass = GameManager.LocalPlayer.Class;
            try
            {
                var targetClass = (PlayerClass)Enum.Parse(typeof(PlayerClass), target);
                return targetClass == localClass;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static void LoadQuestHubSettings()
        {
            ScriptProxy.Variables.QuestHub = Settings.Instance;
        }
        /*
        * 암살자 => Assassin
		* 귀검사 => Blade Dancer
		* 검사 => Blade Master
		* 역사 => Destroyer
		* 기공사 => Force Master
		* 권사 => KFM
		* 기권사 => SoulFighter
		* 소환사 => Summoner
		* 주술사 => Warlock
        */
        public static int QuestSelect(int Assassin, int BladeDancer, int BladeMaster, int Destroyer, int ForceMaster, int KFM, int SoulFighter, int Summoner, int Warlock)
        {
            switch (GameManager.LocalPlayer.Class)
            {
                case PlayerClass.Assassin:
                    return Assassin;
                case PlayerClass.SwordMaster:
                    return BladeDancer;
                case PlayerClass.BladeMaster:
                    return BladeMaster;
                case PlayerClass.Destroyer:
                    return Destroyer;
                case PlayerClass.ForceMaster:
                    return ForceMaster;
                case PlayerClass.KungFuFighter:
                    return KFM;
                case PlayerClass.SoulFighter:
                    return SoulFighter;
                case PlayerClass.Summoner:
                    return Summoner;
                case PlayerClass.Warlock:
                    return Warlock;
                default:
                    return -1;
            }
        }


        public static bool OnCooldown(string spellAlias)
        {
            var lp = GameManager.LocalPlayer.CurrentSkills.FirstOrDefault(i => i.Alias == spellAlias);
            return lp?.CanCast() ?? false;
        }

        public static bool TargetIsCasting()
        {
            return (GameManager.LocalPlayer.CurrentTarget as Npc)?.IsCasting ?? false;
        }
    }

}

