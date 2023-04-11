using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Projectiles.Catacomb;

namespace Spooky.Content.Items.Catacomb
{
	public class BigBoneHammer : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 250; 
			Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.width = 82;           
			Item.height = 76;
			Item.useTime = 20;
			Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 12;
			Item.rare = ItemRarityID.Yellow;  
			Item.value = Item.buyPrice(gold: 25);
            Item.UseSound = SoundID.DD2_MonkStaffSwing;
            Item.shoot = ModContent.ProjectileType<BigBoneHammerProj>();
            Item.shootSpeed = 35f;
        }

		public override bool AltFunctionUse(Player player)
		{
			return true;
		}

		public override bool CanUseItem(Player player)
		{
			for (int i = 0; i < 1000; i++)
			{
				if (Main.projectile[i].active && (Main.projectile[i].type == ModContent.ProjectileType<BigBoneHammerProj>() ||
				Main.projectile[i].type == ModContent.ProjectileType<BigBoneHammerSwung>() || Main.projectile[i].type == ModContent.ProjectileType<BigBoneHammerProj2>()))
				{
					return false;
				}
			}

			return true;
		}

		public override void UseAnimation(Player player)
		{
			if (player.altFunctionUse == 2)
			{
                Item.shoot = ModContent.ProjectileType<BigBoneHammerSwung>();
                Item.shootSpeed = 10f;
            }
			else
			{
                Item.shoot = ModContent.ProjectileType<BigBoneHammerProj>();
				Item.shootSpeed = 65f;
			}
		}
    }
}
