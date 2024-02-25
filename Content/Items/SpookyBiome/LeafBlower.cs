using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.SpookyBiome;
 
namespace Spooky.Content.Items.SpookyBiome
{
	public class LeafBlower : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 10;
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.autoReuse = true; 
			Item.width = 60;           
			Item.height = 26;         
			Item.useTime = 10;         
			Item.useAnimation = 10; 
			Item.useStyle = ItemUseStyleID.Shoot;         
			Item.knockBack = 2;
			Item.rare = ItemRarityID.Blue;  
			Item.value = Item.buyPrice(gold: 1);
			Item.UseSound = SoundID.Item34;
			Item.shoot = ModContent.ProjectileType<LeafProjGreen>();
			Item.shootSpeed = 12f;
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-2, 0);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 45f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

			Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(8));

			int[] Types = new int[] { ModContent.ProjectileType<LeafProjGreen>(), ModContent.ProjectileType<LeafProjRed>(), ModContent.ProjectileType<LeafProjOrange>() };
			Projectile.NewProjectile(source, position.X, position.Y, newVelocity.X, newVelocity.Y, Main.rand.Next(Types), damage, knockback, player.whoAmI, 0f, 0f);
			
			return false;
		}
	}
}
