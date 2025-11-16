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
    public class LittleEyeQuestReset : ModItem
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
			Flags.LittleEyeBounty1 = false;
            Flags.LittleEyeBounty2 = false;
            Flags.LittleEyeBounty3 = false;
            Flags.LittleEyeBounty4 = false;
            Flags.BountyInProgress = false;
            Flags.BountyIntro = false;

			return true;
		}
    }
}