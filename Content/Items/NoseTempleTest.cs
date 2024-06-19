using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Buffs.Minion;
using Spooky.Content.NPCs.Hallucinations;
using Spooky.Content.Projectiles.SpookyBiome;

namespace Spooky.Content.Items
{
    public class NoseTempleTest : ModItem
    {
        public override string Texture => "Spooky/Content/Items/BossSummon/CowBell";

        public override void SetDefaults()
        {                
            Item.width = 20;
            Item.height = 28;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.Blue;
        }

        public override bool? UseItem(Player player)
        {
            Flags.downedMocoIdol1 = false;
            Flags.downedMocoIdol2 = false;
            Flags.downedMocoIdol3 = false;
            Flags.downedMocoIdol4 = false;
            Flags.downedMocoIdol5 = false;
            Flags.downedMocoIdol6 = false;

            return true;
        }
    }
}