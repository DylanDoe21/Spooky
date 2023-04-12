using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Buffs.Minion;
using Spooky.Content.Projectiles.SpookyHell;
using Spooky.Content.Projectiles.Catacomb;

namespace Spooky.Content.Items.SpookyHell
{
	public class BrainJar : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 200;
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
            for (int i = 0; i < 1000; i++)
            {
                if (Main.projectile[i].active && (Main.projectile[i].type == ModContent.ProjectileType<BrainJarProj>() ||
                Main.projectile[i].type == ModContent.ProjectileType<Brainy>()))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
