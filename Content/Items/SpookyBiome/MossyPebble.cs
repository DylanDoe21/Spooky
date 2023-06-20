using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Projectiles.SpookyBiome;
using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome
{
	public class MossyPebble : ModItem
	{
		public override void SetStaticDefaults() 
        {
			Item.ResearchUnlockCount = 99;
		}

		public override void SetDefaults() 
        {
			Item.damage = 5;
			Item.DamageType = DamageClass.Ranged; 
            Item.ammo = Item.type;
            Item.consumable = true;
            Item.width = 14;
			Item.height = 12;
			Item.knockBack = 2f;
            Item.maxStack = 9999;
			Item.value = Item.buyPrice(copper: 35);
			Item.rare = ItemRarityID.White; 
			Item.shoot = ModContent.ProjectileType<MossyPebbleProj>();
		}

		public override void AddRecipes()
        {
            CreateRecipe(50)
            .AddIngredient(ModContent.ItemType<SpookyStoneItem>())
            .Register();
        }
	}
}