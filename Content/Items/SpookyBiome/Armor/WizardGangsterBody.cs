using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Items.SpookyBiome.Misc;
using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome.Armor
{
	[AutoloadEquip(EquipType.Body)]
	public class WizardGangsterBody : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 4;
			Item.width = 38;
			Item.height = 22;
			Item.rare = ItemRarityID.Blue;
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetDamage(DamageClass.Magic) += 0.1f;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddRecipeGroup("SpookyMod:GoldBars", 15)
			.AddIngredient(ItemID.Silk, 15)
			.AddIngredient(ModContent.ItemType<SpookyGlowshroom>(), 30)
            .AddTile(TileID.Anvils)
            .Register();
        }
	}
}