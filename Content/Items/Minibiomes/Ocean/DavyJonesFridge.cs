using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Projectiles.Minibiomes.Ocean;
 
namespace Spooky.Content.Items.Minibiomes.Ocean
{
    public class DavyJonesFridge : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 70;
            Item.mana = 10;
            Item.DamageType = DamageClass.Magic;
            Item.noMelee = true;
			Item.autoReuse = true;
			Item.noUseGraphic = true;
			Item.channel = true;           
            Item.width = 32;
            Item.height = 58;
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 2;
            Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 10);
			Item.UseSound = SoundID.Item1;
			Item.shoot = ModContent.ProjectileType<DavyJonesFridgeProj>();
			Item.shootSpeed = 0f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<DavyJonesLocker>(), 1)
            .AddIngredient(ModContent.ItemType<DunkleosteusHide>(), 12)
            .AddIngredient(ItemID.SoulofFright, 5)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
}