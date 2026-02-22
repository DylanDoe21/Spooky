using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Items.SpookyBiome.Misc;
using Spooky.Content.Projectiles.Slingshots;

namespace Spooky.Content.Items.Slingshots.Ammo
{
	public class BouncyBall : ModItem
	{
		public override void SetStaticDefaults() 
        {
			ItemGlobal.IsSlingshotAmmo[Item.type] = true;
			Item.ResearchUnlockCount = 99;
		}

		public override void SetDefaults() 
        {
			Item.damage = 8;
			Item.DamageType = DamageClass.Ranged; 
            Item.ammo = Item.type;
            Item.consumable = true;
            Item.width = 14;
			Item.height = 14;
			Item.knockBack = 2f;
            Item.maxStack = 9999;
			Item.value = Item.buyPrice(copper: 35);
			Item.rare = ItemRarityID.Blue;
			Item.shoot = ModContent.ProjectileType<BouncyBallProj>();
		}

		public override bool? CanBeChosenAsAmmo(Item weapon, Player player)
		{
			return ItemGlobal.IsSlingshot[weapon.type] ? true : null;
		}
	}
}