using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.NPCs.SpiderCave.Projectiles
{
	public class WhipSpiderTongue : ModNPC
	{
        int SaveDirection;
        float SaveRotation;

        private static Asset<Texture2D> ChainTexture;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 5;
            NPC.damage = 45;
            NPC.defense = 0;
            NPC.width = 22;
            NPC.height = 6;
            NPC.noTileCollide = false;
            NPC.noGravity = true;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.aiStyle = -1;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPC Parent = Main.npc[(int)NPC.ai[3]];

            //only draw if the parent is active
            if (Parent.active && Parent.type == ModContent.NPCType<WhipSpider>())
            {
                ChainTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/SpiderCave/Projectiles/WhipSpiderTongueSegment");

                Vector2 ParentCenter = new Vector2(Parent.Center.X + (Parent.direction == -1 ? -40 : 40), Parent.Center.Y);

                Rectangle? chainSourceRectangle = null;
                float chainHeightAdjustment = 0f;

                Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (ChainTexture.Size() / 2f);
                Vector2 chainDrawPosition = new Vector2(NPC.Center.X, NPC.Center.Y + 3);
                Vector2 vectorFromProjectileToPlayerArms = ParentCenter.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
                Vector2 unitVectorFromProjectileToPlayerArms = vectorFromProjectileToPlayerArms.SafeNormalize(Vector2.Zero);
                float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : ChainTexture.Height()) + chainHeightAdjustment;

                if (chainSegmentLength == 0)
                {
                    chainSegmentLength = 10;
                }

                float chainRotation = unitVectorFromProjectileToPlayerArms.ToRotation() + MathHelper.PiOver2;
                int chainCount = 0;
                float chainLengthRemainingToDraw = vectorFromProjectileToPlayerArms.Length() + chainSegmentLength / 2f;

                while (chainLengthRemainingToDraw > 0f)
                {
                    Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

                    Main.spriteBatch.Draw(ChainTexture.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

                    chainDrawPosition += unitVectorFromProjectileToPlayerArms * chainSegmentLength;
                    chainCount++;
                    chainLengthRemainingToDraw -= chainSegmentLength;
                }
            }

            return true;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(ModContent.BuffType<WhipSpiderAggression>(), 600);
        }

        public override void AI()
		{
            NPC Parent = Main.npc[(int)NPC.ai[3]];

            Player player = Main.player[Parent.target];

            if (NPC.ai[0] <= 25)
            {
                NPC.direction = NPC.spriteDirection = NPC.velocity.X > 0f ? 1 : -1;
                NPC.rotation = NPC.velocity.ToRotation();

                if (NPC.spriteDirection == -1)
                {
                    NPC.rotation += MathHelper.Pi;
                }
            }

            //die if the parent npc is dead
            if (!Parent.active || Parent.type != ModContent.NPCType<WhipSpider>())
            {
                NPC.active = false;
            }

            //set mouth to retract immediately when it hits the targetted player
            if (NPC.Hitbox.Intersects(player.Hitbox))
            {
                NPC.ai[0] = 24;
            }

            NPC.ai[0]++;

            if (NPC.ai[0] == 1)
            {
                Vector2 ChargeSpeed = player.Center - NPC.Center;
                ChargeSpeed.Normalize();
                ChargeSpeed *= 22;
                NPC.velocity = ChargeSpeed;
            }

            if (NPC.ai[0] == 25)
            {
                SaveDirection = NPC.spriteDirection;
                SaveRotation = NPC.rotation;
            }

            if (NPC.ai[0] > 25)
            {
                NPC.spriteDirection = SaveDirection;
                NPC.rotation = SaveRotation;

                Vector2 ChargeSpeed = new Vector2(Parent.Center.X + (Parent.direction == -1 ? -40 : 40), Parent.Center.Y) - NPC.Center;
                ChargeSpeed.Normalize();
                ChargeSpeed *= 22;
                NPC.velocity = ChargeSpeed;

                if (NPC.Distance(Parent.Center) <= 30f)
                {
                    NPC.active = false;
                }
            }
		}
	}
}