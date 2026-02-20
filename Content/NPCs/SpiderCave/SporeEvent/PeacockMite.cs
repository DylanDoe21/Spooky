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
using Spooky.Content.Items.SpiderCave.Misc;
using Spooky.Content.Tiles.Blooms;

namespace Spooky.Content.NPCs.SpiderCave.SporeEvent
{
    public class PeacockMite : ModNPC
    {
        public int MoveSpeedX = 0;
		public int MoveSpeedY = 0;

        bool runOnce = true;
		Vector2[] trailLength = new Vector2[35];
		Rectangle[] trailHitboxes = new Rectangle[35];

		private static Asset<Texture2D> NPCTexture;
        private static Asset<Texture2D> GlowTexture;
		private static Asset<Texture2D> TailTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            for (int i = 0; i < trailLength.Length; i++)
            {
                writer.WriteVector2(trailLength[i]);
            }

            //bools
            writer.Write(runOnce);

            //ints
            writer.Write(MoveSpeedX);
            writer.Write(MoveSpeedY);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            for (int i = 0; i < trailLength.Length; i++)
            {
                trailLength[i] = reader.ReadVector2();
            }

            //bools
            runOnce = reader.ReadBoolean();

            //ints
            MoveSpeedX = reader.ReadInt32();
            MoveSpeedY = reader.ReadInt32();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 420;
            NPC.damage = 40;
            NPC.defense = 15;
            NPC.width = 66;
			NPC.height = 80;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit23 with { Pitch = 0.5f };
            NPC.DeathSound = SoundID.NPCDeath25 with { Pitch = 0.25f };
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SporeEventBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.PeacockMite"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SporeEventBiome>().ModBiomeBestiaryInfoElement)
			});
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

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			if (runOnce)
			{
				return false;
			}

			TailTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/TrailSquare");

			Vector2 drawOrigin = new(TailTexture.Width() * 0.5f, TailTexture.Height() * 0.5f);
			Vector2 previousPosition = NPC.Center;

			for (int k = 0; k < trailLength.Length; k++)
			{
				if (trailLength[k] == Vector2.Zero)
				{
					return false;
				}

				Vector2 drawPos = trailLength[k] - Main.screenPosition;
				Vector2 currentPos = trailLength[k];
				Vector2 betweenPositions = previousPosition - currentPos;

				float max = betweenPositions.Length();

				for (int i = 0; i < max; i++)
				{
					if (i % 2 == 0)
					{
						drawPos = previousPosition + -betweenPositions * (i / max) - Main.screenPosition;

						Color chainDrawColor = Lighting.GetColor((int)trailLength[k].X / 16, (int)(trailLength[k].Y / 16f));

                        Main.EntitySpriteDraw(TailTexture.Value, drawPos + new Vector2(-10, 0).RotatedBy(NPC.rotation), null, chainDrawColor, 0f, drawOrigin, 0.5f, SpriteEffects.None, 0f);
                        Main.EntitySpriteDraw(TailTexture.Value, drawPos + new Vector2(10, 0).RotatedBy(NPC.rotation), null, chainDrawColor, 0f, drawOrigin, 0.5f, SpriteEffects.None, 0f);
						Main.EntitySpriteDraw(TailTexture.Value, drawPos, null, chainDrawColor, 0f, drawOrigin, 1f, SpriteEffects.None, 0f);
					}
				}

				previousPosition = currentPos;
			}

			return false;
		}

		public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);
            GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");

			Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - screenPos, NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);
		}

        public override void AI()
		{
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + 1.57f;

            if (runOnce)
			{
				for (int i = 0; i < trailLength.Length; i++)
				{
					trailLength[i] = Vector2.Zero;
					trailHitboxes[i] = Rectangle.Empty;
				}

				runOnce = false;

				NPC.netUpdate = true;
			}

			//save previous positions, rotations, and direction
            Vector2 current = NPC.Center;
            for (int i = 0; i < trailLength.Length; i++)
            {
                Vector2 previousPosition = trailLength[i];
                trailLength[i] = current;
                trailHitboxes[i] = new Rectangle((int)current.X - 3, (int)current.Y - 3, 6, 6);

                current = previousPosition;
            }

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
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MiteMandibles>(), 3, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FungusSeed>(), 120));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 4; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/PeacockMiteGore" + numGores).Type);
                    }
                }
            }
        }
    }
}