using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Items.SpiderCave
{
	[AutoloadEquip(EquipType.Wings)]
	public class MortarWings : ModItem
	{
		public override void SetStaticDefaults()
		{
			ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new Terraria.DataStructures.WingStats(180, 8f, 2f);
		}

		public override void SetDefaults()
		{
			Item.width = 34;
			Item.height = 28;
			Item.value = Item.buyPrice(gold: 50);
			Item.rare = ItemRarityID.Yellow;
			Item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.noFallDmg = true;

			//hovering logic
			if (player.wings == player.wingsLogic && player.controlJump && player.controlDown && player.grappling[0] < 0 && player.wingTime > 0)
			{
				player.position.Y += -0.001f;
				float num = player.gravity * player.gravDir;
				player.position.Y -= player.velocity.Y;
				player.velocity.Y = num;

				if (player.controlLeft) player.velocity.X -= 0.5f;
				if (player.controlRight) player.velocity.X += 0.5f;

				player.velocity.X = MathHelper.Clamp(player.velocity.X, -15, 15);
			}
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
			//only use the custom animation if you are flying and your wing time hasnt run out
			if (player.controlJump && player.velocity.Y != 0 && player.grappling[0] < 0)
			{
				int EffectRate = 2;

				int frameRate = player.wingTime > 0 ? 2 : 4;
				int maxFrames = 7;

				if (player.wingTime > 0)
				{
					//hovering stuff
					if (player.controlDown)
					{
						if (player.wings == player.wingsLogic)
						{
							player.wingFrameCounter++;
							if (player.wingFrameCounter % frameRate == 0)
							{
								player.wingFrame++;
							}
							if (player.wingFrame <= 4)
							{
								player.wingFrame = 4;
							}
							if (player.wingFrame >= 7)
							{
								player.wingFrameCounter = 0;
								player.wingFrame = 4;
							}

							EffectRate = 4;
						}
					}
					else
					{
						player.wingFrameCounter++;
						if (player.wingFrameCounter % frameRate == 0)
						{
							player.wingFrame++;
						}
						if (player.wingFrame >= 4)
						{
							player.wingFrameCounter = 0;
							player.wingFrame = 1;
						}

						EffectRate = 2;
					}
				}
				else
				{
					player.wingFrameCounter++;
					if (player.wingFrameCounter % frameRate == 0)
					{
						player.wingFrame++;
					}
					if (player.wingFrame >= 4)
					{
						player.wingFrameCounter = 0;
						player.wingFrame = 1;
					}

					EffectRate = 5;
				}

				if (player.miscCounter % (EffectRate * 2) == 0)
				{
					SoundEngine.PlaySound(SoundID.Item13 with { Volume = 0.5f }, player.Center);
				}

				//produce flames
				if (player.miscCounter % EffectRate == 0)
				{
					bool IsHovering = player.wings == player.wingsLogic && player.controlDown && player.wingTime > 0;

					int XOffset = (IsHovering) ? 25 : 15;
					int YOffset = (IsHovering) ? 0 : 16;

					Vector2 playerVelocity = player.velocity * 0.4f + Vector2.UnitY * player.gravDir * 5;
					Vector2 playerOffset = player.Center + new Vector2(XOffset * -player.direction, YOffset * player.gravDir);
					Vector2 playerOffset2 = player.Center + new Vector2(9 * player.direction, YOffset * player.gravDir);

					if (IsHovering)
					{
						if ((player.direction == -1 && player.velocity.X > 0) || (player.direction == 1 && player.velocity.X < 0))
						{
							playerVelocity.X = (player.velocity * 2f + Vector2.UnitY * player.gravDir * 5).X;
						}
					}

					if (player.wingTime > 0)
					{
						//left side
						Vector2 position = -Vector2.UnitY.RotatedBy(MathHelper.TwoPi) * new Vector2(1f, 0.25f);
						Vector2 velocity = new Vector2(playerVelocity.X, IsHovering ? 0 : playerVelocity.Y) + position * 1.25f;
						position = position * 8 + playerOffset;
						Dust dust = Dust.NewDustPerfect(position, DustID.Torch, velocity);
						dust.noGravity = true;
						dust.fadeIn = 0.1f;
						dust.scale = 1.5f;
						dust.shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);

						if (!IsHovering)
						{
							//right side
							Vector2 position2 = -Vector2.UnitY.RotatedBy(MathHelper.TwoPi) * new Vector2(1f, 0.25f);
							position2 = position2 * 3 + playerOffset2;
							Dust dust2 = Dust.NewDustPerfect(position2, DustID.Torch, velocity);
							dust2.noGravity = true;
							dust2.fadeIn = 0.1f;
							dust2.scale = 1.5f;
							dust2.shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
						}
					}
				}
			}
			else
			{
				player.wingFrame = 0;
			}

			return true;
		}
	}
}