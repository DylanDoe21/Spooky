using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.Minibiomes.Christmas;

namespace Spooky.Content.Items.Minibiomes.Christmas
{
    public class MarbleJar : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 18;
            Item.DamageType = DamageClass.Melee;
			Item.noMelee = true;
			Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.width = 32;
            Item.height = 44;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 3;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 1);
            Item.UseSound = SoundID.Item1;
			Item.shoot = ModContent.ProjectileType<MarbleJarProj>();
            Item.shootSpeed = 12f;
        }
    }
}