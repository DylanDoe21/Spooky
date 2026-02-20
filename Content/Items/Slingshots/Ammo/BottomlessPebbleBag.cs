using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Projectiles.Slingshots;
using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Content.Items.Slingshots.Ammo
{
	public class BottomlessPebbleBag : ModItem
	{
		public override void SetStaticDefaults() 
        {
			ItemGlobal.IsSlingshotAmmo[Item.type] = true;
			Item.ResearchUnlockCount = 99;
		}

		public override void SetDefaults() 
        {
			Item.damage = 5;
			Item.DamageType = DamageClass.Ranged; 
            Item.ammo = Item.type;
            Item.consumable = false;
            Item.width = 14;
			Item.height = 12;
			Item.knockBack = 2f;
			Item.value = Item.buyPrice(copper: 35);
			Item.rare = ItemRarityID.Green; 
			Item.shoot = ModContent.ProjectileType<MossyPebbleProj>();
		}

		public override bool? CanBeChosenAsAmmo(Item weapon, Player player)
		{
			return ItemGlobal.IsSlingshot[weapon.type] ? true : null;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<MossyPebble>(), 3996)
			.AddTile(TileID.CrystalBall)
            .Register();
        }
	}
}