using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml;
using System.IO;
using Buddy.Common.Mvvm;
using log4net;
using Buddy.BotCommon.ProfileTags;
using QuestHub.Tags;
using Buddy.Engine;
using Buddy.BladeAndSoul.Game;
using System.Linq;
using Buddy.BladeAndSoul.Game.Objects;
using Buddy.BladeAndSoul.Game.Quests;
using System.ComponentModel;
using System.Collections.Generic;
using System.Xml.Linq;

namespace QuestHub.Context
{
	public class QuestHubContext : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private static readonly ILog Log = LogManager.GetLogger("[QuestHubPlugin]");

        public Settings Settings => Settings.Instance;

		public QuestHubContext()
		{
		}

		protected void OnPropertyChanged(string name)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(name));
			}
		}
	}
}

