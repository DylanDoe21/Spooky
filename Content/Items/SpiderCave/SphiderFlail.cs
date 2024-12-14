using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System;

using Spooky.Content.Projectiles.SpiderCave;
 
namespace Spooky.Content.Items.SpiderCave
{
    public class SphiderFlail : ModItem
    {
        public override void SetDefaults() 
        {
			Item.damage = 35;
            Item.DamageType = DamageClass.Melee;
            Item.channel = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.width = 62;
            Item.height = 52;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 5;
            Item.rare = ItemRarityID.Blue;
           	Item.value = Item.buyPrice(gold: 3);
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<SphiderFlailProj>();
            Item.shootSpeed = 12f;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, new Vector2(velocity.X, velocity.Y).RotatedByRandom((float)Math.PI / 16f), 
            ModContent.ProjectileType<SphiderFlailProj>(), damage, knockback, player.whoAmI);

            return false;
        }
    }
}