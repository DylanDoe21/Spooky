using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;

using Spooky.Content.Buffs.Minion;
using Spooky.Content.Projectiles.Catacomb;
using Spooky.Content.Projectiles.SpookyBiome;

namespace Spooky.Content.Items.Catacomb
{
	public class BigBoneScepter : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Skull Totem Scepter");
			Tooltip.SetDefault("Left click to summon skull wisps that can shoot magic blasts and charge enemies"
			+ "\nRight click to summon a stationary skull idol that buffs your minion stats while inside of it's aura");
			Item.staff[Item.type] = true;
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 120; 
			Item.mana = 50;
			Item.DamageType = DamageClass.Summon;
			Item.width = 82;           
			Item.height = 76;         
			Item.useTime = 45;
			Item.useAnimation = 45;
			Item.useStyle = ItemUseStyleID.HoldUp;         
			Item.knockBack = 5;
			Item.rare = ItemRarityID.Yellow;  
			Item.value = Item.buyPrice(gold: 10);
			Item.UseSound = SoundID.DD2_MonkStaffSwing;
			Item.buffType = ModContent.BuffType<SoulSkullBuff>();
			Item.shoot = ModContent.ProjectileType<SoulSkullMinion>();
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-20, 20);
		}

		public override bool AltFunctionUse(Player player)
		{
			return true;
		}

		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				Item.autoReuse = false;
				Item.UseSound = SoundID.DD2_MonkStaffSwing;
				Item.buffType = ModContent.BuffType<SoulSkullBuff>();
				Item.shoot = ModContent.ProjectileType<SoulSkullMinion>();
				Item.shootSpeed = 0f;
			}
			else
			{
				Item.autoReuse = true;
				Item.UseSound = SoundID.DD2_MonkStaffSwing;
				Item.buffType = ModContent.BuffType<SoulSkullBuff>();
				Item.shoot = ModContent.ProjectileType<SoulSkullMinion>();
				Item.shootSpeed = 0f;
			}

			return true;
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) 
		{
			position = Main.MouseWorld;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (player.altFunctionUse != 2)
			{
				player.AddBuff(Item.buffType, 2);

				var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
				projectile.originalDamage = Item.damage;
			}

			return false;
		}
	}
}
