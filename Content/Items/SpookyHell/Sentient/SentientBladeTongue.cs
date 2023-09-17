using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles;
using Spooky.Content.Projectiles.Sentient;
using Spooky.Content.Tiles.SpookyHell.Furniture;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientBladeTongue : ModItem, ICauldronOutput
    {
        public override void SetDefaults()
        {
            Item.damage = 70;
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
            Item.value = Item.buyPrice(gold: 15);
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<Blank>();
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
                Item.shoot = ModContent.ProjectileType<SentientBladeTongueProj>();
                Item.shootSpeed = 0f;
            }
			else
			{
                Item.noUseGraphic = false;
                Item.shoot = ModContent.ProjectileType<Blank>();
				Item.shootSpeed = 0f;
			}
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
            if (player.altFunctionUse != 2)
			{
			    Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<TonguebladeSlash>(), damage, knockback, player.whoAmI, player.direction * player.gravDir, player.itemAnimationMax);
            
                return false;
            }   

            return true;
		}
    }
}