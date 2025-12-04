using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.NPCs.SpiderCave.SpiderWar
{
    public class JoroFly : ModNPC
    {
        public int MoveSpeedX = 0;
		public int MoveSpeedY = 0;

        private static Asset<Texture2D> NPCTexture;
        private static Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //ints
            writer.Write(MoveSpeedX);
            writer.Write(MoveSpeedY);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //ints
            MoveSpeedX = reader.ReadInt32();
            MoveSpeedY = reader.ReadInt32();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 450;
            NPC.damage = 45;
            NPC.defense = 15;
            NPC.width = 42;
			NPC.height = 48;
            NPC.npcSlots = 0f;
            NPC.knockBackResist = 0.75f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit29 with { Pitch = -0.2f };
            NPC.DeathSound = SoundID.NPCDeath36 with { Volume = 0.5f, Pitch = -0.5f };
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpiderCaveBiome>().Type };
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
		{
			NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * balance * bossAdjustment);
		}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.JoroFly"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderCaveBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
			{
                return true;
            }

            NPCTexture ??= ModContent.Request<Texture2D>(Texture);
            GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");

			//draw npc
            spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(GlowTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White * 0.5f), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0f);

			return false;
		}

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 1)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 3)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override void AI()
		{
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            Vector2 RotateTowards = player.Center - NPC.Center;

            float RotateDirection = (float)Math.Atan2(RotateTowards.Y, RotateTowards.X) + 4.71f;
            float RotateSpeed = 0.05f;

            NPC.rotation = NPC.rotation.AngleTowards(RotateDirection - MathHelper.TwoPi, RotateSpeed);

            int MaxSpeed = 5;

            //flies to players X position
            if (NPC.Center.X >= player.Center.X && MoveSpeedX >= -MaxSpeed - 2) 
            {
                MoveSpeedX--;
            }
            else if (NPC.Center.X <= player.Center.X && MoveSpeedX <= MaxSpeed + 2)
            {
                MoveSpeedX++;
            }

            NPC.velocity.X += MoveSpeedX * 0.01f;
            NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -MaxSpeed - 2, MaxSpeed + 2);
            
            //flies to players Y position
            if (NPC.Center.Y >= player.Center.Y && MoveSpeedY >= -MaxSpeed)
            {
                MoveSpeedY--;
            }
            else if (NPC.Center.Y <= player.Center.Y && MoveSpeedY <= MaxSpeed)
            {
                MoveSpeedY++;
            }

            NPC.velocity.Y += MoveSpeedY * 0.01f;
            NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y, -MaxSpeed, MaxSpeed);

            //prevent npcs of the same type from overlapping
            foreach (var npc in Main.ActiveNPCs)
            {
                if (npc.whoAmI != NPC.whoAmI && npc.type == NPC.type && npc.active && Math.Abs(NPC.position.X - npc.position.X) + Math.Abs(NPC.position.Y - npc.position.Y) < NPC.width)
				{
                    const float pushAway = 0.12f;
					if (NPC.position.X < npc.position.X)
					{
						NPC.velocity.X -= pushAway;
					}
					else
					{
						NPC.velocity.X += pushAway;
					}
					if (NPC.position.Y < npc.position.Y)
					{
						NPC.velocity.Y -= pushAway;
					}
					else
					{
						NPC.velocity.Y += pushAway;
					}
                }
            }
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/JoroFlyGore").Type);
                }
            }
        }
    }
}