using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.Sentient;
using Spooky.Content.Tiles.SpookyHell.Furniture;
using static Humanizer.In;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientBloodRainBow : ModItem, ICauldronOutput
    {
        public override void SetDefaults()
        {
            Item.damage = 30;
			Item.DamageType = DamageClass.Ranged;
			Item.autoReuse = true;
            Item.width = 48;
            Item.height = 76;
            Item.useTime = 35;
			Item.useAnimation = 35;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 1;
            Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 20);
            Item.UseSound = SoundID.Item171;
            Item.shoot = ProjectileID.PurificationPowder;
			Item.useAmmo = AmmoID.Arrow;
			Item.shootSpeed = 0f;
        }

        public override Vector2? HoldoutOffset()
		{
			return new Vector2(-5, 0);
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.itemRotation = MathHelper.PiOver4 * -player.direction;

            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 50f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

            Vector2 newPosition = new Vector2(player.direction == -1 ? player.Center.X - 25 : player.Center.X + 25, player.Center.Y);

            for (int numProjs = 0; numProjs <= 4; numProjs++)
            {
                Projectile.NewProjectile(source, newPosition.X + Main.rand.Next(-25, 25), newPosition.Y + Main.rand.Next(-25, 25), 
                velocity.X, velocity.Y, ModContent.ProjectileType<BloodTooth>(), damage, knockback, player.whoAmI, 0f, 0f);
            }

            return false;
        }
    }
}