using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Projectiles.SpiderCave;
 
namespace Spooky.Content.Items.SpiderCave
{
    public class OrbWeaverBoomerang : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 50;
            Item.DamageType = DamageClass.Melee;
            Item.useTurn = true;    
            Item.noUseGraphic = true; 
            Item.noMelee = true;
            Item.autoReuse = true;             
            Item.width = 22;
            Item.height = 42;
            Item.useTime = 20;     
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 2;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 15);
            Item.UseSound = SoundID.Item1; 
            Item.shoot = ModContent.ProjectileType<OrbWeaverBoomerangProj>();  
            Item.shootSpeed = 25f;
        }
    }
}