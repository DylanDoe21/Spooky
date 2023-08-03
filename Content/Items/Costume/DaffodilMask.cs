using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.Costume
{
	[AutoloadEquip(EquipType.Head)]
	public class DaffodilMask : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 30;
			Item.vanity = true;
			Item.rare = ItemRarityID.Blue;
		}
	}
}