using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.NPCs.PandoraBox;

namespace Spooky.Content.Biomes
{
    public class PandoraBoxBiome : ModBiome
    {
        public override int Music => MusicID.GoblinInvasion; //MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/PandoraBox");

        public override SceneEffectPriority Priority => SceneEffectPriority.Event;

        //bestiary stuff
        public override string BestiaryIcon => "Spooky/Content/Biomes/PandoraBoxBiomeIcon";

        public override bool IsBiomeActive(Player player)
        {
            bool BiomeCondition = PandoraBoxWorld.PandoraEventActive;

            return BiomeCondition;
        }
    }
}