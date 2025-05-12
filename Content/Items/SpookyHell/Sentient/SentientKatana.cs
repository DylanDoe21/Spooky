using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Projectiles.Sentient;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientKatana : ModItem, ICauldronOutput
    {
        public override void SetDefaults()
        {
            Item.damage = 35;
			Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
			Item.autoReuse = true;
            Item.width = 50;
            Item.height = 64;
            Item.useTime = 18;
			Item.useAnimation = 18;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 2;
            Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 20);
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<SentientKatanaSwingSlash>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			int Slash = Projectile.NewProjectile(source, player.MountedCenter, new Vector2(player.direction, 0f), ModContent.ProjectileType<SentientKatanaSwingSlash>(), damage, knockback, player.whoAmI, player.direction * player.gravDir, player.itemAnimationMax);
            Main.projectile[Slash].scale *= Item.scale * (player.meleeScaleGlove ? 1.1f : 1f);

            return false;
		}
    }
}