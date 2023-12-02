using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System;

using Spooky.Content.Projectiles.Sentient;
using Spooky.Content.Tiles.SpookyHell.Furniture;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientKatana : ModItem, ICauldronOutput
    {
        public override void SetDefaults()
        {
            Item.damage = 30;
			Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
			Item.autoReuse = true;
            Item.width = 50;
            Item.height = 64;
            Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 2;
            Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 15);
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<SentientKatanaSwingSlash>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			int Slash = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<SentientKatanaSwingSlash>(), damage, knockback, player.whoAmI, player.direction * player.gravDir, player.itemAnimationMax);
            Main.projectile[Slash].scale *= Item.scale;

            return false;
		}
    }
}