using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.SpiderCave;
 
namespace Spooky.Content.Items.SpiderCave
{
	public class SpiderEggCannon : ModItem
	{
		int numUses = -1;

		public override void SetDefaults()
		{
			Item.damage = 25;
			Item.DamageType = DamageClass.Ranged;  
			Item.noMelee = true;
			Item.autoReuse = true;    
			Item.width = 52;           
			Item.height = 28;         
			Item.useTime = 35;      
			Item.useAnimation = 35;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 1;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(gold: 2);
			Item.UseSound = SoundID.Item97;
			Item.shoot = ModContent.ProjectileType<CannonEggSmall>();
			Item.shootSpeed = 8f;
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-5, 0);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			numUses++;

            if (numUses > 4)
            {
                numUses = 0;
            }

			type = numUses == 4 ? ModContent.ProjectileType<CannonEggBig>() : ModContent.ProjectileType<CannonEggSmall>();

			Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 45f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

			Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI);

			return false;
		}
	}
}
