using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Content.Buffs.Minion;
using Spooky.Content.Projectiles.SpiderCave;
 
namespace Spooky.Content.Items.SpiderCave
{
	public class SpiderWarRemote : ModItem
	{
		public static readonly SoundStyle BeepSound = new("Spooky/Content/Sounds/EMFNoGhost", SoundType.Sound) { Volume = 0.5f, Pitch = -0.5f };

		public override void SetDefaults()
		{
			Item.damage = 200;
			Item.mana = 20;
			Item.DamageType = DamageClass.Summon;
			Item.noMelee = true;
			Item.autoReuse = true;
			Item.width = 20;          
			Item.height = 46;
			Item.useTime = 25;
			Item.useAnimation = 25;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.knockBack = 1;
			Item.rare = ItemRarityID.Yellow;
            Item.value = Item.buyPrice(gold: 50);
			Item.UseSound = BeepSound;
			Item.shoot = ModContent.ProjectileType<SpiderSummonEggProj>();
			Item.shootSpeed = 13f;
		}

		public override bool AltFunctionUse(Player player)
		{
			return true;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Vector2 muzzleOffset = Vector2.Normalize(velocity) * 65f;

			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
			{
				position += muzzleOffset;
			}

			//right click
			if (player.altFunctionUse == 2)
			{
				player.AddBuff(ModContent.BuffType<SpiderWarRemoteNukeBuff>(), 2);
				Projectile.NewProjectile(source, new Vector2(player.Center.X, player.Center.Y - 700), Vector2.Zero, ModContent.ProjectileType<SpiderWarRemoteNuke>(), damage, knockback, player.whoAmI);
			}
			//left click spawns little joro flies in a bomb
			else
			{
				Projectile.NewProjectile(source, new Vector2(player.Center.X, player.Center.Y - 700), Vector2.Zero, ModContent.ProjectileType<SpiderWarRemoteEgg>(), damage / 10, knockback, player.whoAmI);
			}

			return false;
		}
	}
}
