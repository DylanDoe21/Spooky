using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.SpiderCave.Misc
{
	public class MiteMandibles : ModItem
	{
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 50;
        }

		public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 38;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.Blue;  
            Item.value = Item.buyPrice(silver: 1);
        }
	}
}
