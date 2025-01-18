using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

using Spooky.Core;

namespace Spooky.Content.Items.BossBags
{
	public class CranberryJuice : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 30;
		}

		public override void SetDefaults()
		{
			Item.healLife = 100;
			Item.width = 28;
			Item.height = 50;
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.consumable = true;
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
			player.GetModPlayer<SpookyPlayer>().PotionSicknessCranberryTimer = 2;
			return true;
		}

		public override void UpdateInventory(Player player)
		{
			if ((player.statLife < (player.statLifeMax2 / 2) || player.controlQuickHeal) && !player.HasBuff(BuffID.PotionSickness))
			{
				SoundEngine.PlaySound(Item.UseSound, player.Center);

				player.ConsumeItem(Type, false, false);
				player.Heal(Item.healLife);

				player.GetModPlayer<SpookyPlayer>().PotionSicknessCranberryTimer = 2;
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
