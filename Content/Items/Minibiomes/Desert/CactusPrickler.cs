using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.Minibiomes.Desert;
using Spooky.Content.Tiles.Minibiomes.Desert;
 
namespace Spooky.Content.Items.Minibiomes.Desert
{
	public class CactusPrickler : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 16;
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.autoReuse = true;
			Item.width = 34;
			Item.height = 26;         
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 1;
			Item.rare = ItemRarityID.Blue;  
			Item.value = Item.buyPrice(gold: 1);
			Item.UseSound = SoundID.Item39;
			Item.shoot = ModContent.ProjectileType<CactusNeedle>();
			Item.shootSpeed = 18f;
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-3, 0);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 15f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

			Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(12));

			int Needle = Projectile.NewProjectile(source, position.X, position.Y, newVelocity.X, newVelocity.Y, type, damage, knockback, player.whoAmI);
			Main.projectile[Needle].DamageType = DamageClass.Ranged;
			
			return false;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<TarPitCactusBlockItem>(), 20)
            .AddRecipeGroup(RecipeGroupID.IronBar, 12)
            .AddTile(TileID.Anvils)
            .Register();
        }
	}
}
