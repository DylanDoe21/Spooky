using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Projectiles.SpookyBiome;
using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome
{
	public class OldWoodArrow : ModItem
	{
		public override void SetStaticDefaults() 
        {
			Item.ResearchUnlockCount = 99;
		}

		public override void SetDefaults() 
        {
			Item.damage = 5;
			Item.DamageType = DamageClass.Ranged; 
            Item.ammo = AmmoID.Arrow;
            Item.consumable = true;
            Item.width = 10;
			Item.height = 42;
			Item.knockBack = 2f;
            Item.maxStack = 9999;
			Item.value = Item.buyPrice(copper: 35);
			Item.rare = ItemRarityID.White; 
			Item.shoot = ModContent.ProjectileType<OldWoodArrowProj>();
			Item.shootSpeed = 3.5f;
		}

		public override void AddRecipes()
        {
            CreateRecipe(25)
			.AddIngredient(ModContent.ItemType<SpookyWoodItem>())
            .AddIngredient(ModContent.ItemType<SpookyStoneItem>())
			.AddTile(TileID.WorkBenches)
            .Register();
        }
	}
}