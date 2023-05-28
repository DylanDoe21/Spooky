using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Projectiles.SpookyHell;
 
namespace Spooky.Content.Items.SpookyHell.Boss
{
    public class LeechStaff : ModItem
    {
        public override void SetStaticDefaults()
		{
			Item.staff[Item.type] = true;
		}

		public override void SetDefaults()
        {
            Item.damage = 55; 
			Item.mana = 10;                        
            Item.DamageType = DamageClass.Summon;
			Item.noMelee = true;  
			Item.autoReuse = true;                  
            Item.width = 58;
            Item.height = 48;
            Item.useTime = 25;
            Item.useAnimation = 25;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 3;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 20); 
            Item.UseSound = SoundID.DD2_BetsysWrathShot;
            Item.shoot = ModContent.ProjectileType<Leech>();  
            Item.shootSpeed = 8f;
        }

		public override bool CanUseItem(Player player)
		{
			return player.statLife >= (player.statLifeMax2 / 4);
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<ArteryPiece>(), 10)
			.AddIngredient(ModContent.ItemType<CreepyChunk>(), 15)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
	}
}