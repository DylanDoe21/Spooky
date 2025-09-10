using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.Localization;
using Terraria.Chat;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Biomes;
using Spooky.Content.Dusts;
using Spooky.Content.Items.Minibiomes.Ocean;
using Spooky.Content.Projectiles.Minibiomes.Ocean;
using Spooky.Content.Tiles.Blooms;
using Spooky.Content.Tiles.Relic;

namespace Spooky.Content.NPCs.Minibiomes.Ocean
{
	public class LittleDunk : ModNPC
	{
		private readonly PathFinding pathfinder = new PathFinding(15);

		int CurrentFrameX = 0; //0 = swim animation  1 = open mouth animation

		Vector2 PositionGoTo = Vector2.Zero;
	 	List<int> BiomePositionDistances = new List<int>();

		Player TargetedPlayer = null;

		private static Asset<Texture2D> NPCTexture;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 8;
			NPCID.Sets.CountsAsCritter[NPC.type] = true;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
			NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[NPC.type] = true;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
                CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/LittleDunkBestiary"
            };
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			//vector2
			writer.WriteVector2(PositionGoTo);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			//vector2
			PositionGoTo = reader.ReadVector2();
		}

		public override void SetDefaults()
		{
			NPC.lifeMax = 150;
			NPC.damage = 0;
			NPC.defense = 0;
			NPC.width = 76;
			NPC.height = 50;
			NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
			NPC.waterMovementSpeed = 1f;
			NPC.SuperArmor = true;
			NPC.noTileCollide = false;
			NPC.noGravity = true;
			NPC.behindTiles = true;
			NPC.immortal = true;
			NPC.dontTakeDamage = true;
			NPC.chaseable = false;
			NPC.HitSound = SoundID.DD2_SkeletonHurt;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.aiStyle = -1;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.ZombieOceanBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type], quickUnlock: true);

			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
			{
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.LittleDunk"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.ZombieOceanBiome>().ModBiomeBestiaryInfoElement),
			});
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);

			var effects = NPC.velocity.X > 0f ? SpriteEffects.None : SpriteEffects.FlipVertically;

			//draw body
			spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

			return false;
        }

		public override void FindFrame(int frameHeight)
        {
			if (Main.netMode != NetmodeID.Server)
            {
                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 2;
            }

            NPC.frame.X = (int)(NPC.frame.Width * CurrentFrameX);

            NPC.frameCounter++;

			if (NPC.frameCounter > 8 - (NPC.velocity.X > 0 ? NPC.velocity.X : -NPC.velocity.X))
            {
				NPC.frame.Y = NPC.frame.Y + frameHeight;
				NPC.frameCounter = 0;
			}
			if (NPC.frame.Y >= frameHeight * 8)
			{
				NPC.frame.Y = 0 * frameHeight;
			}
        }

		public override bool CheckActive()
        {
            return !AnyPlayersInBiome();
        }

		public bool AnyPlayersInBiome()
        {
            foreach (Player player in Main.ActivePlayers)
            {
                int playerInBiomeCount = 0;

                if (!player.dead && player.InModBiome(ModContent.GetInstance<ZombieOceanBiome>()))
                {
                    playerInBiomeCount++;
                }

                if (playerInBiomeCount >= 1)
                {
                    return true;
                }
            }

            return false;
        }

		public override void AI()
		{
			//rotation stuff
			NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + MathHelper.TwoPi;

			//if there are no active players in the biome then despawn
			if (!AnyPlayersInBiome())
			{
				NPC.ai[0] = 0;
				NPC.ai[1] = 0;
				NPC.ai[2] = 0;

				NPC.velocity.Y += 0.4f;
				NPC.EncourageDespawn(10);

				return;
			}

			//constantly call stepup collision so it doesnt get stuck on blocks
			Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);

			foreach (Player player in Main.ActivePlayers)
			{
				if (!player.dead && player.InModBiome<ZombieOceanBiome>())
				{
					TargetedPlayer = player;
					break;
				}
				else
				{
					TargetedPlayer = null;
					continue;
				}
			}

			bool HasLineOfSightToTarget = Collision.CanHitLine(TargetedPlayer.position, TargetedPlayer.width, TargetedPlayer.height, NPC.position, NPC.width, NPC.height);

			//go to the player instantly if there is no line of sight
			if (!HasLineOfSightToTarget)
			{
				NPC.noTileCollide = true;

				Vector2 desiredVelocity = NPC.DirectionTo(TargetedPlayer.Center) * 12;
				NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);
			}
			//follow the player
			else
			{
				NPC.noTileCollide = false;

				Vector2 PlayerGoTo = TargetedPlayer.Center - new Vector2(0, 30);

				if (NPC.Distance(PlayerGoTo) >= 100)
				{
					float vel = MathHelper.Clamp(NPC.Distance(PlayerGoTo) / 12, 0.1f, NPC.Distance(TargetedPlayer.Center) / 50f);
					NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(PlayerGoTo) * vel, 0.08f);
				}
			}
		}
	}
}