using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Events;
using Spooky.Content.Items.SpookyHell.Boss;
using Spooky.Content.NPCs.EggEvent.Projectiles;

namespace Spooky.Content.NPCs.EggEvent
{
    public class Vesicator : ModNPC
    {
        public int MoveSpeedX = 0;
		public int MoveSpeedY = 0;

        float addedStretch = 0f;
		float stretchRecoil = 0f;

        public bool AfterImages = false;

        public static readonly SoundStyle HitSound = new("Spooky/Content/Sounds/SpookyHell/EnemyHit", SoundType.Sound);
        public static readonly SoundStyle DeathSound1 = new("Spooky/Content/Sounds/SpookyHell/EnemyDeath", SoundType.Sound);
        public static readonly SoundStyle DeathSound2 = new("Spooky/Content/Sounds/SpookyHell/EnemyDeath2", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vesicator");
            Main.npcFrameCount[NPC.type] = 5;

            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                CustomTexturePath = "Spooky/Content/NPCs/EggEvent/VesicatorBestiary"
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
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
            NPC.lifeMax = 3000;
            NPC.damage = 60;
            NPC.defense = 35;
            NPC.width = 138;
            NPC.height = 126;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = HitSound;
			NPC.DeathSound = DeathSound2;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[2] { ModContent.GetInstance<Biomes.EggEventBiome>().Type, ModContent.GetInstance<Biomes.SpookyHellBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("These massive blistered bags of flesh fly around, dropping explosive biomatter in their wake. Beware though, as these somehow living beings are chunks of explosive biomatter themselves."),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			Texture2D Tex = ModContent.Request<Texture2D>(Texture).Value;
			Texture2D GlowTex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/EggEvent/VesicatorGlow").Value;

			float stretch = 0f;

			stretch = Math.Abs(stretch) - addedStretch;
			
			//limit how much he can stretch
			if (stretch > 0.2f)
			{
				stretch = 0.2f;
			}

			//limit how much he can squish
			if (stretch < -0.2f)
			{
				stretch = -0.2f;
			}

			Vector2 scaleStretch = new Vector2(1f - stretch, 1f + stretch);

            spriteBatch.Draw(Tex, NPC.Center + new Vector2(0, NPC.height / 2 + NPC.gfxOffY) - Main.screenPosition, 
			NPC.frame, drawColor, NPC.rotation, new Vector2(NPC.width / 2, NPC.height), scaleStretch, SpriteEffects.None, 0f);

			spriteBatch.Draw(GlowTex, NPC.Center + new Vector2(0, NPC.height / 2 + NPC.gfxOffY) - Main.screenPosition, 
			NPC.frame, Color.White, NPC.rotation, new Vector2(NPC.width / 2, NPC.height), scaleStretch, SpriteEffects.None, 0f);

            return false;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

            if (player.InModBiome(ModContent.GetInstance<Biomes.EggEventBiome>()) && EggEventWorld.EggEventProgress >= 60 && !NPC.AnyNPCs(ModContent.NPCType<Vesicator>()))
            {
                return 10f;
            }

            return 0f;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1;

            //flying
            if (NPC.frameCounter > 6)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0.0;
            }
            if (NPC.frame.Y >= frameHeight * 4)
            {
                NPC.frame.Y = 0 * frameHeight;
            }

            //charging frame
            if (NPC.localAI[0] >= 420)
            {
                NPC.frame.Y = 4 * frameHeight;
            }
        }

        public override void AI()
		{
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);
            
            NPC.spriteDirection = NPC.direction;

            if (stretchRecoil > 0)
			{
				stretchRecoil *= 0.965f;
				stretchRecoil -= 0.02f;
			}
			else
			{
				stretchRecoil = 0;
			}

			addedStretch = -stretchRecoil;

            if (NPC.localAI[0] < 420)
            {
                NPC.rotation = NPC.velocity.X * 0.04f;
            }
            else
            {
                NPC.rotation = 0f;
            }

            NPC.localAI[0]++;

            if (NPC.localAI[0] < 390)
            {
                //flies to players X position
                if (NPC.Center.X >= player.Center.X && MoveSpeedX >= -50) 
                {
                    MoveSpeedX -= 3;
                }
                else if (NPC.Center.X <= player.Center.X && MoveSpeedX <= 50)
                {
                    MoveSpeedX += 3;
                }

                NPC.velocity.X = MoveSpeedX * 0.1f;
                
                //flies to players Y position
                if (NPC.Center.Y >= player.Center.Y - 220f && MoveSpeedY >= -35)
                {
                    MoveSpeedY--;
                }
                else if (NPC.Center.Y <= player.Center.Y - 220f && MoveSpeedY <= 35)
                {
                    MoveSpeedY++;
                }

                NPC.velocity.Y = MoveSpeedY * 0.1f;
            }

            if (NPC.localAI[0] == 420 || NPC.localAI[0] == 450 || NPC.localAI[0] == 480)
            {
                NPC.velocity *= 0.82f;

                stretchRecoil = 0.5f;

                SoundEngine.PlaySound(DeathSound1, NPC.Center);

                int NumProjectiles = Main.rand.Next(2, 3);
                for (int i = 0; i < NumProjectiles; i++)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y + 45, Main.rand.Next(-2, 2), Main.rand.Next(2, 5), 
                        ModContent.ProjectileType<BiomassGravity>(), NPC.damage / 2, 1, NPC.target, 0, 0);
                    }
                }
            }

            if (NPC.localAI[0] >= 500)
            {
                NPC.localAI[0] = 0;
            }
        }

        public override void HitEffect(int hitDirection, double damage) 
        {
            if (NPC.life <= 0) 
            {
                Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center.X, NPC.Center.Y, Main.rand.Next(-2, 2), -3, 
                ModContent.ProjectileType<VesicatorDeath>(), NPC.damage, 1, NPC.target, 0, 0);
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.ByCondition(new DropConditions.PostOrroboroCondition(), ModContent.ItemType<ArteryPiece>(), 3, 1, 3));
        }
    }
}