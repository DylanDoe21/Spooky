using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

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
			Item.value = Item.sellPrice(0, 0, 1, 0);
			Item.rare = ItemRarityID.Blue;
			//Item.shoot = ModContent.ProjectileType<SnotBulletProj>();
			//Item.shootSpeed = 3.5f;
			Item.ammo = AmmoID.Bullet;
		}
	}
}