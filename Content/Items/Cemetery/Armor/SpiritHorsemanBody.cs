using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.Cemetery.Armor
{
	[AutoloadEquip(EquipType.Body)]
	public class SpiritHorsemanBody : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 5;
			Item.width = 34;
			Item.height = 22;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(gold: 2);
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetDamage(DamageClass.Generic) += 0.05f;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<SpookyPlasma>(), 15)
			.AddIngredient(ItemID.Silk, 25)
            .AddTile(TileID.Anvils)
            .Register();
        }
	}
}