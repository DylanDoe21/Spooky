using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.SpookyBiome
{
	public class AutumnLeaf : ModItem
	{
		public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 34;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;  
            Item.value = Item.buyPrice(gold: 1);
        }

		public override void UpdateEquip(Player player) 
		{
			player.GetModPlayer<SpookyPlayer>().AutumnLeaf = true;
		}
	}
}
