using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Projectiles.Sentient;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientMagnetSphere : ModItem, ICauldronOutput
    {
        public override void SetDefaults()
        {
            Item.damage = 80;
            Item.mana = 10;
			Item.DamageType = DamageClass.Magic;
            Item.noMelee = true;
            Item.channel = true;
            Item.useTurn = true;
			Item.width = 36;
			Item.height = 60;
            Item.useTime = 35;
			Item.useAnimation = 35;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 0;
            Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 60);
            Item.UseSound = SoundID.Item81;
            Item.shoot = ModContent.ProjectileType<EnergyHeart>();
            Item.shootSpeed = 0f;
        }

        public override Vector2? HoldoutOffset()
		{
			return new Vector2(-3, 0);
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, player.Center.X + (player.direction == -1 ? -20 : 20), player.Center.Y, 0, 0, type, damage, knockback, player.whoAmI, 0f, 0f);

			return false;
		}
    }
}