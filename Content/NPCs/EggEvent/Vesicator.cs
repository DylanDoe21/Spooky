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
using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.NPCs.EggEvent.Projectiles;

namespace Spooky.Content.NPCs.EggEvent
{
    public class Vesicator : ModNPC
    {
        float addedStretch = 0f;
		float stretchRecoil = 0f;

        public static readonly SoundStyle HitSound = new("Spooky/Content/Sounds/EggEvent/EnemyHit", SoundType.Sound);
        public static readonly SoundStyle DeathSound1 = new("Spooky/Content/Sounds/EggEvent/EnemyDeath", SoundType.Sound);
        public static readonly SoundStyle DeathSound2 = new("Spooky/Content/Sounds/EggEvent/EnemyDeath2", SoundType.Sound);
        public static readonly SoundStyle ScreamSound = new("Spooky/Content/Sounds/EggEvent/VesicatorScream", SoundType.Sound) { PitchVariance = 0.6f };

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/VesicatorBestiary"
            };

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
            NPC.lifeMax = 3500;
            NPC.damage = 50;
            NPC.defense = 20;
            NPC.width = 152;
            NPC.height = 142;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 2, 0, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = HitSound;
			NPC.DeathSound = DeathSound2;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[2] { ModContent.GetInstance<Biomes.SpookyHellBiome>().Type, ModContent.GetInstance<Biomes.SpookyHellEventBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Vesicator"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellEventBiome>().ModBiomeBestiaryInfoElement)
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

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 7)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 5)
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
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];
            
            NPC.spriteDirection = NPC.direction;

            if (stretchRecoil > 0)
			{
				stretchRecoil -= 0.03f;
			}
			else
			{
				stretchRecoil = 0;
			}

			addedStretch = -stretchRecoil;

            NPC.rotation = NPC.velocity.X * 0.04f;

            switch ((int)NPC.ai[0])
            {
                //fly above the player
                case 0:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] <= 300)
                    {
                        Vector2 GoTo = new Vector2(player.Center.X, player.Center.Y - 350);

                        float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 6, Main.rand.Next(7, 12));
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                    }

                    if (NPC.localAI[0] > 300)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[0]++;

                        NPC.netUpdate = true;
                    }

                    break;
                }

                //spit out giant biomass chunk
                case 1:
                {
                    NPC.localAI[0]++;

                    NPC.velocity *= 0.82f;

                    if (NPC.localAI[0] == 50)
                    {
                        SoundEngine.PlaySound(ScreamSound, NPC.Center);
                    }

                    //do squishing animation before spitting
                    if (NPC.localAI[0] == 60 || NPC.localAI[0] == 90 || NPC.localAI[0] == 120)
                    {
                        stretchRecoil = 0.5f;

                        SoundEngine.PlaySound(DeathSound1, NPC.Center);
                    }

                    //spit out giant biomass
                    if (NPC.localAI[0] == 120)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y + 55, Main.rand.Next(-2, 2), 
                        Main.rand.Next(2, 5), ModContent.ProjectileType<GiantBiomassVesicator>(), NPC.damage / 3, 0, NPC.target, Main.rand.Next(0, 2));
                    }

                    if (NPC.localAI[0] > 160)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[0] = 0;

                        NPC.netUpdate = true;
                    }

                    break;
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.ByCondition(new DropConditions.PostOrroboroCondition(), ModContent.ItemType<ArteryPiece>(), 3, 1, 3));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center.X, NPC.Center.Y, NPC.velocity.X, 
                NPC.velocity.Y, ModContent.ProjectileType<VesicatorDeath>(), NPC.damage, 0, NPC.target, 0, 0);
            }
        }
    }
}