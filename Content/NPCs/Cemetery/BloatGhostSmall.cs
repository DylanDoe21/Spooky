using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.Cemetery
{
	public class BloatGhostSmall : ModNPC
	{
        private static Asset<Texture2D> NPCTexture;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };

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
            NPC.lifeMax = 10;
            NPC.damage = 0;
			NPC.defense = 0;
			NPC.width = 14;
			NPC.height = 22;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.noGravity = true;
			NPC.HitSound = SoundID.NPCHit54 with { Pitch = 1.2f };
            NPC.DeathSound = SoundID.NPCDeath6;
			NPC.aiStyle = 64;
			AIType = NPCID.Firefly;
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //draw aura
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);

            //draw aura
            for (int i = 0; i < 360; i += 90)
            {
                Color color = new Color(125 - NPC.alpha, 125 - NPC.alpha, 125 - NPC.alpha, 0).MultiplyRGBA(Color.White);

                Vector2 circular = new Vector2(Main.rand.NextFloat(1f, 2f), 0).RotatedBy(MathHelper.ToRadians(i));

                spriteBatch.Draw(NPCTexture.Value, NPC.Center + circular - screenPos + new Vector2(0, NPC.gfxOffY + 4), NPC.frame, color, NPC.rotation, NPC.frame.Size() / 2, NPC.scale * 1.075f, SpriteEffects.None, 0f);
            }

            return true;
        }
        
        public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter++;
            if (NPC.frameCounter > 7)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 4)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
		}

        public override void AI()
		{
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];
    
            NPC.rotation = NPC.velocity.X * 0.05f;

            if (player.Distance(NPC.Center) <= 150f)
            {
                NPC.localAI[0]++;

                if (NPC.localAI[0] > 20)
                {
                    SoundEngine.PlaySound(SoundID.AbigailUpgrade, NPC.Center);

                    int BigGhost = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y + 20, ModContent.NPCType<BloatGhostBig>());
                    
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NetMessage.SendData(MessageID.SyncNPC, number: BigGhost);
                    }

                    NPC.active = false;
                }
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0) 
            {
                NPC BestiaryParent = new();
                BestiaryParent.SetDefaults(ModContent.NPCType<BloatGhostBig>());
                Main.BestiaryTracker.Kills.RegisterKill(BestiaryParent);

                for (int numDusts = 0; numDusts < 15; numDusts++)
                {
                    int dustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<GlowyDust>(), 0f, -2f, 0, default, 0.1f);
                    Main.dust[dustGore].color = Color.White;
                    Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-2f, 2f);
                    Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-2f, 2f);
                    Main.dust[dustGore].noGravity = true;
                }
            }
        }
	}
}