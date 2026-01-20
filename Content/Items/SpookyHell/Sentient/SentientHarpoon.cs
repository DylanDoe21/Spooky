using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Projectiles.Sentient;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientHarpoon : ModItem, ICauldronOutput
    {
        public override void SetDefaults()
        {
            Item.damage = 25;
			Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.autoReuse = true;
            Item.width = 42;
            Item.height = 48;
            Item.useTime = 15;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 0.5f;
            Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 10);
            Item.UseSound = SoundID.Item111 with { Pitch = 1.2f };
            Item.shoot = ModContent.ProjectileType<SentientHarpoonProj>();
            Item.shootSpeed = 5f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float AIValue = (Main.rand.NextFloat() - 0.75f) * 0.7853982f; //0.5
            Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI, 0f, AIValue);

            return false;
        }
    }
}