using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles;
using Spooky.Content.Projectiles.SpookyBiome;
 
namespace Spooky.Content.Items.SpookyBiome
{
    public class NecromancyTome : ModItem
    {
		public override void SetDefaults()
        {
            Item.damage = 20;
			Item.mana = 8;                        
            Item.DamageType = DamageClass.Magic;
			Item.noMelee = true;  
			Item.autoReuse = true;                  
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 28;
            Item.useAnimation = 28;
            Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 2;
            Item.rare = ItemRarityID.Blue;         
			Item.value = Item.buyPrice(gold: 1); 
            Item.UseSound = SoundID.NPCHit1;
            Item.shoot = ModContent.ProjectileType<Blank>();
			Item.shootSpeed = 6.5f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
            int[] Projectiles = { ModContent.ProjectileType<ZombiePart1>(), ModContent.ProjectileType<ZombiePart2>(), ModContent.ProjectileType<ZombiePart3>() };

			Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, Projectiles[Main.rand.Next(3)], damage, knockback, player.whoAmI, 0f, 0f);
			
			return true;
		}
	}
}