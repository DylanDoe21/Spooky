using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Items.SpookyHell;
using Spooky.Content.Tiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell.Armor
{
	[AutoloadEquip(EquipType.Legs)]
	public class EyeLegs : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 3;
			Item.width = 22;
			Item.height = 16;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(gold: 2);
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetDamage(DamageClass.Summon) += 0.06f;
			player.GetDamage(DamageClass.SummonMeleeSpeed) += 0.06f;
			player.moveSpeed += 0.12f;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
			.AddIngredient(ItemID.DemoniteBar, 12)
			.AddIngredient(ModContent.ItemType<LivingFleshItem>(), 65)
            .AddTile(TileID.Anvils)
            .Register();

			CreateRecipe()
			.AddIngredient(ItemID.CrimtaneBar, 12)
			.AddIngredient(ModContent.ItemType<LivingFleshItem>(), 65)
            .AddTile(TileID.Anvils)
            .Register();
        }
	}
}
