using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.Costume
{
	[AutoloadEquip(EquipType.Head)]
	public class DylanDoeHead : ModItem, ISpecialHelmetDraw
	{
		public string HeadTexture => "Spooky/Content/Items/Costume/DylanDoeHead_Head_Flipped";

		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 30;
			Item.vanity = true;
			Item.rare = ItemRarityID.Quest;
			Item.value = Item.buyPrice(gold: 10);
		}
	}
}