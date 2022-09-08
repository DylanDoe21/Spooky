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
	public class BigBoneHammer : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Skull Smasher");
			Tooltip.SetDefault("Left click to swing the hammer and create shockwave explosions on enemy hits"
			+ "\nHold down right click to swing the hammer around you and charge it up" 
			+ "\nOnce fully charged, releasing right click will throw the hammer");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 200; 
			Item.DamageType = DamageClass.Melee;
			Item.width = 82;           
			Item.height = 76;         
			Item.useTime = 45;
			Item.useAnimation = 45;
			Item.useStyle = ItemUseStyleID.Swing;         
			Item.knockBack = 10;
			Item.rare = ItemRarityID.Yellow;  
			Item.value = Item.buyPrice(gold: 10);
			Item.UseSound = SoundID.DD2_MonkStaffSwing;
			Item.shoot = ModContent.ProjectileType<Blank>();
			Item.shootSpeed = 0f;
		}

		public override bool AltFunctionUse(Player player)
		{
			return true;
		}

		public override bool CanUseItem(Player player)
		{
			for (int i = 0; i < 1000; i++)
			{
				if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<BigBoneHammerProj>())
				{
					return false;
				}
			}

			if (player.altFunctionUse == 2)
			{
				Item.noMelee = true;
				Item.noUseGraphic = true;
				Item.autoReuse = false;
				Item.channel = true;
				Item.UseSound = SoundID.DD2_MonkStaffSwing;
				Item.shoot = ModContent.ProjectileType<BigBoneHammerProj>();
				Item.shootSpeed = 10f;
			}
			else
			{
				Item.noMelee = false;
				Item.noUseGraphic = false;
				Item.autoReuse = true;
				Item.channel = false;
				Item.UseSound = SoundID.DD2_MonkStaffSwing;
				Item.shoot = ModContent.ProjectileType<Blank>();
				Item.shootSpeed = 0f;
			}

			return true;
		}
	}
}
