using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;

using Spooky.Content.Dusts;
using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.NPCs.Cemetery
{
    public class PossessorEvil : ModNPC
    {
        public int MoveSpeedX = 0;
		public int MoveSpeedY = 0;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //ints
            writer.Write(MoveSpeedX);
            writer.Write(MoveSpeedY);

            //floats
            writer.Write(NPC.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //ints
            MoveSpeedX = reader.ReadInt32();
            MoveSpeedY = reader.ReadInt32();

            //floats
            NPC.localAI[0] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 60;
            NPC.damage = 25;
            NPC.defense = 5;
            NPC.width = 52;
			NPC.height = 36;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit54;
            NPC.DeathSound = SoundID.NPCDeath52;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CemeteryBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Possessor"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CemeteryBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //draw aura
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            Vector2 drawOrigin = new(tex.Width * 0.5f, NPC.height * 0.5f);

            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int numEffect = 0; numEffect < 4; numEffect++)
            {
                Color color = new Color(125 - NPC.alpha, 125 - NPC.alpha, 125 - NPC.alpha, 0).MultiplyRGBA(Color.Lerp(Color.White, Color.Purple, numEffect));

                Color newColor = color;
                newColor = NPC.GetAlpha(newColor);
                newColor *= 1f;
                Vector2 vector = new Vector2(NPC.Center.X - 2, NPC.Center.Y) + (numEffect / 4 * 6f + NPC.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 3) * numEffect;
                Main.EntitySpriteDraw(tex, vector, NPC.frame, newColor, NPC.rotation, drawOrigin, NPC.scale * 1.035f, effects, 0);
            }
            
            return true;
		}

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 6)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 4)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            for (int numDusts = 0; numDusts < 15; numDusts++)
            {
                int dustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<GlowyDust>(), 0f, -2f, 0, default, 0.1f);
                Main.dust[dustGore].color = Color.BlueViolet;
                Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-2f, 2f);
                Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-2f, 2f);
                Main.dust[dustGore].noGravity = true;
            }
            
            target.AddBuff(ModContent.BuffType<Possessed>(), 600);
            NPC.active = false;
        }

        public override void AI()
		{
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);
            
            NPC.spriteDirection = NPC.direction;
            NPC.rotation = NPC.velocity.X * 0.05f;

            NPC.localAI[0]++;

            if (NPC.localAI[0] <= 40)
            {
                NPC.velocity *= 0;
            }
            else
            {
                //flies to players X position
                if (NPC.Center.X >= player.Center.X && MoveSpeedX >= -30) 
                {
                    MoveSpeedX--;
                }
                else if (NPC.Center.X <= player.Center.X && MoveSpeedX <= 30)
                {
                    MoveSpeedX++;
                }

                NPC.velocity.X = MoveSpeedX * 0.1f;
                
                //flies to players Y position
                if (NPC.Center.Y >= player.Center.Y && MoveSpeedY >= -25)
                {
                    MoveSpeedY--;
                }
                else if (NPC.Center.Y <= player.Center.Y && MoveSpeedY <= 25)
                {
                    MoveSpeedY++;
                }

                NPC.velocity.Y = MoveSpeedY * 0.1f;
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0) 
            {
                for (int numDusts = 0; numDusts < 15; numDusts++)
                {
                    int dustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<GlowyDust>(), 0f, -2f, 0, default, 0.1f);
                    Main.dust[dustGore].color = Color.BlueViolet;
                    Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-2f, 2f);
                    Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-2f, 2f);
                    Main.dust[dustGore].noGravity = true;
                }
            }
        }
    }
}