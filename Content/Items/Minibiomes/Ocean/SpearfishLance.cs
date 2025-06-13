using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System;

using Spooky.Core;
using Spooky.Content.Projectiles.Minibiomes.Ocean;
 
namespace Spooky.Content.Items.Minibiomes.Ocean
{
    public class SpearfishLance : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 16;
            Item.DamageType = DamageClass.Melee;
            Item.autoReuse = true; 
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.width = 44;
            Item.height = 44;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 1, silver: 50);
            Item.UseSound = SoundID.Item7;
            Item.shoot = ModContent.ProjectileType<SpearfishLanceProj>();
            Item.shootSpeed = 12f;
        }

		public override bool AltFunctionUse(Player player)
		{
			return player.GetModPlayer<SpookyPlayer>().SpearfishChargeCooldown <= 0;
		}

		public override void UseAnimation(Player player)
		{
			if (player.altFunctionUse == 2)
			{
                Item.UseSound = SoundID.Item1 with { Pitch = -1f };
                Item.shoot = ModContent.ProjectileType<SpearfishLanceSlashProj>();
            }
			else
			{
                Item.UseSound = SoundID.Item7;
                Item.shoot = ModContent.ProjectileType<SpearfishLanceProj>();
			}
		}

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<SpearfishLanceProj>()] < 1 && player.ownedProjectileCounts[ModContent.ProjectileType<SpearfishLanceSlashProj>()] < 1;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
			{
                float num82 = (float)Main.mouseX + Main.screenPosition.X - position.X;
                float num83 = (float)Main.mouseY + Main.screenPosition.Y - position.Y;
                if (player.gravDir == -1f)
                {
                    num83 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - position.Y;
                }
                float num84 = (float)Math.Sqrt((double)(num82 * num82 + num83 * num83));
                if ((float.IsNaN(num82) && float.IsNaN(num83)) || (num82 == 0f && num83 == 0f))
                {
                    num82 = (float)player.direction;
                    num83 = 0f;
                    num84 = Item.shootSpeed;
                }
                else
                {
                    num84 = Item.shootSpeed / num84;
                }
                num82 *= num84;
                num83 *= num84;
                float ai4 = Main.rand.NextFloat() * Item.shootSpeed * 0.75f * (float)player.direction;
                velocity = new Vector2(num82, num83);
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ai4, 0.0f);
            }
            else
            {
                Projectile.NewProjectile(source, position, velocity, type, damage * 3, knockback, player.whoAmI);
            }

            return false;
        }
    }
}