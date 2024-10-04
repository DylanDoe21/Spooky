using Spooky.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.Quest
{
	public class SewingThread : ModItem
	{
		public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 62;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;  
            Item.value = Item.buyPrice(gold: 10);
        }

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<SpookyPlayer>().SewingThread = true;
		}
	}
}
