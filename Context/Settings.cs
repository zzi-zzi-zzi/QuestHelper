using System;
using System.IO;
using Newtonsoft.Json;
using Buddy.Common;
using System.ComponentModel;

namespace QuestHub.Context
{
	public class Settings : JsonSettings, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;


		public Settings() : base(Path.Combine(SettingsPath, "QuestHub.json"))
		{
			if (Jiangshi == null)
				Jiangshi = new WorldBossSettings();
		}

		public bool SkipForcedCombat { get; set; }

		public WorldBossSettings Jiangshi { get; set; }
        private static Settings _instance;
        public static Settings Instance => _instance ?? (_instance = new Settings());

        public DefaultDictionary<int, bool> Characters { get; set; }


        /// <summary>
        /// Called when property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

	}

	public class WorldBossSettings : INotifyPropertyChanged
	{
		/// <summary>
		/// Occurs when property changed.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;


		[DefaultValue(true)]
		public bool Daily { get; set; }

		[DefaultValue(true)]
		public bool FarmOutfit { get; set; }

		[DefaultValue(false)]
		public bool FarmWeapon { get; set; }

		[DefaultValue(8)]
		public int EssenceStack { get; set; }

        public bool Kill { get; set; }


        /// <summary>
        /// Called when property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}

