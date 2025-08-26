using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.Minibiomes.Christmas;

namespace Spooky.Content.Items.Minibiomes.Christmas
{
    public class CursedDoll : ModItem
    {
        public override void SetDefaults()
        {
			Item.mana = 15;
			Item.noMelee = true;
			Item.useTurn = false;
			Item.autoReuse = false;
			Item.noUseGraphic = true;
			Item.channel = true;
			Item.width = 52;
            Item.height = 56;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 0;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(gold: 1);
			Item.UseSound = SoundID.Item117;
			Item.shoot = ModContent.ProjectileType<CursedDollProj>();
			Item.shootSpeed = 0f;
        }
		
		public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[ModContent.ProjectileType<CursedDollProj>()] < 1;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, position.X, position.Y, 0, 0, ModContent.ProjectileType<CursedDollProj>(), damage, knockback, player.whoAmI);

			return false;
		}
    }
}