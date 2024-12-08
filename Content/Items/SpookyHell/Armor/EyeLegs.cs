using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Items.SpookyHell;
using Spooky.Content.Items.SpookyHell.Misc;
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
			player.moveSpeed += 0.12f;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
			.AddRecipeGroup("SpookyMod:DemoniteBars", 12)
			.AddIngredient(ModContent.ItemType<CreepyChunk>(), 25)
			.AddIngredient(ModContent.ItemType<LivingFleshItem>(), 65)
            .AddTile(TileID.Anvils)
            .Register();
        }
	}
}
