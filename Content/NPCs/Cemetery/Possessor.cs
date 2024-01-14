using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.Cemetery
{
	public class Possessor : ModNPC
	{
        public static readonly SoundStyle SpawnSound = new("Spooky/Content/Sounds/PossessorLaugh", SoundType.Sound);

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 60;
            NPC.damage = 0;
			NPC.defense = 0;
			NPC.width = 52;
			NPC.height = 36;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.noGravity = true;
			NPC.HitSound = SoundID.NPCHit54;
            NPC.DeathSound = SoundID.NPCDeath52;
			NPC.aiStyle = 114;
			AIType = NPCID.BlackDragonfly;
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

        public override void AI()
		{
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];
            
            NPC.spriteDirection = NPC.direction;
            NPC.rotation = NPC.velocity.X * 0.05f;

            if (player.Distance(NPC.Center) <= 200f)
            {
                SoundEngine.PlaySound(SpawnSound, NPC.Center);

                NPC.Transform(ModContent.NPCType<PossessorEvil>());

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

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0) 
            {
                NPC BestiaryParent = new();
                BestiaryParent.SetDefaults(ModContent.NPCType<PossessorEvil>());
                Main.BestiaryTracker.Kills.RegisterKill(BestiaryParent);

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