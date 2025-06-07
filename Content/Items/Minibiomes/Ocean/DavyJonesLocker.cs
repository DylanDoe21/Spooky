using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Projectiles.Minibiomes.Ocean;
 
namespace Spooky.Content.Items.Minibiomes.Ocean
{
    public class DavyJonesLocker : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 25;
            Item.mana = 10;
            Item.DamageType = DamageClass.Magic;
            Item.noMelee = true;
			Item.autoReuse = true;
			Item.noUseGraphic = true;
			Item.channel = true;           
            Item.width = 32;
            Item.height = 58;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 2;
            Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(gold: 1, silver: 50);
			Item.UseSound = SoundID.Item1;
			Item.shoot = ModContent.ProjectileType<DavyJonesLockerProj>();
			Item.shootSpeed = 0f;
        }
    }
}