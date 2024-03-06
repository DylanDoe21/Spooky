using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Content.Dusts;
using Spooky.Content.Items.Food;
using Spooky.Content.Items.SpookyHell;
using Spooky.Content.NPCs.SpookyHell.Projectiles;

namespace Spooky.Content.NPCs.SpookyHell
{
    public class TortumorGiant : ModNPC
    {
        public static readonly SoundStyle DeathSound = new("Spooky/Content/Sounds/TortumorDeath", SoundType.Sound) { Volume = 0.75f };

		public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 8;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 450;
            NPC.damage = 40;
            NPC.defense = 20;
            NPC.width = 86;
            NPC.height = 78;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0.75f;
            NPC.value = Item.buyPrice(0, 0, 12, 0);
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
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.TortumorGiant"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/SpookyHell/TortumorGiantGlow").Value;

            Main.EntitySpriteDraw(tex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
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
                NPC.velocity = new Vector2(Main.rand.Next(-30, 31), Main.rand.Next(-22, -12));
                NPC.ai[2] = 1;
            }

            if (NPC.ai[0] >= 240)
            {
                NPC.velocity = new Vector2(Main.rand.Next(-30, 31), Main.rand.Next(-30, 31));
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
            //TODO: replace this with new yoyo when its finished
            //npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TortumorStaff>(), 2));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<EyeChocolate>(), 100));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                //spawn gores
                for (int numGores = 1; numGores <= 15; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(Main.rand.Next(-18, 19), Main.rand.Next(-18, 19)), ModContent.Find<ModGore>("Spooky/TortumorGore" + Main.rand.Next(1, 4)).Type, Main.rand.NextFloat(1.2f, 1.5f));
                    }
                }

                //spawn blood explosion clouds
                for (int numExplosion = 0; numExplosion < 15; numExplosion++)
                {
                    int DustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, Color.Purple * 0.65f, Main.rand.NextFloat(0.8f, 1.2f));
                    Main.dust[DustGore].velocity.X *= Main.rand.NextFloat(-5f, 5f);
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