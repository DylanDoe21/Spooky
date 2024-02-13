using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Projectiles.SpookyBiome;
 
namespace Spooky.Content.Items.SpookyBiome
{
    public class ElGourdo : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 100;    
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.useTurn = true;    
            Item.noUseGraphic = true; 
            Item.autoReuse = true;             
            Item.width = 42;
            Item.height = 74;
            Item.useTime = 45;       
            Item.useAnimation = 45;  
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.buyPrice(platinum: 1); 
            Item.UseSound = SoundID.Item1; 
            Item.shoot = ModContent.ProjectileType<ElGourdoProj>();  
            Item.shootSpeed = 5f;
        }
    }
}