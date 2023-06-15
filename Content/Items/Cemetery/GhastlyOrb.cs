using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Projectiles.Cemetery;

namespace Spooky.Content.Items.Cemetery
{
	public class GhastlyOrb : ModItem
	{
		public override void SetStaticDefaults() 
        {
			Item.ResearchUnlockCount = 99;
		}

		public override void SetDefaults() 
        {
			Item.damage = 10;
			Item.DamageType = DamageClass.Ranged;
            Item.ammo = Item.type;
            Item.consumable = true;
            Item.width = 10;
			Item.height = 10;
			Item.knockBack = 2f;
            Item.maxStack = 9999;
			Item.value = Item.buyPrice(copper: 10);
			Item.rare = ItemRarityID.White; 
			Item.shoot = ModContent.ProjectileType<GhastlyOrbProj>();
		}
	}
}