using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.Costume
{
	[AutoloadEquip(EquipType.Head)]
	[LegacyName("PelusaHead")]
	public class DandyHead : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 38;
			Item.vanity = true;
			Item.rare = ItemRarityID.Quest;
			Item.value = Item.buyPrice(gold: 10);
		}
	}
}