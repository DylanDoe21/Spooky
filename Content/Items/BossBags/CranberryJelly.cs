using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

using Spooky.Core;

namespace Spooky.Content.Items.BossBags
{
	public class CranberryJelly : ModItem
	{
        public override void SetStaticDefaults() 
        {
			Item.ResearchUnlockCount = 30;
		}

		public override void SetDefaults()
        {
			Item.healLife = 35;
			Item.width = 26;
            Item.height = 40;
			Item.useTime = 15;
            Item.useAnimation = 15;
			Item.consumable = true;
			Item.rare = ItemRarityID.Blue;
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
	}
}
