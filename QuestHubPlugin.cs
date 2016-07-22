using System;
using System.Linq;
using System.Reflection;
using Buddy.Engine;
using log4net;
using Buddy.Common;

using System.Windows.Forms;
using System.Windows;
using QuestHub.Context;
using System.IO;
using Buddy.BladeAndSoul.Infrastructure;
using MahApps.Metro.Controls;
using Buddy.BotCommon;

namespace QuestHub
{
	public class QuestHubPlugin : IPlugin, IUIButtonProvider
	{
		private static readonly ILog Log = LogManager.GetLogger("[QuestHubPlugin]");
		#region IPulsable

		public bool CanBePulsed => false;

		public void OnRegistered()
		{
            Log.Debug("REGISTER");
		}

		public void OnUnregistered()
		{
		}

		public void Pulse()
		{
		}

		#endregion

		#region IUIButtonProvider
		public string ButtonText => "Quest Hub";

		private readonly string uiPath = Path.Combine(AppSettings.Instance.FullPluginsPath, "QuestHub", "GUI");

		private MetroWindow _gui;
		public void OnButtonClicked(object sender)
		{
			if (_gui == null)
			{
				_gui = new MetroWindow
				{
					DataContext = new QuestHubContext(),
					Content = WPFUtils.LoadWindowContent(Path.Combine(uiPath, "MainView.xaml")),
					MinHeight = 400,
					MinWidth = 200,
					Title = "Quest Hub Settings",
					ResizeMode = ResizeMode.CanResizeWithGrip,

					//SizeToContent = SizeToContent.WidthAndHeight,
					SnapsToDevicePixels = true,
					Topmost = false,
					WindowStartupLocation = WindowStartupLocation.Manual,
					WindowStyle = WindowStyle.SingleBorderWindow,
					Owner = null,
					Width = 550,
					Height = 650,
				};
				_gui.Closed += WindowClosed;
			}
			_gui.Show();
		}

		void WindowClosed(object sender, EventArgs e)
		{
			var context = _gui.DataContext as QuestHubContext;
			if (context != null)
			{
				Log.Info("Save settings!");
				context.Settings.Save();
			}
			else
			{
				Log.InfoFormat("context == null");
			}
			_gui = null;
		}

		#endregion

		#region IAuthored

		public string Author => "Zzi";

		public string Name => "QuestHub";

		public Version Version => new Version(1, 0, 0, 0);

		#endregion

		public bool Enabled { get; set; }

		public void Initialize()
		{
            if (ScriptManager.IsInitialized)
            {
                Log.Info("Register Python Helpers");
                ScriptManager.RegisterShortcutDefinition(typeof(ScriptHelper.Scripts));
            }
            else
            {
                Log.Fatal("scripting not setup QuestHub Profiles will fail to function.");
            }
        }

		public void Uninitialize()
		{
		}
	}
}

