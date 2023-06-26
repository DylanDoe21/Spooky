using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Items.SpookyBiome.Misc;
using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome.Armor
{
	[LegacyName("SpookyLegs")]
	[AutoloadEquip(EquipType.Legs)]
	public class GourdLegs : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 4;
			Item.width = 22;
			Item.height = 18;
			Item.rare = ItemRarityID.Blue;
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetAttackSpeed(DamageClass.Melee) += 0.05f;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<RottenChunk>(), 18)
			.AddIngredient(ModContent.ItemType<SpookyWoodItem>(), 25)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}
}