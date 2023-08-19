using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.Costume
{
	[AutoloadEquip(EquipType.Head)]
	public class PelusaHead : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 38;
			Item.vanity = true;
			Item.rare = ItemRarityID.Quest;
		}
	}
}