using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome
{
    public class SpookFishronGun : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 55;
			Item.mana = 12;
			Item.DamageType = DamageClass.Magic;
			Item.autoReuse = true;
			Item.noMelee = true;
            Item.width = 70;
            Item.height = 20;
            Item.useTime = 4;         
			Item.useAnimation = 8;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 1;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.buyPrice(gold: 25);
            Item.UseSound = SoundID.Item34;
			Item.shoot = ModContent.ProjectileType<SpookFishronGunFire>();
			Item.shootSpeed = 5f;
        }

        public override Vector2? HoldoutOffset()
		{
			return new Vector2(-22, 0);
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 45f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

			Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(5));

			Projectile.NewProjectile(source, position.X, position.Y, newVelocity.X, newVelocity.Y, type, damage, knockback, player.whoAmI);
			
			return false;
		}
    }
}