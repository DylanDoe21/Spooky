using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Items.SpookyBiome.Misc;
using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome.Armor
{
	[AutoloadEquip(EquipType.Legs)]
	public class WizardGangsterLegs : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 3;
			Item.width = 30;
			Item.height = 18;
			Item.rare = ItemRarityID.Blue;
		}

		public override void UpdateEquip(Player player) 
		{
			player.statManaMax2 += 20;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.GoldBar, 12)
			.AddIngredient(ItemID.Silk, 12)
			.AddIngredient(ModContent.ItemType<SpookyGlowshroom>(), 25)
            .AddTile(TileID.Anvils)
            .Register();
        }
	}
}