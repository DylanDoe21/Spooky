using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles;
using Spooky.Content.Projectiles.Cemetery;

namespace Spooky.Content.Items.BossSummon
{
    public class EMFReader : ModItem
    {
        public override void SetDefaults()
        {
			Item.width = 28;
            Item.height = 32;
			Item.noMelee = true;
			Item.autoReuse = false;
			Item.noUseGraphic = true;
			Item.channel = true;
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.rare = ItemRarityID.Blue;
			Item.shoot = ModContent.ProjectileType<Blank>();
			Item.shootSpeed = 0f;
        }

		/*
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (player.ownedProjectileCounts[ModContent.ProjectileType<EMFReaderProj>()] < 1)
            {
				Projectile.NewProjectile(source, position.X, position.Y, 0, 0, ModContent.ProjectileType<EMFReaderProj>(), damage, knockback, player.whoAmI, 0f, 0f);
			}

			return false;
		}
		*/
    }
}