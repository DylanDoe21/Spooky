using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Core;
using Spooky.Content.Dusts;

namespace Spooky.Content.Items.SpookyBiome
{
	public class MoldJar : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 38;
			Item.height = 40;
			Item.accessory = true;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(gold: 1);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetJumpState<MoldJarJump>().Enable();
		}
	}

	public class MoldJarJump : ExtraJump
	{
		public override Position GetDefaultPosition() => new Before(CloudInABottle);

		public override float GetDurationMultiplier(Player player) => 1.35f;

		public override void OnStarted(Player player, ref bool playSound)
		{
			playSound = false;

            SoundEngine.PlaySound(SoundID.Item16, player.Center);

			Vector2 playerVelocity = player.velocity * 0.4f + Vector2.UnitY;
            Vector2 offset = player.Bottom;
		}

		public override void ShowVisuals(Player player)
		{
			if (Main.rand.NextBool())
			{
				int offsetY = (player.gravDir == -1f) ? -6 : player.height;

				Color color = new Color(114, 103, 42); 

				switch (Main.rand.Next(3))
				{
					//brown
					case 0:
					{
						color = new Color(114, 103, 42);
						break;
					}
					//dark orange
					case 1:
					{
						color = new Color(145, 100, 29);
						break;
					}
					//reddish orange
					case 2:
					{
						color = new Color(178, 67, 46);
						break;
					}
				}

				int DustEffect = Dust.NewDust(player.position + new Vector2(-4, offsetY), player.width + 8, 4, ModContent.DustType<SmokeEffect>(), 
				-player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 100, color * 0.5f, Main.rand.NextFloat(0.2f, 0.5f));
				Main.dust[DustEffect].velocity = Main.dust[DustEffect].velocity * 0.5f - player.velocity * new Vector2(0.1f, 0.3f);
				Main.dust[DustEffect].alpha = 100;
			}
		}
	}
}