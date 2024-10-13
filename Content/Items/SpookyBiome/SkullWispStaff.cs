using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Buffs.Minion;
using Spooky.Content.Projectiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome
{
	public class SkullWispStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.staff[Item.type] = true;
		}

		public override void SetDefaults()
		{
			Item.damage = 8;
			Item.mana = 12;          
			Item.DamageType = DamageClass.Summon;
			Item.noMelee = true;
			Item.autoReuse = true;       
			Item.width = 14;           
			Item.height = 32;         
			Item.useTime = 30;         
			Item.useAnimation = 30;         
			Item.useStyle = ItemUseStyleID.Shoot;          
			Item.knockBack = 1;
			Item.rare = ItemRarityID.Blue;  
			Item.value = Item.buyPrice(gold: 1);
			Item.UseSound = SoundID.Item103;     
			Item.buffType = ModContent.BuffType<SkullWispBuff>();
			Item.shoot = ModContent.ProjectileType<SkullWisp>();
			Item.shootSpeed = 5f;
		}

		public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
				player.MinionNPCTargetAim(true);
			}
			
            return base.UseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
            player.AddBuff(Item.buffType, 2);

			var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
			projectile.originalDamage = Item.damage;

			return false;
		}
	}
}
