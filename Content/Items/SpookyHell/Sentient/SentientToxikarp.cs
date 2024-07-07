using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Projectiles.Sentient;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientToxikarp : ModItem, ICauldronOutput
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 40;
			Item.DamageType = DamageClass.Ranged;
            Item.autoReuse = true;  
			Item.noMelee = true;
			Item.width = 56;      
			Item.height = 46;
            Item.useTime = 12;
			Item.useAnimation = 12;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 3;
            Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 25);
            Item.UseSound = SoundID.Item111;
            Item.shoot = ModContent.ProjectileType<ToxicBubble>();
            Item.shootSpeed = 12f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int[] Types = new int[] { ModContent.ProjectileType<ToxicBubbleRed>(), ModContent.ProjectileType<ToxicBubblePurple>(), ModContent.ProjectileType<ToxicBubbleBlue>() };

            if (Main.rand.NextBool(5))
            {
                type = Main.rand.Next(Types);
            }
            else
            {
                type = ModContent.ProjectileType<ToxicBubble>();
            }

            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 60f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

            Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(3));

            Projectile.NewProjectile(source, position.X, position.Y, newVelocity.X + Main.rand.Next(-1, 2),
            newVelocity.Y + Main.rand.Next(-1, 2), type, damage, knockback, player.whoAmI, 0f, 0f);

            return false;
        }
    }
}