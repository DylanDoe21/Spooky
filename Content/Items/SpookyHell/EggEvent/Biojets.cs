using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Items.SpookyHell.EggEvent
{
	[AutoloadEquip(EquipType.Wings)]
	public class Biojets : ModItem
	{
		public static readonly SoundStyle FlyingSound = new("Spooky/Content/Sounds/EggEvent/BiojetterFly", SoundType.Sound) { Pitch = 0.45f, Volume = 0.15f };

		public override void SetStaticDefaults()
		{
			ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new Terraria.DataStructures.WingStats(185, default, 1.5f);
		}

		public override void SetDefaults()
		{
			Item.width = 56;
			Item.height = 40;
			Item.value = Item.buyPrice(gold: 30);
			Item.rare = ItemRarityID.LightPurple;
			Item.accessory = true;
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.noFallDmg = true;
        }

		public override bool WingUpdate(Player player, bool inUse)
		{
			//custom animation stuff
			int animationSpeed;
			if (player.velocity.Y != 0)
			{
				animationSpeed = inUse ? 2 : 10;
			}
			else
			{
				animationSpeed = 0;
				player.wingFrame = 0;
			}

			if (animationSpeed > 0)
			{
				player.wingFrameCounter++;
				if (player.wingFrameCounter >= animationSpeed)
				{
					player.wingFrameCounter = 0;
					player.wingFrame++;

					if (player.wingFrame >= 4)
					{
						player.wingFrame = 0;
					}
				}
			}

			if (inUse)
			{
				int EffectRate = 6;

				//hovering logic
				if (player.wings == player.wingsLogic)
				{
					if (player.controlDown)
					{
						if (player.wingTime > 0)
						{
							player.velocity.Y *= 0.001f;

							if (player.controlLeft || player.controlRight)
							{
								player.velocity.X += 0.5f * player.direction;
								player.velocity.X = MathHelper.Clamp(player.velocity.X, -10, 10);
							}
						}

						EffectRate = 10;
					}
				}

				//play biojetter flight sound
				if (player.miscCounter % (EffectRate * 2) == 0)
				{
					SoundEngine.PlaySound(FlyingSound, player.Center);
				}

				//produce green ooze dusts while flying
				if (player.miscCounter % EffectRate == 0)
				{
					//left side dusts
					int numDusts = 5;
					Vector2 playerVelocity = player.velocity * 0.4f + Vector2.UnitY * player.gravDir * 6;
					Vector2 playerOffset = player.Center + new Vector2(20 * -player.direction, 12 * player.gravDir);
					Vector2 playerOffset2 = player.Center + new Vector2(14 * player.direction, 12 * player.gravDir);

					for (int i = 0; i < numDusts; i++)
					{
						//first side
						Vector2 position = -Vector2.UnitY.RotatedBy(i * MathHelper.TwoPi / numDusts) * new Vector2(1f, 0.25f);
						Vector2 velocity = playerVelocity + position * 1.25f;
						position = position * 8 + playerOffset;
						Dust dust = Dust.NewDustPerfect(position, DustID.GreenBlood, velocity);
						dust.noGravity = true;
						dust.fadeIn = 0.1f;
						dust.velocity.Y += 3;
						dust.scale = 1.5f;
						dust.shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);

						//second side
						Vector2 position2 = -Vector2.UnitY.RotatedBy(i * MathHelper.TwoPi / numDusts) * new Vector2(1f, 0.25f);
						position2 = position2 * 3 + playerOffset2;
						Dust dust2 = Dust.NewDustPerfect(position2, DustID.GreenBlood, velocity);
						dust2.noGravity = true;
						dust2.fadeIn = 0.1f;
						dust2.velocity.Y += 3;
						dust2.scale = 1.5f;
						dust2.shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
					}
				}
			}

			return true;
		}
	}
}