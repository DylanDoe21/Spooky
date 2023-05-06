using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.Costume
{
	[AutoloadEquip(EquipType.Head)]
	public class FlowerPotHead : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 24;
			Item.vanity = true;
			Item.rare = ItemRarityID.Blue;
		}
	}
}