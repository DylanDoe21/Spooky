using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell
{
	public class BrainJar : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 350;
			Item.mana = 60;
            Item.DamageType = DamageClass.Summon;
            Item.noMelee = true;
            Item.noUseGraphic = true;
			Item.autoReuse = true;       
            Item.width = 22;
            Item.height = 34;
            Item.useTime = 45;       
            Item.useAnimation = 45;  
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.buyPrice(platinum: 1);
            Item.UseSound = SoundID.Item66;
			Item.shoot = ModContent.ProjectileType<BrainJarProj>();
            Item.shootSpeed = 15f;
		}

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<BrainJarProj>()] <= 0 && player.ownedProjectileCounts[ModContent.ProjectileType<Brainy>()] <= 0;
        }
    }
}
