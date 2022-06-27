using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.SpookyBiome;
 
namespace Spooky.Content.Items.SpookyBiome
{
    public class NecromancyTome : ModItem
    {
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Necromancy for Dummies");
			Tooltip.SetDefault("Conjures various zombie parts that bounce everywhere"
			+ "\n'Not quite strong enough to summon full zombies'");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
        {
            Item.damage = 12; 
			Item.mana = 10;                        
            Item.DamageType = DamageClass.Magic;
			Item.noMelee = true;  
			Item.autoReuse = true;                  
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 35;
            Item.useAnimation = 35;
            Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 2;
            Item.rare = ItemRarityID.Blue;         
			Item.value = Item.buyPrice(gold: 1); 
            Item.UseSound = SoundID.NPCHit1;
            Item.shoot = ModContent.ProjectileType<Blank>();
			Item.shootSpeed = 5f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
            int[] Projectiles = { ModContent.ProjectileType<ZombiePart1>(), ModContent.ProjectileType<ZombiePart2>(), ModContent.ProjectileType<ZombiePart3>() };

			Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, Projectiles[Main.rand.Next(3)], damage, knockback, player.whoAmI, 0f, 0f);
			
			return true;
		}
	}
}