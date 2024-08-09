using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Content.Dusts;
using Spooky.Content.Items.Food;

namespace Spooky.Content.NPCs.SpookyHell
{
    public class Tortumor : ModNPC
    {
        private static Asset<Texture2D> GlowTexture;

        public static readonly SoundStyle DeathSound = new("Spooky/Content/Sounds/TortumorDeath", SoundType.Sound) { Volume = 0.6f };

		public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 8;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 75;
            NPC.damage = 25;
            NPC.defense = 18;
            NPC.width = 60;
            NPC.height = 60;
            NPC.npcSlots = 1f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit9;
            NPC.DeathSound = DeathSound;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyHellBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Tortumor"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/SpookyHell/TortumorGlow");

            Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
            NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 8)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 8)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }
        
        public override void AI()
		{
            NPC.rotation += (NPC.velocity.X / 30);
            NPC.velocity *= 0.96f;

            NPC.ai[0]++;
        
            //immediately try to fling upward when it spawns in
            if (NPC.ai[2] == 0)
            {
                NPC.velocity = new Vector2(Main.rand.Next(-13, 14), Main.rand.Next(-13, -5));
                NPC.ai[2] = 1;
            }

            if (NPC.ai[0] >= 300)
            {
                NPC.velocity = new Vector2(Main.rand.Next(-13, 14), Main.rand.Next(-13, 14));
                NPC.ai[0] = 0;
                NPC.ai[1] = 0;
            }

            //bounce off of tiles
            if (NPC.ai[1] == 0 && Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
            {
                NPC.velocity = -NPC.oldVelocity;

                NPC.ai[1] = 1;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<EyeChocolate>(), 100));
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
			if (NPC.life <= 0)
            {
                //spawn gores
                for (int numGores = 1; numGores <= 8; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(Main.rand.Next(-12, 13), Main.rand.Next(-12, 13)), ModContent.Find<ModGore>("Spooky/TortumorGore" + Main.rand.Next(1, 4)).Type, Main.rand.NextFloat(1f, 1.35f));
                    }
                }

                //spawn blood explosion clouds
                for (int numExplosion = 0; numExplosion < 8; numExplosion++)
                {
                    int DustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, Color.Indigo * 0.65f, Main.rand.NextFloat(0.5f, 0.8f));
                    Main.dust[DustGore].velocity.X *= Main.rand.NextFloat(-4f, 4f);
                    Main.dust[DustGore].noGravity = true;

                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[DustGore].scale = 0.5f;
                        Main.dust[DustGore].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
            }
        }
    }
}