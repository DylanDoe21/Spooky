using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Buffs.Minion;
using Spooky.Content.Projectiles.Catacomb;

namespace Spooky.Content.Items.Catacomb
{
	public class BigBoneScepter : ModItem
	{
		public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<BigBoneHammer>();
        }

		public override void SetDefaults()
		{
			Item.damage = 90; 
			Item.mana = 50;
			Item.DamageType = DamageClass.Summon;
			Item.noMelee = true;
			Item.autoReuse = true;
			Item.width = 34;
			Item.height = 78;
			Item.useTime = 45;
			Item.useAnimation = 45;
			Item.useStyle = ItemUseStyleID.Shoot;         
			Item.knockBack = 5;
			Item.rare = ItemRarityID.Yellow;  
			Item.value = Item.buyPrice(gold: 25);
			Item.UseSound = SoundID.NPCHit36;
			Item.buffType = ModContent.BuffType<SoulSkullBuff>();
			Item.shoot = ModContent.ProjectileType<SoulSkullMinion>();
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-10, -15);
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
				Item.UseSound = SoundID.Item130;
				Item.buffType = ModContent.BuffType<SoulSkullBuff>();
				Item.shoot = ModContent.ProjectileType<SkullTotem>();
				Item.shootSpeed = 0f;
			}
			else
			{
				Item.autoReuse = true;
				Item.UseSound = SoundID.NPCHit36;
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
			if (player.altFunctionUse == 2)
			{
				foreach (var Proj in Main.ActiveProjectiles)
				{
                    if (Proj.owner == player.whoAmI && Proj.type == ModContent.ProjectileType<SkullTotem>()) 
                    {
						Proj.Kill();
					}
				}

				var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, 0, knockback, player.whoAmI);
			}
			else
			{
				player.AddBuff(Item.buffType, 2);

				var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
				projectile.originalDamage = Item.damage;
			}

			return false;
		}
	}
}
