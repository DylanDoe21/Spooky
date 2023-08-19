using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell
{
    public class LivingFleshStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 42;
			Item.mana = 10;
			Item.DamageType = DamageClass.Magic;
            Item.noMelee = true;
			Item.autoReuse = true;
            Item.channel = true;
			Item.width = 68;
            Item.height = 62;
			Item.useTime = 30;         
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 2;
			Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.buyPrice(gold: 15);
			Item.UseSound = SoundID.Item17;     
			Item.shoot = ModContent.ProjectileType<ControllableEyeBig>();
			Item.shootSpeed = 10f;
        }

        public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[Item.shoot] < 8;
		}

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<FleshStaff>(), 1)
			.AddIngredient(ModContent.ItemType<ArteryPiece>(), 15)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
}