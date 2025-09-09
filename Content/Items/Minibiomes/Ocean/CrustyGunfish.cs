using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.Minibiomes.Ocean;

namespace Spooky.Content.Items.Minibiomes.Ocean
{
	public class CrustyGunfish : ModItem
	{
		int numUses = -1;

		public override void SetDefaults()
        {
			Item.damage = 42;
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.autoReuse = true;
			Item.width = 42;
			Item.height = 30;
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 4;
            Item.rare = ItemRarityID.Yellow;
			Item.value = Item.buyPrice(gold: 20);
			Item.UseSound = SoundID.Item95;
			Item.shoot = ModContent.ProjectileType<MudSplatter>();
			Item.useAmmo = AmmoID.Bullet;
			Item.shootSpeed = 12f;
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-5, 12);
		}

		public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[ModContent.ProjectileType<CrustyGunfishProj>()] < 1;
		}

		public override bool? UseItem(Player player)
		{
			if (numUses < 4)
			{
				Item.useStyle = ItemUseStyleID.Shoot;
				Item.noUseGraphic = false;
			}
			else
			{
				Item.useStyle = ItemUseStyleID.Swing;
				Item.noUseGraphic = true;
			}

			return base.UseItem(player);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			numUses++;
			if (numUses < 5)
            {
				for (int numProjs = 0; numProjs < 5; numProjs++)
				{
					Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * Main.rand.NextFloat(30f, 150f);
					if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
					{
						position += muzzleOffset;
					}

					Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(16));

					Projectile.NewProjectile(source, position.X, position.Y, newVelocity.X, newVelocity.Y, ModContent.ProjectileType<MudSplatter>(), damage, knockback, player.whoAmI);

					position -= muzzleOffset;
				}
			}
			else
			{
				Projectile.NewProjectile(source, position.X, position.Y, velocity.X * 2, velocity.Y * 2, ModContent.ProjectileType<CrustyGunfishProj>(), damage, knockback, player.whoAmI);

				numUses = 0;
			}
			
			return false;
		}

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<DunkleosteusHide>(), 10)
			.AddIngredient(ItemID.Bone, 25)
            .AddIngredient(ItemID.Ectoplasm, 15)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
	}
}
