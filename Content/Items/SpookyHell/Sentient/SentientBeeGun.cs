using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Projectiles.Sentient;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientBeeGun : ModItem, ICauldronOutput
    {
        public override void SetDefaults()
        {
            Item.damage = 50;
            Item.mana = 10;
			Item.DamageType = DamageClass.Magic;
			Item.autoReuse = true;
            Item.noMelee = true;
            Item.width = 46;
            Item.height = 32;
            Item.useTime = 22;
			Item.useAnimation = 22;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 0;
            Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 10);
            Item.UseSound = SoundID.Item111 with { Pitch = -0.5f};
            Item.shoot = ModContent.ProjectileType<EyeTadpole>();
            Item.shootSpeed = 3f;
        }

        public override Vector2? HoldoutOffset()
		{
			return new Vector2(-2, 0);
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 35f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

            Vector2 newVelocity = new Vector2(velocity.X * Main.rand.NextFloat(1f, 1.5f), velocity.Y * Main.rand.NextFloat(1f, 1.5f)).RotatedByRandom(MathHelper.ToRadians(18));

            Projectile.NewProjectile(source, position, newVelocity, type, damage, knockback, player.whoAmI);

			return false;
		}
    }
}