//using System;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Xml.Linq;
//using Buddy.BladeAndSoul.Game;
//using Buddy.BladeAndSoul.Game.Objects;
//using Buddy.BotCommon;
//using Buddy.BotCommon.ProfileTags;
//using Buddy.Coroutines;
//using Buddy.Engine;
//using Buddy.Engine.Profiles;
//using QuestHub.Context;
//using System.Collections.Generic;
//using Buddy.Common;
//using Buddy.BladeAndSoul.Game.Quests;
//using System.Xml.Serialization;

//namespace QuestHub.Tags
//{
//	[ProfileElementName("ForceCombat")]
//	public class ForceCombat : ProfileGroupElement
//    {
//		#region IAuthored
//		public override string Author => "Zzi";

//		public override string Name => "ForceCombat";

//		public override Version Version => new Version(1, 0, 0, 0);
//		#endregion

//		public bool CanSkip { get; set; }

//		public CreatureIdProfileVar Skip { get; set; }

//        public int SkipId { get; set; }

//        private string _qname;
//        public ProfileVar<short> QuestId { get; set; }
//        public ProfileVar<byte> QuestStep { get; set; }
//        public ProfileVar<byte> QuestStepMission { get; set; }
//        public string QuestName
//        {
//            get
//            {
//                return _qname ?? ("Quest_" + this.QuestId);
//            }
//            set
//            {
//                _qname = value;
//            }
//        }

//        public bool HasQuest => GameManager.LocalPlayer.HasQuest(this.QuestId);
//        public bool IsQuestComplete => GameManager.LocalPlayer.IsQuestComplete(this.QuestId);
//        public bool IsObjectiveComplete => ScriptProxy.IsQuestStepMissionComplete((int)this.QuestId, (int)this.QuestStep, (int)this.QuestStepMission);
//        public bool HasQuestConstraint => this.QuestId != 0;
//        public bool HasObjectiveConstraint => this.QuestStep > 0;
//        protected QuestInfo Quest => GameManager.LocalPlayer.ActiveQuests.FirstOrDefault(i => i.QuestId == this.QuestId);

//        [XmlIgnore]
//        public bool QuestConstraintsComplete
//        {
//            get
//            {
//                if (!this.HasQuestConstraint)
//                {
//                    return false;
//                }
//                if (this.HasObjectiveConstraint)
//                {
//                    return this.IsObjectiveComplete;
//                }
//                return this.IsQuestComplete;
//            }
//        }

//        public override bool IsFinished => this.HasQuestConstraint && this.QuestConstraintsComplete;

//        /// <summary>
//        /// used to keep track of if we have attempted to skip the combat sequence but failed for some reason.
//        /// </summary>
//        private bool SkipAttempted = false;

//        /// <summary>
//        /// With="mh_redmokujin_0001" CanSkip="true" Skip="CH_YoungMook_0009"
//        /// </summary>
//        /// <param name="xml">Xml.</param>
        
//            //QuestId="253" QuestStep="3" CanSkip="true" Skip="npc:CH_WB_YoungMook_0001" X="-8468" Y="44228" Z="2220" Map="startzone_p"
//        public override bool Load(XElement xml)
//		{
//			if (base.Load(xml))
//			{
//                this.QuestId = xml.LoadAttributeVar<short>("QuestId", 0);
//                this.QuestStep = xml.LoadAttributeVar<byte>("QuestStep", 0);
//                this.QuestStepMission = xml.LoadAttributeVar<byte>("QuestStepMission", 1);
//                this.QuestName = xml.LoadAttribute<string>("QuestName", null);

//                this.CanSkip = xml.LoadAttribute("CanSkip", false);
//				this.Skip = CreatureIdProfileVar.Load(xml, "Skip");
                
//                return true;
//			}
//			return false;
//		}


//		public override async Task ProfileTagLogic()
//		{
//			if (IsFinished)
//				return;
//            if (CanSkip && Settings.Instance.SkipForcedCombat && !SkipAttempted)
//            {
//                Log.Info("Forced Combat: skipping combat.");
//                SkipAttempted = true;
//                var SkipTarget = Skip.FindMatchingActors().First();
//                if (SkipTarget != null)
//                {
//                    await CommonBehaviors.Interact(() => SkipTarget);
//                }
//                else
//                {
//                    Log.WarnFormat("Unable to find NPC to interact with to skip. Looked for: {0}", Skip.CreatureAlias);
//                }
//            }
//            Log.Info("Forced Combat: Starting Combat.");
//            try
//            {
//                GameEngine.BotPulsator.Suspend(GameEngine.CurrentRoutine, TimeSpan.FromMinutes(5));

//                Log.FatalFormat("Nodes: {0}", GetReferenceElement<ForceCombat>().Children.Count());
//                foreach (var child in GetReferenceElement<ForceCombat>().Children)
//                {
//                    Log.Info("Executing Tag: " + child.GetType());
//                    child.Reset();
//                    while (!child.IsFinished)
//                    {
//                        await child.ProfileTagLogic();
//                        await Coroutine.Yield();
//                    }
//                }

//                Log.Info("Forced Combat: Ending Combat.");
//            }
//            finally
//            {
//                GameEngine.BotPulsator.Resume(GameEngine.CurrentRoutine);
//            }

//			return;
//		}
//    }

//    [ProfileElementName("Target")]
//    public class TargetTag : ProfileElement
//    {
//        #region IAuthored
//        public override string Author => "Zzi";

//        public override string Name => "ForceCombat";

//        public override Version Version => new Version(1, 0, 0, 0);
//        #endregion

//        public CreatureIdProfileVar CreatureId;
//        public bool Approach;
//        public float Distance;
//        public bool _ran;

//        public override void Reset()
//        {
//            _ran = false;
//        }
//        public override bool CanRunDuringCombat => true;

//        public override bool IsFinished => _ran;

//        public override bool Load(XElement xml)
//        {
//            if(base.Load(xml))
//            {
//                CreatureId = CreatureIdProfileVar.Load(xml, "CreatureId");
//                Approach = xml.LoadAttribute("Approach", true);
//                Distance = xml.LoadAttribute("Distance", 5f);
//            }
//            return false;
//        }

//        public override async Task ProfileTagLogic()
//        {
//            var target = CreatureId.FindMatchingActors().FirstOrDefault();

//            if (target == null)
//                return;
//            target.Face();

//            if (Approach)
//                await CommonBehaviors.MoveWithin(() => target.Position, Distance * GameConsts.WorldScale, true, true);

//            _ran = true;
//        }
//    }

//    [ProfileElementName("Cast")]
//    public class CastTag : ProfileElement
//    {
//        #region IAuthored
//        public override string Author => "Zzi";

//        public override string Name => "ForceCombat";

//        public override Version Version => new Version(1, 0, 0, 0);
//        #endregion
//        public string Alias;
//        public bool _ran;

//        public override void Reset()
//        {
//            _ran = false;
//        }
//        public override bool CanRunDuringCombat => true;

//        public override bool IsFinished => _ran;
//        public override bool Load(XElement xml)
//        {
//            if (base.Load(xml))
//            {
//                Alias = xml.LoadAttribute<String>("Alias");
//            }
//            return false;
//        }

//        public override async Task ProfileTagLogic()
//        {
//            try
//            {
//                var skill = GameManager.LocalPlayer.GetSkillByAlias(Alias);
//                if (skill == null)
//                {
//                    Log.WarnFormat("Failed to find skill with alias: {0}", Alias);
//                    return;
//                }
//                if (skill.MaxRange > 0 && GameManager.LocalPlayer.CurrentTarget.Distance > skill.MaxRange)
//                    await CommonBehaviors.MoveWithin(() => GameManager.LocalPlayer.CurrentTarget.Position, (skill.MaxRange - 0.5f), true, true);

//                var castResult = skill.ActorCanCastResult(GameManager.LocalPlayer);
//                Log.DebugFormat("[{0}] {1} CanCast result: {2}", skill.Id, skill.Name, castResult);

//                if (castResult > SkillUseError.None)
//                {
//                    Log.WarnFormat("[{0}] {1} CanCast result: {2}", skill.Id, skill.Name, castResult);
//                    return;
//                }

//                Log.InfoFormat("Casting {0}", skill.Name);
//                skill.Cast();
//                await Coroutine.Sleep(150);
//            }
//            catch (Exception ex)
//            {
//                Log.Debug(ex);
//                Log.WarnFormat("Failed to CastByAlias for {0}", Alias);
//            }

//            _ran = true;
//        }
//    }


//}

