using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Terraria.Audio;

using Spooky.Content.Projectiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome.Boss
{
	public class PumpkinAxe : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Rot-Axe");
            Tooltip.SetDefault("Hitting enemies may thrust spikes upward"); 
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
            Item.damage = 22;           
			Item.DamageType = DamageClass.Melee;
			Item.autoReuse = true;           
			Item.width = 60;           
			Item.height = 48;         
			Item.useTime = 38;
			Item.useAnimation = 38;
			Item.useStyle = ItemUseStyleID.Swing;          
			Item.knockBack = 6;    
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(gold: 1);
			Item.UseSound = SoundID.Item7;
			Item.shoot = ModContent.ProjectileType<Blank>();
            Item.shootSpeed = 7f;
		}

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            if (Main.rand.Next(4) == 0)
            {
				SoundEngine.PlaySound(SoundID.Item70, target.position);

				for (int i = 0; i < Main.rand.Next(1, 2); i++)
				{
					Projectile.NewProjectile(null, target.Center.X, target.Center.Y, Main.rand.Next(-1, 1), Main.rand.Next(-5, -3), 
					ModContent.ProjectileType<PumpkinAxeSpike>(), damage, knockBack, player.whoAmI, 0f, 0f);
				}
			}
        }

        /*
		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<MagicPumpkin>(), 10)
            .AddIngredient(ModContent.ItemType<SpookyPlasma>(), 5)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
		*/
    }
}