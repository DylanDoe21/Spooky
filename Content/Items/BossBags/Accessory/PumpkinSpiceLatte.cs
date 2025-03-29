using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Items.BossBags.Accessory
{
	public class PumpkinSpiceLatte : ModItem
	{
		public override void SetDefaults()
        {
			Item.healLife = 200;
			Item.width = 42;
			Item.height = 28;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.expert = true;
			Item.noMelee = true;
			Item.consumable = false;
            Item.rare = ItemRarityID.Yellow;
			Item.UseSound = SoundID.Item3;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
			Item.holdStyle = ItemHoldStyleID.HoldFront;
        }

		public override bool ConsumeItem(Player player)
		{
			return false;
		}

		public override void HoldStyle(Player player, Rectangle heldItemFrame) 
		{
			player.itemLocation.X = player.MountedCenter.X + 4f * player.direction;
			player.itemLocation.Y = player.MountedCenter.Y + 14f;
			player.itemRotation = 0f;
		}

		public override bool CanUseItem(Player player)
		{
			return !player.HasBuff(BuffID.PotionSickness);
		}

		public override bool? UseItem(Player player)
		{
			int Duration = player.pStone ? (int)(3600 * 0.75) : 3600;
			player.AddBuff(BuffID.PotionSickness, Duration);
			return true;
		}

		public override void UpdateInventory(Player player)
		{
			if (player.controlQuickHeal && !player.HasBuff(BuffID.PotionSickness))
			{
				SoundEngine.PlaySound(Item.UseSound, player.Center);
				player.GetModPlayer<SpookyPlayer>().PotionSicknessLatteTimer = 2;
			}
		}
	}
}
