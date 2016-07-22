using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Linq;
using Buddy.BladeAndSoul.Game;
using Buddy.BotCommon.ProfileTags;
using Buddy.Engine.Profiles;
using Buddy.Engine;
using Buddy.BladeAndSoul.Game.UnrealEngine.Engine;
using Buddy.BladeAndSoul.Game.UnrealEngine;

namespace QuestHub.Tags
{
    [ProfileElementName("Fuse")]
    public class Fuse : QuestProfileElement
    {
        public int ItemId { get; set; }

        public override bool Load(XElement xml)
        {
            if (base.Load(xml))
            {
                this.ItemId = xml.LoadAttribute("ItemId", -1);
                return true;
            }
            return false;
        }
        public override async Task ProfileTagLogic()
        {
            if (this.QuestConstraintsComplete)
                return;
            var equipped = true;
            var item = GameManager.LocalPlayer.EquippedItems.FirstOrDefault(i => i.ItemId == ItemId);
            if(item == null)
            {
                item = GameManager.LocalPlayer.InBagItems.FirstOrDefault(i => i.ItemId == ItemId);
                if(item == null)
                {
                    Log.ErrorFormat("Failed to Fuse to item id: {0}", ItemId);
                    GameEngine.BotPulsator.Stop();
                    return;
                }
                equipped = false;
            }

            InputManager.PressKeybind(GameOptions.FindOptionByShortcut(ShortcutKey.ToggleInventory, GameOptions.GameOptionType.KeybindData).Keybind, true, true);

            if (equipped)
            {
                var inventory = FindUIScreenObject<UCustomUIImage>("Inventory2Panel.Inventory2PlayerStatusHolder.Inventory2_PlayterStatusEquipHolder.Inventory2_Player_EquipHolder.Inventory2_WeaponMesh_Holder.Inventory2_Equip");
                InputManager.KeyDown(System.Windows.Forms.Keys.Shift);
                inventory.Click();
                InputManager.KeyUp(System.Windows.Forms.Keys.Shift);
            }
            else
            {
                var row = Math.Ceiling(item.InventorySlot / 8f);
                var slot = item.InventorySlot % 8;
                if (slot == 0)
                    slot = 8;

                var inventory = GameManager.UiRoot.Controls.First(i => i.Value.Name == "Inventory2Panel").Value.UnrealUIObject.Children.
                                        First(i => i.Name == "Inventory2PlayerStatusHolder").Children.
                                        First(i => i.Name == "Inventory2IconListHolder").Children.
                                        First(i => i.Name == "Inventory2IconList").Children.
                                        Where(i => i.Name == $"Inventory2IconList_Icon_{row}").ToList();

                InputManager.KeyDown(System.Windows.Forms.Keys.Shift);
                ((UCustomUIImage)inventory[slot]).Click();
                InputManager.KeyUp(System.Windows.Forms.Keys.Shift);
            }

            var window = GameManager.UiRoot.Controls.First(i => i.Value.Name == "ItemGrowth2Panel").Value;
            if(window == null || !window.Visible)
            {
                Log.Error("failed to open Growth Panel");
                GameEngine.BotPulsator.Stop(); //stop the bot
                return;
            }
            var button = FindUIScreenObject<UCustomUIImage>("ItemGrowth2Panel.ItemGrowth2Panel_BackgroundImage.ItemGrowth2Panel_Growth_Holder.ItemGrowth2Panel_FeedItemHolder.ItemGrowth2Panel_FeedItemResize.ItemGrowth2Panel_FeedItem");
            button.Click();

            var itemselectwindow = GameManager.UiRoot.Controls.First(i => i.Value.Name == "GeneralItemSelectPanel").Value;

            if(itemselectwindow == null || !itemselectwindow.Visible)
            {
                Log.Error("failed to open item select panel from the growth window");
                GameEngine.BotPulsator.Stop();
                return;
            }

            button = FindUIScreenObject<UCustomUIImage>("GeneralItemSelectPanel.GeneralItemSelectPanel_Background.GeneralItemSelectPanel_ItemList.GeneralItemSelectPanel_ItemList_Column_1");
            button.Click();


            var lbutton = FindUIScreenObject<UCustomUILabelButton>("ItemGrowth2Panel.ItemGrowth2Panel_BackgroundImage.ItemGrowth2Panel_Growth_Holder.ItemGrowth2Panel_GrowthBtn");
            lbutton.Click();

            var ubutton = FindUIScreenObject<UCustomUIButton>("ItemGrowth2Panel.ItemGrowth2Panel_Close");
            ubutton.Click();

            InputManager.PressKeybind(GameOptions.FindOptionByShortcut(ShortcutKey.ToggleInventory, GameOptions.GameOptionType.KeybindData).Keybind, true, true);
        }

        public T FindUIScreenObject<T>(string path, bool visibleOnly = true) where T : UUIObject
        {
            if (string.IsNullOrEmpty(path))
                return null;

            var names = path.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            var baseControl = GameManager.UiRoot.FindControl(names.FirstOrDefault(), visibleOnly);

            if (baseControl == null || !baseControl.Visible)
                return null;

            UUIObject currentControl = baseControl.UnrealUIObject;

            foreach (string name in names)
            {
                // Requesting visible-only, but the control is not currently visible. Ignore it.
                if (visibleOnly && (/*!currentControl.IsSceneGroupVisible() ||*/ currentControl.bHidden))
                    return null;

                foreach (var child in currentControl.Children)
                {
                    if (child.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        currentControl = child;
                        break;
                    }
                }
            }

            return currentControl as T;
        }
    }

	[ProfileElementName("Equip")]
	public class Equip : QuestProfileElement
	{
		public int ItemId { get; set; }

		public override bool Load(XElement xml)
		{
			if (base.Load(xml))
			{
				this.ItemId = xml.LoadAttribute("ItemId", -1);
				return true;
			}
			return false;
		}

		public override async Task ProfileTagLogic()
		{
			if (this.QuestConstraintsComplete)
				return;
			//var item = GameManager.LocalPlayer.EquippedItems.FirstOrDefault(i => i.ItemId == ItemId);
			//if (item != null)
			//	return;
			
			var item = GameManager.LocalPlayer.InBagItems.FirstOrDefault(i => i.ItemId == ItemId);
			if (item == null)
			{
				Log.WarnFormat("Sorry sensei I don't have The item {1}", ItemId);
                GameEngine.BotPulsator.Stop(); //stop the bot
                return;
			}

            var row = Math.Ceiling(item.InventorySlot / 8f);
            var slot = item.InventorySlot % 8;
            if (slot == 0)
                slot = 8;

            InputManager.PressKeybind(GameOptions.FindOptionByShortcut(ShortcutKey.ToggleInventory, GameOptions.GameOptionType.KeybindData).Keybind, true, true);

            var inventory = GameManager.UiRoot.Controls.First(i => i.Value.Name == "Inventory2Panel").Value.UnrealUIObject.Children.First(i => i.Name == "Inventory2PlayerStatusHolder").Children.First(i => i.Name == "Inventory2IconListHolder").Children.First(i => i.Name == "Inventory2IconList").Children.Where(i => i.Name == $"Inventory2IconList_Icon_{row}").ToList();
            ((UCustomUIImage)inventory[slot]).RightClick();

            InputManager.PressKeybind(GameOptions.FindOptionByShortcut(ShortcutKey.ToggleInventory, GameOptions.GameOptionType.KeybindData).Keybind, true, true);
            return;
		}


	}
}

