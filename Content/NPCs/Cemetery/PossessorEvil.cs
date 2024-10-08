using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using ReLogic.Content;
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

        private static Asset<Texture2D> NPCTexture;

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
            NPC.knockBackResist = 0.8f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit54 with { Pitch = 1.2f };
            NPC.DeathSound = SoundID.NPCDeath6;
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
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);

            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            //draw aura
            for (int i = 0; i < 360; i += 90)
            {
                Color color = new Color(125 - NPC.alpha, 125 - NPC.alpha, 125 - NPC.alpha, 0).MultiplyRGBA(Color.Lerp(Color.White, Color.Purple, i / 30));

                Vector2 circular = new Vector2(Main.rand.NextFloat(1f, 2f), 0).RotatedBy(MathHelper.ToRadians(i));

                spriteBatch.Draw(NPCTexture.Value, NPC.Center + circular - screenPos + new Vector2(0, NPC.gfxOffY + 4), NPC.frame, color, NPC.rotation, NPC.frame.Size() / 2, NPC.scale * 1.075f, effects, 0f);
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
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];
            
            NPC.spriteDirection = NPC.direction;
            NPC.rotation = NPC.velocity.X * 0.05f;

            NPC.localAI[0]++;

            if (NPC.localAI[0] <= 40)
            {
                NPC.velocity *= 0;
            }
            else
            {
                int MaxSpeed = 2;

                //flies to players X position
                if (NPC.Center.X >= player.Center.X && MoveSpeedX >= -MaxSpeed - 2) 
                {
                    MoveSpeedX--;
                }
                else if (NPC.Center.X <= player.Center.X && MoveSpeedX <= MaxSpeed + 2)
                {
                    MoveSpeedX++;
                }

                NPC.velocity.X += MoveSpeedX * 0.01f;
                NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -MaxSpeed - 2, MaxSpeed + 2);
                
                //flies to players Y position
                if (NPC.Center.Y >= player.Center.Y && MoveSpeedY >= -MaxSpeed)
                {
                    MoveSpeedY--;
                }
                else if (NPC.Center.Y <= player.Center.Y && MoveSpeedY <= MaxSpeed)
                {
                    MoveSpeedY++;
                }

                NPC.velocity.Y += MoveSpeedY * 0.01f;
                NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y, -MaxSpeed, MaxSpeed);
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