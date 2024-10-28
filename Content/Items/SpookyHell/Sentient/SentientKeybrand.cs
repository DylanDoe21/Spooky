using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Projectiles.Sentient;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientKeybrand : ModItem, ICauldronOutput
    {
		int numUses = -1;

        public override void SetDefaults()
        {
            Item.damage = 180;
			Item.DamageType = DamageClass.Melee;
			Item.noUseGraphic = true;
			Item.autoReuse = true;
			Item.noMelee = true;
			Item.channel = true;
			Item.width = 62;
			Item.height = 56;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 8;
            Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 30);
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<SentientKeybrandProj>();
            Item.shootSpeed = 12f;
        }

		public override bool MeleePrefix() 
		{
			return true;
		}

		public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[ModContent.ProjectileType<SentientKeybrandThrown>()] <= 0;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			numUses++;

			type = numUses == 2 ? ModContent.ProjectileType<SentientKeybrandThrown>() : ModContent.ProjectileType<SentientKeybrandProj>();

			if (numUses > 2)
            {
                numUses = 0;
            }

			if (numUses < 2)
			{
				Projectile.NewProjectileDirect(source, position + (velocity * 20) + (velocity.RotatedBy(-1.57f * player.direction) * 20), Vector2.Zero, type, damage, knockback, player.whoAmI, numUses == 0 ? 0 : 1);
			}
			else
			{
				Projectile.NewProjectile(source, position, velocity, type, damage / 2, knockback, player.whoAmI);
			}
			
			return false;
		}
	}
}