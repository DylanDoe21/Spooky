using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;

using Spooky.Content.Items.Minibiomes.Desert;

namespace Spooky.Content.NPCs.Minibiomes.Desert
{
    public class OpalHandDino : ModNPC  
    {
        private static Asset<Texture2D> ArmFrontTexture;
        private static Asset<Texture2D> ArmBackTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 12;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
				Position = new Vector2(18f, 15f),
				PortraitPositionXOverride = 0f,
				PortraitPositionYOverride = 0f
			};
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 400;
            NPC.damage = 55;
            NPC.defense = 20;
            NPC.width = 74;
			NPC.height = 74;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.2f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.HitSound = SoundID.Item95 with { Volume = 0.8f, Pitch = 1f };
			NPC.DeathSound = SoundID.NPCDeath1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.TarPitsBiome>().Type };
        }

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
			{
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.OpalHandDino"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.TarPitsBiome>().ModBiomeBestiaryInfoElement)
			});
		}

		public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			ArmFrontTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Minibiomes/Desert/OpalHandDinoArmFront");
            ArmBackTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Minibiomes/Desert/OpalHandDinoArmBack");

            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (NPC.localAI[2] > 0)
            {
                Main.EntitySpriteDraw(ArmFrontTexture.Value, NPC.Center - screenPos + new Vector2(0, 4), NPC.frame, 
				NPC.GetNPCColorTintedByBuffs(NPC.GetAlpha(drawColor)), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            }

            if (NPC.localAI[3] > 0)
            {
                Main.EntitySpriteDraw(ArmBackTexture.Value, NPC.Center - screenPos + new Vector2(0, 4), NPC.frame, 
				NPC.GetNPCColorTintedByBuffs(NPC.GetAlpha(drawColor)), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            }
		}

		public override void FindFrame(int frameHeight)
        {
            //walking animation
            NPC.frameCounter++;
            if (NPC.localAI[0] <= 230)
            {
                if (NPC.frameCounter > 5)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 8)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }

                //jumping/falling frame
                if (NPC.velocity.Y < 0)
                {
                    NPC.frame.Y = 2 * frameHeight;
                }
                if (NPC.velocity.Y > 0)
                {
                    NPC.frame.Y = 3 * frameHeight;
                }
            }
            //attacking animation
            if (NPC.localAI[0] > 230)
            {
				if (!NPC.AnyNPCs(ModContent.NPCType<OpalHandDinoClaw>()))
				{
					if (NPC.frame.Y < frameHeight * 8)
					{
						NPC.frame.Y = 8 * frameHeight;
					}

					if (NPC.frameCounter > 5)
					{
						NPC.frame.Y = NPC.frame.Y + frameHeight;
						NPC.frameCounter = 0;
					}
					if (NPC.frame.Y >= frameHeight * 10)
					{
						NPC.frame.Y = 10 * frameHeight;
					}
				}
				else
				{
					NPC.frame.Y = 11 * frameHeight;
				}
            }
        }

		public bool HasValidHand()
		{
			foreach (var npc in Main.ActiveNPCs)
			{
				if (npc.type == ModContent.NPCType<OpalHandDinoClaw>() && npc.ai[3] == NPC.whoAmI)
				{
					return true;
				}
			}

			return false;
		}

		public override void AI()
		{
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;

            if (NPC.velocity.Y == 0)
            {
                NPC.localAI[0]++;
            }

            if (!HasValidHand())
            {
				NPC.defense = NPC.defDefense;

                if (NPC.localAI[0] <= 240)
                {
                    if (NPC.wet)
                    {
                        NPC.aiStyle = 1;
                        AIType = NPCID.Crab;
                    }
                    else
                    {
                        NPC.aiStyle = 3;
                        AIType = NPCID.Crab;
                    }
                }
            }
            else
            {
				NPC.defense = 200;
                NPC.aiStyle = 0;
            }

			if (NPC.localAI[0] == 240)
			{
				int Hand = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y + 15, ModContent.NPCType<OpalHandDinoClaw>(), ai1: 0, ai3: NPC.whoAmI);
				Main.npc[Hand].velocity = 10f * NPC.DirectionTo(player.Center);

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					NetMessage.SendData(MessageID.SyncNPC, number: Hand);
				}
			}
			if (NPC.localAI[0] == 300)
			{
				int Hand = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y + 15, ModContent.NPCType<OpalHandDinoClaw>(), ai1: 1, ai3: NPC.whoAmI);
				Main.npc[Hand].velocity = 10f * NPC.DirectionTo(player.Center);

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					NetMessage.SendData(MessageID.SyncNPC, number: Hand);
				}
			}

            if (NPC.localAI[0] >= 480 && NPC.localAI[1] > 1)
            {
                NPC.localAI[1] = 0;
                NPC.localAI[2] = 0;
                NPC.localAI[3] = 0;
                NPC.localAI[0] = 0;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DinoArmHook>(), 10));
		}

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numDusts = 0; numDusts < 25; numDusts++)
                {                                                                                  
                    int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Asphalt, 0f, -2f, 0, default, 1f);
                    Main.dust[dust].position.X += Main.rand.Next(-25, 25) * 0.05f - 1.5f;
                    Main.dust[dust].position.Y += Main.rand.Next(-25, 25) * 0.05f - 1.5f;
                }

                for (int numGores = 1; numGores <= 4; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/OpalHandDinoGore" + numGores).Type);
                    }
                }
            }
        }
    }
}