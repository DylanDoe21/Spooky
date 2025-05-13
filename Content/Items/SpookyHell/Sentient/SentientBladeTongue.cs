using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Projectiles.Sentient;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientBladeTongue : ModItem, ICauldronOutput
    {
        public override void SetDefaults()
        {
            Item.damage = 75;
			Item.DamageType = DamageClass.Melee;
			Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = false;
            Item.width = 92;
            Item.height = 86;
            Item.useTime = 28;
			Item.useAnimation = 28;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 5;
            Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 30);
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<TonguebladeSlash>();
			Item.shootSpeed = 0f;
        }

        public override bool AltFunctionUse(Player player)
		{
			return true;
		}

        public override bool CanUseItem(Player player)
		{
			if (player.ownedProjectileCounts[ModContent.ProjectileType<SentientBladeTongueProj>()] > 0) 
			{
                return false;
            }

			return true;
		}

        public override void UseAnimation(Player player)
		{
			if (player.altFunctionUse == 2)
			{
                Item.noUseGraphic = true;
                Item.shootSpeed = 0f;
            }
			else
			{
                Item.noUseGraphic = false;
				Item.shootSpeed = 0f;
			}
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
            if (player.altFunctionUse != 2)
			{
			    int Slash = Projectile.NewProjectile(source, player.MountedCenter, new Vector2(player.direction, 0f), ModContent.ProjectileType<TonguebladeSlash>(), damage, knockback, player.whoAmI, player.direction * player.gravDir, player.itemAnimationMax);
                Main.projectile[Slash].scale *= Item.scale * (player.meleeScaleGlove ? 1.1f : 1f);
            }
            else
            {
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<SentientBladeTongueProj>(), damage, knockback, player.whoAmI, player.direction * player.gravDir, player.itemAnimationMax);
            }

            return false;
		}
    }
}