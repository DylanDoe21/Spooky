using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell.Boss
{
    public class EyeFlail : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chained Eyes");
            Tooltip.SetDefault("Shoots out two long range eye flails"
            + "\nThe smaller eye flail travels further but deals less damage"
            + "\nThe bigger eye flail travels less far, but deals more damage");
			SacrificeTotal = 1;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() 
        {
			Item.damage = 85;
            Item.DamageType = DamageClass.Melee;
            Item.channel = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.width = 50;
            Item.height = 64;
            Item.useTime = 24;
            Item.useAnimation = 6;
            Item.useStyle = 5;
            Item.knockBack = 5;
            Item.rare = 5;
           	Item.value = Item.buyPrice(gold: 2);
            Item.UseSound = SoundID.Item10;
            Item.shoot = ModContent.ProjectileType<EyeFlailProj>();
            Item.shootSpeed = 12f;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, new Vector2(velocity.X, velocity.Y).RotatedByRandom((float)Math.PI / 16f), 
            ModContent.ProjectileType<EyeFlailProj>(), damage, knockback, player.whoAmI);
            Projectile.NewProjectile(source, position, new Vector2(velocity.X, velocity.Y).RotatedByRandom((float)Math.PI / 16f), 
            ModContent.ProjectileType<EyeFlailProj2>(), damage, knockback, player.whoAmI);

            return false;
        }
	}
}