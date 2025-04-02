using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Items.SpookyBiome
{
	[AutoloadEquip(EquipType.Wings)]
	public class SpookFishronWings : ModItem
	{
		public override void SetStaticDefaults()
		{
			ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new Terraria.DataStructures.WingStats(180, 8f, 2f);
		}

		public override void SetDefaults()
		{
			Item.width = 56;
			Item.height = 40;
			Item.value = Item.buyPrice(gold: 40);
			Item.rare = ItemRarityID.Yellow;
			Item.accessory = true;
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.noFallDmg = true;
			player.ignoreWater = true;
        }

		public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
		{
			ascentWhenFalling = 0.85f;
			ascentWhenRising = 0.15f;
			maxCanAscendMultiplier = 1f;
			maxAscentMultiplier = 2.5f;
			constantAscend = 0.135f;
		}

		public override bool WingUpdate(Player player, bool inUse)
		{
			if (player.controlJump)
			{
				int PlayerPosOffset = 8;

				if (player.direction == 1)
				{
					PlayerPosOffset = -30;
				}

				int WingDust = Dust.NewDust(new Vector2(player.position.X + (float)(player.width / 2) + (float)PlayerPosOffset, player.position.Y), 18, player.height, 55, 0f, 0f, 100, Color.Orange, 1f);
				Main.dust[WingDust].noGravity = true;
				Main.dust[WingDust].noLight = true;
				Main.dust[WingDust].velocity /= 4f;
				Main.dust[WingDust].velocity -= player.velocity;
				Main.dust[WingDust].shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);

				if (Main.rand.NextBool(2))
				{
					PlayerPosOffset = -24;

					if (player.direction == 1)
					{
						PlayerPosOffset = 8;
					}

					float PlayerYPosOffset = player.position.Y;

					if (player.gravDir == -1f)
					{
						PlayerYPosOffset += (float)(player.height / 2);
					}

					WingDust = Dust.NewDust(new Vector2(player.position.X + (float)(player.width / 2) + (float)PlayerPosOffset, PlayerYPosOffset), 12, player.height / 2, 55, 0f, 0f, 100, Color.Orange, 1f);
					Main.dust[WingDust].noGravity = true;
					Main.dust[WingDust].noLight = true;
					Main.dust[WingDust].velocity /= 4f;
					Main.dust[WingDust].velocity -= player.velocity;
					Main.dust[WingDust].shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
				}
			}

			return base.WingUpdate(player, inUse);
		}
	}
}