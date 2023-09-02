using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.Pets;
using Spooky.Content.Items.SpookyHell.Misc;

namespace Spooky.Content.NPCs.EggEvent
{
    public class Vigilante : ModNPC
    {
        int repeats = Main.rand.Next(2, 5);

        public int SaveDirection;
        public float SaveRotation;

        Vector2 SavePosition;
        Vector2 SavePlayerPosition;

        public static readonly SoundStyle HitSound = new("Spooky/Content/Sounds/EggEvent/EnemyHit", SoundType.Sound);
        public static readonly SoundStyle DeathSound = new("Spooky/Content/Sounds/EggEvent/EnemyDeath", SoundType.Sound);
        public static readonly SoundStyle ScreamSound = new("Spooky/Content/Sounds/EggEvent/VigilanteCharge", SoundType.Sound) { PitchVariance = 0.6f };

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.TrailCacheLength[NPC.type] = 7;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Rotation = MathHelper.PiOver2,
                Position = new Vector2(12f, -12f),
                PortraitPositionXOverride = 6f,
                PortraitPositionYOverride = 0f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);

            NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] 
                {
                    BuffID.Confused,
                }
            };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //ints
            writer.Write(repeats);
            writer.Write(SaveDirection);
            writer.Write(SavePosition.X);
            writer.Write(SavePosition.X);

            //floats
            writer.Write(SaveRotation);
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //ints
            repeats = reader.ReadInt32();
            SaveDirection = reader.ReadInt32();
            SavePosition.X = reader.ReadInt32();
            SavePosition.X = reader.ReadInt32();

            //floats
            SaveRotation = reader.ReadSingle();
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 1000;
            NPC.damage = 50;
            NPC.defense = 10;
            NPC.width = 92;
            NPC.height = 118;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = HitSound;
			NPC.DeathSound = DeathSound;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[2] { ModContent.GetInstance<Biomes.SpookyHellBiome>().Type, ModContent.GetInstance<Biomes.SpookyHellEventBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Vigilante"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellEventBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			if (NPC.ai[0] == 1 && NPC.localAI[0] >= 75) 
			{
                Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
				Vector2 drawOrigin = new(tex.Width * 0.5f, (NPC.height * 0.5f));

				for (int oldPos = 0; oldPos < NPC.oldPos.Length; oldPos++)
				{
					var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
					Vector2 drawPos = NPC.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, NPC.gfxOffY + 4);
					Color color = NPC.GetAlpha(Color.Red) * (float)(((float)(NPC.oldPos.Length - oldPos) / (float)NPC.oldPos.Length) / 2);
					spriteBatch.Draw(tex, drawPos, new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, drawOrigin, NPC.scale, effects, 0f);
				}
			}
            
            return true;
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/EggEvent/VigilanteGlow").Value;

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(tex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
            NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1;

            if (NPC.frameCounter > 6)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0.0;
            }
            if (NPC.frame.Y >= frameHeight * 4)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override bool CheckActive()
        {
            return !EggEventWorld.EggEventActive;
        }

        public override void AI()
		{
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);
            
            if (NPC.ai[0] == 0 || (NPC.ai[0] == 1&& NPC.localAI[0] < 75))
            {
                NPC.spriteDirection = NPC.direction;
            }

            //EoC rotation
            Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
            float RotateX = player.Center.X - vector.X;
            float RotateY = player.Center.Y - vector.Y;
            NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

            switch ((int)NPC.ai[0])
            {
                //fly to different locations above the player
                case 0:
                {
                    NPC.localAI[0]++;
                    
                    if (NPC.localAI[1] < repeats)
                    {
                        if (NPC.localAI[0] == 5)
                        {
                            SavePlayerPosition = new Vector2(player.Center.X + Main.rand.Next(-270, 270), player.Center.Y - Main.rand.Next(180, 220));
                        }

                        if (NPC.localAI[0] > 5 && NPC.localAI[0] <= 120)
                        {
                            Vector2 GoTo = SavePlayerPosition;

                            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 6, Main.rand.Next(7, 12));
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                        }

                        if (NPC.localAI[0] >= 120)
                        {
                            NPC.localAI[0] = 0;
                            NPC.localAI[1]++;

                            NPC.netUpdate = true;
                        }
                    }
                    else
                    {
                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = 0;
                        NPC.ai[0]++;

                        NPC.netUpdate = true;
                    }

                    break;
                }

                //shake, then charge at the player super quickly
                case 1:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] == 5)
                    {
                        SoundEngine.PlaySound(ScreamSound, NPC.Center);

                        NPC.velocity *= 0;

                        SavePosition = NPC.Center;
                    }

                    if (NPC.localAI[0] > 5 && NPC.localAI[0] < 65)
                    {
                        NPC.Center = new Vector2(SavePosition.X, SavePosition.Y);
                        NPC.Center += Main.rand.NextVector2Square(-7, 7);
                    }

                    if (NPC.localAI[0] == 70)
                    {
                        SavePlayerPosition = player.Center;
                    }

                    //save rotation before charging 
                    if (NPC.localAI[0] == 74)
                    {
                        SaveRotation = NPC.rotation;
                        SaveDirection = NPC.direction;
                    }

                    //charge
                    if (NPC.localAI[0] == 75)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersAttack, NPC.Center);

                        Vector2 ChargeDirection = SavePlayerPosition - NPC.Center;
                        ChargeDirection.Normalize();
                                
                        ChargeDirection *= Main.rand.Next(45, 50);
                        NPC.velocity = ChargeDirection;
                    }

                    if (NPC.localAI[0] >= 75)
                    {   
                        NPC.rotation = SaveRotation;
                        NPC.spriteDirection = SaveDirection;
                    }

                    if (NPC.localAI[0] >= 85)
                    {
                        NPC.velocity *= 0.85f;
                    }

                    if (NPC.localAI[0] >= 135)
                    {   
                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = 0;
                        NPC.ai[0] = 0;

                        NPC.netUpdate = true;
                    }

                    break;
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<StickyEye>(), 300));
            npcLoot.Add(ItemDropRule.ByCondition(new DropConditions.PostOrroboroCondition(), ModContent.ItemType<ArteryPiece>(), 3, 1, 3));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 6; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/VigilanteGore" + numGores).Type);
                    }
                }
            }
        }
    }
}