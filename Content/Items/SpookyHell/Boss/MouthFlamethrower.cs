using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.SpookyBiome;
using Spooky.Content.Projectiles.SpookyHell;
 
namespace Spooky.Content.Items.SpookyHell.Boss
{
	public class MouthFlamethrower : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Acid Blaster");
			Tooltip.SetDefault("Creates a continuous stream of acidic flames");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 50;       
			Item.mana = 2;    
			Item.DamageType = DamageClass.Magic;  
			Item.autoReuse = true;       
			Item.width = 70;           
			Item.height = 28;         
			Item.useTime = 4;         
			Item.useAnimation = 8;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 2;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 2);
			Item.UseSound = SoundID.Item34;
			Item.shoot = ModContent.ProjectileType<Blank>();
			Item.shootSpeed = 5f;
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-5, 0);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 25f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

			//shoot in a slightly random spread
			Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedByRandom(MathHelper.ToRadians(10));
			velocity.X = perturbedSpeed.X;
			velocity.Y = perturbedSpeed.Y;

			Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<AcidFlame>(), damage, knockback, player.whoAmI, 0f, 0f);
			
			return true;
		}
	}
}
