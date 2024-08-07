using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Items.SpookyHell
{
	public class SnotArrow : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 12;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 14;
			Item.height = 40;
			Item.maxStack = 9999;
			Item.consumable = true;
			Item.knockBack = 3;
			Item.value = Item.buyPrice(silver: 5);
			Item.rare = ItemRarityID.Blue;
			//Item.shoot = ModContent.ProjectileType<SnotArrowProj>();
			//Item.shootSpeed = 3.5f;
			Item.ammo = AmmoID.Arrow;
		}
	}
}