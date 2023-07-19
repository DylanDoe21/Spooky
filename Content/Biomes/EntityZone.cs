using Terraria;
using Terraria.ModLoader;

using Spooky.Content.NPCs.Hallucinations;

namespace Spooky.Content.Biomes
{
    public class EntityZone : ModBiome
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/Hallucination");

        public override SceneEffectPriority Priority => SceneEffectPriority.Event;

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("Spooky:HallucinationEffect", isActive, player.Center);
        }

        public override bool IsBiomeActive(Player player)
        {
            bool BiomeCondition = NPC.AnyNPCs(ModContent.NPCType<TheEntity>()) || NPC.AnyNPCs(ModContent.NPCType<TheBaby>()) || NPC.AnyNPCs(ModContent.NPCType<TheHorse>());

            return BiomeCondition;
        }
    }
}