using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.Costume
{
	[AutoloadEquip(EquipType.Head)]
	public class BigBoneMask : ModItem, ISpecialArmorDraw
	{
		public string HeadTexture => "Spooky/Content/Items/Costume/BigBoneMask_HeadReal";
		public string HeadFlippedTexture => "Spooky/Content/Items/Costume/BigBoneMask_Head_RealReal_Flipped";

		public override void SetDefaults()
		{
			Item.width = 52;
			Item.height = 28;
			Item.vanity = true;
			Item.rare = ItemRarityID.Blue;
		}
	}
}