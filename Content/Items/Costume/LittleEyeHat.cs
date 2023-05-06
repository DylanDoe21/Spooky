using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.Costume
{
	[AutoloadEquip(EquipType.Head)]
	public class LittleEyeHat : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 36;
			Item.height = 26;
			Item.vanity = true;
			Item.rare = ItemRarityID.Blue;
		}
	}
}