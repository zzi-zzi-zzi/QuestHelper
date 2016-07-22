using Buddy.BladeAndSoul.Game;
using Buddy.BotCommon;
using Buddy.BotCommon.ProfileTags;
using Buddy.Coroutines;
using Buddy.Engine.Profiles;
using QuestHub.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace QuestHub.Tags
{
    [ProfileElementName("WorldBoss")]
    public class WorldBoss : ProfileGroupElement
    {
        public override string Author => "zzi";

        public override string Name => "World Boss";

        public override Version Version => new Version(1, 0, 0);

        public override bool IsFinished {
            get {
                //if we want to kill it make sure we haven't completed this already.
                if (settings.Kill)
                    return Settings.Instance.Characters[GameManager.LocalPlayer.NpcRecordId] || GameManager.LocalPlayer.Level >= MaxLevel;
                return true;
            }
        }

        public string Who { get; set; }

        public int MaxLevel { get; set; }
        public short QuestId { get; set; }
        public int TokenId { get; set; }

        public WorldBossSettings settings;

        public override bool Load(XElement xml)
        {
            if(base.Load(xml))
            {
                this.Who = xml.LoadAttribute<string>("Who");
                this.MaxLevel = xml.LoadAttribute("MaxLevel", 10);
                this.QuestId = xml.LoadAttribute<short>("QuestId", -1);
                this.TokenId = xml.LoadAttribute<int>("TokenId", -1);

                switch (Who)
                {
                    case "Jiangshi":
                        settings = QuestHub.Context.Settings.Instance.Jiangshi;
                        break;
                    default:
                        throw new Exception("Unknown World boss");
                }

            }
            return false;
        }

        public int ItemCount => GameManager.LocalPlayer.InBagItems.Where(i => i.ItemId == TokenId).Select(i => i.StackCount).Sum();
        public bool ShouldTurnInItems => settings.EssenceStack <= ItemCount;

        public override async Task ProfileTagLogic()
        {
            var children = this.GetReferenceElement<WorldBoss>().Children;
            Log.InfoFormat("Doing world boss: {0}", Who);
            if(settings.Daily && QuestId > 0 && !GameManager.LocalPlayer.IsQuestComplete(QuestId))
            {
                var node = children.OfType<PickUpQuestTag>().First();
                while(!node.IsFinished)
                {
                    await node.ProfileTagLogic();
                    await Coroutine.Yield();
                }
            }
            //TODO: Special Logic for spawning bosses (Lycan / level 40 book boss / level 45 pig sacrafice)
            { //combat
                var node = Children.OfType<Grind>().FirstOrDefault();
                if (node != null)
                {
                    node.Reset();
                    while (!node.IsFinished)
                    {
                        await node.ProfileTagLogic();
                        await Coroutine.Yield();
                    }
                }
            }
            var done = false;
            if(!settings.FarmOutfit && !settings.FarmWeapon)
            {
                //done! 
                Settings.Instance.Characters[GameManager.LocalPlayer.NpcRecordId] = true;
                Settings.Instance.Save();
                done = true;
            }

            if (ShouldTurnInItems || done)
            {
                //turnin first.
                if(done){
                    var node = children.OfType<TurnInQuestTag>().FirstOrDefault();

                    if (node != null)
                    {
                        node.Reset();
                        while (!node.IsFinished)
                        {
                            await node.ProfileTagLogic();
                            await Coroutine.Yield();
                        }
                    }
                }
                //item turn in
                {
                    var node = Children.OfType<TokenTurnIn>().FirstOrDefault();
                    if (node != null)
                    {
                        node.Reset();

                        while (!node.IsFinished)
                        {
                            await node.ProfileTagLogic();
                            await Coroutine.Yield();
                        }
                    }
                }
            }
        }
    }

    [ProfileElementName("TokenTurnIn")]
    public class TokenTurnIn : InteractTag
    {
        public override string Author => "zzi";

        public override string Name => "World Boss - Turn In Tokens";

        public override Version Version => new Version(1, 0, 0);


        public override bool Load(XElement xml)
        {
            if(base.Load(xml))
            {
                return true;
            }
            return false;
        }

        public override bool IsFinished => (Parent as WorldBoss).ItemCount > 0;

    }
}
