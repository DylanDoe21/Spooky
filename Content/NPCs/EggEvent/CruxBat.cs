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

using Spooky.Core;
using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.NPCs.EggEvent.Projectiles;

namespace Spooky.Content.NPCs.EggEvent
{
    public class CruxBat : ModNPC
    {
        Vector2 SavePlayerPosition;

        private static Asset<Texture2D> GlowTexture;

        public static readonly SoundStyle ScreamSound = new("Spooky/Content/Sounds/EggEvent/CruxBatScream", SoundType.Sound) { PitchVariance = 0.6f };

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //vector2
            writer.WriteVector2(SavePlayerPosition);

            //floats
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //vector2
			SavePlayerPosition = reader.ReadVector2();

            //floats
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 420;
            NPC.damage = 40;
            NPC.defense = 5;
            NPC.width = 98;
            NPC.height = 122;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit9;
            NPC.DeathSound = SoundID.NPCDeath22;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[2] { ModContent.GetInstance<Biomes.SpookyHellBiome>().Type, ModContent.GetInstance<Biomes.SpookyHellEventBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.CruxBat"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellEventBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/EggEvent/CruxBatGlow");

            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
            NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 4)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 6)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }
        
        public override void AI()
		{
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];
            
            NPC.spriteDirection = NPC.direction;

            NPC.rotation = NPC.velocity.X * 0.03f;

            switch ((int)NPC.ai[0])
            {
                //fly to random locations around the player
                case 0:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[1] < 2)
                    {
                        if (NPC.localAI[0] == 5)
                        {
                            SavePlayerPosition = new Vector2(player.Center.X + Main.rand.Next(-200, 200), player.Center.Y - 250);
                        }

                        //go to a random location
                        if (NPC.localAI[0] > 50 && NPC.localAI[0] < 100)
                        {
                            Vector2 GoTo = SavePlayerPosition;

                            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 5, 15);
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                        }

                        if (NPC.localAI[0] > 100)
                        {
                            NPC.velocity *= 0.9f;
                        }

                        if (NPC.localAI[0] > 150)
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

                //fly at the player, create debuff aura
                case 1:
                {
                    NPC.localAI[0]++;

                    Vector2 GoTo = player.Center;

                    float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 2, 6);
                    NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);

                    //play sound, spawn aura
                    if (NPC.localAI[0] == 60)
                    {
                        SoundEngine.PlaySound(ScreamSound, NPC.Center);

                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CruxAura>(), 0, 0, NPC.target, NPC.whoAmI);
                    }

                    //make sure the aura is always on the center of the crux
                    if (NPC.localAI[0] > 60)
                    {
                        NPC.velocity *= 0.99f;
                    }

                    if (NPC.localAI[0] > 200)
                    {
                        NPC.velocity *= 0;

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
            //npcLoot.Add(ItemDropRule.ByCondition(new DropConditions.PostOrroboroCondition(), ModContent.ItemType<ArteryPiece>(), 3, 1, 3));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
				{
                    //spawn splatter
                    for (int i = 0; i < 5; i++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center.X, NPC.Center.Y, Main.rand.Next(-4, 5), Main.rand.Next(-4, -1), ModContent.ProjectileType<RedSplatter>(), 0, 0);
                    }
                }

                for (int numGores = 1; numGores <= 6; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/CruxBatGore" + numGores).Type);
                    }
                }
            }
        }
    }
}