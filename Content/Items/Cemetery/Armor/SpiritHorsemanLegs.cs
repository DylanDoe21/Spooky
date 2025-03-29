using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.Cemetery.Armor
{
	[AutoloadEquip(EquipType.Legs)]
	public class SpiritHorsemanLegs : ModItem
	{
		public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<SpiritHorsemanHead>();
        }

		public override void SetDefaults() 
		{
			Item.defense = 6;
			Item.width = 22;
			Item.height = 18;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(gold: 3);
		}

		public override void UpdateEquip(Player player) 
		{
			player.moveSpeed += 0.2f;
		}
	}
}