using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Dusts;

namespace Spooky.Content.Buffs.Debuff
{
	public class HazmatMinionToxic : ModBuff
	{
		public override string Texture => "Spooky/Content/Buffs/Debuff/DebuffPlaceholder";

		public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
        {
			if (!npc.friendly)
            {
				//fly visuals
				if (Main.rand.NextBool(18))
				{
					Color[] colors = new Color[] { Color.Lime, Color.Green };

					int DustEffect = Dust.NewDust(npc.position, npc.width, 3, ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, Main.rand.Next(colors) * 0.5f, Main.rand.NextFloat(0.2f, 0.5f));
					Main.dust[DustEffect].velocity.X = 0;
					Main.dust[DustEffect].velocity.Y = -1;
					Main.dust[DustEffect].alpha = 100;
				}

				//damage over time
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }

                npc.lifeRegen -= 10;
			}
		}
    }
}
