using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.NPCs.Minibiomes.Vegetable.Projectiles;

namespace Spooky.Content.NPCs.Minibiomes.Vegetable
{
    public class OozeGarlic : ModNPC  
    {
		float addedStretch = 0f;
		float stretchRecoil = 0f;

		private static Asset<Texture2D> NPCTexture;

		public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 80;
            NPC.damage = 20;
            NPC.defense = 0;
            NPC.width = 56;
			NPC.height = 66;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0.25f;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 22;
			AIType = NPCID.Wraith;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.VegetableBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.OozeGarlic"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.VegetableBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 6)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 4)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);

			float stretch = 0f;

			stretch = Math.Abs(stretch) - addedStretch;

			//limit how much it can stretch
			if (stretch > 0.5f)
			{
				stretch = 0.5f;
			}

			//limit how much it can squish
			if (stretch < -0.5f)
			{
				stretch = -0.5f;
			}

			Vector2 scaleStretch = new Vector2(1f + stretch, 1f - stretch);

			//draw npc manually for stretching
			spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, scaleStretch, SpriteEffects.None, 0f);

			return false;
		}

		public override void AI()
		{
            NPC.TargetClosest();
            Player player = Main.player[NPC.target];

            NPC.rotation = NPC.velocity.X * 0.05f;

			//stretch stuff
			if (stretchRecoil > 0)
			{
				stretchRecoil -= 0.05f;
			}
			else
			{
				stretchRecoil = 0;
			}

			addedStretch = -stretchRecoil;

			bool HasLineOfSight = Collision.CanHitLine(player.position, player.width, player.height, NPC.position, NPC.width, NPC.height);

			if ((player.Distance(NPC.Center) <= 450f && HasLineOfSight) || NPC.localAI[0] >= 200)
            {
			    NPC.localAI[0]++;
            }

            if (NPC.localAI[0] > 200)
            {
                NPC.aiStyle = -1;
                NPC.velocity *= 0.9f;
            }
            else
            {
                NPC.aiStyle = 22;
			    AIType = NPCID.Wraith;
            }

            if (NPC.localAI[0] >= 240)
            {
                if (NPC.localAI[0] % 20 == 0)
                {
                    NPC.velocity.Y = -2;

					stretchRecoil = Main.rand.NextFloat(0.2f, 0.5f);

					SoundEngine.PlaySound(SoundID.NPCDeath13 with { Pitch = -1.2f }, NPC.Center);

					//spawn ooze
					NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(NPC.Center.X, NPC.Center.Y + 30), new Vector2(Main.rand.Next(-3, 4), 2), ModContent.ProjectileType<RottenOoze>(), NPC.damage, 4.5f);
                }
            }

            if (NPC.localAI[0] >= 280)
            {
                NPC.localAI[0] = 0;

                NPC.netUpdate = true;
            }
		}

        public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 4; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/OozeGarlicGore" + numGores).Type);
                    }
                }
            }
        }
    }
}