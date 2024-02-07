using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.SpiderCave.Misc
{
	public class SpiderChitin : ModItem
	{
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 50;
        }

		public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 32;
            Item.rare = ItemRarityID.Blue;  
            Item.value = Item.buyPrice(copper: 50);
            Item.maxStack = 9999;
        }
	}
}
