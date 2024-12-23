using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.NPCs.Minibiomes.TarPits
{
    public class Tarrar : ModNPC  
    {
		int HeightOffset = 28; //height offset that this npc should draw at and for its hitbox to be raised up

		private static Asset<Texture2D> NPCTexture;

		public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 13;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
				Position = new Vector2(0f, 46f),
				PortraitPositionYOverride = 46f,
				Frame = 5
			};
		}

        public override void SetDefaults()
		{
            NPC.lifeMax = 250;
            NPC.damage = 30;
            NPC.defense = 5;
            NPC.width = 76;
			NPC.height = 80;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 0, 50);
            NPC.noGravity = true;
            NPC.noTileCollide = false;
			NPC.HitSound = SoundID.Item95 with { Volume = 0.8f, Pitch = 1.3f };
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.TarPitsBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Tarrar"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.TarPitsBiome>().ModBiomeBestiaryInfoElement)
			});
        }

        public override void FindFrame(int frameHeight)
		{
			if (NPC.ai[0] == 0)
			{
				if (NPC.frame.Y > frameHeight * 0)
				{
					NPC.frameCounter++;
					if (NPC.frameCounter > 4)
					{
						NPC.frame.Y = NPC.frame.Y + frameHeight;
						NPC.frameCounter = 0;
					}
					if (NPC.frame.Y >= frameHeight * 13)
					{
						NPC.frame.Y = 0 * frameHeight;
					}
				}
			}
			else
			{
				NPC.frameCounter++;
				if (NPC.frameCounter > 4)
				{
					NPC.frame.Y = NPC.frame.Y + frameHeight;
					NPC.frameCounter = 0;
				}
				if (NPC.frame.Y >= frameHeight * 7)
				{
					NPC.frame.Y = 5 * frameHeight;
				}
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);

			var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			//draw npc manually for stretching
			spriteBatch.Draw(NPCTexture.Value, new Vector2(NPC.Center.X, NPC.Center.Y - HeightOffset) - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, 1f, effects, 0f);

			return false;
		}

		public override void ModifyHoverBoundingBox(ref Rectangle boundingBox)
		{
			//shrink the npcs bounding box if its not attacking since its much smaller
			if (NPC.ai[0] == 0)
			{
				boundingBox = new Rectangle((int)NPC.position.X, (int)NPC.position.Y + NPC.height - HeightOffset - NPC.height / 3, NPC.width, NPC.height / 3);
			}
			else
			{
				boundingBox = new Rectangle((int)NPC.position.X, (int)NPC.position.Y - HeightOffset, NPC.width, NPC.height);
			}

			base.ModifyHoverBoundingBox(ref boundingBox);
		}

		public override bool ModifyCollisionData(Rectangle victimHitbox, ref int immunityCooldownSlot, ref MultipliableFloat damageMultiplier, ref Rectangle npcHitbox)
		{
			npcHitbox = new Rectangle((int)NPC.position.X, (int)NPC.position.Y - HeightOffset, NPC.width, NPC.height);

			return false;
		}


		//TODO: could use sound effects for emerging and sinking into the tar
		public override void AI()
		{
			int CurrentDirection = NPC.direction;
			int CurrentTarget = NPC.target;

			NPC.TargetClosest();
			Player player = Main.player[NPC.target];

			//make npc face the player if attacking, otherwise face in the direction its moving
			if (NPC.ai[0] == 0)
			{
				if (CurrentTarget >= 0 && CurrentDirection != 0)
				{
					NPC.direction = CurrentDirection;
				}
			}
			else
			{
				NPC.spriteDirection = NPC.direction;
			}

			if (NPC.wet && WorldGen.InWorld((int)(NPC.Center.X + (float)((NPC.width / 2 + 8) * NPC.direction)) / 16, (int)(NPC.Center.Y / 16f), 5))
			{
				float MovementSpeed = NPC.ai[0] == 0 ? 2f : 7f;

				if (NPC.ai[0] == 0)
				{
					NPC.velocity.X = (NPC.velocity.X * 19f + MovementSpeed * (float)NPC.direction) / 20f;
				}
				else
				{
					float DirectionToPlayer = player.Center.X < NPC.Center.X ? -1 : 1;
					NPC.velocity.X = (NPC.velocity.X * 19f + MovementSpeed * DirectionToPlayer) / 20f;
				}

				int NPCPositionX = (int)(NPC.Center.X + (float)((NPC.width / 2 + 8) * NPC.direction)) / 16;
				int NPCPositionY = (int)((NPC.position.Y + (float)NPC.height) / 16f);
				int NPCCenterY = (int)(NPC.Center.Y / 16f);
				int TileYPos = (int)(NPC.position.Y / 16f);

				Tile tile1 = Main.tile[NPCPositionX, NPCCenterY];
				Tile tile2 = Main.tile[NPCPositionX, NPCPositionY];

				if (tile1 == null)
				{
					tile1 = new Tile();
				}
				if (tile2 == null)
				{
					tile2 = new Tile();
				}
				if (NPCPositionX < 5 || NPCPositionX > Main.maxTilesX - 5 || WorldGen.SolidTile(NPCPositionX, NPCCenterY) || WorldGen.SolidTile(NPCPositionX, TileYPos) || WorldGen.SolidTile(NPCPositionX, NPCCenterY) || tile2.LiquidAmount == 0)
				{
					if (NPC.ai[0] == 0)
					{
						NPC.direction *= -1;
					}
					else
					{
						NPC.velocity.X = 0;
					}
				}

				NPC.spriteDirection = NPC.direction;

				if (NPC.velocity.Y > 0f)
				{
					NPC.velocity.Y *= 0.5f;
				}

				NPCPositionX = (int)(NPC.Center.X / 16f);
				NPCCenterY = (int)(NPC.Center.Y / 16f);
				float NPCBottom = NPC.position.Y + (float)NPC.height;

				Tile tile3 = Main.tile[NPCPositionX, NPCCenterY - 1];
				Tile tile4 = Main.tile[NPCPositionX, NPCCenterY];
				Tile tile5 = Main.tile[NPCPositionX, NPCCenterY + 1];

				if (tile3 == null)
				{
					tile3 = new Tile();
				}
				if (tile4 == null)
				{
					tile4 = new Tile();
				}
				if (tile5 == null)
				{
					tile5 = new Tile();
				}

				if (tile3.LiquidAmount > 0)
				{
					NPCBottom = NPCCenterY * 16;
					NPCBottom -= (float)(tile3.LiquidAmount / 16);
				}
				else if (tile4.LiquidAmount > 0)
				{
					NPCBottom = (NPCCenterY + 1) * 16;
					NPCBottom -= (float)(tile4.LiquidAmount / 16);
				}
				else if (tile5.LiquidAmount > 0)
				{
					NPCBottom = (NPCCenterY + 2) * 16;
					NPCBottom -= (float)(tile5.LiquidAmount / 16);
				}

				NPCBottom -= 6f;
				if (NPC.Center.Y > NPCBottom)
				{
					NPC.velocity.Y -= 0.1f;
					if (NPC.velocity.Y < -8f)
					{
						NPC.velocity.Y = -8f;
					}
					if (NPC.Center.Y + NPC.velocity.Y < NPCBottom)
					{
						NPC.velocity.Y = NPCBottom - NPC.Center.Y;
					}
				}
				else
				{
					NPC.velocity.Y = NPCBottom - NPC.Center.Y;
				}
			}

			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				return;
			}

			if (!NPC.wet)
			{
				NPC.netUpdate = true;
				NPC.active = false;
				return;
			}

			Rectangle DetectionBox = new Rectangle((int)NPC.position.X - 120, (int)NPC.position.Y - 120, NPC.width + 240, NPC.height + 240);
			bool HasLineOfSight = Collision.CanHitLine(player.position, player.width, player.height, new Vector2(NPC.position.X, NPC.position.Y - HeightOffset), NPC.width, NPC.height);

			if (DetectionBox.Intersects(player.Hitbox) && HasLineOfSight)
			{
				NPC.ai[0] = 1;
			}
			else
			{
				NPC.ai[0] = 0;
			}
		}

		public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
				Vector2 OffsetPos = new Vector2(NPC.position.X, NPC.position.Y - HeightOffset);
				Vector2 OffsetCenter = new Vector2(NPC.Center.X, NPC.Center.Y - HeightOffset);

				for (int numDusts = 0; numDusts < 25; numDusts++)
                {                                                                                  
                    int dust = Dust.NewDust(OffsetPos, NPC.width, NPC.height, DustID.Asphalt, 0f, -2f, 0, default, 1.5f);
                    Main.dust[dust].position.X += Main.rand.Next(-25, 25) * 0.05f - 1.5f;
                    Main.dust[dust].position.Y += Main.rand.Next(-25, 25) * 0.05f - 1.5f;
                }

                for (int numGores = 1; numGores <= 2; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), OffsetCenter, NPC.velocity, ModContent.Find<ModGore>("Spooky/TarrarGore" + numGores).Type);
                    }
                }
            }
        }

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
			position = new Vector2(NPC.Center.X, NPC.Center.Y + 32);
            return true;
        }
	}
}