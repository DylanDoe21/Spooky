using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.Costume
{
	[AutoloadEquip(EquipType.Head)]
	public class PartySkeletonSkull : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.vanity = true;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(gold: 10);
		}
	}
}