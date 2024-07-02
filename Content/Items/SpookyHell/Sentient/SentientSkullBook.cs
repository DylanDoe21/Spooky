using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Projectiles;
using Spooky.Content.Projectiles.Sentient;
using Spooky.Content.Tiles.SpookyHell.Furniture;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientSkullBook : ModItem, ICauldronOutput
    {
        public override void SetDefaults()
        {
            Item.damage = 38;
            Item.mana = 10;
			Item.DamageType = DamageClass.Magic;
			Item.autoReuse = true;
            Item.noMelee = true;
            Item.width = 50;
            Item.height = 56;
            Item.useTime = 42;
			Item.useAnimation = 42;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 0;
            Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 8);
            Item.UseSound = SoundID.Item8;
            Item.shoot = ModContent.ProjectileType<SentientSkull>();
            Item.shootSpeed = 15f;
        }

        public override Vector2? HoldoutOffset()
		{
			return new Vector2(-4, 0);
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 20f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

            Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI, 0f, 0f);

            return false;
        }
    }
}