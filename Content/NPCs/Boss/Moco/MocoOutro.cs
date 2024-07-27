using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.Boss.Moco
{
    public class MocoOutro : ModNPC
    {
        public override string Texture => "Spooky/Content/NPCs/SpookyHell/Mocling";

        private static Asset<Texture2D> NPCTexture;
        private static Asset<Texture2D> GlowTexture;

        public static readonly SoundStyle FlyingSound = new("Spooky/Content/Sounds/Moco/MocoFlying", SoundType.Sound) { Pitch = 0.45f, Volume = 0.5f };

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 9;
            NPCID.Sets.TrailCacheLength[NPC.type] = 7;
            NPCID.Sets.TrailingMode[NPC.type] = 1;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void SetDefaults()
		{
            NPC.lifeMax = 5;
            NPC.width = 38;
            NPC.height = 36;
            NPC.npcSlots = 0f;
			NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.immortal = true;
			NPC.dontTakeDamage = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int oldPos = 0; oldPos < NPC.oldPos.Length; oldPos++)
            {
                Vector2 drawPos = NPC.oldPos[oldPos] - screenPos + NPC.frame.Size() / 2 + new Vector2(0, NPC.gfxOffY + 4);
                Color color = NPC.GetAlpha(Color.Lime) * (float)(((float)(NPC.oldPos.Length - oldPos) / (float)NPC.oldPos.Length) / 2);

                spriteBatch.Draw(NPCTexture.Value, drawPos, NPC.frame, color, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
            }

            return true;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/SpookyHell/MoclingGlow");

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 2)
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
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.rotation = 0;
            NPC.spriteDirection = NPC.velocity.X < 0 ? -1 : 1;

            NPC.ai[0]++;

            if (NPC.ai[0] < 240)
            {
                if (NPC.ai[0] % 60 == 0)
                {
                    SoundEngine.PlaySound(FlyingSound, NPC.Center);

                    MoveToPlayer(player, 0f, -250f);
                }
                else
                {
                    NPC.velocity *= 0.9f;
                }
            }

            if (NPC.ai[0] == 300)
            {
                SoundEngine.PlaySound(FlyingSound, NPC.Center);

                MoveToPlayer(player, player.Center.X < NPC.Center.X ? -500f : 500f, -250f);
            }

            if (NPC.ai[0] >= 300)
            {
                NPC.EncourageDespawn(60);
            }
        }

        //super fast movement to create a fly-like movement effect
        public void MoveToPlayer(Player target, float TargetPositionX, float TargetPositionY)
        {
            Vector2 GoTo = target.Center + new Vector2(TargetPositionX, TargetPositionY);

            if (NPC.Distance(GoTo) >= 200f)
            { 
                GoTo -= NPC.DirectionTo(GoTo) * 100f;
            }

            Vector2 GoToVelocity = GoTo - NPC.Center;

            float lerpValue = Utils.GetLerpValue(100f, 600f, GoToVelocity.Length(), false);

            float velocityLength = GoToVelocity.Length();

            if (velocityLength > 18f)
            { 
                velocityLength = 18f;
            }

            NPC.velocity = Vector2.Lerp(GoToVelocity.SafeNormalize(Vector2.Zero) * velocityLength, GoToVelocity / 6f, lerpValue);
            NPC.netUpdate = true;
        }
    }
}