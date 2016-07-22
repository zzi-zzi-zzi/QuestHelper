//using System;
//using System.Threading.Tasks;
//using System.Xml.Linq;
//using System.Xml.Serialization;
//using Buddy.BladeAndSoul.Game;
//using Buddy.BotCommon.ProfileTags;
//using Buddy.Coroutines;
//using Buddy.Engine;
//using System.Linq;
//using Buddy.BladeAndSoul.Game.Objects;
//using Buddy.Common.Math;
//using Buddy.Engine.Profiles;
//using static Buddy.BotCommon.CommonBehaviors;
//using Buddy.BladeAndSoul.ViewModels;
//using System.Windows.Forms;
//using System.Collections.Generic;
//using Buddy.BotCommon;

//namespace QuestHub.Tags
//{
//	[ProfileElementName("Loot")]
//	[XmlRoot("Loot")]
//	public class Loot : MoveToTag
//	{
//		static readonly Random r = new Random();

//		public override bool Load(XElement xml)
//		{
//			if (base.Load(xml))
//			{
//				this.Condition = xml.LoadAttribute<string>("Condition", null);
//				this.Distance = xml.LoadAttribute("Distance", 10);
//				this.From = xml.LoadAttribute<string>("From", null);

//				return true;
//			}
//			return false;
//		}

//		public string From { get; set; }
//		public int Distance { get; set; }

//		Func<bool> _expression;

//		/// <summary>
//		/// This is really a should we execute this code y/n. Not a should we skip y/n
//		/// IE. Not HasItem(910000) will return true. so we want !EvaluatedCondition
//		/// </summary>
//		/// <value>The evaluated condition.</value>
//		public bool EvaluatedCondition => _expression.Invoke();

//		string _condition;
//		public string Condition
//		{
//			get { return _condition; }
//			set
//			{
//				if (value != null)
//				{
//					_condition = value;
//					_expression = GameEngine.GetPythonCondition(value);
//				}
//			}
//		}

//		public override bool IsFinished => QuestConstraintsComplete || !EvaluatedCondition;

//		public override async Task ProfileTagLogic()
//		{
//			if (IsFinished)
//				return;

//			await base.ProfileTagLogic(); //mooove

//			var target = GameManager.ActorsOfType<EnvironmentObject>().FirstOrDefault(i => i.Alias == From);
//			if (target == null)
//			{
//				Log.WarnFormat("Failed to find Object with Alias {0}", From);
//				Log.Error("Stopping the bot");
//				GameEngine.BotPulsator.Stop(); //stop the bot
//				return;
//			}
//			try
//			{
//				var iresult = await Interact(() => target, Distance);
//				if (iresult != BehaviorResult.Completed)
//				{
//					target.Face();
//					await Coroutine.Sleep(r.Next(400, 500));
//					InputManager.PressKeybind(GameOptions.FindOptionByShortcut(ShortcutKey.Action1, GameOptions.GameOptionType.KeybindData).Keybind, true, true);
//				}
//			}
//			catch (Exception)
//			{ }

//			while (!QuestConstraintsComplete)
//			{
//                CommonBehaviors.PressContextSkillButton();
//            }

//			return;
//		}
//		//private void Test()
//		//{
//		//	var ids = new List<long>();
//		//	var name = "Blackram Elite Guard";
//		//	foreach (var n in GameManager.ActorsOfType<Npc>().Where(i => i.Name.ToLower() == name.ToLower()))
//		//	{
//		//		if (!ids.Contains(n.CreatureId))
//		//			ids.Add(n.CreatureId);
//		//	}

//		//	Log(string.Join(",", ids.ToArray()));
//		//}
//	}

//}

