using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Projectiles.Sentient;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientGatligator : ModItem, ICauldronOutput
    {
        public override void SetDefaults()
        {
            Item.damage = 35;
			Item.DamageType = DamageClass.Ranged;
			Item.autoReuse = true;
            Item.noMelee = true;
            Item.width = 72;
            Item.height = 32;
            Item.useTime = 5;
			Item.useAnimation = 5;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 1;
            Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 20);
            Item.UseSound = SoundID.Item31;
            Item.shoot = ProjectileID.PurificationPowder;
			Item.useAmmo = AmmoID.Bullet;
			Item.shootSpeed = 20f;
        }

        public override Vector2? HoldoutOffset()
		{
			return new Vector2(-3, 0);
		}

        public override bool CanConsumeAmmo(Item ammo, Player player)
		{
			return Main.rand.Next(3) != 0;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 50f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

            Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(40));

            if (Main.rand.NextBool(10))
            {
                SoundEngine.PlaySound(SoundID.Item171, player.Center);

                type = ModContent.ProjectileType<SentientGatligatorGiblet>();
            }

            Projectile.NewProjectile(source, position.X, position.Y, newVelocity.X + Main.rand.Next(-1, 2),
            newVelocity.Y + Main.rand.Next(-1, 2), type, damage, knockback, player.whoAmI, 0f, 0f);

            return false;
        }
    }
}