using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Dusts;
using Spooky.Content.NPCs.Minibiomes.Christmas.Projectiles;

namespace Spooky.Content.NPCs.Minibiomes.Christmas
{
    public class ChefRobot : ModNPC  
    {
        private static Asset<Texture2D> GlowTexture;

        public static readonly SoundStyle DingSound = new("Spooky/Content/Sounds/ChefRobotDing", SoundType.Sound) { PitchVariance = 0.6f };

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 7;
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 135;
            NPC.damage = 30;
            NPC.defense = 15;
            NPC.width = 48;
			NPC.height = 78;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.2f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.HitSound = SoundID.NPCHit4;
			NPC.DeathSound = SoundID.Item14;
            NPC.aiStyle = 3;
            AIType = NPCID.GoblinScout;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.ChristmasDungeonBiome>().Type };
        }

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
			{
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.ChefRobot"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.ChristmasDungeonBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Minibiomes/Christmas/ChefRobotGlow");

            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Color color = new Color(125 - NPC.alpha, 125 - NPC.alpha, 125 - NPC.alpha, 0).MultiplyRGBA(Color.White);

            for (int i = 0; i < 3; i++)
            {
                int XOffset = Main.rand.Next(-2, 3);
                int YOffset = Main.rand.Next(-2, 3);
                
                Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - Main.screenPosition + new Vector2(XOffset, NPC.gfxOffY + 4 + YOffset), NPC.frame, color, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
            }
        }

		public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 5)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }

            if (NPC.localAI[0] < 330)
            {
                if (NPC.frame.Y >= frameHeight * 4)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
            }
            else
            {
                if (NPC.frame.Y >= frameHeight * 7)
                {
                    NPC.frame.Y = 6 * frameHeight;
                }
            }
        }
        
        public override void AI()
		{
            NPC.spriteDirection = NPC.direction;

            NPC.localAI[0]++;

            if (NPC.localAI[0] < 330)
            {
                NPC.aiStyle = 3;
                AIType = NPCID.GoblinScout;
            }
            else
            {
                NPC.aiStyle = 0;
            }

            Vector2 NPCShootFromPos = NPC.Center + new Vector2(NPC.spriteDirection == -1 ? -29 : 29, -2);

            if (NPC.localAI[0] == 330)
            {   
                SoundEngine.PlaySound(DingSound, NPC.Center);

                NPC.velocity.X = 0;
            }

            if (NPC.localAI[0] >= 360 && NPC.localAI[0] <= 420)
            {
                Vector2 ShootSpeed = new Vector2(NPC.spriteDirection == -1 ? -5 : 5, 0);

                if (NPC.localAI[0] % 20 == 0)
                {
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath, NPC.Center);
                }

                if (NPC.localAI[0] % 3 == 0)
                {
                    Vector2 newVelocity = ShootSpeed.RotatedByRandom(MathHelper.ToRadians(5));

                    NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center + new Vector2(0, 20), newVelocity, ModContent.ProjectileType<ChefRobotFire>(), NPC.damage, 4.5f);
                }
            }

            if (NPC.localAI[0] >= 480)
            {
                NPC.localAI[0] = 0;
                NPC.netUpdate = true;
            }
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0)
            {
                for (int numGores = 1; numGores <= 7; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server)
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ChefRobotGore" + numGores).Type);
                    }
                }
            }
        }
    }
}