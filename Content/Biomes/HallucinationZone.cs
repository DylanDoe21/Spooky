using Terraria;
using Terraria.ModLoader;

using Spooky.Content.NPCs.Hallucinations;

namespace Spooky.Content.Biomes
{
    public class HallucinationZone : ModBiome
    {
        public override int Music => NPC.AnyNPCs(ModContent.NPCType<TheFlesh>()) ? MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/TheFleshDialogue") : MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/Hallucination");

        public override SceneEffectPriority Priority => SceneEffectPriority.Event;

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("Spooky:HallucinationEffect", isActive, player.Center);
        }

        public override bool IsBiomeActive(Player player)
        {
            bool BiomeCondition = NPC.AnyNPCs(ModContent.NPCType<TheMan>()) || NPC.AnyNPCs(ModContent.NPCType<TheBaby>()) || 
            NPC.AnyNPCs(ModContent.NPCType<TheHorse>()) || NPC.AnyNPCs(ModContent.NPCType<TheFlesh>());

            return BiomeCondition;
        }
    }
}