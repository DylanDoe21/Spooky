using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Content.Items.SpiderCave.Misc;
using Spooky.Content.Projectiles.SpiderCave;
using Spooky.Content.Tiles.SpiderCave;
 
namespace Spooky.Content.Items.SpiderCave
{
	public class MiteGun : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 52;
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.autoReuse = true;
			Item.width = 56;
			Item.height = 32;
			Item.useTime = 16;
			Item.useAnimation = 16;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 3;
			Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 15);
			Item.UseSound = SoundID.Item11;
			Item.shoot = ProjectileID.PurificationPowder;
			Item.useAmmo = AmmoID.Bullet;
			Item.shootSpeed = 12f;
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-10, 4);
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) 
		{
			Vector2 muzzleOffset = Vector2.Normalize(velocity) * 25f;

			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0)) 
			{
				position += muzzleOffset;
			}
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Vector2 muzzleOffset = Vector2.Normalize(velocity) * 20f;

			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
			{
				position += muzzleOffset;
			}

			if (Main.rand.NextBool())
			{
				int newMite = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<MiteProjectile>(), damage / 2, knockback, player.whoAmI);
                Main.projectile[newMite].DamageType = DamageClass.Ranged;
                Main.projectile[newMite].ai[0] = Main.rand.NextBool() ? Main.rand.Next(0, 3) : 7;
                Main.projectile[newMite].ai[2] = 1;
				Main.projectile[newMite].penetrate = 4;

				return false;
			}

			return true;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<MiteMandibles>(), 25)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
	}
}
