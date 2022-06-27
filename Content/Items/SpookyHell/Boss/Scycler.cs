using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Content.Projectiles.SpookyHell;
 
namespace Spooky.Content.Items.SpookyHell.Boss
{
    public class Scycler : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scycler");
            Tooltip.SetDefault("Throws piercing discs"
            + "\nOnly up to 5 discs can be out at once");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 55;    
            Item.DamageType = DamageClass.Melee;
            Item.useTurn = true;    
            Item.noUseGraphic = true; 
            Item.autoReuse = true;             
            Item.width = 60;
            Item.height = 44;
            Item.useTime = 25;       
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 2;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 2);   
            Item.UseSound = SoundID.Item1; 
            Item.shoot = ModContent.ProjectileType<ScyclerProj>();  
            Item.shootSpeed = 15f;     
        }

        public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[Item.shoot] < 5;
		}
    }
}