using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.Minibiomes.Armor
{
	[AutoloadEquip(EquipType.Body)]
	public class GoldrushBody : ModItem
	{
		public override void SetStaticDefaults()
        {
			ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<GoldrushLegs>();
		}

		public override void SetDefaults() 
		{
			Item.defense = 3;
			Item.width = 30;
			Item.height = 20;
			Item.rare = ItemRarityID.Blue;
		}

		public override void UpdateEquip(Player player) 
		{
			player.pickSpeed -= 0.1f;
		}
	}
}