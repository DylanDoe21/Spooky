using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.Minibiomes.Desert
{
	public class HallucigeniaSpine : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 26;
			Item.height = 42;
			Item.value = Item.buyPrice(gold: 15);
			Item.rare = ItemRarityID.LightRed;
			Item.accessory = true;
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
			player.GetModPlayer<SpookyPlayer>().HallucigeniaSpine = true;
        }
	}
}