using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Projectiles.Sentient;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientPaintballGun : ModItem, ICauldronOutput
    {
        int useTimer = 0;

        public override void SetDefaults()
        {
            Item.damage = 20;
			Item.DamageType = DamageClass.Ranged;
            Item.noUseGraphic = true;
			Item.autoReuse = true;
            Item.noMelee = true;
            Item.channel = true;
            Item.width = 38;
            Item.height = 28;
            Item.useTime = 18;
			Item.useAnimation = 18;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 0;
            Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 10);
            Item.UseSound = SoundID.Item111 with { Pitch = 0.5f};
            Item.shoot = ModContent.ProjectileType<SentientPaintballGunProj>();
            Item.shootSpeed = 4f;
        }

        public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[ModContent.ProjectileType<SentientPaintballGunProj>()] < 1;
		}
		
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, position.X, position.Y, 0, 0, ModContent.ProjectileType<SentientPaintballGunProj>(), damage, knockback, player.whoAmI);

			return false;
		}
    }
}