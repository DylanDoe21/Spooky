using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Projectiles;
using Spooky.Content.Projectiles.SpookyHell;
using System.Collections.Generic;

namespace Spooky.Content.Items.SpookyHell
{
	public class FleshBow : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Seer");
			Tooltip.SetDefault("Converts arrows into bloody tears\nEvery 10 uses will shoot out a super charged eye");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 35;
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.autoReuse = true;
			Item.channel = true;
			Item.width = 24;
			Item.height = 66;
			Item.useTime = 40;
			Item.useAnimation = 40;
			Item.knockBack = 2;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 8;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(gold: 2);
			Item.UseSound = SoundID.Item17;
			Item.shoot = ProjectileID.WoodenArrowFriendly;
			Item.useAmmo = AmmoID.Arrow;
			Item.shootSpeed = 22f;
		}

		public override void HoldItem(Player player)
		{
			if (player == Main.LocalPlayer)
			{
				int fireTime = 3 * (Item.useAnimation / 4);

				if (!player.channel && player.itemAnimation > fireTime)
				{
					player.itemTime = 0;
					player.itemAnimation = 0;

					if (Main.netMode != NetmodeID.SinglePlayer)
					{
						NetMessage.SendData(MessageID.ItemAnimation, -1, -1, null, player.whoAmI);
					}

					return;
				}

				if (player.itemAnimation > 0)
				{
					player.ChangeDir(player.DirectionTo(Main.MouseWorld).X > 0 ? 1 : -1);

					player.itemRotation = MathHelper.WrapAngle(player.AngleTo(Main.MouseWorld) - ((player.direction < 0) ? MathHelper.Pi : 0)) - player.fullRotation;

					if (Main.netMode != NetmodeID.SinglePlayer)
					{
						NetMessage.SendData(MessageID.SyncPlayer, -1, -1, null, player.whoAmI);
					}
				}

				if (player.itemAnimation == fireTime)
				{
					int type = ProjectileID.BloodShot;

					Vector2 shootDir = Vector2.UnitX.RotatedBy(player.AngleTo(Main.MouseWorld));

					int newProjectile = Projectile.NewProjectile(Item.GetSource_ItemUse(Item), player.MountedCenter + (shootDir * 20), 
					shootDir * Item.shootSpeed, type, Item.damage, Item.knockBack, player.whoAmI);

					Main.projectile[newProjectile].friendly = true;
					Main.projectile[newProjectile].hostile = false;
				}
			}
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			return false;
        }

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(2, 0);
		}
	}
}