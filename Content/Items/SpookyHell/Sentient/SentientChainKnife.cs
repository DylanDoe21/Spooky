using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Projectiles.Sentient;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientChainKnife : ModItem, ICauldronOutput
    {
        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.crit = 15;
			Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.noUseGraphic = true;
			Item.autoReuse = true;
            Item.width = 42;
            Item.height = 48;
            Item.useTime = 15;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 1;
            Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 10);
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<SentientChainKnifeProj>();
            Item.shootSpeed = 25f;
        }

        public override bool CanUseItem(Player player)
		{
			if (player.ownedProjectileCounts[Item.shoot] > 0) 
			{
				return false;
			}

			return true;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 45f;
            position += muzzleOffset;

			Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
			
			return false;
		}
    }
}