using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Content.Projectiles.Minibiomes.Desert;

namespace Spooky.Content.Items.Minibiomes.Desert
{
    public class TarGun : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 12;
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;
			Item.autoReuse = true;
            Item.width = 56;
            Item.height = 18;
            Item.useTime = 10;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 2;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 2);
            Item.UseSound = SoundID.Item17 with { Pitch = -1f };
            Item.shoot = ModContent.ProjectileType<TarGunBlob>();
            Item.useAmmo = AmmoID.Bullet;
			Item.shootSpeed = 15f;
        }

        public override Vector2? HoldoutOffset()
		{
			return new Vector2(-10, 3);
		}
		
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 40f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

			Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<TarGunBlob>(), damage, knockback, player.whoAmI);

			return false;
		}
    }
}