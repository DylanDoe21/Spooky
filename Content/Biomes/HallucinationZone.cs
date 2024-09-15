using Terraria;
using Terraria.ModLoader;

using Spooky.Content.NPCs.Hallucinations;

namespace Spooky.Content.Biomes
{
    public class HallucinationZone : ModBiome
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/TheEntity");

        public override SceneEffectPriority Priority => SceneEffectPriority.Event;

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("Spooky:HallucinationEffect", isActive, player.Center);
        }

        public override bool IsBiomeActive(Player player)
        {
            return NPC.AnyNPCs(ModContent.NPCType<TheMan>());
        }
    }
}