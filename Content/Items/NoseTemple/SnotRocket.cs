using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Items.NoseTemple
{
	public class SnotRocket : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 50;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 30;
			Item.height = 22;
			Item.maxStack = 9999;
			Item.consumable = true;
			Item.knockBack = 3;
			Item.value = Item.sellPrice(0, 0, 1, 0);
			Item.rare = ItemRarityID.Blue;
			//Item.shoot = ModContent.ProjectileType<SnotRocketProj>();
			//Item.shootSpeed = 3.5f;
			Item.ammo = AmmoID.Rocket;
		}
	}
}