using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.SpookyBiome
{
	[AutoloadEquip(EquipType.Front, EquipType.Back)]
	public class WarlockRobe : ModItem
	{
		public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 28;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;  
            Item.value = Item.buyPrice(gold: 1);
        }

		public override void UpdateEquip(Player player) 
		{
			player.maxMinions += 1;
			player.moveSpeed += 0.05f;
		}
	}
}
