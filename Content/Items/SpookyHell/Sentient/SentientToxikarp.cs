using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.Sentient;
using Spooky.Content.Tiles.SpookyHell.Furniture;

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
            Item.useTime = 15;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 3;
            Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 25);
            Item.UseSound = SoundID.Item95;
            Item.shoot = ModContent.ProjectileType<ToxicBubble>();
            Item.shootSpeed = 12f;
        }

        public override Vector2? HoldoutOffset()
		{
			return new Vector2(-10, 0);
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int[] Types = new int[] { ModContent.ProjectileType<ToxicBubble>(), ModContent.ProjectileType<ToxicBubbleBlood>(), 
            ModContent.ProjectileType<ToxicBubblePurple>() };

            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 60f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

            Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(3));

            Projectile.NewProjectile(source, position.X, position.Y, newVelocity.X + Main.rand.Next(-1, 2),
            newVelocity.Y + Main.rand.Next(-1, 2), Main.rand.Next(Types), damage, knockback, player.whoAmI, 0f, 0f);

            return false;
        }
    }
}