using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.Linq;

using Spooky.Core;

namespace Spooky.Content.Items.Debug
{
    public class KrampusQuestReset : ModItem
    {
        public override string Texture => "Spooky/Content/Items/BossSummon/CowBell";

        public override void SetDefaults()
        {                
            Item.width = 20;
            Item.height = 28;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 3;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 1);
        }

		public override bool? UseItem(Player player)
		{
			Flags.KrampusQuest1 = false;
			Flags.KrampusQuest2 = false;
			Flags.KrampusQuest3 = false;
			Flags.KrampusQuest4 = false;
			Flags.KrampusQuest5 = false;
			Flags.KrampusDailyQuest = false;
			Flags.KrampusDailyQuestDone = false;
			Flags.KrampusQuestGiven = false;
			Flags.KrampusQuestlineDone = false;
			Flags.DrawKrampusMapIcon = false;

			return true;
		}
    }
}