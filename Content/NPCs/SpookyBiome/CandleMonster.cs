using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.Food;
using Spooky.Content.NPCs.SpookyBiome.Projectiles;

namespace Spooky.Content.NPCs.SpookyBiome
{
    public class CandleMonster : ModNPC  
    {
        bool HasShotProjectiles = false;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(HasShotProjectiles);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            HasShotProjectiles = reader.ReadBoolean();
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 200;
            NPC.damage = 40;
            NPC.defense = 5;
            NPC.width = 32;
			NPC.height = 60;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 10, 0);
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 0;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyBiomeUg>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.CandleMonster"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyBiomeUg>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/SpookyBiome/CandleMonsterGlow").Value;

            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < 4; i++)
            {
                int XOffset = Main.rand.Next(-2, 3);
                int YOffset = Main.rand.Next(-2, 3);
                
                Main.EntitySpriteDraw(tex, NPC.Center - Main.screenPosition + new Vector2(XOffset, NPC.gfxOffY + 4 + YOffset), 
                NPC.frame, Color.White * 0.5f, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
            }
		}

        public override void FindFrame(int frameHeight)
        {
            //idle frame
            if (NPC.ai[0] < 350)
            {
                NPC.frame.Y = frameHeight * 0;
            }
            if (NPC.ai[0] == 350)
            {
                NPC.frame.Y = frameHeight * 1;
            }
            if (NPC.ai[0] == 380)
            {
                NPC.frame.Y = frameHeight * 2;
            }
            if (NPC.ai[0] == 410)
            {
                NPC.frame.Y = frameHeight * 3;
            }
        }
        
        public override void AI()
		{
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

			NPC.spriteDirection = NPC.direction;

            NPC.ai[0]++;

            if (NPC.ai[0] == 410)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath13, NPC.Center);

                for (int numProjectiles = 0; numProjectiles < 4; numProjectiles++)
                {
                    Vector2 ShootSpeed = player.Center - new Vector2(NPC.Center.X, NPC.Center.Y + 10);
                    ShootSpeed.Normalize();
                    ShootSpeed *= 15f;

                    Vector2 newVelocity = ShootSpeed.RotatedByRandom(MathHelper.ToRadians(18));

                    Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(NPC.Center.X, NPC.Center.Y + 10), newVelocity, ModContent.ProjectileType<MoltenCandleChunk>(), NPC.damage / 4, 0, player.whoAmI);
                }

                NPC.ai[2] = 0;
                HasShotProjectiles = false;

                NPC.netUpdate = true;
            }

            if (NPC.ai[0] >= 420)
            {
                NPC.ai[0] = 0;
            }
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 10; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/CandleMonsterGore").Type);
                    }
                }
            }
        }
    }
}