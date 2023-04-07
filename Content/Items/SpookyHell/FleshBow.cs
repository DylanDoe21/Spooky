using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell
{
	public class FleshBow : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 25;
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
			Item.value = Item.buyPrice(gold: 3);
			Item.UseSound = SoundID.Item17;
			Item.shoot = ProjectileID.WoodenArrowFriendly;
			Item.useAmmo = AmmoID.Arrow;
			Item.shootSpeed = 12f;
		}

		int numUses = 0;

		public override void HoldItem(Player player)
		{
			if (player == Main.LocalPlayer)
			{
				if (!player.channel || player.itemAnimation > Item.useTime)
				{
					player.itemTime = 0;
					player.itemAnimation = 0;

					if (Main.netMode != NetmodeID.SinglePlayer)
					{
						NetMessage.SendData(MessageID.ShotAnimationAndSound, -1, -1, null, player.whoAmI);
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

				if (player.itemAnimation == 1)
				{
					int type = ModContent.ProjectileType<BloodyTear>();

					if (numUses >= 10)
                    {
						type = ModContent.ProjectileType<BowEye>();
						numUses = 0;
					}

					Vector2 shootDir = Vector2.UnitX.RotatedBy(player.AngleTo(Main.MouseWorld));

					Projectile.NewProjectile(Item.GetSource_ItemUse(Item), player.MountedCenter + (shootDir * 20), 
					shootDir * Item.shootSpeed, type, Item.damage, Item.knockBack, player.whoAmI);

					numUses++;
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