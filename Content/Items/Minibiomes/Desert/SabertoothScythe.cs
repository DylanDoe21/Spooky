using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.Minibiomes.Desert;

namespace Spooky.Content.Items.Minibiomes.Desert
{
    public class SabertoothScythe : ModItem
    {
        int numUses = -1;

        public override void SetDefaults()
        {
            Item.damage = 70;
			Item.DamageType = DamageClass.Melee;
			Item.noUseGraphic = true;
			Item.autoReuse = true;
			Item.noMelee = true;
			Item.channel = true;
            Item.width = 60;
            Item.height = 52;
            Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 2;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 15);
            Item.UseSound = SoundID.Item7;
            Item.shoot = ModContent.ProjectileType<SabertoothScytheProj>();
            Item.shootSpeed = 12f;
        }

        public override bool MeleePrefix() 
		{
			return true;
		}

        public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[ModContent.ProjectileType<SabertoothScytheProj>()] <= 0;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			numUses++;
			if (numUses > 1)
            {
                numUses = 0;
            }

			Projectile.NewProjectileDirect(source, position + (velocity * 20) + (velocity.RotatedBy(-1.57f * player.direction) * 20), Vector2.Zero, type, damage, knockback, player.whoAmI, numUses == 0 ? 0 : 1);
			
			return false;
		}
    }
}