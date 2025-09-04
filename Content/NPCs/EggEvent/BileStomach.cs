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
using Spooky.Content.Items.SpookyHell.EggEvent;
using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.NPCs.EggEvent.Projectiles;

namespace Spooky.Content.NPCs.EggEvent
{
    public class BileStomach : ModNPC
    {
        private static Asset<Texture2D> NPCTexture;
        private static Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 11;
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //floats
            writer.Write(NPC.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //floats
            NPC.localAI[0] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 1200;
            NPC.damage = 45;
            NPC.defense = 10;
            NPC.width = 74;
            NPC.height = 72;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 2, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.Item177;
			NPC.DeathSound = SoundID.NPCDeath12;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[2] { ModContent.GetInstance<Biomes.SpookyHellBiome>().Type, ModContent.GetInstance<Biomes.SpookyHellEventBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.BileStomach"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellEventBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/EggEvent/BileStomachGlow");

            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
            NPC.frame, Color.White * 0.2f, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);

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

            if (NPC.ai[0] == 0)
            {
                if (NPC.frame.Y >= frameHeight * 8)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
            }
            else
            {
                if (NPC.frame.Y < frameHeight * 8)
                {
                    NPC.frame.Y = 8 * frameHeight;
                }
                
                if (NPC.frame.Y >= frameHeight * 11)
                {
                    NPC.frame.Y = 8 * frameHeight;
                }
            }
        }

        public override void AI()
		{
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];
            
            NPC.spriteDirection = NPC.direction;
            
            NPC.rotation = NPC.velocity.Y * (NPC.direction == 1 ? 0.04f : -0.04f);

            switch ((int)NPC.ai[0])
            {
                //fly to the player
                case 0:
                {
                    NPC.localAI[0]++;

                    if (NPC.Distance(player.Center) >= 100)
                    {
                        Vector2 GoTo = player.Center;

                        float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 1, 3);
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                    }
                    else
                    {
                        NPC.velocity *= 0.98f;
                    }

                    if (NPC.localAI[0] >= 180)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[0]++;

                        NPC.netUpdate = true;
                    }

                    break;
                }

                //shoot brain confusion laser
                case 1:
                {
                    NPC.velocity *= 0.5f;

                    NPC.localAI[0]++;

                    if (NPC.localAI[0] % 10 == 0)
                    {
                        SoundEngine.PlaySound(SoundID.Item171, NPC.Center);

                        NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(NPC.Center.X + (NPC.direction == -1 ? -8 : 8), NPC.Top.Y),
                        new Vector2((NPC.direction == -1 ? Main.rand.Next(-10, 0) : Main.rand.Next(1, 11)), Main.rand.Next(-10, -4)), ModContent.ProjectileType<StomachBile>(), NPC.damage, 4.5f);
                    }

                    if (NPC.localAI[0] == 180)
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
            npcLoot.Add(ItemDropRule.ByCondition(new DropConditions.PostOrroboroCondition(), ModContent.ItemType<ArteryPiece>(), 5, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BileGlob>()));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
				{
                    //spawn splatter
                    for (int i = 0; i < 8; i++)
                    {
                        NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, new Vector2(Main.rand.Next(-4, 5), Main.rand.Next(-4, -1)), ModContent.ProjectileType<YellowSplatter>(), 0, 0f);
                    }
                }

                for (int numGores = 1; numGores <= 5; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/BileStomachGore" + numGores).Type);
                    }
                }
            }
        }
    }
}