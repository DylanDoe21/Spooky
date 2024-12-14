using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Items.SpiderCave.Misc;
using Spooky.Content.Projectiles.SpiderCave;
using Spooky.Content.Tiles.SpiderCave;
 
namespace Spooky.Content.Items.SpiderCave
{
	public class SpiderCrossbow : ModItem
	{
		public static readonly SoundStyle UseSound = new("Spooky/Content/Sounds/CrossbowCharge", SoundType.Sound);

		public override void SetDefaults()
		{
			Item.damage = 32;
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.autoReuse = false;
			Item.noUseGraphic = true;
			Item.channel = true;
			Item.width = 66;           
			Item.height = 32;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 4;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(gold: 2);
			Item.UseSound = UseSound;
			Item.shoot = ModContent.ProjectileType<SpiderCrossbowProj>();
			Item.useAmmo = AmmoID.Arrow;
			Item.shootSpeed = 0f;
		}

		public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[ModContent.ProjectileType<SpiderCrossbowProj>()] < 1;
		}
		
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, position.X, position.Y, 0, 0, ModContent.ProjectileType<SpiderCrossbowProj>(), damage, knockback, player.whoAmI, 0f, 0f);

			return false;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<SpiderChitin>(), 12)
			.AddIngredient(ModContent.ItemType<WebBlockItem>(), 25)
            .AddTile(TileID.Anvils)
            .Register();
        }
	}
}
