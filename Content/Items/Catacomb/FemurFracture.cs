using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Projectiles.Catacomb;

namespace Spooky.Content.Items.Catacomb
{
	public class FemurFracture : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 45;
			Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.width = 82;           
			Item.height = 76;
			Item.useTime = 20;
			Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(gold: 2);
            Item.UseSound = SoundID.DD2_MonkStaffSwing;
            Item.shoot = ModContent.ProjectileType<FemurFractureSwung>();
            Item.shootSpeed = 10f;
        }

		public override bool CanUseItem(Player player)
		{
			if (player.ownedProjectileCounts[ModContent.ProjectileType<FemurFractureSwung>()] > 0 ||
			player.ownedProjectileCounts[ModContent.ProjectileType<FemurFractureProj>()] > 0) 
			{
				return false;
			}

			return true;
		}
    }
}
