using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Tiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell.Armor
{
	[AutoloadEquip(EquipType.Body)]
	public class EyeBody : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 5;
			Item.width = 36;
			Item.height = 20;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(gold: 2);
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetDamage(DamageClass.Summon) += 0.05f;
			player.GetDamage(DamageClass.SummonMeleeSpeed) += 0.05f;
			player.maxMinions += 1;
			player.maxTurrets += 1;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
			.AddIngredient(ItemID.DemoniteBar, 10)
            .AddIngredient(ModContent.ItemType<EyeBlockItem>(), 38)
			.AddIngredient(ModContent.ItemType<LivingFleshItem>(), 75)
            .AddTile(TileID.Anvils)
            .Register();

			CreateRecipe()
			.AddIngredient(ItemID.CrimtaneBar, 10)
            .AddIngredient(ModContent.ItemType<EyeBlockItem>(), 38)
			.AddIngredient(ModContent.ItemType<LivingFleshItem>(), 75)
            .AddTile(TileID.Anvils)
            .Register();
        }
	}
}
