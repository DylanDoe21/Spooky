using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.SpookyHell.Misc
{
    public class EyeValleyCompass : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 20;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 15);
        }
        
		public override void UpdateInventory(Player player)
		{
			if (Item == player.HeldItem)
			{
				player.GetModPlayer<SpookyPlayer>().EyeValleyCompass = true;
			}
		}
    }
}