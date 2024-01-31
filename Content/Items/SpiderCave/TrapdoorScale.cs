using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.SpiderCave
{
	public class TrapdoorScale : ModItem
	{
		public override void SetDefaults()
        {
			Item.defense = 5;
            Item.width = 26;
            Item.height = 32;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;  
            Item.value = Item.buyPrice(gold: 1);
        }

		public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<SpookyPlayer>().TrapdoorScale = true;
        }
	}
}
