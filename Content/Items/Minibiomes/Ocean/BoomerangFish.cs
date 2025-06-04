using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Projectiles.Minibiomes.Ocean;
 
namespace Spooky.Content.Items.Minibiomes.Ocean
{
    public class BoomerangFish : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.DamageType = DamageClass.Melee;
            Item.useTurn = true;    
            Item.noUseGraphic = true; 
            Item.noMelee = true;
            Item.autoReuse = true;             
            Item.width = 30;
            Item.height = 34;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 2;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 1, silver: 50);
            Item.UseSound = SoundID.Item1; 
            Item.shoot = ModContent.ProjectileType<BoomerangFishProj>();  
            Item.shootSpeed = 15f;
        }

        public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[Item.shoot] < 3;
		}
    }
}