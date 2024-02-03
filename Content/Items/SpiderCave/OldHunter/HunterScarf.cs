using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.SpiderCave.OldHunter
{
	public class HunterScarf : ModItem
	{
		public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 32;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;  
            Item.value = Item.buyPrice(gold: 5);
        }

		public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<SpookyPlayer>().HunterScarf = true;
        }
	}
}
