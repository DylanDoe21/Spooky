using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell
{
    public class LivingFleshBow : ModItem
    {
		int numUses = 0;
        
		public override void SetDefaults()
        {
			Item.damage = 80;
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.autoReuse = true;
			Item.width = 44;
			Item.height = 90;
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.knockBack = 2;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 8;
			Item.rare = ItemRarityID.LightPurple;
			Item.value = Item.buyPrice(gold: 15);
			Item.UseSound = SoundID.Item17;
			Item.shoot = ModContent.ProjectileType<FleshBowChunk1>();
			Item.useAmmo = AmmoID.Arrow;
			Item.shootSpeed = 15f;
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-5, 0);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 35f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

            if (numUses >= 20)
			{
				for (int numProjectiles = -1; numProjectiles <= 1; numProjectiles++)
				{
					Projectile.NewProjectile(Item.GetSource_FromThis(), player.Center,
					Item.shootSpeed * player.DirectionTo(position).RotatedBy(MathHelper.ToRadians(6) * numProjectiles),
					ModContent.ProjectileType<BowEye>(), Item.damage, Item.knockBack, player.whoAmI);
				}

				numUses = 0;
			}
			else
			{
				int[] Types = new int[] { ModContent.ProjectileType<FleshBowChunk1>(), ModContent.ProjectileType<FleshBowChunk2>() };

				int TypeToShoot = -1;
				player.PickAmmo(Item, out TypeToShoot, out _, out _, out _, out _);

				bool ConvertAmmo = TypeToShoot == ProjectileID.WoodenArrowFriendly;

				Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ConvertAmmo ? Main.rand.Next(Types) : TypeToShoot, damage, knockback, player.whoAmI, 0f, 0f);
			}
			
			numUses++;
			
			return false;
        }

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<FleshBow>(), 1)
			.AddIngredient(ModContent.ItemType<ArteryPiece>(), 15)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
}