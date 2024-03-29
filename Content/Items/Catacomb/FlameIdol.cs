using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.Catacomb;

namespace Spooky.Content.Items.Catacomb
{
    public class FlameIdol : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 25;
			Item.mana = 10;
			Item.DamageType = DamageClass.Magic;
			Item.noMelee = true;
			Item.useTurn = false;
			Item.autoReuse = false;
			Item.noUseGraphic = true;
			Item.channel = true;
			Item.width = 40;
            Item.height = 50;
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 4;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(gold: 2);
			Item.UseSound = SoundID.Item117;
			Item.shoot = ModContent.ProjectileType<FlameIdolProj>();
			Item.shootSpeed = 0f;
        }

		public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[ModContent.ProjectileType<FlameIdolProj>()] < 1;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, position.X, position.Y, 0, 0, ModContent.ProjectileType<FlameIdolProj>(), damage, knockback, player.whoAmI, 0f, 0f);

			return false;
		}
    }
}