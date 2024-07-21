using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell
{
	public class SnotBullet : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 8;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 12;
			Item.height = 12;
			Item.maxStack = 9999;
			Item.consumable = true;
			Item.knockBack = 3;
			Item.value = Item.buyPrice(silver: 5);
			Item.rare = ItemRarityID.Blue;
			Item.ammo = AmmoID.Bullet;
			Item.shoot = ModContent.ProjectileType<SnotBulletProj>();
			Item.shootSpeed = 20f;
		}
	}
}