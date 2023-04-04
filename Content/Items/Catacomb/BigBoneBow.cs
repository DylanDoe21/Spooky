using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;
using System;

using Spooky.Content.Projectiles.SpookyBiome;
using Spooky.Content.Projectiles.Catacomb;
 
namespace Spooky.Content.Items.Catacomb
{
	public class BigBoneBow : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Flower Shot");
			/* Tooltip.SetDefault("Left click to rapidly fire piercing roses that can slightly home on enemies"
			+ "\nRight click to shoot a stationary thorn flower that inflicts thorn mark on enemies"
			+ "\nEnemies with the thorn mark debuff will take more damage from this weapon"
			+ "\n10% chance to save ammo"); */
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 100;    
			Item.DamageType = DamageClass.Ranged;  
			Item.noMelee = true;
			Item.autoReuse = true;       
			Item.width = 30;         
			Item.height = 72;        
			Item.useTime = 10;
			Item.useAnimation = 10;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 5;
			Item.rare = ItemRarityID.Yellow;  
			Item.value = Item.buyPrice(gold: 25);
			Item.UseSound = SoundID.Item5;
			Item.shoot = ProjectileID.PurificationPowder;
			Item.shootSpeed = 12f;
			Item.useAmmo = AmmoID.Arrow;
		}

        public override Vector2? HoldoutOffset()
		{
			return new Vector2(-5, 0);
		}

		public override bool AltFunctionUse(Player player)
		{
			for (int i = 0; i < 1000; i++)
			{
				if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<BowThornFlower>())
				{
					return false;
				}
			}

			return true;
		}

		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				Item.useTime = 50;
				Item.useAnimation = 50;
				Item.UseSound = SoundID.Item5;
			}
			else
			{
				Item.useTime = 10;
				Item.useAnimation = 10;
				Item.UseSound = SoundID.Item5;
			}

			return true;
		}

		public override bool CanConsumeAmmo(Item ammo, Player player)
		{
			return Main.rand.Next(10) != 0;
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) 
		{	
			if (player.altFunctionUse == 2)
			{
				type = ModContent.ProjectileType<BowThornFlower>();
			}
			else
			{
				type = ModContent.ProjectileType<BowFlower>();
			}
		}
	}
}
