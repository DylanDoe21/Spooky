using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.SpookyBiome
{
	public class CranberryJuice : ModItem
	{
        public override void SetStaticDefaults() 
        {
			Item.ResearchUnlockCount = 30;
		}

		public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 40;
            Item.consumable = true;
			Item.healLife = 100;
			Item.useTime = 15;
            Item.useAnimation = 15;
            Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item3;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.maxStack = 9999;
			Item.scale = 0.75f;
        }

		public override bool CanUseItem(Player player)
		{
			return !player.HasBuff(BuffID.PotionSickness);
		}

		public override bool? UseItem(Player player)
		{
			player.AddBuff(BuffID.PotionSickness, 1800);
			return true;
		}

		public override void UpdateInventory(Player player)
		{
			if (player.controlQuickHeal && !player.HasBuff(BuffID.PotionSickness))
			{
				SoundEngine.PlaySound(Item.UseSound, player.Center);

				player.ConsumeItem(Type, false, false);
				player.Heal(100);
				player.AddBuff(BuffID.PotionSickness, 1800);
			}

			if (player.statLife < (player.statLifeMax2 / 2) && !player.HasBuff(BuffID.PotionSickness))
			{
				SoundEngine.PlaySound(Item.UseSound, player.Center);

				player.ConsumeItem(Type, false, false);
				player.Heal(100);
				player.AddBuff(BuffID.PotionSickness, 1800);
			}
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<CranberryJelly>(), 1)
            .AddIngredient(ItemID.PixieDust, 3)
            .AddIngredient(ItemID.CrystalShard, 1)
            .AddTile(TileID.Bottles)
            .Register();
        }
    }
}
