using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles;
using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell
{
	public class BoogerBlaster : ModItem
	{
		public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<BoogerBook>();
        }

		public override void SetDefaults()
		{
			Item.damage = 45;
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.autoReuse = false;
			Item.noUseGraphic = true;
			Item.channel = true;
			Item.width = 64;          
			Item.height = 32;
			Item.useTime = 45;
			Item.useAnimation = 45;
			Item.knockBack = 2;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 8;
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.buyPrice(gold: 15);
			Item.UseSound = SoundID.Item95;
			Item.shoot = ModContent.ProjectileType<BoogerBlasterProj>();
			Item.shootSpeed = 0f;
		}

		public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[ModContent.ProjectileType<BoogerBlasterProj>()] < 1;
		}
		
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, position.X, position.Y, 0, 0, ModContent.ProjectileType<BoogerBlasterProj>(), damage, knockback, player.whoAmI, 0f, 0f);

			return false;
		}
	}
}
