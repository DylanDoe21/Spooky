using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.SpookyBiome;
 
namespace Spooky.Content.Items.SpookyBiome
{
	public class LeafBlower : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Pump-Action Leaf Blower");
			Tooltip.SetDefault("Creates piercing leaf tornados"
			+ "\n'It's like a leaf blower, but it creates more leaves!'");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 10;    
			Item.DamageType = DamageClass.Ranged;  
			Item.autoReuse = true;       
			Item.width = 60;           
			Item.height = 26;         
			Item.useTime = 25;         
			Item.useAnimation = 25;        
			Item.useStyle = 5;          
			Item.knockBack = 3;
			Item.rare = 1;  
			Item.value = Item.buyPrice(gold: 1);
			Item.UseSound = SoundID.Item34;
			Item.shoot = ModContent.ProjectileType<Blank>();
			Item.shootSpeed = 2.5f;
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-2, 0);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 70f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

			Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<LeafTornado>(), damage, knockback, player.whoAmI, 0f, 0f);
			
			return true;
		}
	}
}
